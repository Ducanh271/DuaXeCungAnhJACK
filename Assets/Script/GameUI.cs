using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private void Update()
    {
        HienThiThoiGianGame();
    }

    public void HienThiThoiGianGame()
    {
        timeText.SetText(Mathf.FloorToInt(GameManager.Instance.thoiGianChoPhepVeDich).ToString());
    }
    public void ChoiLai()
    {
        SceneManager.LoadScene("Game");
    }
    public void VeMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
