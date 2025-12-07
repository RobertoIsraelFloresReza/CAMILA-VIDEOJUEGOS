using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    public TextMeshProUGUI ammoText;

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
    

    public void SetReloading(bool isReloading)
    {
        if (ammoText != null)
        {
            if (isReloading)
            {
                ammoText.text = "RECARGANDO...";
            }
            // Si no está recargando, se actualizará en UpdateAmmoUI.
        }
    }
    
    public void SetActive(bool isActive)
    {
        if (ammoText != null)
        {
            ammoText.gameObject.SetActive(isActive);
        }
    }
}
