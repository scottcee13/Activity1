# Quest UI — 5-minute setup

Do this **after the Console has zero errors**.

---

## Step 1 — Fix compile errors

If you see:

`GameManager does not exist` / `SceneFlowManager does not exist`

→ Already fixed in `PauseMenuController.cs`. Let Unity recompile (wait for spinner to finish).

---

## Step 2 — Find QuestUIManager

1. Open **DungeonCrawler** scene.
2. Hierarchy **search box** → type: `QuestUI`
3. Click the object that has component **Quest UIManager (Script)**.

Often it lives on the same object as **Quest List Content**, or on a parent like **QuestTrackerPanel**.

---

## Step 3 — Inspector checklist

On **Quest UIManager (Script)**:

```
Quest List Parent     → drag QuestListContent here
Quest Item Prefab     → QuestItem.prefab

Display
  Show Only Active Quest   ✓  (turn ON)
  Show Completed Quests    ✓  (leave on)

Layout
  (defaults are fine)
```

If you **do not** see **Display** or **Show Only Active Quest**:

- Console still has errors → fix those first.
- Or the wrong component is selected (e.g. only Vertical Layout Group).

---

## Step 4 — Required managers (same scene)

Select **`_GAME_MANAGERS`** and confirm:

- **Quest Manager** — 5 quest assets in list
- **Quest Chain Controller** — 5 quest IDs in order
- **Quest Event Bridge** — present (no fields)

---

## Step 5 — Play test

1. Play mode.
2. Talk to Guide (E).
3. Console: `[QuestUIManager] Refreshed 1 quest entries.`
4. HUD shows one line like: **► Speak to Guide**

If the log appears but HUD is empty → **Quest List Parent** is wrong or the panel is disabled in Hierarchy.

---

## Still stuck?

| Symptom | Fix |
|---------|-----|
| No Display section | Clear Console errors, reselect object |
| Refreshed 0 entries | Quest List Parent not assigned |
| Refreshed 1 but no text | Quest Item Prefab not assigned |
| Panel empty | Activate QuestTrackerPanel + QuestListContent |
