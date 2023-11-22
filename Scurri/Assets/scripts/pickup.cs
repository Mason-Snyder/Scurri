using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    GameManager manager; // gamemanager script reference
    private bool isPicked = false; // so the onpickup event stuff doesnt trigger twice
    void Start() // once on creation
    {
        manager = GameObject.Find("Canvas").GetComponent<GameManager>(); // find gamemanager
    }

    private void OnTriggerEnter2D(Collider2D collision) // when touched:
    {
        if (collision.gameObject.CompareTag("Player") && !isPicked) // if its the player and we havent already touched it:
        {
            isPicked = true; // say we touched it
            manager.newItem(); // start all the consequential processes in the gamemanger
            Destroy(gameObject); // poof, gone
        }
    }
}
