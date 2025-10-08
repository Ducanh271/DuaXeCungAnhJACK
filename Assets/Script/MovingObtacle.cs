using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển qua lại
    [SerializeField] private float moveSpeed = 2f; // Tốc độ di chuyển
    private Vector3 startPosition; // Vị trí ban đầu
    private float moveDirection = 1f; // Hướng di chuyển (1 hoặc -1)

    void Start()
    {
        startPosition = transform.position; // Lưu vị trí ban đầu
    }

    void Update()
    {
        // Di chuyển qua lại
        float newX = startPosition.x + Mathf.PingPong(Time.time * moveSpeed, moveDistance) - (moveDistance / 2f);
        transform.position = new Vector3(newX, startPosition.y, startPosition.z);
    }
}