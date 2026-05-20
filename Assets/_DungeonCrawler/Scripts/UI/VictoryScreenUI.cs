using System.Text;
using DungeonCrawler.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class VictoryScreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(() =>
                    Core.GameManager.Instance?.RestartDungeon());
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() =>
                    Core.SceneFlowManager.Instance?.LoadMainMenu());
        }

        public void Populate()
        {
            if (summaryText == null) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Victory! The dungeon is cleared.");
            sb.AppendLine();

            if (QuestManager.Instance != null)
            {
                sb.AppendLine("— Quests —");
                foreach (QuestInstance q in QuestManager.Instance.GetAllQuests())
                {
                    string status = q.status.isCompleted ? "Complete" : "Incomplete";
                    sb.AppendLine($"• {q.data.questTitle}: {status}");
                }
            }

            if (InventoryManager.Instance != null)
            {
                sb.AppendLine();
                sb.AppendLine("— Items Collected —");
                foreach (ItemDataSO item in InventoryManager.Instance.Items)
                    sb.AppendLine($"• {item.displayName}");
            }

            summaryText.text = sb.ToString();
        }
    }
}
