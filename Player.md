```mermaid
classDiagram
    %% Core Data Layer
    class PlayerProfileSO {
        +InternalArtState activeInternalArt
        +Dictionary currentEquipments
        +EquipItem()
        +AddExperience()
        +FillBonusStats()
    }

    class InternalArtState {
        +int currentLevel
        +int currentXP
        +AddExp()
        +GetLevelUpStatsBonus()
    }

    %% Runtime Bridge Layer
    class PlayerStatsLinker {
        -PlayerProfileSO playerProfile
        -UnitStats unitStats
        -UpdatePlayerStats()
    }

    %% Runtime Core Layer
    class UnitStats {
        -StatsContainer finalStats
        +RecalculateFinalStats()
        +GetStat()
        <<Event>> OnStatsChanged
    }

    %% Functional Components (Consumers)
    class Health {
        +int currentHealth
        +int maxHealth
        +TakeDamage()
        -UpdateHealthFromStats()
    }

    class Movement {
        +float moveSpeed
        -Rigidbody2D rb
        -HandleMovement()
        -UpdateStats()
    }

    class PlayerCombat {
        +int damage
        +float attackSpeed
        -AttackLoop()
        -UpdateStats()
    }

    %% Relationships
    PlayerProfileSO *-- InternalArtState : Contains
    PlayerStatsLinker --> PlayerProfileSO : Listens & Reads
    PlayerStatsLinker --> UnitStats : Calculates & Writes
    
    UnitStats --> Health : Updates MaxHP
    Movement --> UnitStats : Reads MoveSpeed
    PlayerCombat --> UnitStats : Reads "Attack/AtkSpeed"
    
    PlayerCombat --> Movement : Controls "(Stop/Move)"
    Health ..|> IDamagable : Implements


sequenceDiagram
    participant GameLogic
    participant Profile as PlayerProfileSO
    participant Art as InternalArtState
    participant Linker as PlayerStatsLinker
    participant Stats as UnitStats
    participant Components as "Combat/Movement/Health"

    GameLogic->>Profile: AddExperience(100)
    activate Profile
    
    Profile->>Art: AddExp(100)
    activate Art
    Art-->>Profile: return true (Leveled Up!)
    deactivate Art

    Note over Profile: Fire OnProfileChanged Event
    Profile->>Linker: Event Triggered
    deactivate Profile

    activate Linker
    Linker->>Profile: FillBonusStats(ref container)
    Linker->>Stats: RecalculateFinalStats(equipmentBonus)
    deactivate Linker

    activate Stats
    Stats->>Stats: Calculate Base + Bonus
    Note over Stats: Fire OnStatsChanged Event
    
    par Update Components
        Stats-->>Components: Event Triggered
        Components->>Stats: GetStat(Attack/HP/Speed)
        Components->>Components: Update local variables
    end
    deactivate Stats
```