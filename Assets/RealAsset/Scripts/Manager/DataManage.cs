using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;

public class DataManage : MonoBehaviour
{
    public GameObject uimanager;
    public enum SortType
    {
        Name,
        Price,
        Like
    }
    DatabaseReference reference;
    public List<Menu> menus = new List<Menu>(); // 메뉴 클래스 배열

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        GetMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            menus.Clear();
            GetMenu();
        }
    }

    void GetMenu()
    {
        reference.Child("menus").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch menus: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("getmenu성공");

                foreach (DataSnapshot menuSnapshot in snapshot.Children)
                {
                    Menu menu = new Menu();
                    menu.id = menuSnapshot.Key; // 메뉴의 고유 ID 저장
                    menu.name = menuSnapshot.Child("name").Value.ToString();
                    menu.description = menuSnapshot.Child("description").Value.ToString();
                    menu.like = int.Parse(menuSnapshot.Child("like").Value.ToString());
                    menu.price = int.Parse(menuSnapshot.Child("price").Value.ToString());

                    menus.Add(menu);
                }
            }
        });
    }

    public void AddNewMenu(string menuName, int price, string description, int like)
    {
        Menu newMenu = new Menu();
        newMenu.name = menuName;
        newMenu.description = description;
        newMenu.like = like;
        newMenu.price = price;

        string json = JsonUtility.ToJson(newMenu);

        reference.Child("menus").Push().SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to add new menu: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("New menu added successfully.");
            }
        });
    }

    public void UpdateMenuLike(string menuId, int newLikeCount)
    {
        Debug.Log(menuId);
        reference.Child("menus").Child(menuId).Child("like").SetValueAsync(newLikeCount).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to update menu like count: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Menu like count updated successfully. New Like Count: " + newLikeCount);
            }
        });
    }

    public void AddUserLike(string menuId, string username)
    {
        var likedByRef = reference.Child("menus").Child(menuId).Child("likedBy");

        likedByRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking likedBy node existence: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (!snapshot.Exists)
                {
                    likedByRef.SetValueAsync("").ContinueWith(innerTask =>
                    {
                        if (innerTask.IsFaulted)
                        {
                            Debug.LogError("Failed to create likedBy node: " + innerTask.Exception);
                        }
                        else
                        {
                            Debug.Log("likedBy node created successfully.");
                            AddLike(likedByRef, username);
                        }
                    });
                }
                else
                {
                    AddLike(likedByRef, username);
                }
            }
        });
    }

    private void AddLike(DatabaseReference likedByRef, string username)
    {
        likedByRef.Child(username).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to add user like: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("User like added successfully.");
            }
        });
    }

    public void RemoveUserLike(string menuId, string username)
    {
        reference.Child("menus").Child(menuId).Child("likedBy").Child(username).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to remove user like: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("User like removed successfully.");
            }
        });
    }

    public void GetUserLikeStatus(string menuId, string username, Image likeImage)
    {
        reference.Child("menus").Child(menuId).Child("likedBy").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get user like status: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.Value is bool && (bool)snapshot.Value)
                {
                    likeImage.sprite = uimanager.GetComponent<MenuButton>().filledHeartSprite;
                }
                else
                {
                    likeImage.sprite = uimanager.GetComponent<MenuButton>().emptyHeartSprite;
                }
            }
        });
    }

}
