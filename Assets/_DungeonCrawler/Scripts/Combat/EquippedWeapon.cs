using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Runtime instance on a spawned weapon model. Holds hitbox reference and stats.
    /// </summary>
    public class EquippedWeapon : MonoBehaviour
    {
        [SerializeField] private WeaponHitbox hitbox;

        public WeaponData Data { get; private set; }

        public WeaponHitbox Hitbox
        {
            get
            {
                if (hitbox == null)
                    hitbox = GetComponentInChildren<WeaponHitbox>(true);
                return hitbox;
            }
        }

        public void Initialize(WeaponData data)
        {
            Data = data;
            WeaponHitbox hb = Hitbox;
            if (hb == null || data == null) return;

            hb.SetDamage(data.weaponDamage);
            hb.SetKnockback(data.knockbackForce);
            if (data.hitSfx != null)
                hb.SetHitSfx(data.hitSfx);
        }
    }
}
