// enemey control brain, by Mason Snyder, last edited 10/13/23

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class AIpatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    public Vector2 destPoint;
    public float // behaviour congif, can add numbered vars for additional radii levels without extra functions
        range = 20,
        trackRadius = 8,
        seekRadius1 = 10,
        homeDegree1 = 1.25f,
        homeDegree,
        speedupRate = 4;
    private enum actions { search, go, track, seekSearch, none }
    [SerializeField] private LayerMask groundLayer, playerLayer; // for raycasting, confirming on navmesh and line of sight respectively
    [SerializeField] private actions action, distAct, lastAction;
    [SerializeField] private bool sightLine;
    private Tilemap navMap;
    private float 
        targetDist,
        dynamicTrackRadius,
        cachedSpeed;
    private bool corou = true; 
    private RaycastHit2D hit;
 
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player =  GameObject.Find("scurri'd");
        playerLayer = LayerMask.GetMask("geometry", "player");
        groundLayer = LayerMask.GetMask("navigation");
        navMap = GameObject.Find("floorWalk").gameObject.GetComponent<Tilemap>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        targetDist = Mathf.Infinity;
        dynamicTrackRadius = trackRadius * 2;
        cachedSpeed = agent.speed;

        StartCoroutine(getDist());
        StartCoroutine(trackSpeed());
    }
    private void FixedUpdate() // check line of sight to target
    {
        hit = Physics2D.Raycast(this.transform.position, (player.transform.position - this.transform.position).normalized, trackRadius, playerLayer);
        if (hit && hit.transform.CompareTag("Player"))
        {
            sightLine = true;
        }
        else
        {
            sightLine = false;
        }
    }

    private void Update()
    {
        targetDist = Vector2.Distance(this.transform.position, player.transform.position);

        if (targetDist < trackRadius && sightLine)
            distAct = actions.track;
        else if (targetDist < seekRadius1)
            distAct = actions.seekSearch;
        else
            distAct = actions.search;
        if (distAct != lastAction)
            action = actions.none;
    }
    IEnumerator getDist() // queue the action to be performed based on targetDist, and adjust behaviour config accordingly
    {
        while (corou)
        {
            if (action != actions.go)
            {
                if (sightLine) // must stay as first if, checks line of sight
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
            else if (Vector3.Distance(transform.position, destPoint) < 2) // allows smooth motion and avoiding getting stuck, queues picking a new dest
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
                lastAction = actions.track;
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
          if (validPath()) // check for being on navmesh
                action = actions.go;
        }
        while (action != actions.go);
        lastAction = actions.search;
        Patrol(); // implement the new destPoint, important that is goes back to the switch and is not set here
    }

    void seekSearch() // destPoint will be within a radius around the player that is itself a percentage of the distance between THIS and player (homeDegree)
    {
        do
        {
            destPoint = new Vector2(transform.position.x + Random.Range(-range, range), transform.position.y + Random.Range(-range, range));
            if ((Vector2.Distance(destPoint, player.transform.position) <= targetDist * homeDegree) && validPath())
                action = actions.go;
        }
        while (action != actions.go);
        lastAction = actions.seekSearch;
        Patrol(); // same as above
    }

    public bool validPath()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destPoint, path);
        return path.status == NavMeshPathStatus.PathComplete && navMap.HasTile(new Vector3Int(Mathf.RoundToInt(destPoint.x), Mathf.RoundToInt(destPoint.y), 0));
    }

    IEnumerator trackSpeed()
    {
        while (true)
        {
            if (sightLine && agent.speed < 10)
            {
                agent.speed += 0.5f;
                trackRadius = dynamicTrackRadius;
            }
            else if (!sightLine)
            {
                agent.speed = cachedSpeed;
                trackRadius = dynamicTrackRadius / 2;
            }
            yield return new WaitForSeconds((10 - agent.speed) / speedupRate);
        }
    }
}
