using UnityEngine;

/// <summary>
/// Singleton que hace persistente el GameObject de música entre escenas.
/// Evita que se duplique cuando se recarga una escena.
/// </summary>
public class MusicManager : MonoBehaviour
{
    // Singleton estático
    private static MusicManager instance;

    void Awake()
    {
        // Si ya existe una instancia de MusicManager...
        if (instance != null)
        {
            // Destruir este duplicado
            Destroy(gameObject);
            return;
        }

        // Esta es la primera instancia
        instance = this;

        // Hacer persistente entre escenas
        DontDestroyOnLoad(gameObject);
    }
}
