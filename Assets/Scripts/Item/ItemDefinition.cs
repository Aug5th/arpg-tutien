using UnityEngine;

public class ItemDefinition : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public ItemType itemType;
    public ItemRarity itemRarity;      
    public string itemName; 
    public Sprite icon; 
    
    [TextArea]
    public string description; 

    public virtual void UseItem()
    {
        Debug.Log($"Using item: {itemName}");
    }

    public virtual void InspectItem()
    {
        Debug.Log($"Inspecting item: {itemName}\nDescription: {description}");
    }
}