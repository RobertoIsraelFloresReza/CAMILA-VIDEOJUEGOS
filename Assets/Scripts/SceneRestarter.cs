using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Juego Terminado. Reiniciando Escena...");
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
