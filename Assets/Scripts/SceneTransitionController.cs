using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour, IInteractable
{[Header("Escena de Destino")]
    public string nextSceneName;

    public string spawnPointOnReturn;

    public void Interact()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("ERROR: El nombre de la escena de destino no está configurado en el Inspector.");
            return;
        }
        Debug.Log($"[DEBUG TRANSITION]: Valor de spawnPointOnReturn al interactuar: '{spawnPointOnReturn}'"); 
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("[DEBUG TRANSITION]: GameManager.Instance es NULL. No se puede guardar el punto de spawn.");
            return;
        }

        GameManager.Instance.SetNextSpawnPoint(spawnPointOnReturn);
        
        Debug.Log($"[DEBUG TRANSITION]: Spawn Point GUARDADO. Cargando escena: {nextSceneName}");
        
        SceneManager.LoadScene(nextSceneName);
    }
}
