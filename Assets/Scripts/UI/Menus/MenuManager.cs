using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Jobs;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] List<MenuPage> menuPages;

    [SerializeField] List<string> currentMenuDirec;

    [SerializeField] string startingMenu;

    private int CurrentPageIndex = 0;

    public void Start()
    {
        currentMenuDirec = new List<string>();
        GoToPage(startingMenu);      
    }

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
                page.gameObject.SetActive(true);
               
                currentMenuDirec.Add(pageType);                
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
    }

    public void GoToPageNoDirec(string pageType)
    {
        //PageType pageType = PageType.MainMenu; // This should be set based on the desired page
        foreach (MenuPage page in menuPages)
        {
            if (page.PageType == pageType)
            {
                page.gameObject.SetActive(true);
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
    }

    public void PreviousPage()
    {
        //only has 1 index
        if (currentMenuDirec.Count < 2)
        {
            Debug.Log("Can't go back anymore");
            return;
        }

        GoToPageNoDirec(currentMenuDirec[currentMenuDirec.Count - 2]);
        currentMenuDirec.RemoveAt(currentMenuDirec.Count-1);
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
