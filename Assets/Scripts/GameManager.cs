using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 1. Patrón Singleton: Acceso estático global
    public static GameManager Instance { get; private set; }

    // 2. Estado de Adquisición de Llaves (Ejemplo)
    // Se usa Dictionary para guardar qué llaves o items han sido recogidos
    private Dictionary<string, bool> itemStates = new Dictionary<string, bool>();

    // 3. Evento genérico para notificar al sistema sobre la recogida de items
    public event Action<string> OnItemCollected; 

    void Awake()
    {
        // Implementación del Singleton Persistente
        if (Instance == null)
        {
            Instance = this;
            // ¡La clave de la persistencia!
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Si ya existe una instancia, destruye el duplicado.
            Destroy(gameObject);
        }
    }

    // Método para ser llamado por las llaves o el WeaponPickup
    public void NotifyItemCollected(string itemID)
    {
        // 1. Guarda el estado globalmente
        if (!itemStates.ContainsKey(itemID))
        {
            itemStates.Add(itemID, true);
        } else {
            itemStates[itemID] = true;
        }

        // 2. Dispara el evento a cualquier script que esté escuchando
        OnItemCollected?.Invoke(itemID);
    }
    
    // Método para que las puertas, ItemSwitcher, o ControlEscopeta revisen el estado.
    public bool HasItem(string itemID)
    {
        return itemStates.ContainsKey(itemID) && itemStates[itemID];
    }
}
