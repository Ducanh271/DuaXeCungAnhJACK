using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Boost, SlowEnemy, Invincibility } // Loại vật phẩm
    public PowerUpType type = PowerUpType.Boost; // Mặc định Boost
    public float duration = 5f; // Thời gian hiệu lực (giây)

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("đã va chạm");
        if (other.CompareTag("Player")) // Chỉ xe người chơi thu thập
        {
            Car playerCar = other.GetComponent<Car>();
            if (playerCar != null)
            {
                ActivatePowerUp(playerCar);
            }
            Debug.Log("Đã chạm vào trong if");
            Destroy(gameObject); // Xóa vật phẩm sau khi thu thập
        }
    }

    private void ActivatePowerUp(Car playerCar)
    {
        switch (type)
        {
            case PowerUpType.Boost:
                playerCar.ActivateBoost(duration);
                break;
            case PowerUpType.SlowEnemy:
                EnemyAI enemy = FindFirstObjectByType<EnemyAI>(); // Thay FindObjectOfType
                if (enemy != null) enemy.ActivateSlow(duration);
                break;
            case PowerUpType.Invincibility:
                playerCar.ActivateInvincibility(duration);
                break;
        }
    }
}