using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Profile", menuName = "Game System/Player/Player Profile")]
public class PlayerProfileSO : ScriptableObject
{
    [Header("Level Stats")]
    public int startingLevel = 1;
    public int baseMaxXP = 150;


    [Header("Runtime Stats")]
    [NonSerialized] public int currentLevel;
    [NonSerialized] public int currentXP;
    [NonSerialized] public int maxXP;

    public event Action<float> OnXpChanged;
    public event Action<int> OnLevelUp;

    private void OnEnable()
    {
        currentLevel = startingLevel;
        currentXP = 0;
        maxXP = baseMaxXP;
        
        // Recalculate maxXP dựa trên starting level nếu cần
        for (int i = 1; i < currentLevel; i++) 
        {
            CalculateMaxXP(); 
        }
    }

    private void CalculateMaxXP()
    {
        maxXP = Mathf.RoundToInt(maxXP * 1.2f);
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;
        Debug.Log($"<color=green>[ProfileSO] Received {amount} XP.</color>");

        while (currentXP >= maxXP)
        {
            LevelUp();
        }

        float xpProgress = (float)currentXP / maxXP;
        OnXpChanged?.Invoke(xpProgress);
    }

    private void LevelUp()
    {
        currentXP -= maxXP;
        currentLevel++;
        CalculateMaxXP();
        
        Debug.Log($"<color=red>[ProfileSO] LEVEL UP! Current Level: {currentLevel}</color>");
        OnLevelUp?.Invoke(currentLevel);
    }
}
