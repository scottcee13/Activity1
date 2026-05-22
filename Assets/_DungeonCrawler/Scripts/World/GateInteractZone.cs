using UnityEngine;

namespace DungeonCrawler.World
{
    /// <summary>
    /// Place on a child with a trigger collider in front of the gate.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GateInteractZone : MonoBehaviour
    {
        [SerializeField] private InteractableGate gate;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Awake()
        {
            if (gate == null)
                gate = GetComponentInParent<InteractableGate>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (gate != null)
                gate.HandlePlayerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (gate != null)
                gate.HandlePlayerExit(other);
        }
    }
}
