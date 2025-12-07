using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const string SAVE_SCENE_KEY = "LastScene";
    private const string SAVE_HEALTH_KEY = "PlayerHealth";

    private const string SAVE_ENEMIES_DEFEATED_KEY = "EnemiesDefeated";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DeleteSavedGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private const string SAVE_DEFEATED_IDS_KEY = "DefeatedIDs";
    public void SaveGameData()
    {
        
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SAVE_SCENE_KEY, currentScene);
        
        FinalZoneManager finalZoneManager = FindObjectOfType<FinalZoneManager>();
        if (finalZoneManager != null)
        {
            int defeatedCount = finalZoneManager.GetEnemiesDefeated(); 
            PlayerPrefs.SetInt(SAVE_ENEMIES_DEFEATED_KEY, defeatedCount);
        
            string defeatedIDs = finalZoneManager.GetDefeatedIDsString(); // <-- Nueva función requerida
            PlayerPrefs.SetString(SAVE_DEFEATED_IDS_KEY, defeatedIDs);
            Debug.Log($"[SAVE] IDs de enemigos muertos: {defeatedIDs}");
        }
        
        SistemaDeVida playerHealth = FindObjectOfType<SistemaDeVida>();
        if (playerHealth != null)
        {
            PlayerPrefs.SetFloat(SAVE_HEALTH_KEY, playerHealth.currentHealth); 
        }

        PlayerPrefs.Save(); 
        Debug.Log($"[SAVE] Juego guardado. Escena: {currentScene}");
    }

    public string GetSavedDefeatedIDsString()
    {
        return PlayerPrefs.GetString(SAVE_DEFEATED_IDS_KEY, "");
    }
    
    public void LoadGameData()
    {
        if (!PlayerPrefs.HasKey(SAVE_SCENE_KEY))
        {
            Debug.LogWarning("[LOAD] No hay datos de guardado para cargar.");
            return;
        }

        string sceneToLoad = PlayerPrefs.GetString(SAVE_SCENE_KEY);
        SceneManager.LoadScene(sceneToLoad);
        
        Debug.Log($"[LOAD] Escena '{sceneToLoad}' cargada. Esperando aplicar datos...");
    }
    
    public void SaveEnemyCount(int count)
    {
        PlayerPrefs.SetInt(SAVE_ENEMIES_DEFEATED_KEY, count);
        Debug.Log($"[SAVE] Enemigos derrotados guardados: {count}");
    }
    
    public int GetSavedEnemyCount()
    {
        return PlayerPrefs.GetInt(SAVE_ENEMIES_DEFEATED_KEY, 0); 
    }
    
    public float GetSavedHealth()
    {
        return PlayerPrefs.GetFloat(SAVE_HEALTH_KEY, 100f); 
    }

    public bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SAVE_SCENE_KEY);
    }
    
    public void DeleteSavedGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("[SAVE] Todos los datos de guardado han sido eliminados.");
    }
}
