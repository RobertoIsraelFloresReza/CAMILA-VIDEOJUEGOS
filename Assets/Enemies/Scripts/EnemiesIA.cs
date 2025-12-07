using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesIA : MonoBehaviour, IStateMachine
{
    [Header("Componentes")]
    public NavMeshAgent agent;
    public Animator animator;
    
    public SistemaDeVida healthController;

    [Header("Sensores")]
    public Transform player;
    private Collider[] detectedObjects = new Collider[10];
    public float detectionRadius = 10f;
    public bool isPlayerSighted;

    [Header("Combate")]
    public float distanciaAtaque = 2.5f;

    [Header("Patrullaje")]
    public Transform[] waypoints;
    
    [Header("Capas de Detección")]
    public LayerMask layersToIgnore;
    
    [Header("Detección")]
    public LayerMask playerLayer;
    
    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip detectionSound;

    IEnumerator Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        RandomizeIdle();

        ChangeState(new PatrollingState(this));

        while (true)
        {
            var numberOfColliders = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectedObjects);
            isPlayerSighted = false;

            for (int i = 0; i < numberOfColliders; i++)
            {
                var col = detectedObjects[i];
                if (!col.CompareTag("Player")) continue;
                player = col.transform;
                var vectorToPlayer = player.position - transform.position;
                var dot = Vector3.Dot(vectorToPlayer, transform.forward);

                if (dot < 0) continue;
                
                int finalLayerMask = ~layersToIgnore;
                
                Physics.Raycast(
                    transform.position + Vector3.up * 1.5f, 
                    vectorToPlayer, 
                    out var hit, 
                    detectionRadius, 
                    playerLayer 
                );
                
                Debug.DrawRay(transform.position + Vector3.up * 1.5f, vectorToPlayer.normalized * detectionRadius, hit.collider != null && hit.collider.transform == player ? Color.green : Color.red, 0.2f);

                if (hit.collider != null && hit.collider.transform != player) continue;

                isPlayerSighted = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public void PlayDetectionSound()
    {
        if (audioSource != null && detectionSound != null)
        {
            audioSource.PlayOneShot(detectionSound);
        }
    }

    void Update()
    {
        CurrentState?.Tick(Time.deltaTime);
    }

    public IState CurrentState { get; set; }
    public void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }


    public void Morir()
    {
        if (CurrentState is DeathState) return;

        ChangeState(new DeathState(this));
    }


    public void SetWalking(bool isWalking)
    {
        if (animator != null) animator.SetBool("isWalking", isWalking);
    }
    public void TriggerAttack()
    {
        if (animator != null) animator.SetTrigger("attack");
    }
    public void RandomizeWalk()
    {
        if (animator != null) animator.SetInteger("RandomWalk", Random.Range(0, 2));
    }
    public void RandomizeIdle()
    {
        if (animator != null) animator.SetInteger("RandomIdle", Random.Range(0, 2));
    }
    public void RandomizeAttack()
    {
        if (animator != null) animator.SetInteger("RandomAttack", Random.Range(0, 3));
    }
}

// ESTADO: PATRULLAJE (Struct)

public struct PatrollingState : IState
{
    public EnemiesIA StateMachine { get; private set; }
    private int wpindex;
    private float threshold;

    // Variables de espera
    private bool isWaiting;
    private float waitTimer;
    private float waitDuration;

    public PatrollingState(EnemiesIA stateMachine)
    {
        StateMachine = stateMachine;
        wpindex = 0;
        threshold = 2f; 
        isWaiting = false;
        waitTimer = 0f;
        waitDuration = 3f;
    }

    public void Enter()
    {
        if (StateMachine.waypoints == null || StateMachine.waypoints.Length == 0) return;

        // ir al primer punto
        StateMachine.agent.destination = StateMachine.waypoints[wpindex].position;
        StateMachine.RandomizeWalk();
        StateMachine.SetWalking(true);
    }

