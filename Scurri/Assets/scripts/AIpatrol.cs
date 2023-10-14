using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class AIpatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    public Vector2 destPoint;
    public float // behaviour congif, can add numbered vars for additional radii levels without extra functions
        range = 20,
        trackRadius = 5,
        seekRadius1 = 10,
        homeDegree1 = 1.25f,
        homeDegree;
    private enum actions { search, go, track, seekSearch, none }
    [SerializeField] LayerMask groundLayer, playerLayer; // for raycasting, confirming on navmesh and line of sight respectively
    [SerializeField] private actions action;
    private float targetDist;
    private bool corou = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player =  GameObject.Find("scurri'd");

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(getDist());
    }

    IEnumerator getDist() // queue the action to be performed based on targetDist, and adjust behaviour config accordingly
    {
        while (corou)
        {
            if (action != actions.go)
            {
                targetDist = Vector2.Distance(this.transform.position, player.transform.position);

                if (Physics2D.Raycast(this.transform.position, (player.transform.position - this.transform.position).normalized, trackRadius, playerLayer)) // must stay as first if, checks line of sight
                    action = actions.track;
                else if (targetDist < seekRadius1) // expandable via seekRadiusN/homeDegreeN
                {
                    action = actions.seekSearch;
                    homeDegree = homeDegree1;
                }
                else
                    action = actions.search; // random from full range if player is not nearby
                Patrol(); // generates new destination 
            }
            else if (Vector3.Distance(transform.position, destPoint) < 1) // allows smooth motion and avoiding getting stuck, queues picking a new dest
                action = actions.none;

            yield return new WaitForSeconds(.05f);
        }

    }

    void Patrol() // calls applicable destination-generating function
    {
        switch (action)
        {
            case actions.search:
                searchNorm();
                break;
            case actions.go:
                agent.SetDestination(destPoint);
                break;
            case actions.seekSearch:
                seekSearch();
                break;
            case actions.track:
                agent.SetDestination(player.transform.position); // very important that destPoint is not used here
                action = actions.none;
                break;
            default:// should never trigger
                action = actions.none;
                break;
        }
    }

    void searchNorm() // full range allowed
    {
        do
        {
            destPoint = new Vector2(transform.position.x + Random.Range(-range, range), transform.position.y + Random.Range(-range, range));
            if (Physics2D.Raycast(destPoint, Vector2.zero, 1, groundLayer)) // check for being on navmesh
                action = actions.go;
        }
        while (action != actions.go);
        Patrol(); // implement the new destPoint, important that is goes back to the switch and is not set here
    }

    void seekSearch() // destPoint will be within a radius around the player that is itself a percentage of the distance between THIS and player (homeDegree)
    {
        do
        {
            destPoint = new Vector2(transform.position.x + Random.Range(-range, range), transform.position.y + Random.Range(-range, range));
            if ((Vector2.Distance(destPoint, player.transform.position) <= targetDist * homeDegree) && Physics2D.Raycast(destPoint, Vector2.zero, 1, groundLayer))
                action = actions.go;
        }
        while (action != actions.go);
        Patrol(); // same as above
    }
}
