using UnityEngine;
using UnityEngine.SceneManagement;

public class CodigoPausa : MonoBehaviour
{
   public GameObject ObjectMenuPausa;
    public bool Pausa = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Pausa == false)
            {
                ObjectMenuPausa.SetActive(true);
                Pausa = true;
                
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if(Pausa == true)  {
                Resumir();
            }
        }
    }
    public void Resumir()
    {
        ObjectMenuPausa.SetActive(false);
        Pausa = false;

        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IrAlMenu(string NombreMenu)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(NombreMenu);

    }
    public void SalirDelJuego()
    {
        Application.Quit();
    }
    public void GuardarJuego()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGameData();
        }
        
        Resumir(); 
    }

    public void CargarJuego()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSavedGame())
        {
            Time.timeScale = 1; 

            SaveManager.Instance.LoadGameData();
        }
        else
        {
            Debug.LogWarning("No hay partida guardada para cargar.");
        }
    }
    

}
