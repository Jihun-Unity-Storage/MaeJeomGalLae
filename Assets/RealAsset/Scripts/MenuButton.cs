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
    public GameObject menuPrefab; // 메뉴를 생성할 프리팹
    public Transform menuPanel; // 메뉴를 생성할 부모 객체 (Menu Panel)
    public TMP_Dropdown sortDropdown;
    public GameObject UIManager;
    public GameObject community_manager;
    public GameObject search_button;
    public TMP_InputField search_field;
    public string SelectedMenu;
    public List<Menu> menus = new List<Menu>(); // 메뉴 클래스 배열
    public List<Menu> searched_menus = new List<Menu>();
    public Sprite filledHeartSprite; // 하트가 채워진 이미지
    public Sprite emptyHeartSprite; // 하트가 비어있는 이미지

    private string currentUserNickname; // 현재 유저 닉네임

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

        // 검색 로직
        searched_menus = menus.Where(menu => menu.name.Contains(searchString)).ToList();

        // 검색 결과 출력
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
        // UI 요소에서 Text 컴포넌트를 가져옴
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
        // UI에 있는 모든 메뉴 제거
        foreach (Transform child in menuPanel)
        {
            Destroy(child.gameObject);
        }

        // menus 리스트에 있는 각 메뉴 정보를 사용하여 UI에 메뉴를 생성
        if (menus.Count <= 0)
        {
            Debug.Log("가져올 데이터가 없습니다.");
        }

        foreach (Menu menu in menus)
        {
            // 메뉴 프리팹을 복제하여 UI에 추가
            GameObject newMenu = Instantiate(menuPrefab, menuPanel);
            menu_number menuDetails = newMenu.GetComponent<menu_number>();
            menuDetails.id = menu.id; // 메뉴 ID 설정
            menuDetails.menu_name = menu.name;
            menuDetails.menu_price = menu.price;
            menuDetails.menu_description = menu.description;
            menuDetails.like = menu.like; // 좋아요 수 설정

            Button menuButton = newMenu.GetComponent<Button>();
            menuButton.onClick.AddListener(() => menu_click(menuDetails));

            GameObject Vertical_layout = newMenu.transform.Find("Veritcal_layout").gameObject;
            GameObject Horizontal_layout = Vertical_layout.transform.Find("Horizontal_layout").gameObject;

            // UI 요소에서 Text 컴포넌트를 가져옴
            TextMeshProUGUI nameText = Vertical_layout.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = Vertical_layout.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI likeText = Horizontal_layout.transform.Find("LikeText").GetComponent<TextMeshProUGUI>();

            // 메뉴 정보를 텍스트로 설정
            nameText.text = menu.name;
            priceText.text = menu.price.ToString() + "원";
            likeText.text = menu.like.ToString();

            // 좋아요 버튼 추가
            Button likeButton = Horizontal_layout.transform.Find("LikeButton").GetComponent<Button>();
            Image likeImage = likeButton.GetComponent<Image>();
            likeImage.sprite = emptyHeartSprite; // 초기 상태 설정
            likeButton.onClick.AddListener(() => LikeButtonClicked(menuDetails, likeImage, likeText));

            // 유저의 좋아요 상태에 따라 하트 이미지 업데이트
            Manager.GetComponent<DataManage>().GetUserLikeStatus(menuDetails.id, currentUserNickname, likeImage);

            // 메뉴 이미지 설정
            Image menuImage = Vertical_layout.transform.Find("menu_image").GetComponent<Image>();
            string imagePath = $"{Application.dataPath}/RealAsset/Menu_images/{menu.name}.png";
            //Debug.Log($"Attempting to load image from: {imagePath}");
            Sprite menuSprite = LoadSpriteFromFile(imagePath);

            // 만약 png 이미지가 없으면 jpg 이미지를 시도
            if (menuSprite == null)
            {
                imagePath = $"{Application.dataPath}/RealAsset/Menu_images/{menu.name}.jpg";
                //Debug.Log($"Attempting to load image from: {imagePath}");
                menuSprite = LoadSpriteFromFile(imagePath);
            }

            // 이미지가 성공적으로 로드되었다면 메뉴 이미지 설정
            if (menuSprite != null)
            {
                menuImage.sprite = menuSprite;
            }
            else
            {
                //Debug.LogWarning($"이미지를 불러올 수 없습니다: {imagePath}");
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
        // 좋아요 수 업데이트 및 하트 이미지 변경
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

        // 좋아요 수 텍스트 업데이트
        likeText.text = menuDetails.like.ToString();

        // Firebase 데이터베이스 업데이트
        Manager.GetComponent<DataManage>().UpdateMenuLike(menuDetails.id, menuDetails.like);
    }

    void UpdateLikeButtonImage(Image likeImage, int likeCount)
    {
        // 하트 이미지 업데이트
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
        // 최신 메뉴 데이터를 가져와서 UI 업데이트
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

        // 정렬된 메뉴 리스트를 사용하여 UI를 업데이트
        CreateMenuUIFromList(menus);
    }

    void OnSortDropdownValueChanged(TMP_Dropdown change)
    {
        // 선택된 옵션에 따라 메뉴를 정렬
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
            PlayerPrefs.Save(); // 변경 사항 저장

            UIManager.GetComponent<TabManager>().MenuDetailScene();
            UIManager.GetComponent<menu_detail>().Set_UI();

            Debug.Log($"선택된 메뉴 저장됨: {menuDetails.menu_name}, 가격: {menuDetails.menu_price}, 설명: {menuDetails.menu_description}");
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
