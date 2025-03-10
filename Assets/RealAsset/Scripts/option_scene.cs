using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;
using TMPro;


public class option_scene : MonoBehaviour
{
    public TextMeshProUGUI option_text;
    private void Update()
    {
        option_text.text = PlayerPrefs.GetString("UserNickname") + "¥‘ «œ¿Ã!";
    }
}