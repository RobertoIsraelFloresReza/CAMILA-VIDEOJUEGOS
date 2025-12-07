using UnityEngine;
using UnityEngine.InputSystem; 

public class ItemSwitcher : MonoBehaviour
{
    public GameObject[] items;

    private int currentItemIndex = 0;
    private bool isWeaponAdquired = false;
    private int lastValidIndex = 0;
    
    
    [Header("Configuración de Items")]
    public int shotgunItemIndex = 1;
    public string shotgunItemID = "Escopeta";
    
    bool hasWeapon = false;

    void Start()
    {
        SelectItem(0,0);
        lastValidIndex = 0;
    }

    void Update()
    {
        hasWeapon=GameManager.Instance != null && GameManager.Instance.HasItem("Escopeta");
        if (Mouse.current == null) return;

        float scrollInput = Mouse.current.scroll.ReadValue().y;

        int previousItemIndex = currentItemIndex;

        if (scrollInput > 0f && hasWeapon) // Rueda hacia arriba
        {
            currentItemIndex--;
            if (currentItemIndex < 0)
            {
                currentItemIndex = items.Length - 1;
            }
        }
        else if (scrollInput < 0f && hasWeapon) // Rueda hacia abajo
        {
            currentItemIndex++;
            if (currentItemIndex >= items.Length)
            {
                currentItemIndex = 0;
            }
        }

        if (previousItemIndex != currentItemIndex)
        {
         //   Debug.Log($"[SWITCHER DEBUG] Scroll detectado. Llamando SelectItem: new={currentItemIndex}, old={previousItemIndex}.");
            SelectItem(currentItemIndex, previousItemIndex);
        }
    }

    void SelectItem(int newIndex, int oldIndex)
    {
        bool currentHasWeapon = GameManager.Instance != null && GameManager.Instance.HasItem("Escopeta");
        int finalIndex = newIndex;
     //   Debug.Log($"[SELECT DEBUG] -> INICIO: newIndex={newIndex}, oldIndex={oldIndex}, hasWeapon={currentHasWeapon}");
    
        if (newIndex == shotgunItemIndex && !hasWeapon)
        {
            finalIndex = oldIndex;
          //  Debug.Log($"[SELECT DEBUG] Bloqueo de Escopeta activado. finalIndex forzado a {finalIndex} (oldIndex).");
        }
        
        
    
        // 2. Aplicar el índice final
        currentItemIndex = finalIndex; 

        AmmoDisplay display = FindObjectOfType<AmmoDisplay>();
        if (display != null)
        {
            bool shouldBeActive = currentHasWeapon && finalIndex == shotgunItemIndex;
            display.SetActive(hasWeapon && finalIndex == shotgunItemIndex); 
         //   Debug.Log($"[SELECT DEBUG] Estado UI Ammo: {shouldBeActive} (hasWeapon={currentHasWeapon}, finalIndex={finalIndex}, ShotgunIndex={shotgunItemIndex}).");
        }

        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(i == finalIndex);
        }
      //  Debug.Log($"[SELECT DEBUG] -> FIN: Item {finalIndex} activado. currentItemIndex ahora es {currentItemIndex}.");
    }
    
    public void ForceSelectWeapon()
    {
        bool currentHasWeapon = GameManager.Instance != null && GameManager.Instance.HasItem(shotgunItemID);
        
     //   Debug.Log("[FORCE DEBUG] -> INICIO DE FUERZA DE SELECCIÓN.");
        if (GameManager.Instance != null && GameManager.Instance.HasItem(shotgunItemID))
        {
            int previousIndex = currentItemIndex;
        
            currentItemIndex = shotgunItemIndex;
        //    Debug.Log($"[FORCE DEBUG] Arma adquirida. Forzando índice: old={previousIndex} a new={shotgunItemIndex}.");
            SelectItem(shotgunItemIndex, previousIndex);
        }else
        {
            Debug.LogWarning("[FORCE DEBUG] Llamada a ForceSelectWeapon pero hasWeapon es FALSE.");
        }
    }
}
