# 5-Room Dungeon Crawler (Unity)

Modular dungeon crawler built for **Activity1**. All new systems live under `Assets/_DungeonCrawler/`.

## Quick Start

1. **Fixing bugs?** Read `Documentation/DEBUG_AND_FIX_GUIDE.md` first.
2. Read `Documentation/ARCHITECTURE.md` for system design.
3. Follow `Documentation/SETUP_GUIDE.md` to wire scenes and prefabs.
3. Add `_GAME_MANAGERS` and **Player** prefab components listed in the setup guide.
4. Create ScriptableObjects (`Dungeon/` menu) for items, dialogue, abilities, boss.
5. Build **DungeonCrawler** scene with 5 room trigger zones.

## Controls (default)

| Key | Action |
|-----|--------|
| WASD | Move |
| Mouse | Look |
| Space | Jump |
| Q | Dash |
| F | Projectile |
| LMB | Melee |
| E | Interact / Pickup |
| Tab | Inventory |
| Esc | Pause |

## Legacy Integration

Existing scripts (`QuestManager`, `PlayerInventory`, `SaveSystem`, `AudioManager`) remain in `Assets/Scripts` and `Assets/ScriptableObjects`. New code connects via `GameEvents` and `QuestEventBridge`.
