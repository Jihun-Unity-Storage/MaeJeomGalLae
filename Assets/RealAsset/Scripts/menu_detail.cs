using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;
using TMPro;
using UnityEditor;
using System.Net;
using System.Linq;

public class menu_detail : MonoBehaviour
{
    public GameObject Community_manager;
    public GameObject UIManager;
    public Image menu_image;
    public TextMeshProUGUI menu_name;
    public TextMeshProUGUI menu_description;
    public TextMeshProUGUI menu_price;
    public Button back_button;
    public Button chat_button; // Button 변수명 수정

    private void Start()
    {
        // 버튼이 설정된 게임 오브젝트에서 직접 참조되도록 수정
        chat_button.onClick.RemoveAllListeners();
        chat_button.onClick.AddListener(Chat_button_Click);

        // PlayerPrefs에서 선택된 메뉴 이름 불러오기
        string selectedMenuName = PlayerPrefs.GetString("SelectedMenuName", "");
        if (!string.IsNullOrEmpty(selectedMenuName))
        {
            // 선택된 메뉴에 대한 UI 업데이트
            Set_UI();
        }
    }

    public void Set_UI()
    {
        string menuName = PlayerPrefs.GetString("SelectedMenuName", "메뉴 이름 없음");
        string menuDescription = PlayerPrefs.GetString("SelectedMenuDescription", "설명 없음");
        int menuPrice = PlayerPrefs.GetInt("SelectedMenuPrice", 0);

        Debug.Log($"Set_UI 호출됨, 메뉴 이름: {menuName}");
        menu_name.text = menuName;
        menu_price.text = menuPrice.ToString() + "원";
        if (string.IsNullOrEmpty(menuDescription))
        {
            menuDescription = $"{menuName}에 대한 설명입니다. 현재는 비어있습니다. ";
            menuDescription = string.Concat(Enumerable.Repeat(menuDescription, 10));
        }

        string imagePathPng = $"{Application.dataPath}/RealAsset/Menu_images/{menuName}.png";
        string imagePathJpg = $"{Application.dataPath}/RealAsset/Menu_images/{menuName}.jpg";

        Debug.Log($"Attempting to load image from: {imagePathPng}");
        Sprite menuSprite = LoadSpriteFromFile(imagePathPng);

        // 만약 png 이미지가 없으면 jpg 이미지를 시도
        if (menuSprite == null)
        {
            Debug.Log($"Attempting to load image from: {imagePathJpg}");
            menuSprite = LoadSpriteFromFile(imagePathJpg);
        }

        if (menuSprite != null)
        {
            menu_image.sprite = menuSprite;
        }
        else
        {
            Debug.LogWarning($"이미지를 불러올 수 없습니다: {imagePathPng} 또는 {imagePathJpg}");
        }
    }

    Sprite LoadSpriteFromFile(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"File does not exist: {path}");
            return null;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        Debug.LogWarning($"Failed to load texture from: {path}");
        return null;
    }

    public void back_button_click()
    {
        UIManager.GetComponent<TabManager>().LookaroundScene();
    }

    public void Chat_button_Click()
    {
        string menuName = PlayerPrefs.GetString("SelectedMenuName", "");
        Debug.Log($"Chat_button_Click 호출됨, 선택된 메뉴: {menuName}");
        if (string.IsNullOrEmpty(menuName))
        {
            Debug.LogError("메뉴 이름이 설정되지 않았습니다.");
            return;
        }
        Community_manager.GetComponent<menu_community>().LoadReviews(menuName);
    }
}
