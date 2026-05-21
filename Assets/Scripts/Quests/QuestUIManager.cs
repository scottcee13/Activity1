using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.Quests;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance { get; private set; }

    public Transform questListParent;
    public GameObject questItemPrefab;

    [Header("Display — check Show Only Active Quest for one current objective")]
    [Tooltip("When on, only the current chain quest appears in the tracker.")]
    public bool showOnlyActiveQuest = true;
    [Tooltip("When Show Only Active Quest is off, hide finished quests from the list.")]
    public bool showCompletedQuests = true;

    [Header("Layout")]
    [SerializeField] private float rowSpacing = 8f;
    [SerializeField] private float defaultRowHeight = 100f;

    private bool subscribed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(BindWhenReady());
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private IEnumerator BindWhenReady()
    {
        float timeout = 5f;
        while (QuestManager.Instance == null && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        Subscribe();
        ActivatePanel();
        EnsureListLayout();
        RefreshUI();
    }

    private void Subscribe()
    {
        if (subscribed || QuestManager.Instance == null) return;

        QuestManager.Instance.OnQuestUpdated += RefreshUI;
        subscribed = true;

        if (QuestChainController.Instance != null)
            QuestChainController.Instance.OnActiveQuestChanged += RefreshUI;
    }

    private void Unsubscribe()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= RefreshUI;

        if (QuestChainController.Instance != null)
            QuestChainController.Instance.OnActiveQuestChanged -= RefreshUI;

        subscribed = false;
    }

    private void ActivatePanel()
    {
        if (questListParent != null && !questListParent.gameObject.activeInHierarchy)
            questListParent.gameObject.SetActive(true);

        Transform panel = questListParent != null ? questListParent.parent : null;
        while (panel != null)
        {
            if (panel.name.Contains("Quest") || panel.name.Contains("HUD"))
            {
                if (!panel.gameObject.activeSelf)
                    panel.gameObject.SetActive(true);
                break;
            }
            panel = panel.parent;
        }
    }

    public void EnsureListLayout()
    {
        if (questListParent == null) return;

        RectTransform listRect = questListParent as RectTransform;
        if (listRect == null)
        {
            Debug.LogWarning("[QuestUIManager] questListParent must be a UI RectTransform.");
            return;
        }

        VerticalLayoutGroup layout = questListParent.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
            layout = questListParent.gameObject.AddComponent<VerticalLayoutGroup>();

        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = rowSpacing;
        layout.padding = new RectOffset(6, 6, 6, 6);

        ContentSizeFitter fitter = questListParent.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = questListParent.gameObject.AddComponent<ContentSizeFitter>();

        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        listRect.anchorMin = new Vector2(0f, 1f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.pivot = new Vector2(0.5f, 1f);
        listRect.anchoredPosition = Vector2.zero;
    }

    public void RefreshUI()
    {
        if (questListParent == null)
        {
            Debug.LogWarning("[QuestUIManager] questListParent is not assigned!");
            return;
        }

        if (questItemPrefab == null)
        {
            Debug.LogWarning("[QuestUIManager] questItemPrefab is not assigned!");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("[QuestUIManager] QuestManager.Instance is null.");
            return;
        }

        if (!subscribed)
            Subscribe();

        EnsureListLayout();

        for (int i = questListParent.childCount - 1; i >= 0; i--)
            Destroy(questListParent.GetChild(i).gameObject);

        string activeId = QuestChainController.Instance != null
            ? QuestChainController.Instance.ActiveQuestId
            : null;

        List<QuestInstance> quests = QuestManager.Instance.GetAllQuests();

        foreach (QuestInstance quest in quests)
        {
            if (showOnlyActiveQuest && activeId != null && quest.data.questID != activeId)
                continue;

            if (!showCompletedQuests && quest.status.isCompleted)
                continue;

            GameObject item = Instantiate(questItemPrefab, questListParent);
            ConfigureQuestRow(item);

            QuestUIItem uiItem = item.GetComponent<QuestUIItem>();
            if (uiItem != null)
            {
                bool isActive = quest.data.questID == activeId;
                uiItem.Setup(quest, isActive);
            }
        }

        if (questListParent is RectTransform listRect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(listRect);

        Canvas.ForceUpdateCanvases();
    }

    private void ConfigureQuestRow(GameObject row)
    {
        RectTransform rowRect = row.GetComponent<RectTransform>();
        if (rowRect != null)
        {
            rowRect.localScale = Vector3.one;
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.sizeDelta = new Vector2(0f, defaultRowHeight);
        }

        LayoutElement layoutElement = row.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = row.AddComponent<LayoutElement>();

        if (layoutElement.preferredHeight < 60f)
            layoutElement.preferredHeight = defaultRowHeight;
    }
}
