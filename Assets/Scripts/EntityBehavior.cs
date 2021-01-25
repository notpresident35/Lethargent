using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour {
    
    public enum State {
        Idle,
        WalkingToPoint,
        FollowingPatrolPath,
        WanderingPatrolArea,
        WanderingAimlessly,
        Interacting,
        Searching,
        Alerted,
        Hunting,
        Chasing,
        Shooting,
        IdleAlert
    }

    [SerializeField] State currentState = State.Idle;

    Queue<Vector3> waypoints = new Queue<Vector3>();

    public void SetState (State newState) {
        currentState = newState;
    }

    // Adds a new waypoint to a patrol path. Useful for cutscenes
    public void SetWaypoint (Vector3 newWaypoint) {
        waypoints.Enqueue (newWaypoint);
    }
}
