using UnityEngine;

public class CmeraRecoil : MonoBehaviour
{
    [Header("Ajustes de Recoil")]
    public float recoilX = 2f;
    public float recoilY = 0.5f;
    
[Header("Velocidad")]
public float recoilSnappiness = 8f;
public float recoilReturnSpeed = 4f; 

[HideInInspector] public Vector3 currentRecoil = Vector3.zero; 
private Vector3 targetRecoil = Vector3.zero;

void LateUpdate()
{
    // Solo calculamos el retroceso, NO lo aplicamos a transform.localRotation
    targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    currentRecoil = Vector3.Slerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSnappiness);

    // ELIMINAR ESTA LÍNEA:
    // transform.localRotation = Quaternion.Euler(currentRecoil); 
}

public void GenerateRecoil()
{
    targetRecoil += new Vector3(recoilX, Random.Range(-recoilY, recoilY), 0f);
}
}
