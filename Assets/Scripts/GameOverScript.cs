using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [Header("Componentes UI")]
        [Tooltip("Arrastra aquí el panel principal de Game Over (el que debe estar desactivado al inicio).")]
        public GameObject gameOverPanel;
    
        [Header("Opciones de Reinicio")]
        [Tooltip("El nombre de la escena a recargar. Si está vacío, recarga la actual.")]
        public string sceneToLoad = "";
    
        void Start()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
    
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    

        public void ShowGameOver()
        {
            Debug.Log("¡Game Over! Mostrando pantalla.");
    
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
    
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Congela el juego
        }
    
        public void RestartButton()
        {
            Time.timeScale = 1f; 
    
            string targetScene = string.IsNullOrEmpty(sceneToLoad) ? SceneManager.GetActiveScene().name : sceneToLoad;
            
            // 3. Cargar la escena
            SceneManager.LoadScene(targetScene);
        }
        

        public void QuitButton()
        {
            Application.Quit();
            Debug.Log("Saliendo del juego...");
        }
}
