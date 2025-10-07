using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class XuLyVaChamChoXe : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            GameManager.Instance.QuaCheckPoint();
        }
        if (other.gameObject.tag == "WinPoint")
        {
            GameManager.Instance.QuaWinPoint();
        }
        if (other.gameObject.tag == "test")
        {
            Debug.Log("đã va chạm");
        }
        
    }
}
