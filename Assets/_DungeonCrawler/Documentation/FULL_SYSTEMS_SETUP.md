# Full Systems Setup Guide

Complete step-by-step wiring for **quests**, **combat**, **audio**, **inventory**, and **pickups** in `DungeonCrawler` scene.

Use this after code fixes. Estimated time: **45–60 minutes**.

---

## Before you start

1. Open project in Unity → wait for **zero compile errors**.
2. Open scene: `Assets/Scenes/DungeonCrawler.unity`.
3. Keep `PHASE1_SCENE_SETUP.md` open for quest chain IDs.

---

## Part A — Managers (`_GAME_MANAGERS`)

Select **`_GAME_MANAGERS`** and verify/add:

| Component | Required fields |
|-----------|-----------------|
| **Quest Manager** | All 5 quest SOs in `All Quest Data` |
| **Quest Chain Controller** | Ordered IDs: `tutorial_talk`, `tutorial_move`, `tutorial_combat`, `puzzle_key`, `boss_slain` |
| **Quest Event Bridge** | (no fields) |
| **Game Systems Bootstrap** | optional validation |
| **Inventory Manager** | **Item Registry** → `Items/ItemRegistry.asset` |
| **Dialogue Manager** | UI panel refs wired |
| **Audio Manager** | **Audio Mixer** → `Assets/Audio/AudioMixer.mixer` |
| **Save System** | Item Database (weapons) |
| **Boss Victory Dialogue** | optional ending dialogue SO |

### Audio Manager auto-sources

On Play, `AudioManager` creates child sources if missing:

- `MusicSource` → Music group  
- `AmbientSource` → Ambient group  
- `SFXSource` → SFX group  
- `UISource` → UI group  
- `VoiceSource` → Voice group (dialogue)

You can also create these manually as children and assign in inspector.

---

## Part B — Quest UI (fixes “quest completes but UI stuck”)

### Important: fix compile errors first

If the Console shows **any red errors**, Unity keeps the **old** script in the Inspector.  
After errors are gone, select **QuestUIManager** again — you should see a **Display** section with **Show Only Active Quest**.

### Where is QuestUIManager?

1. Open scene `DungeonCrawler`.
2. In **Hierarchy**, search: `QuestUIManager` (or open `Canvas` → `HUD` → object that has the **Quest UIManager** component).
3. Click that object.
4. In **Inspector**, find the component **Quest UIManager (Script)**.

### Inspector fields (top to bottom)

| Field | What to assign |
|-------|----------------|
| **Quest List Parent** | Drag `QuestListContent` (child under Quest tracker panel) |
| **Quest Item Prefab** | `Assets/Scripts/Quests/QuestItem.prefab` |
| **Display — Show Only Active Quest** | ✓ ON (recommended) |
| **Show Completed Quests** | leave ON if you show all quests; irrelevant when “only active” is ON |
| **Row Spacing / Default Row Height** | optional layout |

### Parent objects must be active

In Hierarchy, these must have the checkbox **on**:

- `Canvas` (or your main UI canvas)
- `HUD`
- `QuestTrackerPanel` (or similar name)
- `QuestListContent` (this is **Quest List Parent**)

### Test

1. Press **Play**.
2. **Console** should have **zero errors**.
3. After talking to the Guide, you should see: `[QuestUIManager] Refreshed 1 quest entries.`
4. The active quest title shows a **►** prefix.

---

## Part C — Quest flow objects

| Step | Object | Components | Key values |
|------|--------|------------|------------|
| 1 Talk | Guide NPC | `NPCInteractable`, trigger collider | Default dialogue: `Dialogue_TutorialGuide` |
| 2 Walk | `QuestZone_WalkPlaza` | `QuestObjectiveTrigger` | objectiveId: `walk_to_plaza` |
| 3 Kill | First enemy | `HealthComponent`, `QuestEntityMarker` | entityId: `first_enemy` |
| 4 Key | `KeyPickup` | `ItemPickup`, trigger | Item: `Item_AncientKey` |
| 5 Boss | Boss | `HealthComponent`, `QuestEntityMarker` | entityId: `dungeon_boss` |

**Player tag** must be `Player` on character.

---

## Part D — Player combat

On **Player** root:

| Component | Settings |
|-----------|----------|
| `HealthComponent` | Max 100, **Is Player** ✓ |
| `PlayerDamageReceiver` | Invuln ~0.6s |
| `PlayerCombat` | Enemy Layers = **Enemy** (layer 6) |
| `CharacterController` | required |

### Optional weapon hitbox (recommended)

1. Child object `WeaponHitbox` under weapon/hand.
2. Add **Box Collider** → **Is Trigger** ✓, disabled by default.
3. Add **Weapon Hitbox** script.
4. On **Player Combat** → drag **Weapon Hitbox** reference.
5. Assign **Attack Sfx** clip in inspector.

Without hitbox, spherecast fallback still damages enemies in front.

### Animator

Attack trigger name: `primaryAttack` (match your Animator).  
Input: **LMB** or Input System attack.

---

## Part E — Enemy combat

Enemy prefab (`Assets/_DungeonCrawler/Scripts/Prefabs/Enemy.prefab`):

| Component | Notes |
|-----------|--------|
| Layer | **Enemy** (6) |
| `HealthComponent` | max 50, entity id per instance |
| `EnemyHealthBridge` | syncs legacy HP |
| `EnemyFSM` | auto-finds Player tag |
| `EnemyAttack` | damage 12, cooldown 1.5s |
| `NavMeshAgent` | required |

