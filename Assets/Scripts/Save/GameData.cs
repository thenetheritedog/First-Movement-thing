using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float currentHealth;
    public SerializableDictionary<string, bool> bossKilled;
    public SerializableDictionary<string, string> inventoryAndItemIDs;

    public GameData()
    {
        currentHealth = 100;
        bossKilled = new SerializableDictionary<string, bool>();
        inventoryAndItemIDs = new SerializableDictionary<string, string>();
    }
}
