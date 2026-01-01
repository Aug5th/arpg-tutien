sequenceDiagram
    autonumber
    participant Input as MouseInput
    participant Ctrl as PlayerController
    participant Move as Movement
    participant Combat as Combat
    participant Enemy as Enemy(IDamagable)

    Note over Input: Player Click chuột phải vào Enemy
    Input->>Ctrl: OnRightClickTarget(Enemy)
    activate Ctrl

    Note right of Ctrl: 1. Setup mục tiêu
    Ctrl->>Combat: SetTarget(Enemy)
    
    Note right of Ctrl: 2. Tính điểm dừng (AttackRange - 0.1)
    Ctrl->>Move: MoveToTarget(Enemy, stopDistance)
    deactivate Ctrl

    loop Update Loop (Mỗi Frame)
        Move->>Move: Di chuyển tiếp cận...
        
        Combat->>Combat: Check Distance(Player, Enemy)
        
        opt Distance <= AttackRange
            Note over Combat: Trong tầm đánh -> Tấn công
            Combat->>Combat: RotateTowards(Enemy)
            
            alt Cooldown OK?
                Combat->>Enemy: TakeDamage(damage)
                Enemy-->>Combat: (Enemy mất máu / Die)
            end
        end
    end