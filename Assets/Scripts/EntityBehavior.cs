using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityBehavior : MonoBehaviour {
    
    public enum State {
        Idle,
        // Following a set of waypoints
        FollowingPath,
        // Wandering an area (within the boundary of 4 set waypoints)
        WanderingArea,
        // Alert states are identical to their counterparts, but they can be interrupted if they spot an enemy
        IdleAlert,
        FollowingPathAlert,
        WanderingAreaAlert,
        Interacting,
        // Looking for someone/something, but the target's location is unknown
        Searching,
        // Target's location is known, but too far to interact with
        Chasing,
        Attacking
    }

    [System.Serializable]
    public struct TimedBehavior {
        public float _time;
        public State _state;
        public Vector3 [] _waypoints;
    }

    [Header ("State")]
    [SerializeField] State currentState = State.Idle;
    [SerializeField] TimedBehavior[] behaviors;
    [SerializeField] bool alerted;
    [SerializeField] bool active;

    NavMeshAgent agent;
    List<Vector3> waypoints = new List<Vector3>();
    int currentWaypointIndex = 0;
    [SerializeField] int currentBehaviorIndex = 0;

    // Adds a new waypoint to a patrol path. Useful for cutscenes
    public void AddWaypoint (Vector3 newWaypoint) {
        waypoints.Add (newWaypoint);
    }

    // Adds an array of waypoints to a patrol path. Useful for setting up patrol paths
    public void AddWaypoints (Vector3[] newWaypoints) {
        for (int i = 0; i < newWaypoints.Length; i++) {
            waypoints.Add (newWaypoints[i]);
        }
    }

    public void ClearWaypoints () {
        waypoints.Clear ();
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.black;
        for (int i = 0; i < waypoints.Count; i++) {
            Gizmos.DrawSphere (waypoints [i], 1);
        }
        if (agent) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (agent.destination, 2);
        }
    }

    // TODO: Called by OnGameStart
    [ContextMenu ("Start game")]
    void SetupBehavior () {
        
        currentBehaviorIndex = 0;

        for (int i = 0; i < behaviors.Length; i++) {
            if (behaviors[i]._time < TimeSystem.currentTime) {
                currentBehaviorIndex = i;
            } else {
                break;
            }
        }

        active = true;

        SetBehavior (behaviors [currentBehaviorIndex]);
    }

    private void Awake () {
        agent = GetComponent<NavMeshAgent> ();
    }

    private void Update () {

        if (!active) return;

        // TODO: Refactor so it doesn't use one massive switch/case?
        switch (currentState) {
            case State.FollowingPath:
                if (NavMeshHasReachedDestination ()) {
                    currentWaypointIndex++;
                    if (currentWaypointIndex > waypoints.Count - 1) {
                        currentState = State.Idle;
                    } else {
                        agent.destination = waypoints [currentWaypointIndex];
                    }
                }
                break;
            case State.WanderingArea:
                if (NavMeshHasReachedDestination ()) {
                    // Waypoints are used as corners for a boundary that the entity can wander within, and thus are ignored in pathfinding
                    // Bottom-left corner is at index 0, top-left is 1, top-right is 2, bottom-right is 3
                    float leftEdgePos = (waypoints [0].x + waypoints [1].x) / 2;
                    float rightEdgePos = (waypoints [2].x + waypoints [3].x) / 2;
                    float topEdgePos = (waypoints [1].z + waypoints [2].z) / 2;
                    float bottomEdgePos = (waypoints [0].z + waypoints [3].z) / 2;

                    agent.destination = new Vector3 (Random.Range (leftEdgePos, rightEdgePos), 0, Random.Range (bottomEdgePos, topEdgePos));
                }
                break;
            case State.Idle:
                break;
        }

        // If there is another state and that state is due, switch to it
        if (currentBehaviorIndex < behaviors.Length - 1 && TimeSystem.currentTime > behaviors [currentBehaviorIndex + 1]._time) {
            currentBehaviorIndex++;
            SetBehavior (behaviors [currentBehaviorIndex]);
        }
    }

    void SetBehavior (TimedBehavior newBehavior) {

        currentState = newBehavior._state;

        ClearWaypoints ();
        AddWaypoints (newBehavior._waypoints);
        currentWaypointIndex = 0;

        // Waypoints are used as corners for a boundary that the entity can wander within, and thus are ignored in pathfinding
        if (currentState == State.WanderingArea) {
            float leftEdgePos = (waypoints [0].x + waypoints [1].x) / 2;
            float rightEdgePos = (waypoints [2].x + waypoints [3].x) / 2;
            float topEdgePos = (waypoints [1].z + waypoints [2].z) / 2;
            float bottomEdgePos = (waypoints [0].z + waypoints [3].z) / 2;

            agent.destination = new Vector3 (Random.Range (leftEdgePos, rightEdgePos), 0, Random.Range (bottomEdgePos, topEdgePos));
            return;
        }

        if (waypoints.Count > 0) {
            agent.destination = waypoints [currentWaypointIndex];
        }
    }

    bool NavMeshHasReachedDestination () {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
