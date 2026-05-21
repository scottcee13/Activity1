using DungeonCrawler.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] protected HealthComponent target;
        [SerializeField] protected Image fillImage;

        protected virtual void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = player.GetComponent<HealthComponent>();
            }

            if (target != null)
                Bind(target);
        }

        protected virtual void OnDestroy()
        {
            if (target != null)
                target.OnHealthChanged -= UpdateBar;
        }

        public void SetTarget(HealthComponent newTarget)
        {
            if (target != null)
                target.OnHealthChanged -= UpdateBar;

            target = newTarget;

            if (target != null)
                Bind(target);
        }

        protected void Bind(HealthComponent health)
        {
            target = health;
            target.OnHealthChanged += UpdateBar;
            UpdateBar(target.CurrentHealth, target.MaxHealth);
        }

        protected void UpdateBar(int current, int max)
        {
            if (fillImage != null && max > 0)
                fillImage.fillAmount = (float)current / max;
        }
    }
}
