using UnityEngine;

public class ChurchLevelManager : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentObjective("RIP_&_TEAR");
        }
    }
}
