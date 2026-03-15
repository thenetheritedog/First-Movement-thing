using UnityEngine;

public class ItemPickUpManager : MonoBehaviour
{
    [SerializeField] private LayerMask ground;
    public Collider groundHitCollider;
    public Rigidbody spawnRigidbody;
    public ItemSlotManager itemToPickup;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CanvasGroup itemView;
    private void OnCollisionEnter(Collision collision)
    {
        spawnRigidbody.isKinematic = false;
    }
    private void Update()
    {
        if (inventoryManager.itemOver == this.gameObject) 
        {
            itemView.alpha = Mathf.Lerp(itemView.alpha, 1, 5 * Time.deltaTime);
        }
        else
        {
            itemView.alpha = Mathf.Lerp(itemView.alpha, 0, 5 * Time.deltaTime);
        }
    }
    private void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        itemView.alpha = 0;
    }
}
