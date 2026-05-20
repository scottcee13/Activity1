# Complete Setup Guide — 5-Room Dungeon Crawler

Follow these steps **in order** in Unity. Estimated time: 2–4 hours for a playable vertical slice.

---

## Part 0 — Fix & Verify Project Compiles

1. Open the project in Unity and wait for scripts to compile.
2. Confirm **Console** has **zero errors** (warnings about missing references in scenes are OK until you wire objects).
3. If you still see `RaiseQuestObjectiveTriggered` errors, reimport scripts: right-click `Assets/_DungeonCrawler` → **Reimport**.

---

## Part 1 — Project Settings

### 1.1 Tags & Layers

1. **Edit → Project Settings → Tags and Layers**
2. Under **Tags**, ensure **Player** exists. If not: click **+** → name it `Player`.
3. Under **Layers**, add **Enemy** (optional, for combat raycasts):
   - User Layer 6: `Enemy`
4. Select your player GameObject later and set **Tag** to `Player`.

### 1.2 Input

The project uses the **Input System** (`PlayerInputs`) for legacy movement and **keyboard polling** for dungeon systems (E, Q, Space, F, Tab, Esc).

1. **Edit → Project Settings → Player → Active Input Handling**
2. Set to **Both** (recommended) or **Input Manager (Old)** if you only use `Input.GetAxis`.
3. Ensure **Edit → Project Settings → Input Manager** has **Horizontal** and **Vertical** axes (default Unity).

### 1.3 Build Scenes

You will create two scenes in Part 2. After creating them:

1. **File → Build Settings**
2. Click **Add Open Scenes** for each scene, in this order:
   - `MainMenu` (index 0)
   - `DungeonCrawler` (index 1)

---

## Part 2 — Create Scenes

### 2.1 Main Menu Scene

1. **File → New Scene** → Basic (URP) or your template → save as `Assets/Scenes/MainMenu.unity`
2. Create empty GameObject: `_MainMenu`
3. Add component: **DungeonCrawler.UI.MainMenuController**
4. Create **UI → Canvas** (Screen Space Overlay)
5. Under Canvas, create:
   - **UI → Button** named `PlayButton` — text: "Play"
   - **UI → Button** named `QuitButton` — text: "Quit"
   - **UI → Panel** named `SettingsPanel` (inactive by default)
     - Add sliders for Master / Music / SFX (optional for now)
6. On `_MainMenu` → **Main Menu Controller**:
   - Drag **PlayButton** → Play Button
   - Drag **QuitButton** → Quit Button
7. Create empty `_SceneFlow` with **DungeonCrawler.Core.SceneFlowManager**:
   - Main Menu Scene Name: `MainMenu`
   - Dungeon Scene Name: `DungeonCrawler`
8. Add **EventSystem** if not present (**UI → Event System**)

### 2.2 Dungeon Scene (shell)

1. **File → New Scene** → save as `Assets/Scenes/DungeonCrawler.unity`
2. Add a **Plane** or dungeon floor mesh; scale for Room 1 area (~20×20 units)
3. Add **Directional Light**; set dim color for dungeon mood
4. Optional: **Volume** with post-processing (bloom, vignette) for atmosphere
5. Create empty parent: `--- ROOMS ---` with five child empties:
   - `Room1_Entrance`
   - `Room2_Puzzle`
   - `Room3_Parkour`
   - `Room4_Boss`
   - `Room5_Reward`
6. Spread them along the Z axis (e.g. 0, 30, 60, 90, 120) — adjust to your level size

---

## Part 3 — Persistent Managers

In **DungeonCrawler** scene (managers can live here; `AudioManager` may use DontDestroyOnLoad):

1. Create empty: `_GAME_MANAGERS`
2. Add these components to **the same object** or children (one component per object is fine):

| GameObject child | Component | Notes |
|------------------|-----------|--------|
| `_GAME_MANAGERS` | `GameManager` | Player ref assigned later |
| `_GAME_MANAGERS` | `QuestManager` (existing) | See Part 5 |
| `_GAME_MANAGERS` | `QuestEventBridge` | No inspector fields |
| `_GAME_MANAGERS` | `DialogueManager` | Wire UI in Part 7 |
| `_GAME_MANAGERS` | `InventoryManager` | Assign Item Database |
| `_GAME_MANAGERS` | `CheckpointManager` | — |
| `_GAME_MANAGERS` | `AbilityManager` | Abilities on **Player**, not here |
| `_GAME_MANAGERS` | `SaveSystem` (existing) | Assign Item Database |
| `_GAME_MANAGERS` | `UIManager` | Wire panels Part 7 |
| `_GAME_MANAGERS` | `SceneFlowManager` | Same scene names as menu |

