using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuMangager : MonoBehaviour
{
    public GameObject[] 
        screens; // list of screen images. menu, deaths, win
    public GameObject
        mainMenu, // the mainmenu parent so can be enabled/disabled
        image; // the icon to appear in the corner of buttons 
    private Vector2 
        zero = Vector2.zero; // 0,0 

    void Start() // once at start
    {
        screens[settingsPref.neededScreen].SetActive(true); // turns on the proper image, default 0 (menu)
        if (settingsPref.neededScreen == 0) // if it is the menu screen:
            mainMenu.SetActive(true); // turn on the menu
    }

    public void play() // called from play button
    {
        SceneManager.LoadScene("map"); // load the game
    }

    public void showClick(int diff) // called from difficulty buttons, gets passed difficulty 0,1 or, 2
    {
        image.GetComponent<RectTransform>().anchoredPosition = zero; // move the image to the corner
        settingsPref.chosenDifficulty = diff; // sets global difficulty setting
    }
}
