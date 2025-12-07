using UnityEngine;

public class PersistentID : MonoBehaviour
{
    [SerializeField]
    private string uniqueID; 

    public string GetID()
    {
        return uniqueID;
    }
}
