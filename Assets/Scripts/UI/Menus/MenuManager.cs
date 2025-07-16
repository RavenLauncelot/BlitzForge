using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Jobs;

public class MenuManager : MonoBehaviour
{
    List<MenuPage> menuPages;
    private int CurrentPageIndex = 0;   

    public void StartLevel(int levelNumber)
    {
        //Close menu
        //Start scene
    }

    public void GoToPage(string pageType)
    {
        //PageType pageType = PageType.MainMenu; // This should be set based on the desired page
        foreach(MenuPage page in menuPages)
        {
            if (page.PageType == pageType)
            {
                page.enabled = true;
            }
            else
            {
                page.enabled = false;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        // If running in the editor, stop playing the scene
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
