using UnityEngine;
using System;
using DefaultNamespace;

public class HouseKeyController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform keyModel;
    public string interactionTag = "Player";
    
    public void Interact()
    {
        var status = GlobalEvents.HouseKeyOn;
        EventManager.Invoke(status); // notificaion de que el switch esta activo
        keyModel.gameObject.SetActive(false);
    }
}
