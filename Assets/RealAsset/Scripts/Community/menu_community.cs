using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;
using System;

public class menu_community : MonoBehaviour
{
    [SerializeField]
    public List<Review> Reviews = new List<Review>();
    public Transform ReviewsContent;
    public GameObject ReviewPrefab;

    private DatabaseReference _reference;

    public TMP_InputField ReviewInput;
    public string UserNickname;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Firebase �ʱ�ȭ ����: {task.Exception}");
                return;
            }
            _reference = FirebaseDatabase.DefaultInstance.RootReference;

            // ���õ� �޴��� ���� ���� �ε�
            //LoadReviews(Global_data.selected_menu_name);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
           
            Debug.Log($"�Է� �ؽ�Ʈ: {ReviewInput.text}");
            SubmitReview(Global_data.selected_menu_name, ReviewInput.text);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearReviewUI();
            Debug.Log("���� �ʱ�ȭ");
        }
        //Debug.Log($"���õ� �޴�: {Global_data.selected_menu_name}");
    }

    public void SubmitReview(string menuName, string reviewText)
    {

        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        UserNickname = PlayerPrefs.GetString("UserNickname");
        Debug.Log("submitreview�� UserNickname : " + UserNickname);
        if (string.IsNullOrEmpty(menuName))
        {
            menuName = PlayerPrefs.GetString("SelectedMenuName", "");
        }
        if (string.IsNullOrEmpty(menuName) || string.IsNullOrEmpty(reviewText))
        {
            Debug.LogWarning("�޴� �̸� �Ǵ� ���� �ؽ�Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        Review newReview = new Review(UserNickname, reviewText, DateTime.Now.ToString());
        string json = JsonUtility.ToJson(newReview);

        _reference.Child("reviews").Child(menuName).Push().SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"���� ���� ����: {task.Exception}");
            }
            else
            {
                Debug.Log("���� ���� ����");
                // 1�� ���� �� ���� �ε�
                StartCoroutine(DelayedLoadReviews(menuName, 1f));
            }
        });
    }

    private IEnumerator DelayedLoadReviews(string menuName, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadReviews(menuName);
    }

    public void LoadReviews(string menuName)
    {
        if (string.IsNullOrEmpty(menuName))
        {
            Debug.LogError("LoadReviews ȣ�� �� �޴� �̸��� ��ȿ���� �ʽ��ϴ�.");
            menuName = PlayerPrefs.GetString("SelectedMenuName", "");
        }

        Debug.Log($"�޴�: {menuName}�� ���� ���並 �ε� ���Դϴ�. ȣ�� ��ġ: menu_community");
        Reviews.Clear();

        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        ShowLoadingIndicator();

        _reference.Child("reviews").Child(menuName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            HideLoadingIndicator();

            if (task.IsFaulted)
            {
                Debug.LogError($"���� �ε� ����: {task.Exception}");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    Debug.Log("���䰡 �����ϴ�.");
                    ClearReviewUI();
                    return;
                }

                ClearReviewUI();
                Reviews.Clear();

                foreach (DataSnapshot reviewSnapshot in snapshot.Children)
                {
                    
                    string reviewJson = reviewSnapshot.GetRawJsonValue();
                    Debug.Log($"���� JSON: {reviewJson}");

                    Review review = JsonUtility.FromJson<Review>(reviewJson);
                    if (review != null)
                    {
                        Debug.Log($"���� �߰�: {review.Text}");
                        Reviews.Add(review);
                    }
                    else
                    {
                        Debug.LogError("���� ������ȭ ����");
                    }
                    
                }

                DisplayAllReviews();
            }
        });
    }



    private void DisplayAllReviews()
    {
        foreach (var review in Reviews)
        {
            DisplayReviewOnUI(review);
        }
    }

    private void DisplayReviewOnUI(Review review)
    {
        GameObject newReview = Instantiate(ReviewPrefab, ReviewsContent);

        // Find the UI elements in the prefab and set their text
        TextMeshProUGUI reviewText = newReview.transform.Find("ReviewText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI userDetails = newReview.transform.Find("UserDetails").GetComponent<TextMeshProUGUI>();

        reviewText.text = review.Text;

        // Parse the timestamp and format it
        DateTimeOffset dateTimeOffset;
        if (DateTimeOffset.TryParse(review.Timestamp, out dateTimeOffset))
        {
            string formattedString = dateTimeOffset.ToString("yyyy.MM.dd HH:mm");
            userDetails.text = $"{review.Username} | {formattedString}";
        }
        else
        {
            userDetails.text = $"{review.Username} | Invalid Timestamp";
        }
    }

    public void ClearReviewUI()
    {
        Debug.Log("���� UI �ʱ�ȭ");
        foreach (Transform child in ReviewsContent)
        {
            Destroy(child.gameObject);
        }
        Reviews.Clear();
    }

    private void ShowLoadingIndicator()
    {
        // �ε� �ε������� ǥ�� ����
    }

    private void HideLoadingIndicator()
    {
        // �ε� �ε������� ���� ����
    }

    

    // �޴� ��ư Ŭ�� �ڵ鷯 (����)
    public void OnMenuButtonClick(string menuName)
    {
        LoadReviews(menuName);
    }
}

