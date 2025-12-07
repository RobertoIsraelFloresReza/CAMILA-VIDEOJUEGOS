using UnityEngine;

public class CabinLevelManager : MonoBehaviour
{
    private const string CriticalDoorID = "Door0"; 

    void Start()
    {
        if (GameManager.Instance != null)
        {
            bool doorAlreadyOpened = GameManager.Instance.GetObjectState(CriticalDoorID);

            if (doorAlreadyOpened)
            {
                GameManager.Instance.SetCurrentObjective("NEW_DOOR_KEY");
            }
            else
            {
                GameManager.Instance.SetCurrentObjective("START_01");
            }
        }
    }
}