**First tutorial enemy in scene:** add `QuestEntityMarker` → `first_enemy`.

Death fires `GameEvents.OnEnemyKilled` → quest bridge → UI refresh.

---

## Part F — Boss combat

1. Boss prefab/scene instance: `entityId` = `dungeon_boss`.
2. Add **Boss Controller** + **NavMesh** on arena floor (Bake NavMesh).
3. Create **Boss Arena Trigger** empty:
   - Box Collider **Is Trigger** ✓
   - **Boss Arena Trigger**:
     - Boss Root → boss object (can start disabled)
     - Boss Health Bar → UI with **Boss Health Bar UI**
     - Arena Gate → door blocker optional

### Boss health bar UI

1. Under Canvas create `BossHealthBar` panel (inactive).
2. Image fill (Filled type) for HP.
3. Add **Boss Health Bar UI** → Fill Image assigned.
4. **Bar Root** = panel itself.

---

## Part G — Inventory icons (fixes missing sprites)

### Root cause fixed

`InventoryUIView` now sets **icon sprite** on row prefab root `Image` via `InventoryItemRowUI`.

### Scene setup

1. **Inventory Panel** → **Inventory UI View**:
   - **List Parent** → grid Content transform
   - **Row Prefab** → `GridRowPrefab.prefab` (updated with `InventoryItemRowUI`)
2. On **Item_AncientKey** SO:
   - Assign **Icon** sprite (required — UI shows empty if null)
3. **Item Registry** lists `Item_AncientKey`.

### Test

Pick up key → **Tab** → icon + name visible.

---

## Part H — Item pickup

`KeyPickup` object:

```
KeyPickup
├── Mesh (optional)
└── Box Collider (Is Trigger ✓)
    Item Pickup:
      - Item: Item_AncientKey
      - Pickup Sfx: (optional clip)
      - Prompt: uses InteractPromptUI or local prompt
```

Press **E** in trigger → item added → object destroyed → quest updates → inventory refreshes.

---

## Part I — Audio (drop-in workflow)

### 1. AudioManager in scene

- Mixer assigned.
- Play scene → child **AudioSources** auto-created.

### 2. Play clips from any script

```csharp
AudioManager.Instance.PlaySFX(myClip);
AudioManager.Instance.PlayMusic(bgmClip);
AudioManager.Instance.PlayDialogue(voiceClip);
AudioManager.Instance.PlayUI(uiClip);
AudioManager.Instance.PlayAmbient(ambientClip);
```

### 3. Level BGM

Object with **Level Audio Starter**:

- BGM clip
- Ambient clip

### 4. Pause menu sliders

On **Pause Menu** → **Pause Menu Controller**:

| Slider | Method |
|--------|--------|
| Master | auto |
| Music | auto |
| SFX | auto |
| Dialogue | maps to Voice mixer |
| Ambient | auto |
| UI | optional |

Create sliders in UI if missing.

### 5. Mixer exposed params (already in project)

`MasterVolume`, `MusicVolume`, `SFXVolume`, `VoiceVolume`, `UIVolume`, `AmbientVolume`

---

## Part J — Interact prompt UI

Under **Canvas**:

```
InteractPromptPanel (inactive at start)
└── PromptText (TMP)
```

Add **Interact Prompt UI** → assign Panel + Prompt Text.

NPCs and pickups use shared **"Press E to Pick Up"** / **"Press E to talk"** when no local prompt object.

---

## Part K — End-to-end test checklist

- [ ] Talk to Guide → quest UI shows **► Speak to Guide** then completes
- [ ] Walk into zone → UI updates to next quest
- [ ] Kill `first_enemy` → UI updates immediately
- [ ] Pick up key → icon in inventory + quest completes
- [ ] Kill boss → quest completes + optional ending dialogue
- [ ] Pause sliders change volume (hear difference)
- [ ] `PlaySFX(clip)` works with clip dragged in inspector
- [ ] Save/load keeps quests + inventory items

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Quest UI never updates | QuestUIManager on active Canvas; `Show Only Active Quest`; check Console for Refresh log |
| All quests complete at once | Quest Chain Controller present; only one enemy has `first_enemy` |
| No damage to enemies | Player **Enemy Layers** mask; enemy on layer 6 |
| Enemy doesn't chase | Player tag; NavMesh baked; EnemyFSM player ref |
| No inventory icon | Assign sprite on Item SO; Row prefab = GridRowPrefab |
| No sound | AudioManager in scene; mixer assigned; clip not null |
| Double quest progress | Only one QuestManager in scene |

---

## Files changed in this fix (reference)

**Modified:** `QuestUIManager.cs`, `QuestUIItem.cs`, `QuestManager.cs`, `QuestChainController.cs`, `QuestEventBridge.cs`, `InventoryUIView.cs`, `InventoryManager.cs`, `ItemPickup.cs`, `AudioManager.cs`, `PauseMenuController.cs`, `PlayerCombat.cs`, `HealthComponent.cs`, `EnemyAttack.cs`, `EnemyFSM.cs`, `EnemyHealthBridge.cs`, `HealthBarUI.cs`, `GridRowPrefab.prefab`, `Boss.prefab`

**Created:** `InventoryItemRowUI.cs`, `WeaponHitbox.cs`, `PlayerDamageReceiver.cs`, `BossHealthBarUI.cs`, `BossArenaTrigger.cs`

---

## Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| LMB | Attack |
| E | Interact / pickup |
| Tab | Inventory |
| Esc | Pause + audio settings |
