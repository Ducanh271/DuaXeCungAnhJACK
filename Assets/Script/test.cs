using UnityEngine;

public class test : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
                    Debug.Log("Đã chạm vào");

        if (other.CompareTag("Player")) // Chỉ xe người chơi thu thập
        {
            Debug.Log("Đã chạm vào");
            Destroy(gameObject); // Xóa vật phẩm sau khi thu thập
        }
    }

   
}
