using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalZoneManager : MonoBehaviour
{
    [Header("Configuración de Victoria")]
    [Tooltip("Arrastra aquí tu script VictoryScreen.")]
    public VictoryScript victoryScreen;

    private int totalEnemies;
    private int enemiesDefeated = 0;

    void Start()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalEnemies = enemies.Length;
        Debug.Log($"[FINAL ZONE] Total de enemigos iniciales: {totalEnemies}");

        if (totalEnemies == 0)
        {
            Debug.LogWarning("[FINAL ZONE] No se encontraron enemigos. Victoria instantánea.");
            if (victoryScreen != null) victoryScreen.ShowVictory();
        }
    }
    
    public void EnemyWasDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"Enemigos restantes: {totalEnemies - enemiesDefeated}");

        if (enemiesDefeated >= totalEnemies)
        {
            if (victoryScreen != null)
            {
                victoryScreen.ShowVictory();
            }
        }
    }
}
