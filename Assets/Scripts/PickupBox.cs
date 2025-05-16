using UnityEngine;

public class PickupBox : BoxBase
{
    
    private Transform originalParent;
    
    [SerializeField]private Rigidbody originalRigidBody;

    private void Awake()
    {
        originalParent = transform.parent;
    }
    
    public override void Interact(PlayerInteraction interactor)
    {
        if (Status == BoxStatus.Idle)
        {
            PickUp(interactor);
        }
        else if (Status == BoxStatus.PickedUp && interactor.IsHolding(this))
        {
            Drop(interactor);
        }
    }
    
    private void PickUp(PlayerInteraction interactor)
    {
        Status = BoxStatus.PickedUp;
        transform.SetParent(interactor.holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        originalRigidBody.isKinematic = true;
        interactor.HoldBox(this);
    }

    private void Drop(PlayerInteraction interactor)
    {
        Status = BoxStatus.Idle;
        transform.SetParent(originalParent);
        originalRigidBody.isKinematic = false;
        interactor.DropBox();
    }
    
    
}