# Combat, Audio, Game Over & Victory — Quick Setup

---

## NavMesh boss error (FIXED)

**Error:** `SetDestination can only be called on an active agent that has been placed on a NavMesh`

**Cause:** Boss tried to move before being on a NavMesh (or before arena activation).

**Fix in code:**
- `BossController` waits for `ActivateBoss()` (called from `BossArenaTrigger`)
- Only calls `SetDestination` when `agent.isOnNavMesh`

**You must also:**
1. Bake NavMesh with **Nav Mesh Surface** (see **NAVMESH_BAKE_GUIDE.md** — NOT Window → AI → Navigation)
2. Boss arena trigger calls `ActivateBoss()` when player enters

**If boss is always active in scene (no trigger):**  
On Boss → **Boss Controller** → uncheck **Wait For Arena Activation**

---

## Knockback

Added automatically when anything takes damage with force:

- Player hits enemy → `WeaponHitbox` knockback
- Enemy hits player → `EnemyAttack` knockback
- Boss melee → knockback

Optional: add **Knockback Receiver** to Player prefab (auto-added on first hit if missing).

Tune on **Enemy Attack** / **Weapon Hitbox** / **Boss Controller** → **Knockback Force**.

---

## Player not damaged by enemies (FIXED)

Damage now goes through **CombatDamage** which hits:
1. `HealthComponent` on Player (preferred)
2. `PlayerHealth` legacy fallback

**Checklist:**
- Player **Tag** = `Player`
- Player has **Health Component** with **Is Player** ✓
- Enemy gets within **Attack Reach** (default 2.5m)
- Remove test invuln: don't hold damage off in inspector

---

## Game Over panel

When health reaches 0:
- `GameManager` pauses game
- `UIManager` shows **GameOverUI** panel
- Buttons auto-wire if named `RestartButton` / `Main Menu`

**UIManager** on `_GAME_MANAGERS` now has **Game Over Panel** → `GameOverUI` (wired in scene).

Add child buttons under GameOverUI if missing:
- `RestartButton` → reloads dungeon
- `MainMenuButton` → loads MainMenu scene

---

## Victory after boss

1. Add **Boss Victory Dialogue** to `_GAME_MANAGERS` (optional ending dialogue SO)
2. If **no** ending dialogue assigned → victory panel shows **immediately** on boss death
3. If dialogue assigned → victory after dialogue ends

**Victory panel buttons** (auto-found):
- `RestartButton` → play dungeon again
- Main menu button → `MainMenu` scene

Ensure **Scene Flow Manager** is on `_GAME_MANAGERS` with:
- Main Menu Scene Name: `MainMenu`
- Dungeon Scene Name: `DungeonCrawler`

---

## Where to attach music clips

### Option A — Recommended: `LevelAudioSetup`

1. Select **`_GAME_MANAGERS`** (or create empty `LevelAudio`)
2. **Add Component** → **Level Audio Setup** (`DungeonCrawler.Core`)
3. Assign:
   - **Background Music** → your BGM `.mp3`
   - **Ambient Loop** → forest/wind loop
   - **Intro Voice** (optional)

Plays automatically on scene start when **AudioManager** exists.

### Option B — Existing `LevelAudioStarter`

Same fields on `LevelAudioStarter` component (`bgm`, `ambient`).

### Option C — From code

```csharp
AudioManager.Instance.PlayMusic(myClip);
AudioManager.Instance.PlaySFX(hitClip);
```

Clips drag into inspector fields on your scripts, then pass to Play methods.

**AudioManager** must be in scene with **Audio Mixer** assigned. Sources auto-create on Play.

---

## Boss arena wiring

```
BossArenaTrigger (trigger volume)
  Boss Root → Boss object (can start disabled)
  Boss Health Bar → BossHealthBar UI
  Arena Gate → optional door blocker
```

On player enter → boss activates + NavMesh warp + health bar shows.

---

## Test checklist

- [ ] Enemy in attack range damages player; player HP bar drops
- [ ] Player dies → GameOverUI appears; Restart works
- [ ] Boss dies → VictoryPanel appears
- [ ] Victory Main Menu → loads MainMenu
- [ ] No NavMesh errors in Console
- [ ] BGM plays on level start after clips assigned
