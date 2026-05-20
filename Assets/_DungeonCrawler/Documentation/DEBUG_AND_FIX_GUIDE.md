# Debug & Fix Guide — Your Project (Read This First)

This document explains **what was wrong**, **what we changed in code**, and **exact steps you must do in the Unity Editor** to finish setup.

---

## Issue 1 — Jittery Player Movement

### Cause (found in your `DungeonCrawler` scene)

Your **Player** object had **two scripts** both calling `CharacterController.Move()`:

| Script | Update loop | Problem |
|--------|-------------|---------|
| `CharacterControllerMovement` | **FixedUpdate** | Moves controller |
| `ThirdPersonPlayerMovement` | **Update** | Moves controller again |

Moving the same `CharacterController` in **FixedUpdate and Update** causes stutter/jitter.  
You also had a **CapsuleCollider** + **CharacterController** on the same object (double collision).

`ThirdPersonCamera` was snapping the camera rig to `target.position` (player feet), fighting orbit logic.

### Code fixes (already applied)

- `PlayerMotor` — single movement in **Update**, one combined `Move()` per frame
- `PlayerMovementBootstrap` — disables legacy movers + extra capsule
- `PlayerAimProvider` — aim from camera pivot
- `ThirdPersonCamera` — fixed orbit (no snap to feet)
- Legacy scripts auto-disable when `PlayerMotor` / bootstrap exists

### Your steps in Unity

1. Select **Player** in `DungeonCrawler` scene.
2. **Add Component** → `Player Movement Bootstrap` (`DungeonCrawler.Player.PlayerMovementBootstrap`).
3. **Add Component** → `Player Motor` if missing.
4. Under camera setup, select child **CameraPivot** → **Add Component** → `Player Aim Provider`.
5. On **ThirdPersonCamera** root object, set **Follow Distance** = `5` (new field).
6. **Disable** (or remove) `Character Controller Movement` and `Third Person Player Movement` if bootstrap doesn’t run first play mode test.
7. **Disable** `Capsule Collider` on Player (keep **Character Controller** only).
8. Press Play — movement should be smooth.

### Test

- Walk in circles: no stutter.
- Console should log bootstrap disabled duplicate scripts once.

---

## Issue 2 — Projectile Spawns Wrong / Shoots Up

### Cause

- `firePoint` was **null** on prefab (`ProjectileShootAbility` had no reference).
- Spawn used `firePoint.rotation` while FirePoint had default rotation under a tilted model.
- Velocity used `transform.forward` before rotation was set.

### Code fixes

- Auto-creates **FirePoint** at chest height if missing.
- Uses `PlayerAimProvider` / camera forward for direction.
- `Projectile.Initialize(..., direction)` sets rotation + velocity together.

### Your steps in Unity

1. Select **Player** (or dungeon Player prefab).
2. Find/create child **FirePoint**:
   - Position (local): `X=0, Y=1.4, Z=0.6` (adjust per model)
   - Rotation (local): `0, 0, 0`
3. On **Projectile Shoot Ability**:
   - **Fire Point** → drag FirePoint transform
   - **Projectile Prefab** → assign your projectile prefab
4. On **Projectile prefab**:
   - **Rigidbody**: Use Gravity = **off**, Collision Detection = **Continuous Dynamic**
   - **Sphere Collider** (not trigger)
   - Layer: Default (hits environment + enemies)
5. Ensure **CameraPivot** has `PlayerAimProvider`.
6. Press **F** in play mode — projectile flies where you look.

### Test

- Aim at horizon → projectile goes forward, not up.
- Spawn position is at chest, not under floor.

---

## Issue 3 — Player Passes Through Walls

### Cause

Medieval Desert City prefabs are **visual meshes only** — no colliders on buildings.

### Solution — bulk colliders (Editor tool added)

**Menu: Tools → Dungeon → Add Box Colliders To Selected (Static)**

1. In Hierarchy, create empty `Environment` parent.
2. Select all building/terrain objects (or one district parent).
3. Run **Tools → Dungeon → Add Box Colliders To Selected (Static)**.
4. Objects get **BoxCollider** fitted to mesh bounds + marked **Static**.

**When to use Mesh Collider instead:**  
**Tools → Dungeon → Add Mesh Colliders To Selected (Convex)** — for odd shapes (use sparingly; expensive).

### Best practices

| Use | When |
|-----|------|
| **BoxCollider** | Buildings, walls, props (fast) |
| **MeshCollider (non-convex)** | Static terrain only |
| **MeshCollider (convex)** | Rare dynamic props |
| Mark **Static** | Baked/static geometry |
| **Layer** | Environment on Default; Player uses CharacterController (no Rigidbody needed) |

### Player collision

- **Only CharacterController** on Player (no extra CapsuleCollider).
- Character Controller: Height `2`, Radius `0.5`, Center `Y=1` if model pivot is at feet.

### Test

- Walk into walls — should block.
- No jitter when sliding along walls (after movement fix).

---

## Issue 4 — Control Prompts & Quest UI Not Showing

### Causes found

| Problem | Detail |
|---------|--------|
| **ControlPromptUI missing** | Not in scene; triggers call `Instance` → null |
| **QuestUIManager timing** | Could run before `QuestManager.Awake` |
| **Panel inactive** | Quest list parent may be disabled in hierarchy |

### Code fixes

- `ControlPromptTrigger` logs clear warning if UI missing
- `QuestUIManager` waits for QuestManager + activates parent
- `GameSystemsBootstrap` validates systems (add to managers)

### Control Prompt UI hierarchy (create this)

```
Canvas (Screen Space Overlay)
└── ControlPromptPanel          ← Panel, anchor top-center
    ├── Background (Image, semi-transparent)
    └── PromptText (TextMeshPro) ← Large readable text
```

