using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[]
        itemLocs, // empties used for a transform
        healths; // stores the acorns so can be destroyed in order
    public GameObject
        itemPrefab, // the item thats fed to the spawn function
        itemImage, // the icon in the corner when an item is collected
        healthImage, // icon in the other corner for health
        canvas; // the canvas, used as a parent for instantiation
    public int
        itemNo, // index for itemLocs and parallel itemLocsFlags
        numItems, // count of how many have been collected
        goalItems = 5, // the required number before another isnt spawned
        reusabilityFraction = 25; // % chance that a spot will be reusable by the system 
    private bool[]
        itemLocFlags; // flags which spots not to use as the system progresses
    private Vector2
        nextUIPos, // indicates spawn point relative to canvas corner
        nextHealthPos; // same as above, other corner
    void Start() // runs once on scene load
    {
        itemLocFlags = new bool[itemLocs.Length]; // initialize flags to false
        itemNo = Random.Range(0, itemLocs.Length); // choose random spot
        itemLocFlags[itemNo] = true; // ensures original location isnt reused
        GameObject.Instantiate(itemPrefab, itemLocs[itemNo].transform.position, Quaternion.identity); // make the first item
        nextUIPos = new Vector2(-65, -50); // baseline for itemImage
        nextHealthPos = new Vector2(65, -50); // baseline for HealthImage
        healths = new GameObject[settingsPref.chosenDifficulty + 1]; // makes the array of health pictures proper size

        for (int i = 0; i <= settingsPref.chosenDifficulty; i++) // for each life determined by difficulty:
        {
            RectTransform newImage = GameObject.Instantiate(healthImage, canvas.transform, false).GetComponent<RectTransform>(); // new healthImage, transform is kept as a variable, false means transform is relative to parent
            newImage.anchoredPosition = nextHealthPos; // make the newImage in the right spot
            healths[i] = newImage.gameObject; // store the image gameobs so can destroy correct image on death
            nextHealthPos = new Vector2(nextHealthPos.x + 50, nextHealthPos.y); // move the transform for next item to the right
        }
    }

    public void newItem() // called when collect an item 
    {
        if (++numItems != goalItems) // increments numItems, then checks it against goalItems
        {
            int notThisOne = itemNo; // so same spot cant be used twice in a row, but still has a chance to not flag
            do
                itemNo = Random.Range(0, itemLocs.Length); // get random location
            while (itemLocFlags[itemNo] || itemNo == notThisOne); // repeat above if itemNo is flagged or same as last one

            GameObject.Instantiate(itemPrefab, itemLocs[itemNo].transform.position, Quaternion.identity); // spawn new item, no parent
            if (Random.Range(0, 100) > reusabilityFraction) // roll the dice
                itemLocFlags[itemNo] = true; // if RNJesus likes you, the location doesnt get flagged
        }

        RectTransform newImage = GameObject.Instantiate(itemImage, canvas.transform, false).GetComponent<RectTransform>(); // new itemimage for the corner is made, relative child of canvas
        newImage.anchoredPosition = nextUIPos; // set the transform
        nextUIPos = new Vector2(nextUIPos.x - 50, nextUIPos.y); // move the spawn point to the left for the next one preemptively
    }
}
