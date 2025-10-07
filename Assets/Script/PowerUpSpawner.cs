using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUpPrefabs; // Mảng Prefab vật phẩm
    [SerializeField] private float spawnInterval = 10f; // Spawn mỗi 10 giây
    [SerializeField] private Vector3 spawnArea = new Vector3(50f, 0f, 50f); // Khu vực spawn (kích thước đường đua)

    void Start()
    {
        StartCoroutine(SpawnPowerUps());
    }

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomPowerUp();
        }
    }

    private void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs.Length == 0) return;

        // Chọn random Prefab
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        // Vị trí random trên đường đua (giả sử đường đua là phẳng, điều chỉnh theo NavMesh nếu cần)
        Vector3 spawnPos = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            0.5f, // Cao độ trên mặt đất
            Random.Range(-spawnArea.z, spawnArea.z)
        );

        // Kiểm tra vị trí hợp lệ (trên NavMesh, tùy chọn)
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, 1))
        {
            spawnPos = hit.position;
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}