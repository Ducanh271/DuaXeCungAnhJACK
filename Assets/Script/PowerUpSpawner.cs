using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUpPrefabs; // Mảng Prefab vật phẩm
    [SerializeField] private Transform[] waypoints; // Mảng các waypoint trên đường đua
    [SerializeField] private float spawnInterval = 0.05f; // Spawn mỗi 3 giây
    [SerializeField] private int maxPowerUps = 20; // Giới hạn số lượng vật phẩm cùng lúc
     [SerializeField] private int timeDisappear = 1; // Giới hạn số lượng vật phẩm cùng lúc
    private int currentPowerUpCount = 0; // Đếm số vật phẩm hiện tại

    void Start()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogError("⚠️ Vui lòng gán ít nhất một PowerUp Prefab trong Inspector!");
            return;
        }
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("⚠️ Vui lòng gán ít nhất một Waypoint trong Inspector!");
            return;
        }
        StartCoroutine(SpawnPowerUps());
    }

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (currentPowerUpCount < maxPowerUps)
            {
                SpawnRandomPowerUp();
            }
        }
    }

    private void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0 || waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("Không có Prefab hoặc Waypoint để spawn!");
            return;
        }

        // Chọn random Prefab và Waypoint
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Transform waypoint = waypoints[Random.Range(0, waypoints.Length)];

        // Spawn vật phẩm tại vị trí Waypoint
        GameObject powerUp = Instantiate(prefab, waypoint.position, Quaternion.identity);
        currentPowerUpCount++;

        // Tự hủy vật phẩm sau một khoảng thời gian (tùy chọn)
        Destroy(powerUp, 30f); // Hủy sau 10 giây, điều chỉnh theo nhu cầu
    }

    private void OnDestroy()
    {
        currentPowerUpCount--; // Giảm đếm khi vật phẩm bị hủy
    }
}