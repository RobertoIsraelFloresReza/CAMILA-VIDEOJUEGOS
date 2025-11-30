using UnityEngine;
using System;

public class HouseKeyController : MonoBehaviour
{
    [SerializeField] private Transform keyModel;
    public string interactionTag = "Player";
    
    private void OnMouseDown()
    {
        var status = GlobalEvents.HouseKeyOn;
        EventManager.Invoke(status); // notificaion de que el switch esta activo
        keyModel.gameObject.SetActive(false);
    }
}
