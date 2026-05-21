# How to Bake NavMesh (Unity AI Navigation 2.x)

Your project uses **`com.unity.ai.navigation` 2.0** — the old **Window → AI → Navigation** menu is **gone**. Baking is done with a **Nav Mesh Surface** component.

---

## Step 1 — Confirm package is installed

1. **Window → Package Manager**
2. Search: **AI Navigation**
3. Status should be **Installed** (your project already has `com.unity.ai.navigation` 2.0.9)

If not installed → **Install**.

---

## Step 2 — Create walkable floor object

1. In **DungeonCrawler** scene, select your **floor** (Plane, terrain parent, or dungeon ground).
2. If the whole city is many objects, create an empty parent:
   - Right-click Hierarchy → **Create Empty** → name `NavMeshFloor`
   - Drag all **walkable** meshes under it (ground, floors, bridges — not walls/roofs).

---

## Step 3 — Add Nav Mesh Surface

**Method A (menu):**

1. Select `NavMeshFloor` (or your floor object).
2. **GameObject → AI → Nav Mesh Surface**

**Method B (component):**

1. Select floor object.
2. **Add Component** → search **Nav Mesh Surface**

---

## Step 4 — Bake

1. With the object selected, find **Nav Mesh Surface** in the Inspector.
2. Settings (defaults usually fine):
   - **Agent Type**: Humanoid
   - **Collect Objects**: All or Children (if using parent)
   - **Include Layers**: Default (walkable geometry)
3. Click **Bake** at the bottom of the component.

You should see a **blue overlay** on the ground in the Scene view.

---

## Step 5 — Verify

1. Place **Enemy** or **Boss** on the blue area.
2. Press Play — no `SetDestination` / `isStopped` NavMesh errors.
3. Enemy should chase player on the mesh.

---

## Boss setup (with arena trigger)

1. Boss can start **disabled** in Hierarchy.
2. **Boss Controller** → **Wait For Arena Activation** ✓
3. **Boss Arena Trigger** on doorway volume:
   - **Boss Root** → boss object
   - Player walks in → boss activates + warps to NavMesh

**No arena trigger?** Uncheck **Wait For Arena Activation** on boss and ensure boss stands on blue NavMesh area.

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| No **AI** menu under GameObject | Install **AI Navigation** package |
| Bake does nothing | Floor needs MeshRenderer + MeshFilter; check **Collect Objects** |
| Blue mesh missing | Scene view Gizmos on; NavMesh visible toggle in Surface component |
| Agent still off mesh | Move spawn point onto blue area; rebake after moving geometry |
| Holes in mesh | Add more floor objects under same Surface; Bake again |

---

## Quick test object

1. **3D Object → Plane** (scale 10,1,10)
2. Add **Nav Mesh Surface** → **Bake**
3. Drop **Enemy** prefab on plane center → Play → should patrol/chase.
