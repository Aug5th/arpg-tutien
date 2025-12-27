using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Item/Equipment Item")]
public class EquipmentItemDefinition : ItemDefinition
{
    [Header("Equipment Stats")]
    public EquipmentType equipmentType;
    public StatsContainer bonusStats;
}