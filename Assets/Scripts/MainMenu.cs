using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject credits;

    private GameObject currentPanel;

    void Start()
    {
        ShowPanel(mainMenu);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
        }

        panelToShow.SetActive(true);
        currentPanel = panelToShow;
    }

    public void ShowMainMenu()
    {
        ShowPanel(mainMenu);
    }

    public void ShowOptions()
    {
        ShowPanel(options);
    }

    public void ShowCredits()
    {
        ShowPanel(credits);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CabinMap");
    }
}