    public void Tick(float deltaTime)
    {
        if (StateMachine.waypoints == null || StateMachine.waypoints.Length == 0) return;

        // 1. eperando
        if (isWaiting)
        {
            // Aseguramos que se quede quieto y en Idle
            StateMachine.SetWalking(false);

            waitTimer += deltaTime;
            if (waitTimer >= waitDuration)
            {
                TerminarEspera();
            }
            return; 
        }

        // 2. caminando
        var agentPos = StateMachine.transform.position;
        var currentWp = StateMachine.waypoints[wpindex].position;
        var distanceBetween = Vector3.Distance(agentPos, currentWp);

        // Si llegamos al destino...
        if (distanceBetween < threshold)
        {
            EmpezarEspera();
        }

        if (StateMachine.isPlayerSighted)
        {
            StateMachine.PlayDetectionSound();
            StateMachine.ChangeState(new ChasingState(StateMachine));
        }
    }

    public void Exit()
    {
        StateMachine.SetWalking(false);
    }

    private void EmpezarEspera()
    {
        isWaiting = true;
        waitTimer = 0f;

        StateMachine.agent.isStopped = true; // Frenamos
        StateMachine.agent.velocity = Vector3.zero;
        StateMachine.SetWalking(false);      // Animación Idle
        StateMachine.RandomizeIdle();
    }

    private void TerminarEspera()
    {
        isWaiting = false;

        //calculamos el siguiente punto
        wpindex = (wpindex + 1) % StateMachine.waypoints.Length;
        StateMachine.agent.destination = StateMachine.waypoints[wpindex].position;

        StateMachine.agent.isStopped = false; // Reactivamos caminar
        StateMachine.SetWalking(true);     
        StateMachine.RandomizeWalk();
    }
}


// ESTADO: PERSECUCIÓN
public struct ChasingState : IState
{
    public EnemiesIA StateMachine { get; private set; }
    private float timerLost;
    private float timerAttack;

    public ChasingState(EnemiesIA stateMachine)
    {
        StateMachine = stateMachine;
        timerLost = 0f;
        timerAttack = 0f;
    }

    public void Enter()
    {
        StateMachine.RandomizeWalk();
        StateMachine.SetWalking(true);
    }

    public void Tick(float deltaTime)
    {
        if (StateMachine.player == null) return;

        float distanceToPlayer = Vector3.Distance(StateMachine.transform.position, StateMachine.player.position);

        // Ataque
        if (distanceToPlayer <= StateMachine.distanciaAtaque)
        {
            StateMachine.agent.isStopped = true;
            StateMachine.SetWalking(false);

            timerAttack += deltaTime;
            if (timerAttack >= 2.0f)
            {
                SistemaDeVida playerHealth = StateMachine.player.GetComponent<SistemaDeVida>();

                if (playerHealth != null)
                {
                    float enemyDamage = 50000f; 
                    playerHealth.TakeDamage(enemyDamage);

                }
                
                StateMachine.RandomizeAttack();
                StateMachine.TriggerAttack();
                timerAttack = 0f;
            }
        }
        else
        {
            StateMachine.agent.isStopped = false;
            StateMachine.SetWalking(true);
            StateMachine.agent.destination = StateMachine.player.position;
            timerAttack = 2.0f;
        }

        // dejar de ver al player
        if (!StateMachine.isPlayerSighted)
        {
            timerLost += deltaTime;
        }
        else
        {
            timerLost = 0f;
        }

        if (timerLost >= 3f)
        {
            StateMachine.ChangeState(new PatrollingState(StateMachine));
        }
    }

    public void Exit()
    {
        StateMachine.SetWalking(false);
        if (StateMachine.agent.isActiveAndEnabled) StateMachine.agent.isStopped = false;
    }
}


// ESTADO: MUERTE
public struct DeathState : IState
{
    public EnemiesIA StateMachine { get; private set; }

    public DeathState(EnemiesIA stateMachine)
    {
        StateMachine = stateMachine;
    }

    public void Enter()
    {
        if (StateMachine.agent.isActiveAndEnabled)
        {
            StateMachine.agent.isStopped = true;
            StateMachine.agent.enabled = false; 
        }

        var collider = StateMachine.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        if (StateMachine.animator != null) StateMachine.animator.SetBool("isDead", true);

        Object.Destroy(StateMachine.gameObject, 4.0f);
    }

    public void Tick(float deltaTime)
    {

    }

    public void Exit()
    {
    }
}