3. **InventoryManager** → drag `Assets/ScriptableObjects/ItemDatabase` (or your database asset) to **Legacy Database**
4. **SaveSystem** → same **Item Database**

---

## Part 4 — Player Prefab

### 4.1 Create hierarchy

1. Drag your character model into scene (or **3D Object → Capsule** placeholder)
2. Rename root to `Player`, **Tag: Player**
3. Structure:

```
Player
├── CameraPivot          (empty, Y ≈ 1.6)
│   └── MainCamera       (move scene camera here, add AudioListener)
├── FirePoint            (empty, in front of chest — for projectiles)
└── Model                (mesh + Animator)
```

### 4.2 Required components (on Player root)

Add in this order:

| Component | Settings |
|-----------|----------|
| `CharacterController` | Height 2, Radius 0.5, Center Y=1 |
| `CharacterControllerMovement` (legacy, optional) | If using Input System animations |
| `PlayerMotor` | Camera Root → **CameraPivot** |
| `PlayerHealth` (legacy) | Max Health 100 |
| `HealthComponent` | Max Health 100, **Is Player** ✓ |
| `PlayerHealthBridge` | — |
| `AbilityManager` | Leave abilities list empty; add ability components next |
| `DashAbility` | Create AbilityDataSO (Part 5), assign |
| `JumpAbility` | Assign jump AbilityDataSO |
| `ProjectileShootAbility` | Fire Point → FirePoint, Projectile Prefab (Part 6) |
| `PlayerCombat` | Enemy Layers → **Enemy** layer mask |
| `PlayerWeapon` (legacy, optional) | Weapon array if using SO weapons |

**Ability components:** Each ability needs an **Ability Data SO** with matching `abilityId`: `dash`, `jump`, `projectile`.

On **AbilityManager**, drag the three ability components into the **Abilities** list.

### 4.3 Camera

**Option A — Improved camera (recommended for parkour):**

1. On **MainCamera**, remove default follow if any
2. Add **DungeonCrawler.Player.ImprovedThirdPersonCamera**
   - Target → Player root
   - Pivot → CameraPivot
   - Collision Mask → Default / Environment
3. Disable or remove old `ThirdPersonCameraController` if it conflicts

**Option B — Legacy:** Keep `ThirdPersonCameraController` on Player with pivot assigned.

### 4.4 Save as prefab

1. Drag **Player** from Hierarchy to `Assets/_DungeonCrawler/Prefabs/Player.prefab`
2. Delete scene instance or keep one instance in dungeon scene
3. **GameManager** → assign **Player** transform

---

## Part 5 — ScriptableObjects

### 5.1 Ability data

Right-click Project → **Create → Dungeon → Ability Data**

Create three assets in `Assets/_DungeonCrawler/ScriptableObjects/Abilities/`:

| Asset name | abilityId | defaultKey | cooldown |
|------------|-----------|------------|----------|
| Ability_Jump | `jump` | Space | 0.5 |
| Ability_Dash | `dash` | Q | 2 |
| Ability_Projectile | `projectile` | F | 1 |

Assign each to the matching ability component on the Player.

### 5.2 Items

**Create → Dungeon → Item Data**

Example: `Item_AncientKey`

- itemId: `ancient_key`
- displayName: Ancient Key
- itemType: Key
- Optional: link WeaponData if it also unlocks a weapon

### 5.3 Dialogue

**Create → Dungeon → Dialogue Data**

Example: `Dialogue_TutorialGuide`

- dialogueId: `tutorial_intro`
- Node 0: lines from guide NPC, **End After Lines** ✓
- grantsHintId: (leave empty or `tutorial_hint`)

Example: `Dialogue_GuardQuiz`

- dialogueId: `guard_quiz`
- Node 0: guard question + **Choices** (2–3 options)
- Choice index **1** = correct (match `DoorPuzzleGuard.correctChoiceIndex`)

### 5.4 Quests (existing menu)

**Create → Quests → Quest Data** — create in `Assets/_DungeonCrawler/ScriptableObjects/Quests/`:

| questID | Title | objectiveType | requiredAmount |
|---------|-------|---------------|----------------|
| tutorial_move | Learn Movement | Exploration | 1 |
| tutorial_talk | Speak to Guide | Dialogue | 1 |
| tutorial_combat | First Blood | Kill | 1 |
| puzzle_key | Find the Key | Exploration | 1 |
| boss_slain | Defeat the Guardian | Kill | 1 |

On **QuestManager** → drag all quest assets into **All Quest Data** list.

### 5.5 Boss

**Create → Dungeon → Boss Data** → `Boss_Guardian`

