using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo Input System

public class ItemSwitcher : MonoBehaviour
{
    // Arrastraremos nuestros items (escopeta, linterna) aquí
    public GameObject[] items;

    private int currentItemIndex = 0;

    void Start()
    {
        // Al empezar, nos aseguramos de que solo el primer item esté activo
        SelectItem(0);
    }

    void Update()
    {
        // Revisamos si hay un mouse conectado
        if (Mouse.current == null) return;

        // Leemos el valor "Y" de la rueda del ratón
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        int previousItemIndex = currentItemIndex;

        if (scrollInput > 0f) // Rueda hacia arriba
        {
            // Pasamos al item anterior
            currentItemIndex--;
            // Si llegamos al primero (índice -1), damos la vuelta al final de la lista
            if (currentItemIndex < 0)
            {
                currentItemIndex = items.Length - 1;
            }
        }
        else if (scrollInput < 0f) // Rueda hacia abajo
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
            SelectItem(currentItemIndex);
        }
    }

    // Esta función activa un item y desactiva todos los demás
    void SelectItem(int index)
    {
        // Recorremos todos los items en nuestra lista
        for (int i = 0; i < items.Length; i++)
        {
            // Compara: ¿Es 'i' el 'index' que queremos activar?
            // Si i == index, SetActive(true).
            // Si i != index, SetActive(false).
            items[i].SetActive(i == index);
        }
    }
}
