using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[DEBUG SPAWN]: Iniciando el script PlayerSpawn.");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[DEBUG SPAWN]: GameManager.Instance es NULL. El sistema de spawn fallará.");
            return;
        }

        string spawnID = GameManager.Instance.ConsumeNextSpawnPoint();
        Debug.Log($"[DEBUG SPAWN]: ID de spawn consumido: {spawnID}");

        if (spawnID != "DefaultSpawn")
        {
            GameObject spawnPoint = GameObject.Find(spawnID);
            
            if (spawnPoint != null)
            {
                Debug.Log($"[DEBUG SPAWN]: Punto de spawn '{spawnID}' ENCONTRADO en posición {spawnPoint.transform.position}.");

                GameObject player = GameObject.FindGameObjectWithTag("Player"); 
                
                if (player != null)
                {
                    Debug.Log("[DEBUG SPAWN]: Objeto Jugador ENCONTRADO.");
                    
                    CharacterController cc = player.GetComponent<CharacterController>();

                    if (cc != null)
                    {
                        Debug.Log("[DEBUG SPAWN]: CharacterController ENCONTRADO. Intentando teletransporte forzado.");
                        
                        cc.enabled = false;
                        
                        player.transform.position = spawnPoint.transform.position;
                        player.transform.rotation = spawnPoint.transform.rotation;
                        
                        cc.enabled = true;
                        
                        Debug.Log($"[DEBUG SPAWN]: Teletransporte COMPLETADO. Nueva Posición: {player.transform.position}");
                    }
                    else
                    {
                        Debug.LogWarning("[DEBUG SPAWN]: No se encontró CharacterController. Moviendo Transform directamente.");
                        player.transform.position = spawnPoint.transform.position;
                        player.transform.rotation = spawnPoint.transform.rotation;
                    }

                    Debug.Log($"Jugador aparecido en el punto: {spawnID}");
                }
                else
                {
                    Debug.LogError("[DEBUG SPAWN]: ¡FALLO! Objeto Jugador NO encontrado (Tag 'Player' incorrecto o ausente).");
                }
            }
            else
            {
                Debug.LogError($"[DEBUG SPAWN]: ¡FALLO! Objeto de spawn con ID '{spawnID}' NO encontrado. Revisa el nombre.");
            }
        }
        else
        {
            Debug.Log("[DEBUG SPAWN]: Usando punto de spawn por defecto. No se requiere movimiento.");
        }
    }
}