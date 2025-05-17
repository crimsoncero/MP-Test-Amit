using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string MENU = "Menu";
    public static string GAME = "Scene";

    public static void LoadGameSceneAsync()
    {
        SceneManager.LoadSceneAsync(GAME);
    }

    public static void LoadMenuSceneAsync()
    {
        SceneManager.LoadSceneAsync(MENU);
    }
}
