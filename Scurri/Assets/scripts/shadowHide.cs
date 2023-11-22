using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class shadowHide : MonoBehaviour
{
    public Light2D[] consideredLights; // a list of light2d scripts to be considered as logical sources
    public float
        startFade, // the distance from light that visibility begins
        endFade; // the distance from light where full opacity is achieved
   [SerializeField] private int
        closestLight; // index for consideredLights representing the closest one
    private float
        lightDist; // TBD
    private RaycastHit2D hit; // to store a sightline raycast later
    private LayerMask geometry; // to determine whats considered in that sightline raycast
    private Color tempClr; // used to set the alpha later
    void Start()
    {
        geometry = LayerMask.GetMask("geometry"); // populate layermask with everything that shouldnt be seethrough
    }

    private void FixedUpdate() // every physics update
    {
        closestLight = getClosestLight(); // calls function to determine closestlight via int index
        tempClr = GetComponent<SpriteRenderer>().color; // extracts the sprite rgba
        hit = Physics2D.Raycast(this.transform.position, (consideredLights[closestLight].gameObject.transform.position - this.transform.position).normalized, startFade, geometry); // get the first object (in the layermask) between source and the nearestlight, casting distance of startfade
        if (hit && hit.collider.gameObject.name == consideredLights[closestLight].gameObject.name) // if the hit was a hit, and the hit was the closest light (redundancy necessary):
                tempClr.a = 1 - Vector2.Distance(gameObject.transform.position, consideredLights[0].gameObject.transform.position) / startFade; // alpha is decimal determined by this equation concerning distances and light strengths and such
        else // if we are not visable to the lightsource:
            tempClr.a = 0; // invisible
        GetComponent<SpriteRenderer>().color = tempClr; // apply the alpha changes
    }

    private int getClosestLight() // returns index to closest considered lightsource
    {
        int index = 0; // prep for the linear sort
        float temp2 = Vector2.Distance(gameObject.transform.position, consideredLights[0].gameObject.transform.position); // distance between this and first source stored
        for (int i = 1; i < consideredLights.Length; i++) // for every light2d in the array:
        {
            float temp = Vector2.Distance(gameObject.transform.position, consideredLights[i].gameObject.transform.position); // check distance between this and the source
            if (temp < temp2) // if its less than the previous smallest:
            {
                index = i; // set index to this new smallest
                temp2 = temp; // store the new smallest value
            }
        }
        return index; // return the final index to closest light
    }
}
