using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;
using System.Linq;

public class MenuButton : MonoBehaviour
{
    public enum SortType
    {
        Name,
        Price,
        Like
    }

    public GameObject Manager;
    public GameObject menuPrefab; // �޴��� ������ ������
    public Transform menuPanel; // �޴��� ������ �θ� ��ü (Menu Panel)
    public TMP_Dropdown sortDropdown;
    public GameObject UIManager;
    public GameObject community_manager;
    public GameObject search_button;
    public TMP_InputField search_field;
    public string SelectedMenu;
    public List<Menu> menus = new List<Menu>(); // �޴� Ŭ���� �迭
    public List<Menu> searched_menus = new List<Menu>();
    public Sprite filledHeartSprite; // ��Ʈ�� ä���� �̹���
    public Sprite emptyHeartSprite; // ��Ʈ�� ����ִ� �̹���

    private string currentUserNickname; // ���� ���� �г���

    private void Start()
    {
        UnityEngine.UI.Button search_component = search_button.GetComponent<UnityEngine.UI.Button>();
        search_component.onClick.AddListener(() => search_button_click());
        sortDropdown.onValueChanged.AddListener(delegate {
            OnSortDropdownValueChanged(sortDropdown);
        });

        currentUserNickname = PlayerPrefs.GetString("UserNickname");
        StartCoroutine(Delay_loading(0.5f));
    }

    public void SearchMenus(string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            Debug.Log("Search string is empty or null.");
            return;
        }

        // �˻� ����
        searched_menus = menus.Where(menu => menu.name.Contains(searchString)).ToList();

        // �˻� ��� ���
        foreach (Menu menu in searched_menus)
        {
            Debug.Log("Found menu: " + menu.name);
        }