- bossId: `dungeon_boss`
- maxHealth: 500
- phase2HealthPercent: 0.5

---

## Part 6 — Combat Prefabs

### 6.1 Projectile

1. **3D Object → Sphere**, scale 0.2
2. Add **Rigidbody** (use gravity off), **Sphere Collider**
3. Add **DungeonCrawler.Combat.Projectile**
4. Save as `Assets/_DungeonCrawler/Prefabs/Projectile.prefab`
5. Assign to **ProjectileShootAbility** on Player

### 6.2 Enemy (basic)

1. Duplicate or use existing enemy model
2. Layer: **Enemy**
3. Add **NavMeshAgent**, **EnemyFSM**, **EnemyMovement**, **EnemyAttack** (legacy)
4. Add **HealthComponent** (max 50, entityId: `goblin`, Is Player ✗)
5. Add **EnemyHealthBridge** + legacy **EnemyHealth** (optional)
6. Save prefab → place in Room 2 and wire spawner in Room 4

### 6.3 Boss

1. Large enemy prefab + **BossController**
2. Assign BossDataSO, player transform, ranged fire point, projectile prefab
3. **HealthComponent** max health = boss SO value
4. Child empty **SpawnPoints** for **EnemySpawner**

---

## Part 7 — UI Setup (Dungeon Scene)

### 7.1 Canvas

1. **UI → Canvas** → name `GameUI`
2. Canvas Scaler: Scale With Screen Size, ref 1920×1080

### 7.2 HUD (child of Canvas)

| Element | Component | Notes |
|---------|-----------|--------|
| Panel `HUD` | — | |
| Image `HealthFill` | Image Type: Filled | |
| — | **HealthBarUI** | Target → Player HealthComponent, Fill → HealthFill |
| Panel `AbilityBar` | **AbilityCooldownHUD** | Slot abilityIds: dash, jump, projectile |
| Quest list | Existing **QuestUIManager** | Parent + quest item prefab |

### 7.3 Dialogue panel

1. Panel `DialoguePanel` (inactive)
2. TMP texts: Speaker, Body
3. Vertical layout `ChoicesParent` + Button prefab for choices
4. **DialogueManager** (on _GAME_MANAGERS):
   - Dialogue Panel, Speaker Text, Body Text, Choices Parent, Choice Button Prefab

### 7.4 Inventory panel

1. Panel `InventoryPanel` (inactive)
2. Grid + row prefab with TMP_Text
3. **InventoryUIView** on panel
4. Equipped weapon icon + name texts

### 7.5 Pause menu

1. Panel `PauseMenu` (inactive)
2. Buttons: Resume, Restart, Main Menu
3. Sliders → **PauseMenuController** → wire to AudioManager methods
4. **UIManager**: Hud Root, Pause Menu, Inventory Panel, Victory Panel

### 7.6 Victory screen

1. Panel `VictoryPanel` (inactive)
2. TMP summary + Restart / Main Menu buttons
3. **VictoryScreenUI** on panel

### 7.7 Control prompts

1. Panel `ControlPrompt` (inactive) + TMP message
2. **ControlPromptUI** on Canvas or child

### 7.8 Wire UIManager

On `_GAME_MANAGERS` **UIManager**:

- Hud Root → HUD panel
- Pause Menu → PauseMenu
- Inventory Panel → InventoryPanel
- Victory Panel → VictoryPanel

---

## Part 8 — Room 1 (Entrance / Tutorial)

1. Under `Room1_Entrance`, add floor/walls (cubes or dungeon kit)
2. Place **Guide NPC** (capsule + model):
   - Box Collider **Is Trigger** ✓
   - **NPCInteractable** → Dialogue_TutorialGuide
   - Child UI "Press E" (optional)
3. **ControlPromptTrigger** volumes:
   - Message: `WASD — Move`
   - Second trigger: `E — Talk | LMB — Attack`
4. **CheckpointTrigger** at room exit:
   - roomId: `room_1`
5. **SequentialQuestController** on Room1 empty:
   - Quest IDs in order: tutorial_move, tutorial_talk, tutorial_combat
   - Call `ActivateNext()` from Unity Events when player enters triggers (or manual test)
6. **DungeonRoomController** on large trigger volume:
   - Room Type: Entrance, roomId: `room_1`

**Quick test:** Enter play mode, talk to NPC (E), check quest UI updates.

---

## Part 9 — Room 2 (Puzzle)

1. Place 2–3 hint NPCs with different **DialogueDataSO** (each sets **grantsHintId** e.g. `hint_a`, `hint_b`)
2. Enemies dropping key:
   - On death, player collects **ItemPickup** prefab (trigger + Item_AncientKey)
   - Or spawn pickup on enemy **HealthComponent.OnDeath** (manual placement is easier)
