using System.Collections.Generic;
using UnityEngine;

public class DungeonProgressManager : MonoBehaviour
{
    public static DungeonProgressManager Instance { get; private set; }

    [SerializeField] private List<DungeonSectionGate> sectionGates = new List<DungeonSectionGate>();
    [SerializeField] private int currentSectionIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int CurrentSectionIndex => currentSectionIndex;

    public void UnlockNextSection()
    {
        if (currentSectionIndex >= sectionGates.Count) return;

        DungeonSectionGate gate = sectionGates[currentSectionIndex];
        if (gate != null) gate.OpenGate();
        currentSectionIndex++;
    }
}
