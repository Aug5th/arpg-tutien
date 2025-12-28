using UnityEngine;

[RequireComponent(typeof(UnitStats))]
public class PlayerStatsLinker : MonoBehaviour
{
    [SerializeField] private PlayerProfileSO playerProfile;
    private UnitStats unitStats;

    [Header("Starting Configuration (For Testing)")]
    public InternalArtItemDefinition startingInternalArt;

    void Awake()
    {
        unitStats = GetComponent<UnitStats>();
    }

    void Start()
    {
        UpdatePlayerStats();
        /* 
        Equip starting Internal Art for testing purposes
        - Internal Art is a type of EquipmentItemDefinition => apply item bonus stats
        - Internal Art start at level 1 by default => apply level 1 (index 0) bonus stats
        */
        if (startingInternalArt != null && playerProfile != null)
        {
            playerProfile.EquipItem(startingInternalArt);
        }
    }

    void OnEnable()
    {
        if (playerProfile != null)
        {
            // Listen for changes from Profile (Level up, Equip)
            playerProfile.OnProfileChanged += UpdatePlayerStats;
        }
    }

    void OnDisable()
    {
        if (playerProfile != null)
        {
            playerProfile.OnProfileChanged -= UpdatePlayerStats;
        }
    }

    private void UpdatePlayerStats()
    {
        if (unitStats == null || playerProfile == null) return;

        // Create container for total bonus (Equipment + Internal Art)
        StatsContainer bonusContainer = new StatsContainer();
        
        // Ask Profile to fill data into this container
        playerProfile.FillBonusStats(ref bonusContainer);

        // Call UnitStats to recalculate: Base + This Bonus
        // (2nd parameter is temporary Buff, leave null or handle separately)
        unitStats.RecalculateFinalStats(equipmentBonus: bonusContainer);
        
        Debug.Log("[Linker] Player stats updated from Profile!");
    }
}