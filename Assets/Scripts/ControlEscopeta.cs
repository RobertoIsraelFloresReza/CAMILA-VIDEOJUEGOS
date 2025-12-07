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
    
    public string weaponItemID = "Escopeta";
    
    public float shotgunDamage = 5000f;
    
    [Header("Retroceso de la Vista")]
    private CmeraRecoil cameraRecoilScript;

    [Header("3. Retroceso (Recoil)")]
    public float recoilX = -5f;      
    public float recoilDuration = 0.1f; 
    private Vector3 originalRotation; 

    [Header("4. Efectos Visuales")]
    public Light muzzleFlashLight;  
    public float flashDuration = 0.05f;
    public ParticleSystem hitParticles; 
    
    void OnEnable()
    {
        EventManager.Subscribe(GlobalEvents.WeaponAdquired, SetWeaponAdquired);
        originalRotation = transform.localRotation.eulerAngles;
     //   Debug.Log($"[ON ENABLE] Activando arma. Rotación actual: {transform.localRotation.eulerAngles}");

     UpdateUI();
        
        nextFireTime = Time.time;
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(GlobalEvents.WeaponAdquired, SetWeaponAdquired);
    }

    private void SetWeaponAdquired()
    {
        isWeaponAdquired = true;
        currentAmmo = maxAmmo; // Inicializamos la munición al recogerla
      //  Debug.Log("ControlEscopeta: ¡Arma adquirida! Lista para usar.");
    }
    private AmmoDisplay ammoDisplay;
    private bool isWeaponAdquired = false;
    void Start()
    {
        ammoDisplay = FindObjectOfType<AmmoDisplay>();
        if (ammoDisplay == null)
        {
            Debug.LogError("Error: No se encontró el componente AmmoDisplay en la escena.");
        }
        
        if (GameManager.Instance != null && GameManager.Instance.HasItem(weaponItemID))
        {
            isWeaponAdquired = true;
            currentAmmo = maxAmmo; 
            UpdateUI(); 
        }

        if (GameManager.Instance != null && GameManager.Instance.HasItem(weaponItemID))
        {
            isWeaponAdquired = true;
            currentAmmo = maxAmmo;
            UpdateUI(); 
        }
        
        if (GameManager.Instance != null && GameManager.Instance.HasItem(weaponItemID))
        {
            isWeaponAdquired = true;
            currentAmmo = maxAmmo;
        }
    
        if (!isWeaponAdquired && GameManager.Instance != null)
        {
            GameManager.Instance.OnItemCollected += CheckWeaponAcquired;
        }
        
        if (playerCamera != null)
        {
            cameraRecoilScript = playerCamera.GetComponent<CmeraRecoil>();
    
            if (cameraRecoilScript == null)
            {
                Debug.LogError("Error: No se encontró el componente CameraRecoil en la cámara. Asegúrate de que esté adjunto.");
            }
        }
    }
    
    void UpdateUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.UpdateAmmoUI(currentAmmo, maxAmmo);
            ammoDisplay.SetActive(isWeaponAdquired);
        }
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnItemCollected -= CheckWeaponAcquired;
        }
    }
    
    private void CheckWeaponAcquired(string itemID)
    {
        if (itemID == weaponItemID)
        {
            isWeaponAdquired = true;
            currentAmmo = maxAmmo;
            UpdateUI();
          //  Debug.Log("ControlEscopeta: ¡Arma adquirida vía Singleton!");
            GameManager.Instance.OnItemCollected -= CheckWeaponAcquired; 
        }
    }
    
    void Shoot()
    {
        currentAmmo--;
        UpdateUI();
    
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, shootDistance))
        {
            SistemaDeVida targetHealth = hit.collider.GetComponent<SistemaDeVida>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(shotgunDamage);

                if (hitParticles != null)
                {
                    Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
        shootSound.Play();

        StartCoroutine(DoRecoil());
        StartCoroutine(FlashEffect());

        if (cameraRecoilScript != null)
        {
            cameraRecoilScript.GenerateRecoil();
        }
    }
    
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

        transform.localRotation = Quaternion.Euler(originalRotation);
    }

// Coroutine para el Flash de la Boca del Arma (Muzzle Flash)
    IEnumerator FlashEffect()
    {
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
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
        
       // Debug.Log($"[UPDATE] Ammo: {currentAmmo}/{maxAmmo}, nextFireTime: {nextFireTime:F2}, Time.time: {Time.time:F2}");

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
          //  Debug.Log("--- Disparo Autorizado por FireRate ---");
            
         //   Debug.Log("--- Clic Detectado ---");
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
        //Debug.Log("Recargando...");
        reloadSound.Play();
        if (ammoDisplay != null) ammoDisplay.SetReloading(true);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateUI();
        //Debug.Log("¡Recarga completa! Munición: " + currentAmmo + "/" + maxAmmo);
    }
    
    
}
