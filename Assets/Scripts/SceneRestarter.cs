using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    // Función que se llama cuando el jugador muere
    public void RestartGame()
    {
        // Puedes agregar lógica para una pantalla de "Game Over" aquí
        Debug.Log("Juego Terminado. Reiniciando Escena...");
        
        // Obtiene la escena actual y la recarga
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
