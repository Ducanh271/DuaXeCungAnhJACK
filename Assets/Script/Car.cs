using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Bổ sung để hỗ trợ Coroutine

public class Car : MonoBehaviour
{
    [Header("Thông số xe")]
    [SerializeField] private float tocDoXe = 15000f;
    [SerializeField] private float lucReXe = 300f;
    [SerializeField] private GameObject phanhEffect;
    [SerializeField] private Joystick joystick;

    [Header("Âm thanh xe")]
    [SerializeField] private AudioSource engineSound; // Động cơ
    [SerializeField] private AudioSource brakeSound;  // Phanh
    [SerializeField] private float minPitch = 0.2f;
    [SerializeField] private float maxPitch = 0.5f;

    [Header("UI Buttons")]
    [SerializeField] private Button gasButton;
    [SerializeField] private Button brakeButton;

    private Rigidbody rb;
    private float dauVaoDiChuyen;
    private float dauVaoRe;

    private bool isAccelerating = false;
    private bool isBraking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Kiểm tra joystick
        if (joystick == null)
            Debug.LogWarning("⚠️ Joystick chưa gán.");

        // Kiểm tra âm thanh
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.volume = 0.1f;
            engineSound.pitch = minPitch;
            engineSound.Play();
        }
        if (brakeSound != null) brakeSound.loop = false;

        // Gán event cho nút UI (dùng EventTrigger giữ nút)
        if (gasButton != null)
        {
            gasButton.onClick.AddListener(() => { }); // Placeholder, dùng EventTrigger
        }
        if (brakeButton != null)
        {
            brakeButton.onClick.AddListener(() => { }); // Placeholder, dùng EventTrigger
        }
    }

    private void Update()
    {
        // Xử lý phím điều khiển
        if (Input.GetKeyDown(KeyCode.A)) isAccelerating = true;
        if (Input.GetKeyUp(KeyCode.A)) isAccelerating = false;

        if (Input.GetKeyDown(KeyCode.S)) isBraking = true;
        if (Input.GetKeyUp(KeyCode.S)) isBraking = false;
    }

    private void FixedUpdate()
    {
        // Lấy input từ joystick
        dauVaoRe = joystick != null ? joystick.Horizontal : 0f;
        dauVaoDiChuyen = joystick != null ? joystick.Vertical : 0f;

        // Ưu tiên input từ phím nếu có
        if (isAccelerating) dauVaoDiChuyen = 1f;
        if (isBraking) dauVaoDiChuyen = -1f;

        DiChuyenXe();
        ReXe();

        // Phanh
        if (isBraking)
        {
            PhanhXe();
        }
        else
        {
            phanhEffect.SetActive(false);
            FadeOutBrakeSound();
        }

        UpdateEngineSound();
    }

    #region Di chuyển xe
    private void DiChuyenXe()
    {
        rb.AddRelativeForce(Vector3.forward * dauVaoDiChuyen * tocDoXe * Time.fixedDeltaTime);
    }

    private void ReXe()
    {
        Quaternion re = Quaternion.Euler(Vector3.up * dauVaoRe * lucReXe * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * re);
    }
    #endregion

    #region Phanh
    private void PhanhXe()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.02f);
            phanhEffect.SetActive(true);
            FadeInBrakeSound();
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            phanhEffect.SetActive(false);
            FadeOutBrakeSound();
        }
    }

    private void FadeInBrakeSound()
    {
        if (brakeSound == null) return;
        if (!brakeSound.isPlaying)
        {
            brakeSound.volume = 0f;
            brakeSound.Play();
        }
        brakeSound.volume = Mathf.MoveTowards(brakeSound.volume, 1f, Time.deltaTime * 3f);
    }

    private void FadeOutBrakeSound()
    {
        if (brakeSound == null || !brakeSound.isPlaying) return;
        brakeSound.volume = Mathf.MoveTowards(brakeSound.volume, 0f, Time.deltaTime * 3f);
        if (brakeSound.volume <= 0.05f) brakeSound.Stop();
    }
    #endregion

    #region Âm thanh động cơ
    private void UpdateEngineSound()
    {
        if (engineSound == null) return;
        float speed = rb.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(speed / 20f);
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        engineSound.volume = Mathf.Lerp(0.1f, 0.5f, speedRatio);

        if (speed < 0.1f && dauVaoDiChuyen == 0)
        {
            engineSound.pitch = minPitch;
            engineSound.volume = 0.1f;
        }
    }
    #endregion

    #region Power-Ups
    public void ActivateBoost(float duration)
    {
        StartCoroutine(BoostCoroutine(duration));
    }

    private IEnumerator BoostCoroutine(float duration)
    {
        float originalSpeed = tocDoXe;
        tocDoXe *= 1.5f; // Tăng tốc 50%
        yield return new WaitForSeconds(duration);
        tocDoXe = originalSpeed; // Trở về bình thường
    }

    public void ActivateInvincibility(float duration)
    {
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        // Tắt va chạm với chướng ngại vật (tag "Obstacle")
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), true);
        yield return new WaitForSeconds(duration);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), false);
    }
    #endregion

    #region UI Button Events
    public void OnGasButtonDown() { isAccelerating = true; }
    public void OnGasButtonUp() { isAccelerating = false; }

    public void OnBrakeButtonDown() { isBraking = true; }
    public void OnBrakeButtonUp() { isBraking = false; }
    #endregion
}