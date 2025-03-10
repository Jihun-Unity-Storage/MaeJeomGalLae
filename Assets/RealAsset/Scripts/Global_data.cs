using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global_data
{
    private static string _selectedMenuName;
    public static string selected_menu_name
    {
        get { return _selectedMenuName; }
        set
        {
            Debug.Log($"Global_data.selected_menu_name ¼³Á¤: {value}");
            _selectedMenuName = value;
        }
    }

    public static int selected_menu_price;
    public static string selected_menu_description;
}

