using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AvoidingFlyer_Dila : MonoBehaviour
{
    public float targetVelocity = 10.0f;
    public int numberOfRays = 17;
    public float angle = 90;
    public float rayRange = 2;
    
    public Transform[] waypoints; // Waypoints
    private int currentWaypointIndex = 0; // Current waypoint index
    private float minDistanceToWaypoint = 0.1f; // Distance to consider reaching the waypoint

    // Update is called once per frame
    void Update()
    {
        // Check if the object has reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < minDistanceToWaypoint)
        {
            // Move to the next waypoint in the array
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Calculate direction towards the current waypoint
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        // Raycasting to detect obstacles and adjust movement
        var deltaPosition = Vector3.zero;
        for (int i = 0; i < numberOfRays; ++i)
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
            var direction = rotation * rotationMod * targetDirection; // Use targetDirection instead of Vector3.forward

            var ray = new Ray(this.transform.position, direction);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, rayRange))
            {
                deltaPosition -= (1.0f / numberOfRays) * targetVelocity * direction;
            }
            else
            {
                deltaPosition += (1.0f / numberOfRays) * targetVelocity * direction;
            }
        }

        // Move the object towards the waypoint
        this.transform.position += deltaPosition * Time.deltaTime;

        // Optionally rotate towards the current waypoint
        this.transform.LookAt(waypoints[currentWaypointIndex].position);
    }

    private void OnDrawGizmos()
    {
        // Draw the rays for debugging
        for (int i = 0; i < numberOfRays; ++i)
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
            var direction = rotation * rotationMod * (waypoints.Length > 0 ? (waypoints[currentWaypointIndex].position - transform.position).normalized : Vector3.forward);
            Gizmos.DrawRay(this.transform.position, direction * rayRange);
        }
    }
}
