using Photon.Pun;
using UnityEngine;
    public  enum BoxStatus
    {
        Idle,
        PickedUp
    }

public abstract  class BoxBase : MonoBehaviourPun,IInteractable
{
    
    [field: SerializeField] public BoxStatus Status { get; protected set; } = BoxStatus.Idle;
    
    public abstract void Interact(PlayerInteraction interactor);
    
    
}
