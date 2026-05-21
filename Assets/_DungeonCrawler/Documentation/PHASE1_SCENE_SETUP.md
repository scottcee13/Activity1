# Phase 1 — Scene wiring (15–20 min)

Code for the linear quest chain, targeted events, dialogue routing, key item, and inventory save is in place. Finish these steps in **DungeonCrawler** scene.

---

## 1. `_GAME_MANAGERS`

1. Select **`_GAME_MANAGERS`**.
2. Add **Game Systems Bootstrap** (if missing).
3. Add **Quest Chain Controller** (if not auto-added on play).
   - **Quest Ids In Order** should list:
     - `tutorial_talk`
     - `tutorial_move`
     - `tutorial_combat`
     - `puzzle_key`
     - `boss_slain`
4. On **Inventory Manager** → assign **Item Registry**:
   - `Assets/_DungeonCrawler/Scripts/ScriptableObjects/Items/ItemRegistry.asset`

---

## 2. Guide NPC (`NPCInteractable`)

1. Select your Guide NPC.
2. **NPC Interactable**:
   - **Default Dialogue** → `Dialogue_TutorialGuide`
   - **Quest Dialogues** → Size 1:
     - Quest Id: `tutorial_talk`
     - When Quest Complete: **off**
     - Dialogue: `Dialogue_TutorialGuide`
3. Optional second entry after talk quest done:
   - Quest Id: `tutorial_talk`, **When Quest Complete: on**, different dialogue SO

---

## 3. Walk-to-location trigger

1. Create empty **`QuestZone_WalkPlaza`** where the player should go.
2. Add **Box Collider** → **Is Trigger** ✓.
3. Add **Quest Objective Trigger**:
   - **Objective Id**: `walk_to_plaza`

---

## 4. First enemy

1. Select the **first tutorial enemy** in the scene (not every enemy).
2. Add **Quest Entity Marker**:
   - **Entity Id**: `first_enemy`
3. Ensure it has **Health Component** (Enemy prefab already does).

---

## 5. Key pickup

1. Create **`KeyPickup`** (mesh optional).
2. **Box Collider** → **Is Trigger** ✓.
3. **Item Pickup** → **Item**: `Item_AncientKey`
4. Place in scene where the player finds the key.

---

## 6. Boss

1. Select **Boss** in scene or open **Boss.prefab**.
2. Add **Quest Entity Marker** → **Entity Id**: `dungeon_boss`  
   *(or set Health Component entity id directly)*

---

## 7. Interact prompt UI (optional but recommended)

Under **Canvas**:

```
InteractPromptPanel (inactive)
└── PromptText (TMP) "Press E to interact"
```

Add **Interact Prompt UI** on the panel; assign Panel + Prompt Text.

NPCs/pickups can leave local prompt empty to use this shared UI.

---

## 8. Test order

1. Play → talk to Guide (E) → quest **Speak to Guide** completes after dialogue ends.
2. Walk into **walk_to_plaza** zone → **Learn Movement** completes.
3. Kill enemy with **first_enemy** marker → **First Blood** completes.
4. Pick up key (E) → **Find the Key** completes.
5. Kill boss (**dungeon_boss**) → **Defeat the Guardian** completes.

Console logs: `[QuestChain] Active quest: ...` and `[QuestChain] Completed: ...`

---

## 9. Save test

- Collect key → trigger your save (checkpoint / manual).
- Stop Play → Play again → key should still be in inventory (Tab) if **Item Registry** is assigned on **Inventory Manager**.

---

## Troubleshooting

| Issue | Fix |
|-------|-----|
| Wrong quest completes | Check only one quest is active; verify target ids on quest SOs |
| Talk quest never completes | `Dialogue_TutorialGuide` dialogueId must be `tutorial_intro` |
| Walk quest never completes | Trigger objective id must be `walk_to_plaza` |
| Key quest on wrong pickup | Item `itemId` must be `ancient_key` |
| Boss quest completes on any kill | Boss needs `dungeon_boss` entity id |
