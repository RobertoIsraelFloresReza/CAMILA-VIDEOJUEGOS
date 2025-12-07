using UnityEngine;

public class LakeLevelManager : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentObjective("START_02");
        }
    }
}
