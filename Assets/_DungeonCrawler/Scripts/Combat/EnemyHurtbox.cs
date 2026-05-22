using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Scaled trigger hurtbox so attacks register on large enemies (CharacterController does not scale with transform).
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyHurtbox : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider hurtCollider;
        [SerializeField] private float heightMultiplier = 2f;
        [SerializeField] private float radiusMultiplier = 0.55f;

        private void Awake()
        {
            BuildHurtbox();
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            BuildHurtbox();
        }

        public void BuildHurtbox()
        {
            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            scale = Mathf.Max(1f, scale);

            if (hurtCollider == null)
            {
                hurtCollider = GetComponent<CapsuleCollider>();
                if (hurtCollider == null)
                    hurtCollider = gameObject.AddComponent<CapsuleCollider>();
            }

            hurtCollider.isTrigger = true;
            hurtCollider.direction = 1;
            hurtCollider.height = heightMultiplier * scale;
            hurtCollider.radius = radiusMultiplier * scale;
            hurtCollider.center = new Vector3(0f, hurtCollider.height * 0.5f, 0f);

            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.height = hurtCollider.height;
                cc.radius = hurtCollider.radius;
                cc.center = hurtCollider.center;
            }
        }
    }
}
