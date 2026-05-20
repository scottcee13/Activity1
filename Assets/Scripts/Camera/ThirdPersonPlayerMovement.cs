using UnityEngine;

public class ThirdPersonPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public CharacterController controller;
    public Transform cameraRoot;

    void Update()
    {
        if (GetComponent<DungeonCrawler.Player.PlayerMotor>() != null)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // Camera-relative movement
            Vector3 camForward = cameraRoot.forward;
            Vector3 camRight = cameraRoot.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

            // Move player
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Smoothly rotate player toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}