        CreateMenuUIFromList(searched_menus);
    }

    private IEnumerator Delay_loading(float delay)
    {
        yield return new WaitForSeconds(delay);

        menus = Manager.GetComponent<DataManage>().menus;
        CreateMenuUIFromList(menus);

        yield return new WaitForSeconds(delay);
    }

    public void SetSelectedMenu()
    {
        GameObject Vertical_layout = this.transform.Find("Veritcal_layout").gameObject;
        GameObject Horizontal_layout = Vertical_layout.transform.Find("Horizontal_layout").gameObject;
        // UI ��ҿ��� Text ������Ʈ�� ������
        TextMeshProUGUI nameText = Vertical_layout.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = Vertical_layout.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        SelectedMenu = nameText.text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            menus = Manager.GetComponent<DataManage>().menus;
            CreateMenuUIFromList(menus);
        }
    }

    void CreateMenuUIFromList(List<Menu> menus)
    {
        // UI�� �ִ� ��� �޴� ����
        foreach (Transform child in menuPanel)
        {
            Destroy(child.gameObject);
        }

        // menus ����Ʈ�� �ִ� �� �޴� ������ ����Ͽ� UI�� �޴��� ����
        if (menus.Count <= 0)
        {
            Debug.Log("������ �����Ͱ� �����ϴ�.");
        }

        foreach (Menu menu in menus)
        {
            // �޴� �������� �����Ͽ� UI�� �߰�
            GameObject newMenu = Instantiate(menuPrefab, menuPanel);
            menu_number menuDetails = newMenu.GetComponent<menu_number>();
            menuDetails.id = menu.id; // �޴� ID ����
            menuDetails.menu_name = menu.name;
            menuDetails.menu_price = menu.price;
            menuDetails.menu_description = menu.description;
            menuDetails.like = menu.like; // ���ƿ� �� ����

            Button menuButton = newMenu.GetComponent<Button>();
            menuButton.onClick.AddListener(() => menu_click(menuDetails));

            GameObject Vertical_layout = newMenu.transform.Find("Veritcal_layout").gameObject;
            GameObject Horizontal_layout = Vertical_layout.transform.Find("Horizontal_layout").gameObject;

            // UI ��ҿ��� Text ������Ʈ�� ������
            TextMeshProUGUI nameText = Vertical_layout.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = Vertical_layout.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI likeText = Horizontal_layout.transform.Find("LikeText").GetComponent<TextMeshProUGUI>();

            // �޴� ������ �ؽ�Ʈ�� ����
            nameText.text = menu.name;
            priceText.text = menu.price.ToString() + "��";
            likeText.text = menu.like.ToString();

            // ���ƿ� ��ư �߰�
            Button likeButton = Horizontal_layout.transform.Find("LikeButton").GetComponent<Button>();
            Image likeImage = likeButton.GetComponent<Image>();
            likeImage.sprite = emptyHeartSprite; // �ʱ� ���� ����
            likeButton.onClick.AddListener(() => LikeButtonClicked(menuDetails, likeImage, likeText));

            // ������ ���ƿ� ���¿� ���� ��Ʈ �̹��� ������Ʈ
            Manager.GetComponent<DataManage>().GetUserLikeStatus(menuDetails.id, currentUserNickname, likeImage);

            // �޴� �̹��� ����
            Image menuImage = Vertical_layout.transform.Find("menu_image").GetComponent<Image>();
            string imagePath = $"{Application.dataPath}/RealAsset/Menu_images/{menu.name}.png";
            //Debug.Log($"Attempting to load image from: {imagePath}");
            Sprite menuSprite = LoadSpriteFromFile(imagePath);

            // ���� png �̹����� ������ jpg �̹����� �õ�
            if (menuSprite == null)
            {
                imagePath = $"{Application.dataPath}/RealAsset/Menu_images/{menu.name}.jpg";
                //Debug.Log($"Attempting to load image from: {imagePath}");
                menuSprite = LoadSpriteFromFile(imagePath);
            }

            // �̹����� ���������� �ε�Ǿ��ٸ� �޴� �̹��� ����
            if (menuSprite != null)
            {
                menuImage.sprite = menuSprite;
            }
            else
            {
                //Debug.LogWarning($"�̹����� �ҷ��� �� �����ϴ�: {imagePath}");
            }
        }
    }

    Sprite LoadSpriteFromFile(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            //Debug.LogWarning($"File does not exist: {path}");
            return null;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        //Debug.LogWarning($"Failed to load texture from: {path}");
        return null;
    }

    void LikeButtonClicked(menu_number menuDetails, Image likeImage, TextMeshProUGUI likeText)
    {
        // ���ƿ� �� ������Ʈ �� ��Ʈ �̹��� ����
        bool isLiked = likeImage.sprite == filledHeartSprite;
        if (isLiked)
        {
            likeImage.sprite = emptyHeartSprite;
            menuDetails.like--;
            Manager.GetComponent<DataManage>().RemoveUserLike(menuDetails.id, currentUserNickname);
        }
        else
        {
            likeImage.sprite = filledHeartSprite;
            menuDetails.like++;
            Manager.GetComponent<DataManage>().AddUserLike(menuDetails.id, currentUserNickname);
        }

        // ���ƿ� �� �ؽ�Ʈ ������Ʈ
        likeText.text = menuDetails.like.ToString();

        // Firebase �����ͺ��̽� ������Ʈ
        Manager.GetComponent<DataManage>().UpdateMenuLike(menuDetails.id, menuDetails.like);
    }

    void UpdateLikeButtonImage(Image likeImage, int likeCount)
    {
        // ��Ʈ �̹��� ������Ʈ
        if (likeCount > 0)
        {
            likeImage.sprite = filledHeartSprite;
        }
        else
        {
            likeImage.sprite = emptyHeartSprite;
        }
    }

    void RefreshMenuList()
    {
        // �ֽ� �޴� �����͸� �����ͼ� UI ������Ʈ
        menus = Manager.GetComponent<DataManage>().menus;
        CreateMenuUIFromList(menus);
    }

    void SortMenu(SortType sortType)
    {
        switch (sortType)
        {
            case SortType.Name:
                menus.Sort((a, b) => string.Compare(a.name, b.name));
                break;
            case SortType.Price:
                menus.Sort((a, b) => a.price.CompareTo(b.price));
                break;
            case SortType.Like:
                menus.Sort((a, b) => b.like.CompareTo(a.like));
                break;

        }

        // ���ĵ� �޴� ����Ʈ�� ����Ͽ� UI�� ������Ʈ
        CreateMenuUIFromList(menus);
    }

    void OnSortDropdownValueChanged(TMP_Dropdown change)
    {
        // ���õ� �ɼǿ� ���� �޴��� ����
        switch (change.options[change.value].text)
        {
            case "Name":
                SortMenu(SortType.Name);
                break;
            case "Price":
                SortMenu(SortType.Price);
                break;
            case "Like":
                SortMenu(SortType.Like);
                break;
        }
    }

    public void menu_click(menu_number menuDetails)
    {
        if (menuDetails != null)
        {
            PlayerPrefs.SetString("SelectedMenuName", menuDetails.menu_name);
            PlayerPrefs.SetString("SelectedMenuDescription", menuDetails.menu_description);
            PlayerPrefs.SetInt("SelectedMenuPrice", menuDetails.menu_price);
            PlayerPrefs.Save(); // ���� ���� ����

            UIManager.GetComponent<TabManager>().MenuDetailScene();
            UIManager.GetComponent<menu_detail>().Set_UI();

            Debug.Log($"���õ� �޴� �����: {menuDetails.menu_name}, ����: {menuDetails.menu_price}, ����: {menuDetails.menu_description}");
        }
        else
        {
            Debug.LogError("Menu details are not provided!");
        }
    }

    public void search_button_click()
    {
        SearchMenus(search_field.text);
    }
}
