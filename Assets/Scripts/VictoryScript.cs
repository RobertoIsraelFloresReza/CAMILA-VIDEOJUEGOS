using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScript : MonoBehaviour
{
    [Header("Componentes UI")]
    [Tooltip("Arrastra aquí el panel principal de Victoria.")]
    public GameObject victoryPanel;

    void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }


    public void ShowVictory()
    {
        Debug.Log("¡Victoria! Mostrando pantalla final.");

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }


    public void ContinueButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}
