using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    const string GAME_SCENE_NAME = "Scenes/GameScene";
    const string MENu_SCENE_NAME = "Scenes/MenuScene";
    public void PlayGame()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME, LoadSceneMode.Single);
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene(MENu_SCENE_NAME, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
