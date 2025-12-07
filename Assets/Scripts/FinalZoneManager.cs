using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalZoneManager : MonoBehaviour
{
    [Header("Configuración de Victoria")]
    [Tooltip("Arrastra aquí tu script VictoryScreen.")]
    public VictoryScript victoryScreen;

    private int totalEnemies;
    private int enemiesDefeated = 0;
    
    private System.Collections.Generic.List<string> defeatedEnemyIDs = new System.Collections.Generic.List<string>(); 

    public string GetDefeatedIDsString()
    {
        return string.Join(",", defeatedEnemyIDs);
    }
    
void Start()
{
    int enemiesCurrentlyAlive;
    int enemiesRemaining = 0;
    
    if (SaveManager.Instance != null && SaveManager.Instance.HasSavedGame())
    {
        string idString = SaveManager.Instance.GetSavedDefeatedIDsString();
        
        if (!string.IsNullOrEmpty(idString))
        {
            defeatedEnemyIDs = new System.Collections.Generic.List<string>(idString.Split(','));
            foreach (string id in defeatedEnemyIDs)
            {
                DestroyEnemyByID(id); 
            }
        }
        
        enemiesCurrentlyAlive = CountActiveEnemies();
        
        enemiesDefeated = defeatedEnemyIDs.Count; 
        totalEnemies = enemiesCurrentlyAlive + enemiesDefeated;
        
        Debug.Log($"[LOAD] Sincronización completa. Vivos={enemiesCurrentlyAlive}, Derrotados={enemiesDefeated}, Total={totalEnemies}");

    }
    else 
    {
        enemiesCurrentlyAlive = CountActiveEnemies(); 
        
        enemiesDefeated = 0;
        totalEnemies = enemiesCurrentlyAlive;
        Debug.Log($"[START] Primera vez. Total de enemigos iniciales: {totalEnemies}");
    }

    enemiesRemaining = totalEnemies - enemiesDefeated;

    if (GameManager.Instance != null)
    {
        GameManager.Instance.UpdateEnemyCount(enemiesRemaining); 
    }
    
    Debug.Log($"[FINAL ZONE] Total de enemigos iniciales: {totalEnemies}");
    
    if (enemiesRemaining <= 0 && totalEnemies > 0) 
    {
        Debug.LogWarning("[FINAL ZONE] Se cargó con 0 enemigos restantes. Activando victoria.");
        if (victoryScreen != null) 
        {
            victoryScreen.ShowVictory();
        }
    }
}

private int CountActiveEnemies()
{
    EnemiesIA[] enemiesAliveScripts = FindObjectsOfType<EnemiesIA>(); 
    int count = 0;
    foreach (var enemy in enemiesAliveScripts)
    {
        if (enemy.gameObject.activeInHierarchy)
        {
            count++;
        }
    }
    return count;
}
    public int GetEnemiesDefeated()
    {
        return enemiesDefeated;
    }
    
    private void DestroyEnemyByID(string id)
    {
        PersistentID[] pids = FindObjectsOfType<PersistentID>();

        foreach (PersistentID pid in pids)
        {
            if (pid.GetID() == id)
            {
                pid.gameObject.SetActive(false); 
                return; 
            }
        }
    }
    
    public void RegisterDefeat(string defeatedID)
    {
        if (!defeatedEnemyIDs.Contains(defeatedID))
        {
            defeatedEnemyIDs.Add(defeatedID);
            enemiesDefeated++;
        }
    
        int enemiesRemaining = totalEnemies - enemiesDefeated;
        Debug.Log($"Enemigos restantes: {enemiesRemaining}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnemyCount(enemiesRemaining);
        }

        if (enemiesRemaining <= 0)
        {
            if (victoryScreen != null)
            {
                victoryScreen.ShowVictory();
            }
        }
    }
    
    public void EnemyWasDefeated()
    {
        enemiesDefeated++;
        int enemiesRemaining = totalEnemies - enemiesDefeated;
        Debug.Log($"Enemigos restantes: {enemiesRemaining}");
        PersistentID dyingEnemyID = GetComponent<PersistentID>();
    
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnemyCount(enemiesRemaining);
        }

        if (enemiesRemaining <= 0)
        {
            if (victoryScreen != null)
            {
                victoryScreen.ShowVictory();
            }
        }
    }
}
