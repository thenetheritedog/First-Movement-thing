using NUnit.Framework;
using Unity.Collections;
using UnityEditor.Rendering;
using UnityEngine;


public class InventoryManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private PlayerManager playerManager;
    public GameObject itemPrefab;
    public ItemSlotManager currentWeapon;
    [SerializeField] private string id;
    private string idOfItems;
    public ItemSlotManager lastItem;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    void Awake()
    {
    }

    
    public void SwapAnItem(ItemSlotManager itemSlotManager)
    {
        switch (itemSlotManager.itemBaseInformation.itemType)
        {
            case ("weapon"):
                currentWeapon = itemSlotManager;
                Debug.Log(currentWeapon.itemBaseInformation.itemType);
                break;
            default:
                Debug.Log("gang what the hell are you doing");
                break;
        }
    }
    public void AddItemToInventory(string fileLocation, int levelOrQuantity, string[] effects = null)
    {
        
        GameObject newItem = Instantiate(itemPrefab);
        ItemSlotManager newItemStats = newItem.GetComponent<ItemSlotManager>();
        newItemStats.itemFileLocation = fileLocation;
        newItemStats.effects = effects;
        newItemStats.level = levelOrQuantity;
        if (effects != null)
        {
            string combinedEffects = "";
            for (int i = 0; i < effects.Length; i++)
            {
                combinedEffects += (";" + effects[i]);
            }
            newItemStats.combinedAttributes = new string(fileLocation + ";" + levelOrQuantity + combinedEffects);
        }
        else
        {
            newItemStats.combinedAttributes = new string(fileLocation + ";" + levelOrQuantity);
        }
        newItem.transform.SetParent(playerManager.menu.transform, true);
        newItemStats.playerManager = playerManager;
        lastItem = newItemStats;
        
    }
     public void SaveData(ref GameData data)
     {
        ItemSlotManager[] allItems = FindObjectsOfType<ItemSlotManager>();
        idOfItems = "";
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i].combinedAttributes != string.Empty)
                idOfItems += "," + allItems[i].combinedAttributes;
        }
        if (idOfItems != "") 
        {
            idOfItems = idOfItems.Remove(0, 1);
        }
        if (data.inventoryAndItemIDs.ContainsKey(id))
        {
            data.inventoryAndItemIDs.Remove(id);
        }
        data.inventoryAndItemIDs.Add(id, idOfItems);
    }
    public void LoadData(GameData data)
    {
        string idOfItems;
        data.inventoryAndItemIDs.TryGetValue(id, out idOfItems);
        if (idOfItems == null)
        {
            return;
        }
        string[] itemInformationGiven = idOfItems.Split(',');
        if (itemInformationGiven[0] == "")
            return;
        foreach(string x in itemInformationGiven)
        {
            var itemValuesUncombined = x.Split(";");
            if (itemValuesUncombined.Length < 2)
            {
                AddItemToInventory(itemValuesUncombined[0], int.Parse(itemValuesUncombined[1]));
                return;
            }
            string[] effects = new string[itemValuesUncombined.Length - 2];
            for (int j = 0; j < effects.Length; j++)
            {
                effects[j] = itemValuesUncombined[j + 2];
            }
            AddItemToInventory(itemValuesUncombined[0], int.Parse(itemValuesUncombined[1]), effects);
        }
    }
}
