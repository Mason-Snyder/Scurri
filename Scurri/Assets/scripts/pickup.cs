using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    GameManager manager;
    private bool isPicked = false;
    void Start()
    {
        manager = GameObject.Find("Canvas").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isPicked)
        {
            isPicked = true;
            manager.newItem();
            Destroy(gameObject);
        }
    }
}
