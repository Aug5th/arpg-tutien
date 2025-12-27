using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Item/Consumable Item")]
public class ConsumableItemDefinition : ItemDefinition
{
    [Header("Consumable Stats")]
    public ConsumableType consumableType;
    public List<StatValue> effectStats;
    public float duration; // in seconds, 0 for instant

    public override void UseItem()
    {
        Debug.Log($"Consuming item: {itemName} with effects: {effectStats}");
        // Implement consumption logic here
    }
}