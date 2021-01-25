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
        // Using/interacting with something/someone
        Interacting,
        // Running after target's current or last seen location
        Chasing,
        // Wandering near where the target last was to try and find them
        Searching,
        // Target's location is known, and target is hostile
        Attacking
    }

    [System.Serializable]
    public struct TimedBehavior {
        public float _time;
        public bool _interruptibleBySearch;
        public bool _interruptibleByDialogue;
        public bool _interruptibleByNextBehavior;
        public State _state;
        public Vector3 [] _waypoints;
    }

    [SerializeField] bool active;

    [Header ("State")]
    [SerializeField] State currentState = State.Idle;
    [SerializeField] int currentBehaviorIndex;
    [SerializeField] TimedBehavior[] behaviors;

    [Header ("Detection")]
    [SerializeField] Transform eyes;
    [SerializeField] Transform visionTarget;
    [SerializeField] LayerMask visionMask;
    [SerializeField] float visionRange;
    [SerializeField] float visionAlertTime;
    [SerializeField] float chasingVisionBreakTime;
    [SerializeField] float searchTime;
    [SerializeField] float preferredAttackRange;
    [SerializeField] float maxAttackRange;
    [SerializeField] bool alerted;
    [SerializeField] bool behaviorDelayed;
    [SerializeField] bool visionTargetHostile;

    NavMeshAgent agent;
    List<Vector3> waypoints = new List<Vector3>();
    int currentWaypointIndex = 0;
    float visionAlertIterator;
    float searchIterator;

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

    private void Start () {
        SetupBehavior ();
    }

    private void Update () {

        if (!active) return;

        // TODO: Refactor so it doesn't use one massive switch/case? Not necessary, but might be nice
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
                    PickWanderDestination ();
                }
                break;
            case State.Interacting:
                // TODO: Interaction
                break;
            case State.Chasing:
                if (TargetIsVisible ()) {
                    agent.destination = visionTarget.position;
                    searchIterator = 0;
                    if (visionTargetHostile && agent.remainingDistance < preferredAttackRange) {
                        currentState = State.Attacking;
                        agent.destination = transform.position;
                    }
                } else {
                    searchIterator += Time.deltaTime;
                    if (searchIterator > chasingVisionBreakTime) {
                        // Entity lost sight of target; start searching for it nearby
                        currentState = State.Searching;
                        searchIterator = 0;
                    }
                }
                break;
            case State.Searching:
                if (TargetIsVisible ()) {
                    searchIterator = 0;
                    if (visionTargetHostile && (transform.position - visionTarget.position).magnitude < preferredAttackRange) {
                        currentState = State.Attacking;
                        agent.destination = transform.position;
                    } else {
                        currentState = State.Chasing;
                        agent.destination = visionTarget.position;
                    }
                } else {
                    if (NavMeshHasReachedDestination ()) {
                        PickRandomDestination ();
                    }
                    searchIterator += Time.deltaTime;
                    if (searchIterator > searchTime) {
                        // Entity lost target; give up the search and go back to previous behavior
                        currentState = behaviors [currentBehaviorIndex]._state;
                        searchIterator = 0;
                        alerted = false;
                        if (behaviorDelayed) {
                            SetBehavior (behaviors [currentBehaviorIndex]);
                            behaviorDelayed = false;
                        }
                    }
                }
                break;
            case State.Attacking:
                // TODO: shooting and stuff
                if ((transform.position - visionTarget.position).magnitude > maxAttackRange || !TargetIsVisible ()) {
                    currentState = State.Chasing;
                    agent.destination = visionTarget.position;
                } else if ((transform.position - visionTarget.position).magnitude > preferredAttackRange) {
                    agent.destination = visionTarget.position;
                } else {
                    agent.destination = transform.position;
                }
                break;
        }

        // If the NPC can see their target, alert them!
        if (TargetIsVisible () && !alerted) {
            if (!behaviors [currentBehaviorIndex]._interruptibleBySearch) {
                Debug.LogWarning ("Why is this entity searching for an object if its current state is not interruptible? Set visionTarget to null or let this behavior be interrupted!");
                return;
            }
            visionAlertIterator += Time.deltaTime;
            if (visionAlertIterator > visionAlertTime) {
                alerted = true;
                currentState = State.Chasing;
                visionAlertIterator = 0;
            }
        } else {
            visionAlertIterator = 0;
        }

        // If the next state is due and the current state is interruptible, switch to the next state
        if (currentBehaviorIndex < behaviors.Length - 1 && behaviors[currentBehaviorIndex]._interruptibleByNextBehavior && TimeSystem.currentTime > behaviors [currentBehaviorIndex + 1]._time) {
            currentBehaviorIndex++;
            // Don't actually set the state of the entity if they are searching/chasing/attacking something unless the new behavior is uninterruptible. 
            if (alerted && behaviors [currentBehaviorIndex]._interruptibleBySearch) {
                behaviorDelayed = true;
                return;
            } else {
                SetBehavior (behaviors [currentBehaviorIndex]);
            }
        }
    }

    // Picks a random location on the navmesh inside a boundary
    // Waypoints here are not used in pathfinding.
    // Instead, they are used as vertices of a rectangle that defines a boundary for entity to randomly wander around in
    // Bottom-left corner is at index 0, top-left is 1, top-right is 2, bottom-right is 3
    void PickWanderDestination () {
        float leftEdgePos = (waypoints [0].x + waypoints [1].x) / 2;
        float rightEdgePos = (waypoints [2].x + waypoints [3].x) / 2;
        float topEdgePos = (waypoints [1].z + waypoints [2].z) / 2;
        float bottomEdgePos = (waypoints [0].z + waypoints [3].z) / 2;
        Vector3 position = new Vector3 (Random.Range (leftEdgePos, rightEdgePos), 0, Random.Range (bottomEdgePos, topEdgePos));

        NavMeshHit navHit;
        NavMesh.SamplePosition (position, out navHit, visionRange, -1);
        agent.destination = navHit.position;
    }

    // Picks a random new location on the navmesh within vision distance from the agent's previous destination
    void PickRandomDestination () {
        NavMeshHit navHit;
        NavMesh.SamplePosition (agent.destination + Random.insideUnitSphere * visionRange, out navHit, visionRange, -1);
        agent.destination = navHit.position;
    }

    void SetBehavior (TimedBehavior newBehavior) {

        currentState = newBehavior._state;

        ClearWaypoints ();
        AddWaypoints (newBehavior._waypoints);
        currentWaypointIndex = 0;

        // Wandering state has waypoints, but ignores them for pathfinding
        if (currentState == State.WanderingArea) {
            agent.destination = transform.position;
            return;
        }

        if (waypoints.Count > 0) {
            agent.destination = waypoints [currentWaypointIndex];
        }
    }

    bool NavMeshHasReachedDestination () {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    bool TargetIsVisible () {
        if (!visionTarget) { return false; }
        Vector3 dist = visionTarget.position - eyes.position;
        if (dist.magnitude > visionRange) { return false; }
        Debug.DrawRay (eyes.position, dist.normalized * visionRange, Color.red, 1);
        RaycastHit hit;
        Physics.Raycast (eyes.position, dist.normalized, out hit, visionRange, visionMask, QueryTriggerInteraction.Ignore);
        if (hit.transform == null) { return false; }
        return hit.transform.tag == Statics.PlayerTagName;
    }
}
