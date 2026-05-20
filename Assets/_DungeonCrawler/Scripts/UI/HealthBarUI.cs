using DungeonCrawler.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private HealthComponent target;
        [SerializeField] private Image fillImage;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.GetComponent<HealthComponent>();
            }

            if (target != null)
            {
                target.OnHealthChanged += UpdateBar;
                UpdateBar(target.CurrentHealth, target.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (target != null)
                target.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar(int current, int max)
        {
            if (fillImage != null && max > 0)
                fillImage.fillAmount = (float)current / max;
        }
    }
}
