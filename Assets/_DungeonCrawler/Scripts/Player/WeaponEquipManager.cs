using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Player
{
    public class WeaponEquipManager : MonoBehaviour
    {
        [Header("Socket")]
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private string[] handBoneNames = { "mixamorig:RightHand", "RightHand", "Hand_R", "hand_r" };

        [Header("Start")]
        [SerializeField] private bool equipStartingWeaponOnStart;

        private GameObject equippedInstance;
        private EquippedWeapon equippedWeaponComponent;
        private WeaponData currentData;

        public WeaponData CurrentWeapon => currentData;
        public bool IsWeaponEquipped => currentData != null;

        private void Awake()
        {
            ResolveSocket();
        }

        private void OnEnable()
        {
            GameEvents.OnWeaponEquipped += EquipFromEvent;
        }

        private void OnDisable()
        {
            GameEvents.OnWeaponEquipped -= EquipFromEvent;
        }

        private void Start()
        {
            if (equipStartingWeaponOnStart)
            {
                WeaponData start = GetComponent<PlayerWeapon>()?.GetSlotWeapon(0);
                if (start != null)
                    Equip(start);
            }
        }

        public void ConfigureStartUnequipped()
        {
            equipStartingWeaponOnStart = false;
            Unequip();
        }

        private void EquipFromEvent(WeaponData data)
        {
            ApplyEquip(data);
        }

        public void Equip(WeaponData data)
        {
            if (!ApplyEquip(data)) return;
            GameEvents.RaiseWeaponEquipped(data);
        }

        private bool ApplyEquip(WeaponData data)
        {
            if (data == null) return false;
            if (currentData == data && IsWeaponEquipped) return false;

            UnequipVisualOnly();
            currentData = data;

            if (data.weaponPrefab != null && weaponSocket != null)
            {
                equippedInstance = Instantiate(data.weaponPrefab, weaponSocket);
                equippedInstance.transform.localPosition = data.gripLocalPosition;
                equippedInstance.transform.localRotation = Quaternion.Euler(data.gripLocalEuler);
                StripPhysicsFromWeaponVisual(equippedInstance);

                equippedWeaponComponent = equippedInstance.GetComponent<EquippedWeapon>();
                if (equippedWeaponComponent == null)
                    equippedWeaponComponent = equippedInstance.AddComponent<EquippedWeapon>();

                equippedWeaponComponent.Initialize(data);
            }

            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
                combat.BindEquippedWeapon(equippedWeaponComponent, data);

            return true;
        }

        public void Unequip()
        {
            if (!IsWeaponEquipped) return;

            UnequipVisualOnly();
            currentData = null;

            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
                combat.ClearEquippedWeapon();

            GameEvents.RaiseWeaponUnequipped();
        }

        public void ToggleEquip(WeaponData data)
        {
            if (IsWeaponEquipped)
            {
                Unequip();
                return;
            }

            if (data != null)
                Equip(data);
        }

        private void UnequipVisualOnly()
        {
            if (equippedInstance != null)
                Destroy(equippedInstance);

            equippedInstance = null;
            equippedWeaponComponent = null;
        }

        private static void StripPhysicsFromWeaponVisual(GameObject weaponRoot)
        {
            foreach (Rigidbody rb in weaponRoot.GetComponentsInChildren<Rigidbody>(true))
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }

            foreach (Projectile projectile in weaponRoot.GetComponentsInChildren<Projectile>(true))
                Destroy(projectile);

            foreach (Collider col in weaponRoot.GetComponentsInChildren<Collider>(true))
            {
                WeaponHitbox hitbox = col.GetComponent<WeaponHitbox>();
                if (hitbox == null)
                    col.enabled = false;
            }
        }

        private void ResolveSocket()
        {
            if (weaponSocket != null) return;

            Animator animator = GetComponentInChildren<Animator>();
            if (animator == null) return;

            foreach (string boneName in handBoneNames)
            {
                Transform bone = animator.transform.Find(boneName);
                if (bone == null)
                    bone = FindDeepChild(animator.transform, boneName);

                if (bone != null)
                {
                    weaponSocket = bone;
                    break;
                }
            }

            if (weaponSocket == null)
            {
                GameObject holder = new GameObject("WeaponSocket");
                holder.transform.SetParent(animator.transform, false);
                holder.transform.localPosition = new Vector3(0.25f, 1.05f, 0.15f);
                holder.transform.localRotation = Quaternion.identity;
                weaponSocket = holder.transform;
            }
        }

        private static Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform found = FindDeepChild(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
