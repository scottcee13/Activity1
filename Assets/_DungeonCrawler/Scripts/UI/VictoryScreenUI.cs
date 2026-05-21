using System.Text;
using DungeonCrawler.Core;
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

        private void Awake()
        {
            AutoFindReferences();
            gameObject.SetActive(false);
            WireButtons();
        }

        private void AutoFindReferences()
        {
            if (summaryText == null)
                summaryText = GetComponentInChildren<TMP_Text>(true);

            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                string n = btn.name.ToLowerInvariant();
                if (restartButton == null && (n.Contains("restart") || n.Contains("play")))
                    restartButton = btn;
                if (mainMenuButton == null && (n.Contains("main") || n.Contains("menu") || n.Contains("quit")))
                    mainMenuButton = btn;
            }
        }

        private void WireButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnPlayAgain);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(OnMainMenu);
            }
        }

        public void Populate()
        {
            AutoFindReferences();
            WireButtons();

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

        private void OnPlayAgain()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.RestartDungeon();
        }

        private void OnMainMenu()
        {
            Time.timeScale = 1f;
            SceneFlowManager.Instance?.LoadMainMenu();
        }
    }
}
