using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemDefinition itemDefinition;
    public int quantity;

    public InventorySlot(ItemDefinition item, int amount)
    {
        this.itemDefinition = item;
        this.quantity = amount;
    }

    public void AddAmount(int amount) => quantity += amount;
}

[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Game System/Player/Player Inventory")]
public class PlayerInventorySO : ScriptableObject
{
    [Header("Runtime Data")]
    [NonSerialized] public int currentGems = 0;

    public List<InventorySlot> items = new List<InventorySlot>(); 

    public event Action OnInventoryChanged;
    public event Action<int> OnGemChanged;

    void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        currentGems = 0;
        items = new();
    }

    public void AddGems(int amount)
    {
        currentGems += amount;
        Debug.Log($"<color=cyan>[InventorySO] Received {amount} Gems. Total: {currentGems}</color>");
        OnGemChanged?.Invoke(currentGems);
    }

    public bool SpendGems(int amount)
    {
        if (currentGems >= amount)
        {
            currentGems -= amount;
            OnGemChanged?.Invoke(currentGems);
            return true;
        }
        return false;
    }

    public void AddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return;

        InventorySlot existingSlot = items.Find(slot => slot.itemDefinition == item);

        if (existingSlot != null)
        {
            existingSlot.AddAmount(amount);
            Debug.Log($"[InventorySO] Stacked item: {item.itemName} (+{amount})");
        }
        else
        {
            items.Add(new InventorySlot(item, amount));
            Debug.Log($"[InventorySO] New item: {item.itemName} (x{amount})");
        }

        OnInventoryChanged?.Invoke();
    }
}
