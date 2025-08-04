using UnityEngine;

public class UIGameManager : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.StartLevel();
    }

    public void BackToMenu()
    {
        GameManager.instance.LoadScene("Menu");
    }

    public void LoadScene(string sceneName)
    {
        GameManager.instance.LoadScene(sceneName);
    }
}
