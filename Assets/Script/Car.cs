using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Car : MonoBehaviour
{
    [Header("Thông số xe")]
    [SerializeField] private float tocDoXe = 15000f;
    [SerializeField] private float lucReXe = 300f;
    [SerializeField] private GameObject phanhEffect;
    [SerializeField] private Joystick joystick;

    [Header("Âm thanh xe")]
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource brakeSound;
    [SerializeField] private float minPitch = 0.2f;
    [SerializeField] private float maxPitch = 0.5f;

    [Header("UI Buttons")]
    [SerializeField] private Button gasButton;
    [SerializeField] private Button brakeButton;

    [Header("Power-Up Status")]
    [SerializeField] private TextMeshProUGUI powerUpStatusText; // Tham chiếu đến Text UI

    private Rigidbody rb;
    private float dauVaoDiChuyen;
    private float dauVaoRe;
    private bool isAccelerating = false;
    private bool isBraking = false;

    // Biến đơn giản cho power-up
    private string activePowerUp = ""; // Loại power-up hiện tại
    private float powerUpDuration = 0f; // Thời gian còn lại

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("⚠️ Rigidbody không được gắn vào GameObject này!");
            enabled = false;
            return;
        }

        if (joystick == null) Debug.LogWarning("⚠️ Joystick chưa gán.");
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.volume = 0.1f;
            engineSound.pitch = minPitch;
            engineSound.Play();
        }
        if (brakeSound != null) brakeSound.loop = false;

        if (gasButton != null) gasButton.onClick.AddListener(() => { });
        if (brakeButton != null) brakeButton.onClick.AddListener(() => { });
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A)) isAccelerating = true; if (Input.GetKeyUp(KeyCode.A)) isAccelerating = false; if (Input.GetKeyDown(KeyCode.S)) isBraking = true; if (Input.GetKeyUp(KeyCode.S)) isBraking = false;
        // Cập nhật và hiển thị trạng thái power-up
        UpdatePowerUpStatus();
    }

    private void FixedUpdate()
    {
        dauVaoRe = joystick != null ? joystick.Horizontal : 0f;
        dauVaoDiChuyen = joystick != null ? joystick.Vertical : 0f;
        if (isAccelerating) dauVaoDiChuyen = 1f;
        if (isBraking) dauVaoDiChuyen = -1f;

        DiChuyenXe();
        ReXe();
        if (isBraking) PhanhXe();
        else
        {
            phanhEffect.SetActive(false);
            FadeOutBrakeSound();
        }
        UpdateEngineSound();
    }

    #region Di chuyển xe
    private void DiChuyenXe() { if (rb != null) rb.AddRelativeForce(Vector3.forward * dauVaoDiChuyen * tocDoXe * Time.fixedDeltaTime); }
    private void ReXe() { if (rb != null) { Quaternion re = Quaternion.Euler(Vector3.up * dauVaoRe * lucReXe * Time.fixedDeltaTime); rb.MoveRotation(rb.rotation * re); } }
    #endregion

    #region Phanh
    private void PhanhXe()
    {
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.02f);
            phanhEffect.SetActive(true);
            FadeInBrakeSound();
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            phanhEffect.SetActive(false);
            FadeOutBrakeSound();
        }
    }
    private void FadeInBrakeSound() { if (brakeSound != null && !brakeSound.isPlaying) { brakeSound.volume = 0f; brakeSound.Play(); } brakeSound.volume = Mathf.MoveTowards(brakeSound.volume, 1f, Time.deltaTime * 3f); }
    private void FadeOutBrakeSound() { if (brakeSound != null && brakeSound.isPlaying) { brakeSound.volume = Mathf.MoveTowards(brakeSound.volume, 0f, Time.deltaTime * 3f); if (brakeSound.volume <= 0.05f) brakeSound.Stop(); } }
    #endregion

    #region Âm thanh động cơ
    private void UpdateEngineSound() { if (engineSound != null) { float speed = rb != null ? rb.linearVelocity.magnitude : 0f; float speedRatio = Mathf.Clamp01(speed / 20f); engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio); engineSound.volume = Mathf.Lerp(0.1f, 0.5f, speedRatio); if (speed < 0.1f && dauVaoDiChuyen == 0) { engineSound.pitch = minPitch; engineSound.volume = 0.1f; } } }
    #endregion

    #region Power-Ups
    public void ActivateBoost(float duration)
    {
        activePowerUp = "Boost";
        powerUpDuration = duration;
        StartCoroutine(BoostCoroutine(duration));
    }

    private IEnumerator BoostCoroutine(float duration)
    {
        float originalSpeed = tocDoXe;
        tocDoXe *= 1.5f;
        yield return new WaitForSeconds(duration);
        tocDoXe = originalSpeed;
        activePowerUp = ""; // Reset khi hết hiệu lực
        powerUpDuration = 0f;
    }

    public void ActivateInvincibility(float duration)
    {
        activePowerUp = "Invincibility";
        powerUpDuration = duration;
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), true);
        yield return new WaitForSeconds(duration);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), false);
        activePowerUp = ""; // Reset khi hết hiệu lực
        powerUpDuration = 0f;
    }

    // Xử lý SlowEnemy đơn giản
    public void ActivateSlowEnemy(float duration)
    {
        activePowerUp = "SlowEnemy";
        powerUpDuration = duration;
        EnemyAI enemy = FindFirstObjectByType<EnemyAI>();
        if (enemy != null) enemy.ActivateSlow(duration);
        StartCoroutine(SlowEnemyCoroutine(duration));
    }

    private IEnumerator SlowEnemyCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        activePowerUp = ""; // Reset khi hết hiệu lực
        powerUpDuration = 0f;
    }
    #endregion

    #region UI Button Events
    public void OnGasButtonDown() { isAccelerating = true; }
    public void OnGasButtonUp() { isAccelerating = false; }
    public void OnBrakeButtonDown() { isBraking = true; }
    public void OnBrakeButtonUp() { isBraking = false; }
    #endregion

    #region Power-Up Status
    private void UpdatePowerUpStatus()
    {
        if (powerUpDuration > 0f)
        {
            powerUpDuration -= Time.deltaTime;
            if (powerUpStatusText != null)
            {
                powerUpStatusText.text = $"{activePowerUp} Active: {(int)powerUpDuration + 1}s";
            }
        }
        else if (powerUpStatusText != null)
        {
            powerUpStatusText.text = ""; // Ẩn khi hết hiệu lực
        }
    }
    #endregion
}