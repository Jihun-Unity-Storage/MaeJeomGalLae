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
                Debug.LogError($"Firebase 초기화 실패: {task.Exception}");
                return;
            }
            _reference = FirebaseDatabase.DefaultInstance.RootReference;

            // 선택된 메뉴에 대한 리뷰 로드
            //LoadReviews(Global_data.selected_menu_name);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
           
            Debug.Log($"입력 텍스트: {ReviewInput.text}");
            SubmitReview(Global_data.selected_menu_name, ReviewInput.text);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearReviewUI();
            Debug.Log("리뷰 초기화");
        }
        //Debug.Log($"선택된 메뉴: {Global_data.selected_menu_name}");
    }

    public void SubmitReview(string menuName, string reviewText)
    {

        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        UserNickname = PlayerPrefs.GetString("UserNickname");
        Debug.Log("submitreview의 UserNickname : " + UserNickname);
        if (string.IsNullOrEmpty(menuName))
        {
            menuName = PlayerPrefs.GetString("SelectedMenuName", "");
        }
        if (string.IsNullOrEmpty(menuName) || string.IsNullOrEmpty(reviewText))
        {
            Debug.LogWarning("메뉴 이름 또는 리뷰 텍스트가 비어 있습니다.");
            return;
        }

        Review newReview = new Review(UserNickname, reviewText, DateTime.Now.ToString());
        string json = JsonUtility.ToJson(newReview);

        _reference.Child("reviews").Child(menuName).Push().SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"리뷰 제출 실패: {task.Exception}");
            }
            else
            {
                Debug.Log("리뷰 제출 성공");
                // 1초 지연 후 리뷰 로드
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
            Debug.LogError("LoadReviews 호출 시 메뉴 이름이 유효하지 않습니다.");
            menuName = PlayerPrefs.GetString("SelectedMenuName", "");
        }

        Debug.Log($"메뉴: {menuName}에 대한 리뷰를 로드 중입니다. 호출 위치: menu_community");
        Reviews.Clear();

        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        ShowLoadingIndicator();

        _reference.Child("reviews").Child(menuName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            HideLoadingIndicator();

            if (task.IsFaulted)
            {
                Debug.LogError($"리뷰 로드 실패: {task.Exception}");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    Debug.Log("리뷰가 없습니다.");
                    ClearReviewUI();
                    return;
                }

                ClearReviewUI();
                Reviews.Clear();

                foreach (DataSnapshot reviewSnapshot in snapshot.Children)
                {
                    
                    string reviewJson = reviewSnapshot.GetRawJsonValue();
                    Debug.Log($"원본 JSON: {reviewJson}");

                    Review review = JsonUtility.FromJson<Review>(reviewJson);
                    if (review != null)
                    {
                        Debug.Log($"리뷰 추가: {review.Text}");
                        Reviews.Add(review);
                    }
                    else
                    {
                        Debug.LogError("리뷰 역직렬화 실패");
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
        Debug.Log("리뷰 UI 초기화");
        foreach (Transform child in ReviewsContent)
        {
            Destroy(child.gameObject);
        }
        Reviews.Clear();
    }

    private void ShowLoadingIndicator()
    {
        // 로딩 인디케이터 표시 로직
    }

    private void HideLoadingIndicator()
    {
        // 로딩 인디케이터 숨김 로직
    }

    

    // 메뉴 버튼 클릭 핸들러 (예시)
    public void OnMenuButtonClick(string menuName)
    {
        LoadReviews(menuName);
    }
}

