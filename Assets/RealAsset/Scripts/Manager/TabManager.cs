using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;

public class TabManager : MonoBehaviour
{
    public GameObject[] tabs; // 탭에 해당하는 게임 오브젝트 배열
    //1 : 메인화면 2: 메뉴 둘러보기 3 : 커뮤니티 4: 설정 5 : 메뉴 디테일
    public GameObject navigationtab;
    public Button[] tabButtons; // 탭 버튼 배열

    public Button logoButton; // 로고 버튼

    private int currentTabIndex = 0; // 현재 선택된 탭의 인덱스
    public GameObject community_scene;
    void Start()
    {
        navigationtab.SetActive(true);
        community_scene.SetActive(false);
        // 초기화: 첫 번째 탭 활성화, 나머지는 비활성화
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == currentTabIndex)
                tabs[i].SetActive(true);
            else
                tabs[i].SetActive(false);
        }
        
        // 탭 버튼에 클릭 이벤트 리스너 추가
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // 클로저를 사용하여 현재 인덱스를 유지
            tabButtons[i].onClick.AddListener(() => OnTabButtonClick(index));
        }

        // 초기화: 선택된 버튼 이외의 버튼을 투명하게 처리
        SetButtonTransparency();
        logoButton.onClick.AddListener(OnLogoButtonClick);
    }

    // 탭 버튼 클릭 이벤트 핸들러
    void OnTabButtonClick(int index)
    {
        // 이전 탭 비활성화
        tabs[currentTabIndex].SetActive(false);

        // 새로운 탭 활성화
        tabs[index].SetActive(true);

        // 현재 선택된 탭 인덱스 업데이트
        currentTabIndex = index;

        // 선택된 버튼 이외의 버튼을 투명하게 처리
        SetButtonTransparency();
    }

    // 선택된 버튼 이외의 버튼을 투명하게 처리하는 함수
    void SetButtonTransparency()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            Color color = tabButtons[i].image.color;
            color.a = (i == currentTabIndex) ? 1f : 0.5f; // 선택된 버튼일 때는 불투명(1), 선택되지 않은 버튼일 때는 투명(0.5)
            tabButtons[i].image.color = color;
        }

    }
    public void LookaroundScene()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
        tabs[1].SetActive(true);


        navigationtab.SetActive(true);
    }
    public void MenuDetailScene()
    {
        //Debug.Log("hi");
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
        tabs[4].SetActive(true);


        navigationtab.SetActive(false);

    }
    void OnLogoButtonClick()
    {
        // 첫 번째 탭으로 이동
        tabs[currentTabIndex].SetActive(false);
        tabs[0].SetActive(true);
        currentTabIndex = 0;

        // 선택된 버튼 이외의 버튼을 투명하게 처리
        SetButtonTransparency();
    }

}
