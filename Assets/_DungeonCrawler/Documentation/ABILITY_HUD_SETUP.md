# Ability Icons & Cooldown HUD — Setup Guide

## How it works

```
Ability Data SO (icon, abilityId, cooldown)
        ↓ assigned on
DashAbility / JumpAbility / ProjectileShootAbility (on Player)
        ↓ listed in
AbilityManager → Abilities list
        ↓ read by
AbilityCooldownHUD (on Canvas) — matches abilityId to UI slots
```

When you press a key, `AbilityManager` calls `TryActivate()` → cooldown runs → **cooldown overlay** fills on the matching icon.

**Default keys**

| abilityId | Key |
|-----------|-----|
| `jump` | Space |
| `dash` | Q |
| `projectile` | F |

`abilityId` strings are **case-sensitive** and must match exactly between Ability Data SO and HUD slots.

---

## Part 1 — Player abilities (required)

Select your **Player** (scene instance or `Assets/_DungeonCrawler/Scripts/Prefabs/Player.prefab`).

### A. Ability Manager

- Component: **Ability Manager**
- **Abilities** size = **3**
- Drag these components from the same Player object:
  1. **Dash Ability**
  2. **Jump Ability**
  3. **Projectile Shoot Ability**

### B. Each ability component — assign Data

| Component | Data field → asset |
|-----------|-------------------|
| Dash Ability | `Ability_Dash` |
| Jump Ability | `Ability_Jump` |
| Projectile Shoot Ability | `Ability_Projectile` |

Path: `Assets/_DungeonCrawler/Scripts/ScriptableObjects/Abilities/`

If **Data** is empty, abilities won't activate and icons won't load.

### C. Projectile only

- **Fire Point** → child transform in front of chest
- **Projectile Prefab** → your projectile prefab

---

## Part 2 — Ability icons (ScriptableObjects)

For each asset (`Ability_Dash`, `Ability_Jump`, `Ability_Projectile`):

1. Select the asset in Project window.
2. Set **Ability Id**: `dash`, `jump`, `projectile` (lowercase).
3. Set **Cooldown** (e.g. Dash = 2, Jump = 0.5, Projectile = 1).
4. **Icon** → drag a **Sprite** (PNG with Texture Type = Sprite).

No icon assigned = empty square in HUD (ability still works).

---

## Part 3 — HUD layout (Canvas)

Create under your **Canvas → HUD**:

```
AbilityBar (empty RectTransform, anchor bottom-center)
├── Slot_Dash
│   ├── Icon (Image)              ← ability icon sprite shown here
│   ├── CooldownOverlay (Image)   ← dark fill ON TOP, Image Type = Filled
│   └── KeyHint (TMP) "Q"         ← optional
├── Slot_Jump
│   ├── Icon
│   ├── CooldownOverlay
│   └── KeyHint "Space"
└── Slot_Projectile
    ├── Icon
    ├── CooldownOverlay
    └── KeyHint "F"
```

### Icon Image

- **Source Image**: any sprite (overwritten at runtime from Ability Data)
- **Preserve Aspect**: on
- Size: ~64×64

### Cooldown Overlay Image

- Same rect as Icon, **stretch full slot**
- Color: black, alpha ~0.7
- **Image Type**: **Filled**
- **Fill Method**: **Radial 360** (clock wipe) or **Vertical** (top-down)
- **Fill Origin**: Top or Left
- Start with **Fill Amount** = 0

### Ability Bar root

- Add component: **Ability Cooldown HUD**
- **Slots** size = 3:

| abilityId | icon | cooldownOverlay | keyHintText |
|-----------|------|-----------------|-------------|
| `dash` | Slot_Dash/Icon | Slot_Dash/CooldownOverlay | Slot_Dash/KeyHint |
| `jump` | Slot_Jump/Icon | ... | ... |
| `projectile` | Slot_Projectile/Icon | ... | ... |

**abilityId must match** the SO exactly (`jump` not `Jump`).

---

## Part 4 — Test

1. Press Play.
2. Console should show:
   - `[AbilityCooldownHUD] Bound slot 'dash'...`
   - (same for jump, projectile)
3. Press **Q** → dash + overlay fills then empties.
4. Press **Space** → jump.
5. Press **F** → projectile (if prefab assigned).

### Troubleshooting

| Problem | Fix |
|---------|-----|
| No icons | Assign **Icon** sprite on each Ability Data SO |
| Cooldown never shows | Cooldown Overlay must be **Filled** Image; assign in HUD slot |
| Ability does nothing | **Data** missing on ability component; check **Ability Id** spelling |
| HUD never binds | **AbilityManager** on Player; abilities list has 3 entries |
| Jump doesn't work | abilityId must be `jump` (lowercase) |

---

## Scene vs prefab

If you edit the **prefab** but play with a **scene Player** copy, update the **scene Player** too (or replace with prefab instance).
