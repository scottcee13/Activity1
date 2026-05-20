using UnityEngine;

/// <summary>
/// Orbits around the player via cameraPivot. Rig stays behind the player (no snap-to-feet jitter).
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Transform cameraPivot;
    public float mouseSensitivity = 200f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float followDistance = 5f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraPivot != null && cameraPivot.GetComponent<DungeonCrawler.Player.PlayerAimProvider>() == null)
            cameraPivot.gameObject.AddComponent<DungeonCrawler.Player.PlayerAimProvider>();
    }

    void LateUpdate()
    {
        if (target == null || cameraPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraPivot.position = target.position + Vector3.up * 1.6f;
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            Transform camT = cam.transform;
            camT.position = cameraPivot.position - cameraPivot.forward * followDistance;
            camT.rotation = Quaternion.LookRotation(cameraPivot.forward, Vector3.up);
        }
    }
}
