using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuMangager : MonoBehaviour
{
    public GameObject[] screens;
    public GameObject mainMenu, image;
    private Vector2 zero = Vector2.zero;

    void Start()
    {
        screens[settingsPref.neededScreen].SetActive(true);
        if (settingsPref.neededScreen == 0)
            mainMenu.SetActive(true);
    }

    public void play()
    {
        SceneManager.LoadScene("map");
    }

    public void showClick(int diff)
    {
       // image.SetActive(true);
        image.GetComponent<RectTransform>().anchoredPosition = zero;
        settingsPref.chosenDifficulty = diff;
       
    }
}
