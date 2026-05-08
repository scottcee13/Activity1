using UnityEngine;

public class DungeonSectionGate : MonoBehaviour
{
    [SerializeField] private GameObject gateVisual;
    [SerializeField] private Collider gateCollider;

    private bool isOpen;

    private void Reset()
    {
        gateCollider = GetComponent<Collider>();
        gateVisual = gameObject;
    }

    public void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;

        if (gateCollider != null) gateCollider.enabled = false;
        if (gateVisual != null) gateVisual.SetActive(false);
    }

    public void CloseGate()
    {
        isOpen = false;
        if (gateCollider != null) gateCollider.enabled = true;
        if (gateVisual != null) gateVisual.SetActive(true);
    }
}
