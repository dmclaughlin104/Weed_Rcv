using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//short script to handle the change between the menu and the main s
[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{

    //load main scene
    public void StartNew()
    {
        SceneManager.LoadScene(1);

    }

    //load menu
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    //an exit method to stop playing the game
    public void Exit()
    {
    //checking if game is playing played on the Unity editor or elsewhere
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #else
                Application.Quit(); // original code to quit Unity player
    #endif
    }




}

