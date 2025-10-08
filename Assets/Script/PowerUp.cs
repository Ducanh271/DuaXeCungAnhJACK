using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Boost, SlowEnemy, Invincibility }
    public PowerUpType type = PowerUpType.Boost;
    public float duration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Car playerCar = other.GetComponent<Car>();
            if (playerCar != null)
            {
                ActivatePowerUp(playerCar);
            }
            Debug.Log("Đã chạm vào");
            Destroy(gameObject);
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
                playerCar.ActivateSlowEnemy(duration);
                break;
            case PowerUpType.Invincibility:
                playerCar.ActivateInvincibility(duration);
                break;
        }
    }

  
}