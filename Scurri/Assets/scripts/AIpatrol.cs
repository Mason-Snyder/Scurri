using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIpatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    private enum action {search, go, track}

    [SerializeField] LayerMask groundLayer, playerLayer;

    // patrol stuff
    public Vector3 destPoint;
    private bool walkpointSet;
    [SerializeField] private float range = 20, targetDist;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player =  GameObject.Find("scurri'd");

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(getDist());
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    IEnumerator getDist()
    {
        targetDist = Vector3.Distance(this.transform.position, player.transform.position);
        yield return new WaitForSeconds(0.5f);
    }

    void Patrol()
    {
        if (!walkpointSet) 
            SearchForDest();
        else if(walkpointSet)
            agent.SetDestination(destPoint);
        if(Vector3.Distance(transform.position, destPoint) < 1)
            walkpointSet = false;
    }

    void SearchForDest()
    {
        float y = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y+y, transform.position.z);

        if(Physics2D.Raycast(destPoint, Vector2.zero, 1, groundLayer) && (Vector3.Distance(destPoint, player.transform.position) <=  targetDist * 1.75))
            walkpointSet = true;
    }
}
