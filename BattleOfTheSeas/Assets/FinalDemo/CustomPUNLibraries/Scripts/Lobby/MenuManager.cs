using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; 
    
    [SerializeField] private Menu[] _menus;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        Menu menuFound = Array.Find(_menus, menu => menu.MenuName == menuName);
        
        if(menuFound)
            OpenMenu(menuFound);
        else if (menuFound.IsOpen)
            CloseMenu(menuFound);
    }

    public void OpenMenu(Menu otherMenu)
    {
        foreach (var menu in _menus)
        {
            if(menu.IsOpen)
                CloseMenu(menu);
        }
        
        otherMenu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
