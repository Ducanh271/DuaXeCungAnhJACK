using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private float startDelay = 5f; // Thời gian trì hoãn (5 giây)
    private float startTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("⚠️ NavMeshAgent không được gắn vào GameObject này!");
            enabled = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startTime = Time.time; // Ghi lại thời điểm bắt đầu

        if (player == null)
        {
            Debug.LogError("Không tìm thấy xe người chơi! Hãy gán tag 'Player' cho xe.");
        }
    }

    void Update()
    {
        if (player != null && !GameManager.Instance.ketThucGame && !GameManager.Instance.isGamePaused)
        {
            // Kiểm tra nếu đã qua 5 giây
            if (Time.time - startTime >= startDelay)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance > agent.stoppingDistance)
                {
                    agent.SetDestination(player.position);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayerCaught();
        }
    }
    // Thêm vào cuối class EnemyAI, sau OnCollisionEnter

public void ActivateSlow(float duration)
{
    StartCoroutine(SlowCoroutine(duration));
}

private IEnumerator SlowCoroutine(float duration)
{
    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    float originalSpeed = agent.speed;
    agent.speed *= 0.5f; // Giảm tốc 50%
    yield return new WaitForSeconds(duration);
    agent.speed = originalSpeed; // Trở về bình thường
}
}