**On ControlPromptPanel:**

- Add script: **Control Prompt UI**
  - **Panel** → ControlPromptPanel itself
  - **Message Text** → PromptText

**On trigger volume:**

- Empty object `Prompt_Movement`, **Box Collider** Is Trigger ✓
- Add **Control Prompt Trigger**, message: `WASD to move`
- Player tag required on character

### Quest Tracker UI hierarchy

```
Canvas
└── HUD
    └── QuestTrackerPanel       ← Active ✓
        └── QuestListContent    ← Vertical Layout Group
```

**On scene object QuestUIManager:**

- **Quest List Parent** → QuestListContent RectTransform
- **Quest Item Prefab** → your QuestUIItem prefab

**QuestManager** (_GAME_MANAGERS):

- **All Quest Data** → must list at least one QuestDataSO

### Your steps

1. Create **ControlPromptPanel** as above.
2. Add **Game Systems Bootstrap** to `_GAME_MANAGERS`.
3. Ensure **QuestTrackerPanel** is **active** in hierarchy.
4. Enter play mode — Console should show `[QuestUIManager] Refreshed N quest entries.`
5. Walk into prompt trigger — Console: `[ControlPromptUI] Showing: ...`

---

## Issue 5 — Audio Stopped Working

### Causes

- `LevelAudioStarter` called `AudioManager.Instance` with no null check (failed silently if order wrong).
- **musicDemo** on AudioManager empty (not used by LevelAudioStarter — BGM comes from LevelAudio clips).
- Multiple **AudioListener** (only one allowed — on Main Camera).

### Your audio hierarchy (verify)

```
AudioManager
├── MusicSource      (AudioSource → Output: Music mixer group)
├── AmbientSource
├── SfxSource
├── UiSource
├── VoiceSource
└── LevelAudio       (LevelAudioStarter)
      - BGM: assign clip
      - Ambient: assign clip
```

**Checklist:**

1. **AudioManager** in scene with **Audio Mixer** assigned.
2. Each child **AudioSource** routes to correct mixer group.
3. **Exactly one Audio Listener** — on Main Camera only.
4. **LevelAudio** has BGM + Ambient clips assigned (your scene already has GUIDs — verify not missing).
5. **Edit → Project Settings → Audio** — Volume not zero.
6. Pause menu sliders use `AudioManager.SetMusicVolume` etc.

### Test

- Play scene → Console: `[LevelAudioStarter] Playing BGM.`
- If not: check AudioManager exists before LevelAudio runs.

---

## Issue 6 — Inventory Panel (Full Setup)

### Hierarchy

```
Canvas
└── InventoryPanel              ← inactive by default, Tab toggles
    ├── Background (Image)
    ├── Title (TMP) "Inventory"
    ├── EquippedWeaponRow
    │   ├── WeaponIcon (Image)
    │   └── WeaponNameText (TMP)
    ├── ItemGrid (Scroll View)
    │   └── Viewport
    │         └── Content         ← Grid Layout Group
    └── CloseHint (TMP) "Tab to close"
```

### Slot prefab (`InventorySlotRow`)

```
InventorySlotRow
└── ItemNameText (TMP)
```

Optional: Icon Image for item sprite.

### GameObjects & scripts

| Object | Script |
|--------|--------|
| `_GAME_MANAGERS` | **Inventory Manager** + Item Database reference |
| **InventoryPanel** | **Inventory UI View** — list parent = Content, row prefab = InventorySlotRow |
| World pickup | **Item Pickup** + trigger collider + ItemDataSO |

### Item ScriptableObject

**Create → Dungeon → Item Data**

- itemId: unique string (`ancient_key`)
- linkedWeapon: optional WeaponData for weapons

### Pickup object

```
KeyPickup
├── Mesh (optional)
└── Box Collider Is Trigger ✓
    Item Pickup: item = Item_AncientKey
```

Press **E** in trigger to collect.

### Equip weapon

When item has **linkedWeapon**, `InventoryManager` auto-equips and fires `GameEvents.OnWeaponEquipped` → `PlayerCombat` uses it.

### Test

- Pick up item → Tab → see item in grid.
- Equipped name/icon updates.

---

## Scene Player vs Prefab

Your **scene Player** uses legacy scripts. Your **`_DungeonCrawler/Scripts/Prefabs/Player.prefab`** has dungeon systems but **unassigned** firePoint/projectile.

**Recommendation for defense:**  
Replace scene Player with the dungeon prefab OR add bootstrap + components to scene Player as in Issue 1.

---

## Quick Testing Checklist

- [ ] Movement smooth (bootstrap on Player)
- [ ] Projectile fires forward (FirePoint + prefab assigned)
- [ ] Walls block player (colliders on environment)
- [ ] Control prompt appears (ControlPromptUI in Canvas)
- [ ] Quest list populates (QuestManager + QuestUIManager wired)
- [ ] BGM plays (LevelAudio + AudioManager)
- [ ] Inventory opens with Tab (InventoryUIView + InventoryManager)

---

## Files Changed in This Fix

- `PlayerMotor.cs`, `PlayerMovementBootstrap.cs`, `PlayerAimProvider.cs`
- `ThirdPersonCamera.cs`, `ThirdPersonPlayerMovement.cs`, `CharacterControllerMovement.cs`
- `ProjectileShootAbility.cs`, `Projectile.cs`
- `ControlPromptUI.cs`, `ControlPromptTrigger.cs`, `QuestUIManager.cs`
- `LevelAudioStarter.cs`, `GameSystemsBootstrap.cs`
- `EnvironmentColliderUtility.cs` (Editor menu)

See **SETUP_GUIDE.md** for full game setup; this file focuses on **debugging your current issues**.
