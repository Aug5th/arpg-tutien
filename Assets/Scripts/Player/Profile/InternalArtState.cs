using System;
using UnityEngine;

[Serializable]
public class InternalArtState
{
    // Reference to the static data definition
    public InternalArtItemDefinition definition;
    public int currentLevel = 1;
    public int currentXP = 0;
    public int maxXP;

    public InternalArtState(InternalArtItemDefinition def)
    {
        definition = def;
        currentLevel = 1;
        currentXP = 0;
        RecalculateMaxXP();
    }

    /// <summary>
    /// Updates the maxXP required for the next level based on the definition list.
    /// </summary>
    private void RecalculateMaxXP()
    {
        if (definition != null && definition.exeReqToUpNextLevel.Count > 0 && currentLevel < definition.maxLevel)
        {
            maxXP = definition.GetExpRequirementForNextLevel(currentLevel);
        }
    }

    /// <summary>
    /// Adds experience points and handles level-up logic.
    /// </summary>
    /// <param name="amount">Amount of XP to add</param>
    /// <returns>True if the item leveled up, false otherwise</returns>
    public bool AddExp(int amount)
    {
        if (definition == null || currentLevel >= definition.maxLevel) return false;

        currentXP += amount;
        bool leveledUp = false;

        // Loop to handle multiple level-ups if XP amount is large
        while (currentXP >= maxXP && currentLevel < definition.maxLevel)
        {
            currentXP -= maxXP;
            currentLevel++;
            RecalculateMaxXP();
            leveledUp = true;
            Debug.Log($"<color=yellow>[Internal Art] {definition.itemName} leveled up to {currentLevel}!</color>");
        }

        // Reset XP if max level is reached
        if (currentLevel >= definition.maxLevel)
        {
            currentXP = 0;
        }

        return leveledUp;
    }

    /// <summary>
    /// Retrieves the specific StatContainer assigned to the current level.
    /// </summary>
    /// <returns>StatsContainer for the current level</returns>
    public StatsContainer GetLevelUpStatsBonus()
    {
        return definition.GetBonusStatsAtLevel(currentLevel);
    }
}