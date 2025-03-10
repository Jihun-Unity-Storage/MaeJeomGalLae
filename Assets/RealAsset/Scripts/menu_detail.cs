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
    public Button chat_button; // Button ������ ����

    private void Start()
    {
        // ��ư�� ������ ���� ������Ʈ���� ���� �����ǵ��� ����
        chat_button.onClick.RemoveAllListeners();
        chat_button.onClick.AddListener(Chat_button_Click);

        // PlayerPrefs���� ���õ� �޴� �̸� �ҷ�����
        string selectedMenuName = PlayerPrefs.GetString("SelectedMenuName", "");
        if (!string.IsNullOrEmpty(selectedMenuName))
        {
            // ���õ� �޴��� ���� UI ������Ʈ
            Set_UI();
        }
    }

    public void Set_UI()
    {
        string menuName = PlayerPrefs.GetString("SelectedMenuName", "�޴� �̸� ����");
        string menuDescription = PlayerPrefs.GetString("SelectedMenuDescription", "���� ����");
        int menuPrice = PlayerPrefs.GetInt("SelectedMenuPrice", 0);

        Debug.Log($"Set_UI ȣ���, �޴� �̸�: {menuName}");
        menu_name.text = menuName;
        menu_price.text = menuPrice.ToString() + "��";
        if (string.IsNullOrEmpty(menuDescription))
        {
            menuDescription = $"{menuName}�� ���� �����Դϴ�. ����� ����ֽ��ϴ�. ";
            menuDescription = string.Concat(Enumerable.Repeat(menuDescription, 10));
        }

        string imagePathPng = $"{Application.dataPath}/RealAsset/Menu_images/{menuName}.png";
        string imagePathJpg = $"{Application.dataPath}/RealAsset/Menu_images/{menuName}.jpg";

        Debug.Log($"Attempting to load image from: {imagePathPng}");
        Sprite menuSprite = LoadSpriteFromFile(imagePathPng);

        // ���� png �̹����� ������ jpg �̹����� �õ�
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
            Debug.LogWarning($"�̹����� �ҷ��� �� �����ϴ�: {imagePathPng} �Ǵ� {imagePathJpg}");
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
        Debug.Log($"Chat_button_Click ȣ���, ���õ� �޴�: {menuName}");
        if (string.IsNullOrEmpty(menuName))
        {
            Debug.LogError("�޴� �̸��� �������� �ʾҽ��ϴ�.");
            return;
        }
        Community_manager.GetComponent<menu_community>().LoadReviews(menuName);
    }
}
