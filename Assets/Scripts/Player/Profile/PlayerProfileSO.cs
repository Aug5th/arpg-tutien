using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player Profile", menuName = "Game Systems/Player Profile")]
public class PlayerProfileSO : ScriptableObject
{
    [Header("Runtime Data")]
    [NonSerialized] public InternalArtState activeInternalArt;
    [NonSerialized] public Dictionary<EquipmentType, EquipmentItemDefinition> currentEquipments;

    // Events
    public event Action OnProfileChanged;
    public event Action<float> OnXpChanged;
    public event Action<int> OnLevelUp;
    public event Action OnEquipmentChanged;

    private void OnEnable()
    {
        // Reset Runtime Data
        currentEquipments = new Dictionary<EquipmentType, EquipmentItemDefinition>();
    }

    public void EquipInternalArt(InternalArtItemDefinition artDef)
    {
        activeInternalArt = new InternalArtState(artDef);
        Debug.Log($"[Profile] Initialized Art: {artDef.itemName}");
    }

    public void AddExperience(int amount)
    {
        if (activeInternalArt == null) return;

        bool leveledUp = activeInternalArt.AddExp(amount);

        // Update UI Progress Bar
        float progress = (float)activeInternalArt.currentXP / activeInternalArt.maxXP;
        OnXpChanged?.Invoke(progress);

        if (leveledUp)
        {
            OnLevelUp?.Invoke(activeInternalArt.currentLevel);
            OnProfileChanged?.Invoke(); // Trigger stats recalculation
        }
    }

    public void EquipItem(EquipmentItemDefinition item)
    {
        if (item == null) return;

        // If it's an Internal Art, we treat it differently than standard equipment slots
        if (item is InternalArtItemDefinition artDef)
        {
            EquipInternalArt(artDef);
        }

        if (currentEquipments.ContainsKey(item.equipmentType))
        {
            UnEquipItem(item.equipmentType);
        }

        currentEquipments[item.equipmentType] = item;
        OnEquipmentChanged?.Invoke();
        OnProfileChanged?.Invoke();
    }

    public void UnEquipItem(EquipmentType slot)
    {
        if (currentEquipments.ContainsKey(slot))
        {
            currentEquipments.Remove(slot);
            OnEquipmentChanged?.Invoke();
            OnProfileChanged?.Invoke();
        }
    }

    public void FillBonusStats(ref StatsContainer container)
    {
        // 1. Add stats from Internal Art (Level-based)
        if (activeInternalArt != null)
        {
            container.AddFrom(activeInternalArt.GetLevelUpStatsBonus());
        }

        // 2. Add stats from Equipments
        foreach (var equipment in currentEquipments)
        { 
            if (equipment.Value != null)
            {
                Debug.Log("Applying equipment stats from: " + equipment.Value.bonusStats.GetStat(StatType.Attack));
                container.AddFrom(equipment.Value.bonusStats);
            }
        }
    }
}