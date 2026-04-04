using UnityEngine;

public class ExplorationTracker : MonoBehaviour
{
    public float stepDistance = 1f;

    private Vector3 lastPosition;
    private float distanceAccumulated;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        distanceAccumulated += distance;
        lastPosition = transform.position;

        while (distanceAccumulated >= stepDistance)
        {
            distanceAccumulated -= stepDistance;
            QuestManager.Instance.AddProgress(ObjectiveType.Exploration, 1);
            Debug.Log("Exploration step +1");
        }
    }
}