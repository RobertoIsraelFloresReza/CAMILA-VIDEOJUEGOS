using System.Collections; // Necesario para Coroutines
using UnityEngine;
using UnityEngine.InputSystem;
public class ControlEscopeta : MonoBehaviour
{
    public int maxAmmo = 8;
    private int currentAmmo;
    public float reloadTime = 2.0f; 
    private bool isReloading = false;
    public float fireRate = 1.0f; 
    private float nextFireTime = 0f; 
    public float shootDistance = 100f; 
    public Camera playerCamera;
    
    public AudioSource shootSound;

    public AudioSource reloadSound;
    
    [Header("Retroceso de la Vista")]
    private CmeraRecoil cameraRecoilScript;

    [Header("3. Retroceso (Recoil)")]
    public float recoilX = -5f;      // Ángulo de retroceso en el eje X (hacia arriba)
    public float recoilDuration = 0.1f; // Tiempo que dura el retroceso
    private Vector3 originalRotation; // Rotación inicial del arma

    [Header("4. Efectos Visuales")]
    public Light muzzleFlashLight;   // Arrastra aquí el componente Light del "muzzle flash"
    public float flashDuration = 0.05f; // Duración en segundos del flash
    public ParticleSystem hitParticles; // Partículas que aparecerán al impactar algo (opcional)

    void Start()
    {
        if (playerCamera != null)
        {
            cameraRecoilScript = playerCamera.GetComponent<CmeraRecoil>();
        
            if (cameraRecoilScript == null)
            {
                Debug.LogError("Error: No se encontró el componente CameraRecoil en la cámara. Asegúrate de que esté adjunto.");
            }
        }
    }
    
    void Shoot()
    {
        // ANTES: if (Time.time < nextFireTime) return; <-- ¡QUITAR ESTO!

        currentAmmo--;
        // ANTES: nextFireTime = Time.time + fireRate; <-- ¡QUITAR ESTO!
    
        Debug.Log("¡BANG! Quedan " + currentAmmo + " balas.");
        shootSound.Play();

        StartCoroutine(DoRecoil());
        StartCoroutine(FlashEffect());

        // 2. Disparar el retroceso de la cámara
        if (cameraRecoilScript != null)
        {
            cameraRecoilScript.GenerateRecoil();
        }
    }
    
    // Coroutine para el efecto de Retroceso (Recoil)
    IEnumerator DoRecoil()
    {
        Quaternion targetRotation = Quaternion.Euler(originalRotation.x + recoilX, originalRotation.y, originalRotation.z);
    
        float timer = 0f;

        while (timer < recoilDuration)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, timer / recoilDuration * 10f);
            timer += Time.deltaTime;
            yield return null;
        }
    
        timer = 0f;
    
        // Regreso suave a la posición original
        while (timer < recoilDuration)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(originalRotation), timer / recoilDuration * 5f);
            timer += Time.deltaTime;
            yield return null;
        }

        // Asegura que termine en la posición original
        transform.localRotation = Quaternion.Euler(originalRotation);
    }

// Coroutine para el Flash de la Boca del Arma (Muzzle Flash)
    IEnumerator FlashEffect()
    {
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            // Espera un breve instante (ej. 0.05 segundos)
            yield return new WaitForSeconds(flashDuration); 
            muzzleFlashLight.enabled = false;
        }
    }

void Update()
    {
        if (isReloading)
        {
            return;
        }


        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {

            nextFireTime = Time.time + fireRate; 
            
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                Debug.Log("¡Clic! ¡Sin munición!");
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }


    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");
        reloadSound.Play();

        // Espera el tiempo de recarga
        yield return new WaitForSeconds(reloadTime);

        // Recarga completa
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("¡Recarga completa! Munición: " + currentAmmo + "/" + maxAmmo);
    }
}
