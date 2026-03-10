using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public string name;
    public string description;
    public string itemType;
    public ItemSlotManager itemSlotManager;
    public Collider weaponCollider;
    public Collider pickUpCollider;
    public MeshRenderer weaponMaterial;
    public bool pickUpItem = false;
    [SerializeField] private LayerMask player;

    private void OnCollisionEnter(UnityEngine.Collision other)
    {
        GameObject playerObj = other.gameObject;
        if (playerObj.layer != player && pickUpItem)
            return;
        InventoryManager playerInventory = playerObj.GetComponent<InventoryManager>();
        playerInventory.AddItemToInventory(itemSlotManager.itemFileLocation, itemSlotManager.level, itemSlotManager.effects);
    }
}
