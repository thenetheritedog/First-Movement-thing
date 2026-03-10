using UnityEngine;

public class ItemPickUpManager : MonoBehaviour
{
    [SerializeField] private LayerMask ground;
    public Collider groundHitCollider;
    public Rigidbody spawnRigidbody;
    public ItemSlotManager itemToPickup;

    private void OnCollisionEnter(Collision collision)
    {
        spawnRigidbody.isKinematic = false;
    }
}
