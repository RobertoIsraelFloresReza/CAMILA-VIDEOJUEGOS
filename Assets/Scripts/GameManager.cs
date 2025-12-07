using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private ObjectiveDisplay objectiveDisplay;

    private Dictionary<string, bool> itemStates = new Dictionary<string, bool>();

    public event Action<string> OnItemCollected; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NotifyItemCollected(string itemID)
    {
        if (!itemStates.ContainsKey(itemID))
        {
            itemStates.Add(itemID, true);
        } else {
            itemStates[itemID] = true;
        }

        OnItemCollected?.Invoke(itemID);
    }
    
    public bool HasItem(string itemID)
    {
        return itemStates.ContainsKey(itemID) && itemStates[itemID];
    }
    
    private Dictionary<string, bool> sceneObjectStates = new Dictionary<string, bool>();

    public void SetObjectState(string objectID, bool state)
    {
        if (sceneObjectStates.ContainsKey(objectID))
            sceneObjectStates[objectID] = state;
        else
            sceneObjectStates.Add(objectID, state);
    }

    public bool GetObjectState(string objectID)
    {
        return sceneObjectStates.ContainsKey(objectID) && sceneObjectStates[objectID];
    }
    
    private Dictionary<string, string> questDescriptions = new Dictionary<string, string>()
    {
        {"START_01", "Encuentra la llave para entrar a la cabaña"}, // YA
        {"START_02", "Encuentra la llave en el invernadero por el lago"},
        {"START_03", "Encuentra la llave en el techo del edificio"},
        {"START_04", "Encuentra la llave en la cima de la montaña"},
        {"RIP_&_TEAR","Acaba con todos los enemigos"},
        {"KEY_DOOR_A_ACQUIRED", "Regresa a la cabaña y entra"}, // YA
        {"LOCK_KEY_FOUND", "Encuentra el portón que abre esta llave y explora el área."}, // YA
        {"GATE_DOOR_OPEN", "Busca la siguiente llave de la cabaña en la nueva Zona"}, // YA
        {"NEW_DOOR_KEY", "Regresa a la cabaña y encuentra la puerta que abre la llave"}, // YA
        {"NEW_QUEST","Necesitas más llaves para explorar la cabaña, explora otras zonas"}, // YA
        {"FIND_KEY","Encuentra la llave para abrir esta puerta"}, // YA
    };
    
    [Header("Gestión de Escenas y Posición")]
    public string nextSpawnPointID = "DefaultSpawn";
    
    public void SetCurrentObjective(string objectiveID)
    {
        if (objectiveDisplay == null)
        {
            objectiveDisplay = FindObjectOfType<ObjectiveDisplay>();
        
            if (objectiveDisplay == null)
            {
                Debug.LogWarning("ObjectiveDisplay no se encontró en esta escena. No se pudo actualizar el objetivo.");
                return;
            }
        }
    
        if (questDescriptions.ContainsKey(objectiveID))
        {
            string newObjective = questDescriptions[objectiveID];
        
            objectiveDisplay.UpdateObjective(newObjective);
        
            Debug.Log($"Nuevo Objetivo: {newObjective}");
        }
        else
        {
            Debug.LogError($"ID de objetivo '{objectiveID}' no encontrado.");
        }
    }
    
    public void SetNextSpawnPoint(string spawnID)
    {
        if (string.IsNullOrEmpty(spawnID)) 
        {
            Debug.LogError("Intentando establecer un spawn point vacío. Usando DefaultSpawn.");
            spawnID = "DefaultSpawn";
        }
        nextSpawnPointID = spawnID;
    }
    
    public string ConsumeNextSpawnPoint()
    {
        string id = nextSpawnPointID;
        nextSpawnPointID = "DefaultSpawn"; 
        return id;
    }
    
    private EnemyCountDisplay enemyCountDisplay;
    public void UpdateEnemyCount(int enemiesRemaining)
    {
        // 1. Si no hay referencia, busca el display en la escena.
        if (enemyCountDisplay == null)
        {
            enemyCountDisplay = FindObjectOfType<EnemyCountDisplay>();
        
            if (enemyCountDisplay == null)
            {
                Debug.LogWarning("EnemyCountDisplay no se encontró para actualizar el conteo.");
                return;
            }
        }
    
        // 2. Actualizar la UI
        enemyCountDisplay.SetCount(enemiesRemaining);
    }
}
