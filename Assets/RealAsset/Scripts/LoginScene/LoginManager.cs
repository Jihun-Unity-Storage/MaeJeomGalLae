using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;
using TMPro;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;


public class LoginManager : MonoBehaviour
{
    public TextMeshProUGUI textForLog;


    public TMP_InputField emailField;
    public TMP_InputField passwordInputField;
    public TMP_InputField NicknameInputField;
    public TextMeshProUGUI nicknameText;
    public Button signupButton;
    public Button loginButton;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
    [SerializeField]
    private string UserNickname;
    [SerializeField]
    private string UserID;
    [SerializeField]
    private string UserEmail;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //PlayerPrefs.SetString("UserEmail", UserEmail);
            //PlayerPrefs.SetString("UserId", UserID);
            //PlayerPrefs.SetString("UserNickname", UserNickname); // PlayerPrefs ������ ���⼭ ����

            Debug.Log(PlayerPrefs.GetString("UserNickname"));

        }

        textForLog.text = "nickname : " + PlayerPrefs.GetString("UserNickname")
            + "\nEmail : " + PlayerPrefs.GetString("UserEmail")
            + "\nUID : " + PlayerPrefs.GetString("UserId");

        letsgo();
    }
    private void Start()
    {
        PlayerPrefs.DeleteAll();    
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
            }
        });
        signupButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
    }

    public void SignUp()
    {
        string email = emailField.text + "@sunrint.hs.kr";
        string password = passwordInputField.text;
        string nickname = NicknameInputField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to create user: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result.User;
            string uid = newUser.UserId;

            // �г����� �����ͺ��̽��� ����
            databaseReference.Child("users").Child(uid).Child("nickname").SetValueAsync(nickname).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCompleted)
                {
                    Debug.Log("Nickname saved successfully for user: " + uid);
                }
                else if (saveTask.IsFaulted)
                {
                    Debug.LogError("Failed to save nickname for user: " + uid);
                }
            });
        });
    }

    public void Login()
    {
        string email = emailField.text + "@sunrint.hs.kr";
        string password = passwordInputField.text;
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to sign in: " + task.Exception);
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;

            UserID = user.UserId;
            UserEmail = user.Email;

            PlayerPrefs.SetString("UserEmail", UserEmail.ToString());
            PlayerPrefs.SetString("UserId", UserID.ToString()); // PlayerPrefs ������ ���⼭ ����
            Debug.Log(PlayerPrefs.GetString("UserEmail"));
            DisplayNickname(PlayerPrefs.GetString("UserId"));

            // ���� ������ ��ȯ
            //UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
            // �α��� �� �г����� ǥ��
        });
    }

    private void DisplayNickname(string uid)
    {
        Debug.Log("in displayNickname : " + uid);
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(uid).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {

                Debug.Log("in task");
                DataSnapshot snapshot = task.Result;
                UserNickname = snapshot.Value.ToString();
                Debug.Log(UserNickname);
                nicknameText.text = "Welcome, " + UserNickname + "!";

                PlayerPrefs.SetString("UserNickname", UserNickname.ToString());

                Debug.Log(PlayerPrefs.GetString("UserNickname") +"���� ������ �ƴϿ��� ��");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch nickname: " + task.Exception);
            }
        });
        Debug.Log(PlayerPrefs.GetString("UserNickname")+"��?");
    }

    public void letsgo()
    {
        if(PlayerPrefs.GetString("UserNickname") != "")
        {
            SceneManager.LoadScene("MainScene");
        }else
        {
            PlayerPrefs.SetString("UserEmail", UserEmail);
            PlayerPrefs.SetString("UserId", UserID);

            PlayerPrefs.SetString("UserNickname", UserNickname);
        }
    }
    
}

