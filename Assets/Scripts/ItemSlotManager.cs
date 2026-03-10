using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ItemSlotManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public string itemFileLocation;
    public int level;
    public string[] effects;
    public TextMeshProUGUI displayName;
    public string combinedAttributes;
    public GameObject itemObject;
    public ItemManager itemBaseInformation;
    private UnityEngine.UI.Image image;

    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        itemObject = Resources.Load<GameObject>(itemFileLocation);
        if (itemObject != null)
        {
            itemBaseInformation = itemObject.GetComponent<ItemManager>();
            displayName.text = itemBaseInformation.name + ":" + level;
            
        }
    }
    void Update()
    {
        if (playerManager == null)
            return;
        if (playerManager.inventoryManager.lastItem.itemFileLocation == itemFileLocation && playerManager.inventoryManager.lastItem != this.GetComponent<ItemSlotManager>() && itemBaseInformation.itemType == "Item")
        {
            playerManager.inventoryManager.lastItem.level += level;
            playerManager.inventoryManager.lastItem.combinedAttributes = itemFileLocation + ";" + playerManager.inventoryManager.lastItem.level;
            Debug.Log("i dunno");
            Destroy(this.gameObject);
        }
    }
    public void Select()
    {
        playerManager.inventoryManager.SwapAnItem(gameObject.GetComponent<ItemSlotManager>());
    }

}
