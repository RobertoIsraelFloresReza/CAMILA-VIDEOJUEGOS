using UnityEngine;
using  UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    void Start()
    {
        Invoke("WaitForEnd", 8);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void WaitForEnd()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
