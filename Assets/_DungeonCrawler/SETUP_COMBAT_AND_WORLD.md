# Dungeon Crawler — Combat, Weapons, Gates & World Setup

## New scripts (create in Unity automatically)

| Script | Location | Purpose |
|--------|----------|---------|
| `PlayerMovementLock` | `_DungeonCrawler/Scripts/Player/` | Blocks move/sprint/dash during attacks |
| `WeaponEquipManager` | `_DungeonCrawler/Scripts/Player/` | Spawns weapon prefabs on hand socket |
| `CombatAnimationEvents` | `_DungeonCrawler/Scripts/Combat/` | Animation event bridge (hit + end) |
| `AttackStateBehaviour` | `_DungeonCrawler/Scripts/Combat/` | Optional animator SM backup for player attack end |
| `EquippedWeapon` | `_DungeonCrawler/Scripts/Combat/` | Runtime weapon instance + hitbox |
| `EnemyDeathHandler` | `_DungeonCrawler/Scripts/Combat/` | Death anim, disable AI, destroy |
| `InteractableGate` | `_DungeonCrawler/Scripts/World/` | E to open gate |
| `WorldBoundary` | `_DungeonCrawler/Scripts/World/` | Invisible map edge walls |

## Modified scripts

- `WeaponData.cs` — `weaponPrefab`, grip offsets, `swingSfx`, `hitSfx`, `knockbackForce`
- `PlayerCombat.cs` — movement lock, hitbox only on impact frame
- `PlayerMotor.cs`, `DashAbility.cs` — respect `PlayerMovementLock`
- `WeaponHitbox.cs` — per-swing duplicate prevention, SFX/VFX
- `EnemyAttack.cs`, `EnemyFSM.cs`, `EnemyMovement.cs` — cooldowns, anim events, NavMesh
- `BossController.cs` — disables `EnemyFSM`, uses `EnemyAttack` + anim events
- `HealthComponent.cs` — delegates death to `EnemyDeathHandler`
- `PlayerWeapon.cs` — Q/R weapon switch (E reserved for interact)
- `EnvironmentColliderUtility.cs` — boundary + static batch tools

---

## 1. Layers & tags

**Edit → Project Settings → Tags and Layers**

- Tag: `Player` (added to TagManager)
- Layer 6: `Enemy` (already present)
- Player root: Tag **Player**, Layer **Default**
- Enemy/Boss prefabs: Layer **Enemy**
- `PlayerCombat` → **Enemy Layers** mask: enable layer **Enemy** (bit 64)

---

## 2. World collision & boundaries

### Bulk environment colliders

1. In Hierarchy, select environment parent(s): terrain, buildings, walls, props.
2. **Tools → Dungeon → Mark Selected Environment Static**
3. **Tools → Dungeon → Add Box Colliders To Selected (Static)**  
   - Use **Mesh Colliders (Convex)** only for odd-shaped props if box fits poorly.

### Invisible map bounds

1. Select all playable environment meshes.
2. **Tools → Dungeon → Create World Boundary From Selection**
3. Adjust `WorldBoundary` component: `Center`, `Size`, `Wall Height`.
4. Play mode: player cannot leave the box.

### Ground

- Ensure every walkable terrain/building floor mesh has a **BoxCollider** (via tool above).
- Bake **NavMesh** after colliders exist: **Window → AI → Navigation → Bake**.

---

## 3. Player prefab setup

On `Assets/_DungeonCrawler/Scripts/Prefabs/Player.prefab`:

| Component | Action |
|-----------|--------|
| `PlayerMovementBootstrap` | Keep enabled |
| `PlayerMotor` | Keep enabled |
| `PlayerMovementLock` | **Add** |
| `PlayerCombat` | **Add** if missing; assign Animator, Enemy layer mask |
| `WeaponEquipManager` | **Add**; assign hand socket or leave empty (auto-finds `mixamorig:RightHand`) |
| `CombatAnimationEvents` | **Add** on Player root |
| `PlayerWeapon` | Assign `Weapon Catalog` + `Equip Manager` |
| `KnockbackReceiver` | Keep |
| `PlayerDamageReceiver` | Keep |

### Weapon socket

- Expand character rig → find **Right Hand** bone.
- Create empty child `WeaponSocket` OR assign bone to `WeaponEquipManager.weaponSocket`.

### Weapon hitbox child

For each weapon prefab:

1. Create prefab with mesh + child `Hitbox` with **BoxCollider** (Is Trigger) + `WeaponHitbox`.
2. Assign `targetLayers` = Enemy.
3. On `WeaponData` asset: assign `weaponPrefab`, grip position/rotation, damage, SFX.

---

## 4. Animation events (critical)

### Why “Add Event” is greyed out

Clips inside **imported FBX files are read-only**. Unity blocks editing them in the Animation window. This is normal — not a permissions bug.

**Fix:** Use a **writable copy** (`.anim` file) with events baked in.

### Automatic setup (recommended)

1. Menu: **Tools → Dungeon → Setup Combat Animation Events (Copy Clips)**
2. This creates writable clips in `Assets/_DungeonCrawler/Animations/CombatClips/`:
   - `Player_PrimaryAttack_Combat.anim`
   - `Player_SecondaryAttack_Combat.anim`
   - `Enemy_Attack_Combat.anim`
