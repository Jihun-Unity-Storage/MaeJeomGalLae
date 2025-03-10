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
        // PlayerPrefs���� �г��� ��������
        string nickname = PlayerPrefs.GetString("UserNickname");
        string email = PlayerPrefs.GetString("UserEmail");
        // ������ �г����� UI Text�� ǥ��
        nicknameText.text = "�ȳ��ϼ���, " + nickname + "��!";
    }
}
