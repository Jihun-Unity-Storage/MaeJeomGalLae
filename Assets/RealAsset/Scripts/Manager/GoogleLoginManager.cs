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



public class GoogleLoginManager : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // Firebase ���� ��ü ��������
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError("Failed to initialize Firebase.");
            }
        });
    }

    public void OnGoogleSignInButtonClick()
    {
        // Google �α��� â ����
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = null;
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential("idtoken", null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.Log("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
        });
    }
}
