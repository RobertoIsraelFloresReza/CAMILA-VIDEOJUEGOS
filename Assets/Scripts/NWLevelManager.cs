using UnityEngine;

public class NWLevelManager : MonoBehaviour
{

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentObjective("START_03");
        }
    }
}
