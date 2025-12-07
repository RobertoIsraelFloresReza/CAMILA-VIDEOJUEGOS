using UnityEngine;

public class MountainLevelManager : MonoBehaviour
{

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentObjective("START_04");
        }
    }
}
