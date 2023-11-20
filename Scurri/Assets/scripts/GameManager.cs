using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[]
        itemLocs;
    public GameObject
        itemPrefab,
        itemImage,
        canvas;
    public int
        itemNo,
        numItems,
        goalItems,
        reusabilityFraction = 25; // % chance that the spot will be reusable by the system 
    private bool[]
        itemLocFlags;
    private Vector2
        nextUIPos;
    void Start()
    {
        itemLocFlags = new bool[itemLocs.Length]; // initialize flags to false and choose first item to flag and make
        goalItems = itemLocs.Length;
        itemNo = Random.Range(0, itemLocs.Length);
        itemLocFlags[itemNo] = true;
        GameObject.Instantiate(itemPrefab, itemLocs[itemNo].transform.position, Quaternion.identity);
        nextUIPos = new Vector2(-65, -50);
    }

    void Update()
    {
        
    }

    public void newItem()
    {
        if (++numItems != goalItems) // for new item generation, numbased map triggers will come later
        {
            int notThisOne = itemNo;
            do
                itemNo = Random.Range(0, itemLocs.Length); // get unused location
            while (itemLocFlags[itemNo] || itemNo == notThisOne);

            GameObject.Instantiate(itemPrefab, itemLocs[itemNo].transform.position, Quaternion.identity);
            if (Random.Range(0, 100) > reusabilityFraction)
                itemLocFlags[itemNo] = true;
        }

        RectTransform newImage = GameObject.Instantiate(itemImage, canvas.transform, false).GetComponent<RectTransform>();
        newImage.anchoredPosition = nextUIPos; // instantiate image then increase transform
        nextUIPos = new Vector2(nextUIPos.x - 50, nextUIPos.y);
    }
}
