using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // player
    public Transform cameraPivot;     // child pivot
    public float mouseSensitivity = 200f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Follow player position
        transform.position = target.position;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate camera around player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}