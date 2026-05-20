using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    [SerializeField]
    private GameObject mainCamera;

    [SerializeField]
    private float shakeIntensity = 1.01f;
    private bool isCameraShaking = false;

    public GameObject damagePanel;

    public Slider healthBar;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main != null ? Camera.main.gameObject : null;

        damagePanel.SetActive(false);

        GameObject sliderObj = GameObject.Find("HealthBar");

        if (sliderObj != null)
        {
            healthBar = sliderObj.GetComponent<Slider>();
            healthBar.value = 1f;
        }
    }

    public void Update()
    {
        if (isCameraShaking)
        {
            StartCoroutine(CameraShakeRoutine());
        }
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += DamagedUI;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= DamagedUI;
    }

    IEnumerator CameraShakeRoutine()
    {
        // Orbit cameras (ThirdPersonCamera) control position each LateUpdate — only shake local offset.
        Transform cam = mainCamera.transform;
        Vector3 localOrigin = cam.localPosition;
        cam.localPosition = localOrigin + (Random.insideUnitSphere * shakeIntensity);
        yield return new WaitForSeconds(0.25f);
        cam.localPosition = localOrigin;
        isCameraShaking = false;
    }

    IEnumerator RedScreenRoutine()
    {
        damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        damagePanel.SetActive(false);
    }

    private void DamagedUI(int dmg)
    {
        isCameraShaking = true;
        StartCoroutine(RedScreenRoutine());
        healthBar.value = Mathf.Clamp(healthBar.value - dmg * 0.01f, 0f, 1f);
    }
}