3. Each clip gets:
   - `OnAttackHit` at ~45% of clip length
   - `OnAttackEnd` at ~95% of clip length
4. **Character.controller** and **Enemy.controller** attack states are auto-reassigned to these copies.

If auto-assign fails, drag the new `.anim` from `CombatClips` onto **PrimaryAttack**, **SecondaryAttack**, and **Attack** states in the Animator window.

**Tweak timing:** Select a `.anim` in Project → Inspector shows **Animation Events** (editable on the copy). Move `OnAttackHit` to the visual impact frame.

### Manual workaround (duplicate clip yourself)

1. In Project, click the **arrow** next to an FBX (e.g. `Kachujin G Rosales@Standing Melee Attack Downward.fbx`).
2. Select the nested **AnimationClip** (not the FBX root).
3. **Ctrl+D** to duplicate → Unity creates a `.anim` you can edit.
4. Move the duplicate to `Assets/_DungeonCrawler/Animations/CombatClips/`.
5. Select the `.anim` → Inspector → **Animation Events** → **+**:
   - `OnAttackHit` at impact time
   - `OnAttackEnd` at end of clip
6. Open **Animator** → assign this `.anim` to **PrimaryAttack** / **SecondaryAttack** / **Attack** (replaces FBX sub-clip).

Function names must be exactly: **`OnAttackHit`** and **`OnAttackEnd`** (on `CombatAnimationEvents` at the Player/Enemy/Boss root).

### Prefab requirement

Add **`CombatAnimationEvents`** to **Player**, **Enemy**, and **Boss** prefab roots (same object as **Animator**).

Optional backup: **AttackStateBehaviour** on player attack states (unlocks movement if `OnAttackEnd` is missing).

### Player hurt

Add Animator trigger **`hurt`** on Player controller; transition from Any State (short hurt clip, no exit time blocking).

---

## 5. Animator parameters

### Player (`Character.controller`)

- Triggers: `primaryAttack`, `secondaryAttack`, `hurt`
- Bools: `isWalking`, `isRunning`

### Enemy (`Enemy.controller`)

- Triggers: `Attack`, **`Death`** (for death handler)
- Bools: `isPatrolling`, `isChasing`

---

## 6. Enemy prefab

`Assets/_DungeonCrawler/Scripts/Prefabs/Enemy.prefab`:

| Component | Notes |
|-----------|-------|
| `NavMeshAgent` | Speed 3–4, radius 0.5 |
| `EnemyFSM` | detection 10, attack 2.5 |
| `EnemyAttack` | damage, cooldown, reach, SFX |
| `EnemyMovement` | patrol points optional |
| `HealthComponent` | entity id e.g. `goblin` |
| `EnemyDeathHandler` | **Add**; death trigger `Death` |
| `CombatAnimationEvents` | **Add** |
| `KnockbackReceiver` | optional |

---

## 7. Boss prefab

`Boss.prefab`:

- `BossController` — assign `BossDataSO`, player, fire point, projectile.
- `EnemyAttack` — melee damage synced with boss data.
- `CombatAnimationEvents` — **Add**
- `EnemyDeathHandler` — **Add**
- `EnemyFSM` / `EnemyMovement` — auto-disabled by `BossController`
- Ensure boss starts on NavMesh; `waitForArenaActivation` = true until arena trigger fires.

---

## 8. Interactable gate

1. Gate root: `InteractableGate` + **blocking** BoxCollider (not trigger).
2. Child `InteractTrigger`: BoxCollider **Is Trigger**, sized in front of gate.
3. Move trigger logic: put `InteractableGate` on parent; add trigger collider on **same object** as script OR child with trigger — duplicate `OnTriggerStay` by adding trigger to gate root's second collider.

**Recommended hierarchy:**

```
GateRoot (InteractableGate, NavMeshObstacle)
├── DoorLeaf (mesh, blocking BoxCollider → disabled when open)
└── InteractZone (BoxCollider trigger only)
```

Assign `doorLeaf`, `closedEuler`, `openEuler`, `blockingCollider`.

**NavMesh:** Add `NavMeshObstacle` on closed door; disable when open.

---

## 9. Controls

| Input | Action |
|-------|--------|
| LMB | Primary attack |
| RMB | Secondary attack |
| Q / R | Previous / next weapon (equips from catalog) |
| T | Toggle equip / unequip current weapon |
| U | Unequip weapon |
| E | Interact (gates, NPCs, pickups) |
| WASD | Move (locked during attack) |
| Ctrl | Sprint |
| Dash ability key | Blocked during attack |

---

## 10. Testing checklist

- [ ] Player cannot walk through walls or leave boundary
- [ ] Attack locks movement until clip ends
- [ ] Hit SFX/knockback/damage occur on impact frame only
- [ ] Enemy damages player with cooldown
- [ ] Boss chases and attacks after arena activation
- [ ] Enemies play death, then despawn
- [ ] Gate opens once with E, pathing works after open
- [ ] Equipped weapon visible on hand and hitbox damages enemies
