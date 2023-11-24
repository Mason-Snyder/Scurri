using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathTouch : MonoBehaviour
{
    GameManager manager; // gamemanager reference
    private bool
        isTouched = false; // gatekeeper flag

    private void Start() // once at start
    {
        manager = GameObject.Find("Canvas").GetComponent<GameManager>(); // initialize manager
    }

    private void OnTriggerEnter2D(Collider2D collision) // when touch a thing:
    {
        if (!isTouched && collision.CompareTag("Player")) // if gatekeeper hasnt flagged and its the player:
        {
            isTouched = true; // flag gatekeeper
            manager.death(); // do the death stuff 
            StartCoroutine(hitReset()); // start reset flag
        }
    }

    IEnumerator hitReset() // to reset flag:
    {
        yield return new WaitForSeconds(1); // wait seconds
        isTouched = false; // unflag
    }
}
