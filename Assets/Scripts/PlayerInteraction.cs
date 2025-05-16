using System;
using Photon.Pun;
using UnityEngine;

public class PlayerInteraction : MonoBehaviourPun
{
    private const string RPC_TRY_PICKUP = "RPC_TryPickup";
    private const string RPC_Try_Drop = "RPC_TryDrop";
    private const string RPC_Try_Give = "RPC_TryGive";
    
    
    public Transform holdPoint;
    public float interactRange = 2f;
    private PickupBox heldBox;
    private PickupBox nearbyBox;
    private PlayerInteraction nearbyPlayer;
    private int currentScore = 0;
    
    public void HoldBox(PickupBox box) => heldBox = box;
    public void DropBox() => heldBox = null;
    public bool IsHolding(PickupBox box) => heldBox == box;
    public int CurrentScore => currentScore;
    public bool IsHoldingAnyBox => heldBox != null;
    
    private void Start()
    {
        if (photonView.IsMine) // only update ui for client character
        {
            GameManager.Instance.RegisterPlayer(this);
        }
    }

    public void OnEnterPickupZone(PickupBox box)
    {
        if (heldBox is null && box.Status != BoxStatus.PickedUp)
        {
            nearbyBox = box;
            if (photonView.IsMine) // only update ui for client character
            {
                UIManager.Instance.ShowBoxMenu(true);
            }
        }
    }

    public void OnExitPickupZone(PickupBox box)
    {
        if (IsHolding(box)) return;
        nearbyBox = null;
        if (photonView.IsMine) // only update ui for client character
        {
            UIManager.Instance.HideEverything();
        }
    }
    
    public void OnEnterGiftZone(PlayerInteraction player)
    {
        if (!IsHoldingAnyBox) return;

        if (nearbyPlayer is null)
        {
            nearbyPlayer = player;
            
            if (photonView.IsMine) // only update ui for client character
            {
                UIManager.Instance.ShowGiveButton(true);
            }
        }
    }

    public void OnExitGiftZone(PlayerInteraction player) 
    {
        if (!IsHoldingAnyBox) return;

        if (nearbyPlayer != player) return;

        nearbyPlayer = null;
        if (photonView.IsMine) // only update ui for client character
        {
            UIManager.Instance.ShowGiveButton(false);
        }
    }
    public void TryPickup()
    {
       photonView.RPC(RPC_TRY_PICKUP, RpcTarget.All);
    }

    public void TryDrop()
    {
        photonView.RPC(RPC_Try_Drop, RpcTarget.All);
    }

    public void TryGive()
    {
        photonView.RPC(RPC_Try_Give, RpcTarget.All);
    }

    public void OnReceive()
    {
        if (photonView.IsMine) // only update ui for client character
        {
            UIManager.Instance.ShowBoxMenu(true);
            UIManager.Instance.ShowButtonsForState(true);
        }
    }
    
    public void AddCoins(int amount)
    {
        currentScore += amount;
        GameManager.Instance.UpdatePlayerScore(this);        
        
        if (photonView.IsMine) // only update ui for client character
        {
            UIManager.Instance.UpdateScore(currentScore);
        }
        
        
    }

    [PunRPC]
    private void RPC_TryPickup()
    {
        if (nearbyBox != null)
        {
            nearbyBox.Interact(this);
            if (photonView.IsMine) // only update ui for client character
            {
                UIManager.Instance.ShowButtonsForState(true);
            }
            //UI?
        }
    }

    [PunRPC]
    private void RPC_TryDrop()
    {
        if (heldBox != null)
        {
            heldBox.Interact(this);
            if (photonView.IsMine) // only update ui for client character
            {
                UIManager.Instance.ShowButtonsForState(false);
                UIManager.Instance.HideEverything();
            }
          
        }
    }
    
    [PunRPC]
    private void RPC_TryGive()
    {
        if (heldBox != null)
        {
            var box = heldBox;
            box.Interact(this);
            box.Interact(nearbyPlayer);
            
            nearbyPlayer.OnReceive();
            
            if (photonView.IsMine) // only update ui for client character
            {
                UIManager.Instance.ShowButtonsForState(false);
                UIManager.Instance.ShowGiveButton(false);
                UIManager.Instance.HideEverything();
            }
          
        }
    }

    
}