3. **DoorPuzzleGuard** on guard NPC trigger:
   - requiredItemId: `ancient_key`
   - requiredHintId: `hint_a` (or your hint id)
   - correctChoiceIndex: `1`
   - dialogueIdToValidate: `guard_quiz`
   - Door To Open → blocking cube/door object
4. **QuestObjectiveTrigger** past door → objectiveId: `puzzle_door_opened`
5. **DungeonRoomController** → Puzzle, roomId: `room_2`

---

## Part 10 — Room 3 (Parkour)

1. Platforms with gaps; lethal floor below
2. **ParkourResetZone** on kill floor (trigger):
   - Respawn Point → empty at start of parkour
   - Reset Velocity ✓
3. **HazardDamage** on spikes (damage per tick)
4. Tune **PlayerMotor** jump via **JumpAbility** force
5. Gap requiring **Dash** (Q)
6. **DungeonRoomController** → Parkour, roomId: `room_3`
7. Test **ImprovedThirdPersonCamera** collision so camera doesn’t clip walls

---

## Part 11 — Room 4 (Boss)

1. Bake **NavMesh**: Window → AI → Navigation → Bake (walkable floor)
2. Place boss prefab + **BossController** setup
3. **EnemySpawner** with wave (adds on phase 2):
   - Spawn Points list
   - Wave 0: enemy prefab, count 3
4. Boss **HealthComponent** + world-space health bar UI (HealthBarUI targeting boss)
5. **DungeonRoomController** → Boss, roomId: `room_4`
6. Quest **boss_slain** progresses via **Kill** objective when boss dies (entityId on HealthComponent should match design; bridge adds kill on any enemy death — use requiredAmount 1)

---

## Part 12 — Room 5 (Reward)

1. Victory arena + lore object/NPC (optional)
2. Large trigger with **DungeonRoomController**:
   - Room Type: **Reward**
   - Trigger Victory On Enter ✓
3. Entering calls **GameManager.TriggerVictory()** → **VictoryScreenUI** populates

---

## Part 13 — Audio

1. Ensure **AudioManager** in scene with mixer + Music/SFX sources
2. Pause menu sliders call `SetMusicVolume` / `SetSFXVolume`
3. Assign clips to pickups, combat (optional null clips until you add assets)

---

## Part 14 — Save & Checkpoints

1. After **CheckpointTrigger**, verify file at:
   - Windows: `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>\checkpoint.json`
2. **SaveSystem** saves weapons + quests on checkpoint
3. Test: collect item → checkpoint → stop play → play again (load in Start)

---

## Part 15 — Link Main Menu → Dungeon

1. Open **MainMenu** scene
2. Ensure **SceneFlowManager** exists (DontDestroyOnLoad on play from menu)
3. Play → **Play** button loads **DungeonCrawler**
4. From pause → **Main Menu** loads back

---

## Part 16 — Testing Checklist

- [ ] No compile errors in Console
- [ ] Player moves (WASD), camera rotates (mouse)
- [ ] Space jumps, Q dashes, F fires projectile
- [ ] E talks to NPC, dialogue UI shows
- [ ] Tab opens inventory after pickup
- [ ] Esc pauses, sliders change volume
- [ ] Quest HUD updates on talk/kill/explore
- [ ] Room 2 door opens with key + correct answer
- [ ] Parkour reset teleports player
- [ ] Boss takes damage and dies
- [ ] Victory panel shows quests/items

---

## Common Issues

| Problem | Fix |
|---------|-----|
| Player doesn’t move | CharacterController present; GameManager not stuck paused |
| E does nothing | NPC collider is trigger; Player tag set |
| Quests don’t update | QuestManager has SOs listed; QuestEventBridge on managers |
| Door won’t open | Item id and hint id match SOs; correct choice index |
| Double camera | Only one follow script active |
| Null refs on UI | Wire all fields on DialogueManager / UIManager |
| Compile error CS0117 RaiseQuestObjectiveTriggered | Update `_DungeonCrawler` scripts (alias added in GameEvents) |
| CS0131 HealthComponent | Fixed — don’t assign to `?.enabled` |

---

## Controls Reference

| Key | Action |
|-----|--------|
| WASD | Move |
| Mouse | Look |
| Left Control | Sprint |
| Space | Jump |
| Q | Dash |
| F | Projectile |
| LMB | Melee |
| E | Interact / pickup |
| Tab | Inventory |
| Esc | Pause |

---

For system design details, see **ARCHITECTURE.md**.
