using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CoinInteractableObject : MonoBehaviourPun
{
    // Pun RPC Method Names
    public const string RPC_SPAWN_COIN = "SpawnCoin";
    public const string RPC_DISABLE_COIN = "DisableCoin";
    
    [Header("Coin Setting")]
    [SerializeField] private int value = 1;
    
    private bool _isActive;

    public bool IsActive => _isActive;

    [PunRPC]
    public void SpawnCoin()
    {
        _isActive = true;
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void DisableCoin()
    {
        _isActive = false;
        gameObject.SetActive(false);
    }
    public void OnInteract(PlayerInteraction interactor)
    {
        //Give points
        interactor.AddCoins(value);
        DisableCoin();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerInteraction interaction = other.GetComponent<PlayerInteraction>();
            if (interaction != null)
            {
                OnInteract(interaction);
            }
        }
    }
}
