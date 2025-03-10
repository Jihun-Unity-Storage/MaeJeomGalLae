using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;

    private void Start()
    {
        // PlayerPrefs에서 닉네임 가져오기
        string nickname = PlayerPrefs.GetString("UserNickname");
        string email = PlayerPrefs.GetString("UserEmail");
        // 가져온 닉네임을 UI Text에 표시
        nicknameText.text = "안녕하세요, " + nickname + "님!";
    }
}
