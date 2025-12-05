using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo Input System

public class ItemSwitcher : MonoBehaviour
{
    // Arrastraremos nuestros items (escopeta, linterna) aquí
    public GameObject[] items;

    private int currentItemIndex = 0;
    private bool isWeaponAdquired = false;
    private int lastValidIndex = 0;
    
    
    [Header("Configuración de Items")]
    public int shotgunItemIndex = 0;
    public string shotgunItemID = "Escopeta";
    
    bool hasWeapon = false;

    void Start()
    {
        // Al empezar, nos aseguramos de que solo el primer item esté activo
        SelectItem(0,0);
        lastValidIndex = 0;
    }

    void Update()
    {
        hasWeapon=GameManager.Instance != null && GameManager.Instance.HasItem("Escopeta");
        // Revisamos si hay un mouse conectado
        if (Mouse.current == null) return;

        // Leemos el valor "Y" de la rueda del ratón
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        int previousItemIndex = currentItemIndex;

        if (scrollInput > 0f && hasWeapon) // Rueda hacia arriba
        {
            // Pasamos al item anterior
            currentItemIndex--;
            // Si llegamos al primero (índice -1), damos la vuelta al final de la lista
            if (currentItemIndex < 0)
            {
                currentItemIndex = items.Length - 1;
            }
        }
        else if (scrollInput < 0f && hasWeapon) // Rueda hacia abajo
        {
            // Pasamos al siguiente item
            currentItemIndex++;
            // Si nos pasamos del último, volvemos al primero (índice 0)
            if (currentItemIndex >= items.Length)
            {
                currentItemIndex = 0;
            }
        }

        // Si el índice cambió (es decir, si movimos la rueda),
        // llamamos a nuestra función para actualizar el item.
        if (previousItemIndex != currentItemIndex)
        {
           // Debug.Log($"INTENTO DE CAMBIO: old={previousItemIndex}, new={currentItemIndex}.");
            SelectItem(currentItemIndex, previousItemIndex);
        }
        else
        {
            SelectItem(currentItemIndex, currentItemIndex); 
        }
    }

    // Esta función activa un item y desactiva todos los demás
    void SelectItem(int newIndex, int oldIndex)
    {
        int finalIndex = newIndex;
        
        if (newIndex == shotgunItemIndex && !hasWeapon)
        {
         //   Debug.Log("!!! BLOQUEO ACTIVADO: Volviendo a índice " + oldIndex + ".");
            // Forzamos el índice a ser el anterior (la linterna o lo que estuviera antes)
            finalIndex = oldIndex; 
        }
            currentItemIndex = newIndex; // Si es válido, confirmamos el nuevo índice.

           // Debug.Log($"[SELECT] Índice final activo: {finalIndex}. currentItemIndex ahora es {currentItemIndex}.");
        // Activación/Desactivación
        for (int i = 0; i < items.Length; i++)
        {
            // Solo activamos el item que coincide con el índice final
            items[i].SetActive(i == finalIndex);
        }
    }
}
