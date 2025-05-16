using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] PlayerInteraction _playerInteraction;

    private void OnTriggerEnter(Collider other)
    {
        // Can't Recieve a box while holding one currently.
        if (_playerInteraction.IsHoldingAnyBox)
            return;
        
        if (other.CompareTag("Player"))
        {
            PlayerInteraction otherInteraction = other.GetComponent<PlayerInteraction>();
            if (otherInteraction.IsHoldingAnyBox)
            {
                otherInteraction.OnEnterGiftZone(_playerInteraction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_playerInteraction.IsHoldingAnyBox)
            return;
        
        if (other.CompareTag("Player"))
        {
            PlayerInteraction otherInteraction = other.GetComponent<PlayerInteraction>();
            if (otherInteraction.IsHoldingAnyBox)
            {
                otherInteraction.OnExitGiftZone(_playerInteraction);
            }
        }
    }
}
