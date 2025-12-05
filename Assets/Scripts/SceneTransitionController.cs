using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour, IInteractable
{[Header("Escena de Destino")]
    [Tooltip("El nombre EXACTO de la siguiente escena a cargar (debe estar en Build Settings). Ej: 'Level2'")]
    public string nextSceneName;

    // Esta función se llama cuando el jugador interactúa con este objeto
    public void Interact()
    {
        // 1. Verificación de seguridad
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("ERROR: El nombre de la escena de destino no está configurado en el Inspector.");
            return;
        }

        // 2. Cargar la siguiente escena
        Debug.Log($"[TRANSICIÓN] Interacción detectada. Cargando escena: {nextSceneName}");
        
        // El corazón de la función: carga la escena por su nombre.
        SceneManager.LoadScene(nextSceneName);
    }
}
