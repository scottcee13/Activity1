using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    [SerializeField]
    private GameObject mainCamera;
    private Vector3 camOriginPos;

    [SerializeField]
    private float shakeIntensity = 1.01f;
    private bool isCameraShaking = false;

    public GameObject damagePanel;

    public Slider healthBar;

    private void Start()
    {
        camOriginPos = mainCamera.transform.position;
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
        mainCamera.transform.position = camOriginPos + (Random.insideUnitSphere * shakeIntensity);
        yield return new WaitForSeconds(0.25f);
        mainCamera.transform.position = camOriginPos;
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
