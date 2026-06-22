using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script will move a building between X points as an obstacle for the player
/// </summary>
public class MovingBuilding : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] List<Transform> Waypoints = new List<Transform>();
    [Range(0f, 1f)]
    [SerializeField] float CycleSpeed = 0.3f;
    [SerializeField] float DistanceToSwitch = 5f;

    [Header("DEBUG")]
    [SerializeField] bool stopped = false;
    private Transform currentWaypoint;
    private Transform nextWaypoint;
    private int WaypointIndex = 0;
    private int nextWaypointIndex = 0;
    private void Start()
    {
        // set the location of the building to the start

        transform.position = Waypoints[0].position;

        currentWaypoint = Waypoints[0];

        nextWaypoint = Waypoints[1];

        nextWaypointIndex = WaypointIndex + 1;
    }

    private void Update()
    {

        if (Waypoints.Count < 2 || stopped)
        {
            // If there isnt two places to move between just do nothing

            return;
        }

        

            // Get a position between your point and the next then move between them
            Vector3 CurrentPosition = Vector3.Lerp(transform.position, nextWaypoint.position, CycleSpeed/64);
            transform.position = CurrentPosition;

            // Check the distance between my waypoint and the next
            if (Vector3.Distance(transform.position, Waypoints[nextWaypointIndex].position) < DistanceToSwitch)
            {
                WaypointIndex = (WaypointIndex + 1) % Waypoints.Count;
                nextWaypointIndex = (nextWaypointIndex + 1) % Waypoints.Count;

                currentWaypoint = Waypoints[WaypointIndex];
                nextWaypoint = Waypoints[nextWaypointIndex];

            }
            
        


        
    }

    private void ToggleMovement()
    {
        // Use this method if you want to toggle the building from moving or not

        if (stopped == false)
        {
            stopped = true;
        }
        else
        {
            stopped = false;
        }
    }

}
