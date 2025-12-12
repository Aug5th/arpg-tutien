using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerStats : MonoBehaviour
{
    public StatsContainer baseStats;   // set in Inspector

    // Removed [SerializeField] to prevent "ArgumentNullException: _unity_self" error when stopping the game.
    // This object is for runtime calculation only.
    private StatsContainer finalStats = new StatsContainer();

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Start()
    {
        RecalculateFinalStats(null);
    }

    public void RecalculateFinalStats(StatsContainer internalArtBonus)
    {
        finalStats.AddFrom(baseStats);
        finalStats.AddFrom(internalArtBonus);

        int newMaxHP = Mathf.Max(1, Mathf.RoundToInt(finalStats.GetStat(StatType.MaxHP)));
        health.SetMaxHealthFromStats(newMaxHP, keepPercent: true);
    }

    public float GetStat(StatType type) => finalStats.GetStat(type);
    public int Attack => Mathf.RoundToInt(GetStat(StatType.Attack));
}