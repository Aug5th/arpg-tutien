
# Player System Documentation

Tài liệu này mô tả kiến trúc tổng thể của Object **Player**, bao gồm luồng dữ liệu (Data Flow) và luồng điều khiển hành vi (Behavior Logic).

## 1. High-Level Class Diagram (Cấu trúc tổng thể)

Sơ đồ này chia hệ thống thành 3 tầng:
1.  **Data Layer:** Lưu trữ dữ liệu bền vững (Inventory, XP, Level).
2.  **Bridge Layer:** Đồng bộ dữ liệu xuống Runtime.
3.  **Runtime Behavior:** Xử lý logic game (Di chuyển, Tấn công, Input).

```mermaid
classDiagram
    %% --- LAYER 1: DATA & PROFILE (ScriptableObjects) ---
    namespace DataLayer {
        class PlayerProfileSO {
            +InternalArtState activeInternalArt
            +Dictionary currentEquipments
            +EquipItem()
            +AddExperience(int amount)
            +FillBonusStats()
            <<Event>> OnProfileChanged
        }

        class InternalArtState {
            +InternalArtItemDefinition definition
            +int currentLevel
            +int currentXP
            +AddExp()
            +GetLevelUpStatsBonus()
        }
    }

    %% --- LAYER 2: RUNTIME STATS (MonoBehaviour) ---
    namespace StatsLayer {
        class PlayerStatsLinker {
            -PlayerProfileSO playerProfile
            -UnitStats unitStats
            -UpdatePlayerStats()
        }

        class UnitStats {
            -StatsContainer finalStats
            +BaseDefinition baseDefinition
            +RecalculateFinalStats()
            +GetStat(StatType)
            <<Event>> OnStatsChanged
        }

        class Health {
            +int currentHealth
            +int maxHealth
            +TakeDamage()
            +Heal()
            <<Event>> OnDeath
        }
    }

    %% --- LAYER 3: BEHAVIOR & CONTROL (Refactored) ---
    namespace BehaviorLayer {
        class PlayerController {
            -Movement movement
            -Combat combat
            +float autoDetectRadius
            -HandleMoveInput(Vector2)
            -HandleAttackInput(IDamagable)
            -TryAutoAcquireTarget()
        }

        class Combat {
            +int damage
            +float attackSpeed
            +LayerMask validTargetLayers
            +SetTarget(IDamagable)
            +CancelTarget()
            -PerformAttack()
        }

        class Movement {
            +float moveSpeed
            +MoveTo(Vector2)
            +MoveToTarget(Transform, stopDist)
            +StopMoving()
        }
        
        class MouseInput {
            <<static>> +OnRightClick
            <<static>> +OnRightClickTarget
        }
    }

    %% --- RELATIONSHIPS ---
    
    %% Data & Stats Connections
    PlayerProfileSO *-- InternalArtState : Contains
    PlayerStatsLinker --> PlayerProfileSO : Listens & Reads
    PlayerStatsLinker --> UnitStats : Calculates & Writes
    
    UnitStats --> Health : Updates MaxHP
    Health ..|> IDamagable : Implements

    %% Behavior Connections (Refactored)
    MouseInput --o PlayerController : Input Events
    
    PlayerController --> Movement : Commands (Legs)
    PlayerController --> Combat : Commands (Arms)
    
    %% Dependency Injection (Stats -> Behavior)
    Movement ..> UnitStats : Reads MoveSpeed
    Combat ..> UnitStats : Reads Atk/AtkSpeed
    
    %% Note: Combat and Movement are DECOUPLED (No direct link)
```

---

## 2. Chi tiết chức năng (Component Breakdown)

### A. Data & Stats (Dữ liệu & Chỉ số)
* **`PlayerProfileSO`**: [ScriptableObject] File save của nhân vật. Quản lý việc trang bị đồ và tính toán XP cho "Nội công" (`InternalArtState`).
* **`PlayerStatsLinker`**: [MonoBehaviour] Cầu nối. Mỗi khi Profile thay đổi (lên cấp, đổi đồ), nó lấy chỉ số cộng thêm và nạp vào `UnitStats`.
* **`UnitStats`**: [MonoBehaviour] "Single Source of Truth" cho chỉ số thực tế. Nó cộng gộp: `Base Stats` + `Equipment Bonus` + `Buffs`.
* **`Health`**: [MonoBehaviour] Quản lý máu. Tự động cập nhật `MaxHealth` khi `UnitStats` thay đổi.

