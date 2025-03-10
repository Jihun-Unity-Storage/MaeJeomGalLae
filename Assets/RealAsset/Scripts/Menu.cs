using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;

[System.Serializable]
public class Menu
{
    public string id;
    public string name;
    public string description;
    public int like;
    public int price;
}
