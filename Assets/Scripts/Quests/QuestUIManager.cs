using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public Transform questListParent;
    public GameObject questItemPrefab;

    [Header("Layout (auto-applied to Quest List Parent if missing)")]
    [SerializeField] private float rowSpacing = 8f;
    [SerializeField] private float defaultRowHeight = 100f;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += RefreshUI;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= RefreshUI;
    }

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        float timeout = 3f;
        while (QuestManager.Instance == null && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (questListParent != null && !questListParent.gameObject.activeInHierarchy)
            questListParent.gameObject.SetActive(true);

        EnsureListLayout();
        RefreshUI();
    }

    /// <summary>
    /// Quest rows stack on top of each other without a Vertical Layout Group on the parent.
    /// </summary>
    public void EnsureListLayout()
    {
        if (questListParent == null) return;

        RectTransform listRect = questListParent as RectTransform;
        if (listRect == null)
        {
            Debug.LogWarning("[QuestUIManager] questListParent must be a UI RectTransform (e.g. QuestListContent).");
            return;
        }

        VerticalLayoutGroup layout = questListParent.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
        {
            layout = questListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            Debug.Log("[QuestUIManager] Added Vertical Layout Group to quest list parent.");
        }

        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = rowSpacing;
        layout.padding = new RectOffset(6, 6, 6, 6);

        ContentSizeFitter fitter = questListParent.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = questListParent.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

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
            Debug.LogWarning("[QuestUIManager] QuestManager.Instance is null — cannot refresh.");
            return;
        }

        EnsureListLayout();

        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        List<QuestInstance> quests = QuestManager.Instance.GetAllQuests();

        foreach (QuestInstance quest in quests)
        {
            GameObject item = Instantiate(questItemPrefab, questListParent);
            ConfigureQuestRow(item);
            QuestUIItem uiItem = item.GetComponent<QuestUIItem>();
            if (uiItem != null)
                uiItem.Setup(quest);
        }

        RectTransform listRect = questListParent as RectTransform;
        if (listRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(listRect);

        Debug.Log($"[QuestUIManager] Refreshed {quests.Count} quest entries.");
    }

    private void ConfigureQuestRow(GameObject row)
    {
        RectTransform rowRect = row.GetComponent<RectTransform>();
        if (rowRect != null)
        {
            rowRect.localScale = Vector3.one;
            rowRect.localRotation = Quaternion.identity;
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = Vector2.zero;
            rowRect.sizeDelta = new Vector2(0f, defaultRowHeight);
        }

        LayoutElement layoutElement = row.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = row.AddComponent<LayoutElement>();

        if (layoutElement.preferredHeight < 60f)
            layoutElement.preferredHeight = defaultRowHeight;
    }
}
