using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerInteraction : MonoBehaviourPunCallbacks
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
            
            if(GameManager.Instance.PlayerScores.TryGetValue(photonView.OwnerActorNr, out var score))
                currentScore = score;
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
                UIManager.Instance.ShowButtonsForState(false);
            }
        }
    }

    public void OnExitPickupZone(PickupBox box)
    {
        if (IsHolding(box)) return;
        nearbyBox = null;
        if (photonView.IsMine) // only update ui for client character
        {
            UIManager.Instance.ShowBoxMenu(false);
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
                UIManager.Instance.ShowBoxMenu(true);
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
       photonView.RPC(RPC_TRY_PICKUP, RpcTarget.AllBuffered);
    }

    public void TryDrop()
    {
        photonView.RPC(RPC_Try_Drop, RpcTarget.AllBuffered);
    }

    public void TryGive()
    {
        photonView.RPC(RPC_Try_Give, RpcTarget.AllBuffered);
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
                UIManager.Instance.ShowBoxMenu(false);
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
                UIManager.Instance.ShowBoxMenu(false);
            }
          
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        
        if (GameManager.Instance.MatchState == MatchStateEnum.Ended) return;
        
        if (otherPlayer.ActorNumber == photonView.OwnerActorNr)
        {
            // If a player holding a box left/dc, drop their box
            TryDrop();
        }
    }
}