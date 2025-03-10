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
    public GameObject[] tabs; // �ǿ� �ش��ϴ� ���� ������Ʈ �迭
    //1 : ����ȭ�� 2: �޴� �ѷ����� 3 : Ŀ�´�Ƽ 4: ���� 5 : �޴� ������
    public GameObject navigationtab;
    public Button[] tabButtons; // �� ��ư �迭

    public Button logoButton; // �ΰ� ��ư

    private int currentTabIndex = 0; // ���� ���õ� ���� �ε���
    public GameObject community_scene;
    void Start()
    {
        navigationtab.SetActive(true);
        community_scene.SetActive(false);
        // �ʱ�ȭ: ù ��° �� Ȱ��ȭ, �������� ��Ȱ��ȭ
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == currentTabIndex)
                tabs[i].SetActive(true);
            else
                tabs[i].SetActive(false);
        }
        
        // �� ��ư�� Ŭ�� �̺�Ʈ ������ �߰�
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // Ŭ������ ����Ͽ� ���� �ε����� ����
            tabButtons[i].onClick.AddListener(() => OnTabButtonClick(index));
        }

        // �ʱ�ȭ: ���õ� ��ư �̿��� ��ư�� �����ϰ� ó��
        SetButtonTransparency();
        logoButton.onClick.AddListener(OnLogoButtonClick);
    }

    // �� ��ư Ŭ�� �̺�Ʈ �ڵ鷯
    void OnTabButtonClick(int index)
    {
        // ���� �� ��Ȱ��ȭ
        tabs[currentTabIndex].SetActive(false);

        // ���ο� �� Ȱ��ȭ
        tabs[index].SetActive(true);

        // ���� ���õ� �� �ε��� ������Ʈ
        currentTabIndex = index;

        // ���õ� ��ư �̿��� ��ư�� �����ϰ� ó��
        SetButtonTransparency();
    }

    // ���õ� ��ư �̿��� ��ư�� �����ϰ� ó���ϴ� �Լ�
    void SetButtonTransparency()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            Color color = tabButtons[i].image.color;
            color.a = (i == currentTabIndex) ? 1f : 0.5f; // ���õ� ��ư�� ���� ������(1), ���õ��� ���� ��ư�� ���� ����(0.5)
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
        // ù ��° ������ �̵�
        tabs[currentTabIndex].SetActive(false);
        tabs[0].SetActive(true);
        currentTabIndex = 0;

        // ���õ� ��ư �̿��� ��ư�� �����ϰ� ó��
        SetButtonTransparency();
    }

}
