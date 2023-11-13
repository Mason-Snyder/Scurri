using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class shadowHide : MonoBehaviour
{
    public Light2D[] consideredLights;
    public float
        startFade,
        endFade;
   [SerializeField] private int
        closestLight;
    private float
        lightDist;
    private RaycastHit2D hit;
    private LayerMask geometry;
    private Color tempClr;
    void Start()
    {
        geometry = LayerMask.GetMask("geometry");
    }

    private void FixedUpdate()
    {
        closestLight = getClosestLight();
        tempClr = GetComponent<SpriteRenderer>().color;
        hit = Physics2D.Raycast(this.transform.position, (consideredLights[closestLight].gameObject.transform.position - this.transform.position).normalized, startFade, geometry);
        if (hit && hit.collider.gameObject.name == consideredLights[closestLight].gameObject.name)
                tempClr.a = 1 - Vector2.Distance(gameObject.transform.position, consideredLights[0].gameObject.transform.position) / startFade;
        else
            tempClr.a = 0;
        GetComponent<SpriteRenderer>().color = tempClr;
    }
    void Update()
    {
        
    }

    private int getClosestLight()
    {
        int index = 0;
        float temp2 = Vector2.Distance(gameObject.transform.position, consideredLights[0].gameObject.transform.position); // find closest source
        for (int i = 1; i < consideredLights.Length; i++)
        {
            float temp = Vector2.Distance(gameObject.transform.position, consideredLights[i].gameObject.transform.position);
            if (temp < temp2)
                index = i;
        }
        return index;
    }
}
