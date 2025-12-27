using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Item/Internal Art Item")]
public class InternalArtItemDefinition : EquipmentItemDefinition
{
    [Header("Internal Art Info")]
    public int maxLevel = 1;
    [Header("Configs per Level")]
    public List<StatsContainer> bonusStatByLevel = new List<StatsContainer>();
    public List<int> exeReqToUpNextLevel = new List<int>();

    private void OnValidate()
    {
        equipmentType = EquipmentType.InternalArt;

        if (maxLevel < 1) maxLevel = 1;

        if (bonusStatByLevel.Count != maxLevel)
        {
            while (bonusStatByLevel.Count < maxLevel)
            {
                bonusStatByLevel.Add(new StatsContainer());
            }
            while (bonusStatByLevel.Count > maxLevel)
            {
                bonusStatByLevel.RemoveAt(bonusStatByLevel.Count - 1);
            }
        }

        if (exeReqToUpNextLevel.Count != maxLevel)
        {
            while (exeReqToUpNextLevel.Count < maxLevel)
            {
                exeReqToUpNextLevel.Add(100);
            }
            while (exeReqToUpNextLevel.Count > maxLevel)
            {
                exeReqToUpNextLevel.RemoveAt(exeReqToUpNextLevel.Count - 1);
            }
        }
    }
}