### B. Controller & Behavior (Điều khiển - Mới Refactor)
* **`PlayerController`**: [MonoBehaviour] **Bộ não**.
    * Lắng nghe `MouseInput`.
    * Quyết định khi nào thì `Movement` chạy, khi nào thì `Combat` đánh.
    * Chứa logic **Auto-Target** (tự tìm quái khi đứng yên).
* **`Movement`**: [MonoBehaviour] **Đôi chân**.
    * Chỉ biết di chuyển tới điểm A hoặc bám theo Object B.
    * Lấy `MoveSpeed` từ `UnitStats`.
* **`Combat`**: [MonoBehaviour] **Đôi tay**.
    * Chỉ biết tấn công mục tiêu được giao (`SetTarget`).
    * Lấy `Damage`, `AttackSpeed` từ `UnitStats`.
    * Có thể dùng chung cho Enemy (chỉ cần đổi `validTargetLayers`).

---

## 3. Sequence Diagram: Full Gameplay Loop

Sơ đồ dưới đây mô tả một vòng lặp game đầy đủ: Từ lúc nhận XP -> Lên cấp -> Cập nhật chỉ số -> Người chơi điều khiển đánh quái.

```mermaid
sequenceDiagram
    participant GameSystem
    participant Profile as PlayerProfileSO
    participant Linker as PlayerStatsLinker
    participant Stats as UnitStats
    participant Controller as PlayerController
    participant Combat as Combat
    participant Move as Movement

    %% --- PHASE 1: PROGRESSION (Level Up) ---
    note over GameSystem: 1. Player nhận kinh nghiệm
    GameSystem->>Profile: AddExperience(500)
    activate Profile
    Profile->>Profile: InternalArt Level Up!
    Profile->>Linker: Event: OnProfileChanged
    deactivate Profile

    activate Linker
    Linker->>Profile: FillBonusStats() (Lấy chỉ số mới)
    Linker->>Stats: RecalculateFinalStats(bonus)
    deactivate Linker

    activate Stats
    Stats->>Stats: Tính lại Base + Bonus
    Stats-->>Combat: Event: OnStatsChanged (Update Dmg/Spd)
    Stats-->>Move: Event: OnStatsChanged (Update MoveSpd)
    deactivate Stats

    %% --- PHASE 2: ACTION (Combat) ---
    note over Controller: 2. Người chơi Click vào Quái
    
    GameSystem->>Controller: MouseInput Event
    activate Controller
    
    Controller->>Combat: SetTarget(Enemy)
    Controller->>Move: MoveToTarget(Enemy, range)
    
    deactivate Controller

    loop Update Loop
        Move->>Move: Di chuyển tiếp cận...
        Combat->>Combat: Check Distance <= AttackRange
        
        opt In Range
            Combat->>Combat: PerformAttack()
            Combat-->>GameSystem: Deal Damage Logic
        end
    end
```

## 4. Hướng dẫn Setup Prefab (Quick Setup)

Để tạo một Player hoàn chỉnh từ các script trên:

1.  Tạo GameObject **Player**.
2.  Gắn **`Rigidbody2D`** (Gravity Scale = 0, Freeze Rotation Z).
3.  Gắn **`UnitStats`** (Kéo file BaseStats SO vào).
4.  Gắn **`Health`** (Sẽ tự link với UnitStats).
5.  Gắn **`Movement`** (Sẽ tự link với UnitStats).
6.  Gắn **`Combat`** (Sẽ tự link với UnitStats).
    * *Lưu ý:* Set `Valid Target Layers` là **Enemy**.
7.  Gắn **`PlayerController`** (Sẽ tự tìm Movement và Combat).
    * *Tùy chỉnh:* `Auto Detect Radius` = 5.
8.  Gắn **`PlayerStatsLinker`** (Kéo file `PlayerProfileSO` vào).