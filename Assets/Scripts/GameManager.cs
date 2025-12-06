using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
}
