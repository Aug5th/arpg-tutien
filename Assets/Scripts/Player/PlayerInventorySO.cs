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
    [Header("Currency")]
    [NonSerialized] public int currentGems = 0;

    private Dictionary<ItemType, List<InventorySlot>> _inventoryTabs;
    public Dictionary<ItemType, List<InventorySlot>> InventoryTabs => _inventoryTabs;
    
    private Dictionary<ItemType, int> _tabLimits;
    public Dictionary<ItemType, int> TabLimits => _tabLimits;

    public event Action OnInventoryChanged;
    public event Action<int> OnGemChanged;

    void OnEnable() => Initialize();

    private void Initialize()
    {
        currentGems = 0;

        // Initialize Lists for each Tab
        _inventoryTabs = new Dictionary<ItemType, List<InventorySlot>>();
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            _inventoryTabs[type] = new List<InventorySlot>();
        }

        // Initialize limits for each Tab (Very clean in code)
        _tabLimits = new Dictionary<ItemType, int>
        {
            { ItemType.Equipment, 20 },
            { ItemType.InternalArt, 5 },
            { ItemType.Consumable, 15 },
            { ItemType.QuestItem, 10},
            { ItemType.Material, 50 }
        };
    }

    public void AddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return;

        // Get the correct List for that Tab - O(1) very fast
        var targetTab = _inventoryTabs[item.itemType];

        // Check if item exists to stack
        var existingSlot = targetTab.Find(s => s.itemDefinition == item);

        if (existingSlot != null)
        {
            existingSlot.AddAmount(amount);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            // Check limit of this Tab
            if (targetTab.Count < _tabLimits[item.itemType])
            {
                targetTab.Add(new InventorySlot(item, amount));
                OnInventoryChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning($"[Inventory] Tab {item.itemType} is full ({_tabLimits[item.itemType]} slots)!");
            }
        }
    }

    public List<InventorySlot> GetListByTab(ItemType type)
    {
        return _inventoryTabs.ContainsKey(type) ? _inventoryTabs[type] : new List<InventorySlot>();
    }

    public void AddGems(int amount)
    {
        currentGems += amount;
        Debug.Log($"<color=cyan>[InventorySO] Received {amount} Gems. Total: {currentGems}</color>");
        OnGemChanged?.Invoke(currentGems);
    }
}