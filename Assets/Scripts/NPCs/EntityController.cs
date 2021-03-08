using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour {

    public enum State {
        Idle,
        // Following a set of waypoints
        FollowingPath,
        // Wandering an area (within the boundary of 4 set waypoints)
        WanderingArea,
        // Walking to then using/interacting with something/someone
        Interacting,
        // Running after target's current or last seen location
        Chasing,
        // Wandering near where the target last was to try and find them
        Searching,
        // Target's location is known, and target is hostile
        Attacking
    }

    [System.Serializable]
    public struct Behavior {
        public bool _interruptibleBySearch;
        public bool _interruptibleByDialogue;
        public bool _interruptibleByNewBehavior;
        public State _state;
        public Transform _visionTarget;
        public bool _visionTargetHostile;
        public Vector3 [] _waypoints;
    }

    public bool active = false;
    public bool alerted;/* { get; private set; }*/
    public bool talking;

    [SerializeField] State currentState = State.Idle;

    [Header ("Detection")]
    [SerializeField] Transform eyes;
    [SerializeField] Transform visionTarget;
    [SerializeField] LayerMask visionMask;
    [SerializeField] float visionRange;
    [SerializeField] float visionAlertTime;
    [SerializeField] float chasingVisionBreakTime;
    [SerializeField] float searchTime;
    [SerializeField] bool visionTargetHostile;

    [Header ("Interaction")]
    [SerializeField] float maxAttackRange;
    [SerializeField] float preferredAttackRange;

    [Header ("Behavior")]
    [SerializeField] Behavior nextBehavior;
    [SerializeField] Behavior currentBehavior;
    [SerializeField] bool behaviorComplete = true;
    [SerializeField] List<Vector3> waypoints = new List<Vector3> ();
    [SerializeField] int currentWaypointIndex = 0;

    [SerializeField] List<bool> appearsInCutscenes;

    NavMeshAgent agent;
    Animator anim;
    float visionAlertIterator;
    float searchIterator;
    bool wasActiveBeforeCutscene;

    private void Awake () {
        agent = GetComponent<NavMeshAgent> ();
        anim = GetComponent<Animator> ();
        nextBehavior = default (Behavior);
        currentBehavior = default (Behavior);
        behaviorComplete = true;
    }

    private void Update () {

        if (!active) return;

        HandleQueuedBehavior ();

        switch (currentState) {
            case State.FollowingPath:
                if (NavMeshAgentHasReachedDestination ()) {
                    currentWaypointIndex++;
                    if (currentWaypointIndex > waypoints.Count - 1) {
                        currentState = State.Idle;
                        anim.Play ("Idle", 0);
                        anim.Play ("Idle", 2);
                        behaviorComplete = true;
                    } else {
                        agent.destination = waypoints [currentWaypointIndex];
                    }
                }
                break;
            case State.WanderingArea:
                if (NavMeshAgentHasReachedDestination ()) {
                    PickWanderDestination ();
                    behaviorComplete = true;
                }
                break;
            case State.Interacting:
                if (NavMeshAgentHasReachedDestination ()) {
                    // TODO: Interaction
                    // if (interactionIsComplete) {
                    // Typically, item interactions will not be interruptible by the next behavior, but they can sometimes be interrupted by dialogue/combat
                    // This can lead to weird situations where the NPC will never get to pick up an item but will still attempt to use it later
                    // Try to avoid this scenario by either making the interaction ignore all interrupts or making its result not important to the flow of the game 
                        behaviorComplete = true;
                    // }
                }
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
                    if (NavMeshAgentHasReachedDestination ()) {
                        PickRandomDestination ();
                    }
                    searchIterator += Time.deltaTime;
                    if (searchIterator > searchTime) {
                        // Entity lost target; give up the search and go back to previous behavior
                        searchIterator = 0;
                        alerted = false;
                        currentState = currentBehavior._state;
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
            case State.Idle:
                behaviorComplete = true;
                break;
        }

        // If the NPC can see their target, alert them!
        if (TargetIsVisible () && !alerted) {
            if (!currentBehavior._interruptibleBySearch) {
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
    }

    // Sets the current behavior, queueing the new behavior for later if certain interrupt conditions are met
    public void SetBehavior (Behavior newBehavior) {

        // Queue up new behavior rather than applying it immmediately if:
        // 1. There is a current behavior and it is uninterruptable and incomplete,
        // 2. The entity is alerted and the new behavior is interruptable by searches, or
        // 3. The entity is in dialogue and the new behavior is interruptable by dialogue
        if ((!behaviorComplete && !currentBehavior.Equals (default (Behavior)) && !currentBehavior._interruptibleByNewBehavior) || 
            (alerted && newBehavior._interruptibleBySearch) || 
            (talking && newBehavior._interruptibleByDialogue)) {

            nextBehavior = newBehavior;
            return;
        }

        behaviorComplete = false;
        currentBehavior = newBehavior;
        currentState = newBehavior._state;

        // Waypoints
        ClearWaypoints ();
        AddWaypoints (newBehavior._waypoints);
        currentWaypointIndex = 0;

        // Vision
        visionTarget = newBehavior._visionTarget;
        visionTargetHostile = newBehavior._visionTargetHostile;

        if (currentState == State.FollowingPath) {
            anim.Play ("WalkingForward", 0);
            anim.Play ("WalkingForward", 2);
        }

        // Wandering state ignores normal pathfinding
        if (currentState == State.WanderingArea) {
            agent.destination = transform.position;
            return;
        }

        // Start pathfinding
        if (waypoints.Count > 0) {
            agent.destination = waypoints [currentWaypointIndex];
        }
    }

    // Overrides the current behavior, no matter what it is or what it should be interrupted by
    // Super immersion breaking if, say, the player is in combat and the hostile NPC just stops fighting, so avoid using this in gameplay
    public void SetBehaviorOverrideQueue (Behavior newBehavior) {

        behaviorComplete = false;
        currentBehavior = newBehavior;
        currentState = newBehavior._state;

        // Waypoints
        ClearWaypoints ();
        AddWaypoints (newBehavior._waypoints);
        currentWaypointIndex = 0;

        // Vision
        visionTarget = newBehavior._visionTarget;
        visionTargetHostile = newBehavior._visionTargetHostile;

        // Wandering state ignores normal pathfinding
        if (currentState == State.WanderingArea) {
            agent.destination = transform.position;
            return;
        }

        // Start pathfinding
        if (waypoints.Count > 0) {
            agent.destination = waypoints [currentWaypointIndex];
        }
    }

    // Applies a queued behavior if none of the queue conditions are met
    void HandleQueuedBehavior () {

        if (nextBehavior.Equals (default (Behavior)) || currentBehavior.Equals (default (Behavior))) { return; }

        // Inverse of the if statement in SetBehavior
        if (!(!behaviorComplete && !currentBehavior._interruptibleByNewBehavior) &&
            !(alerted && nextBehavior._interruptibleBySearch) &&
            !(talking && nextBehavior._interruptibleByDialogue)) {

            SetBehavior (nextBehavior);
            nextBehavior = default (Behavior);
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

    // Ensures that the agent is close to its destination and that it isn't already calculating a new path
    bool NavMeshAgentHasReachedDestination () {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    // Raycasts to the target, checking both range and LOS
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

    // Adds a new waypoint to a patrol path. Useful for cutscenes
    public void AddWaypoint (Vector3 newWaypoint) {
        waypoints.Add (newWaypoint);
    }

    // Adds an array of waypoints to a patrol path. Useful for setting up patrol paths
    public void AddWaypoints (Vector3 [] newWaypoints) {
        for (int i = 0; i < newWaypoints.Length; i++) {
            waypoints.Add (newWaypoints [i]);
        }
    }

    // Removes all waypoints
    public void ClearWaypoints () {
        waypoints.Clear ();
    }

    private void OnEnable () {
        CutsceneManager.CutsceneStart += StartCutscene;
        CutsceneManager.CutsceneStop += StopCutscene;
    }

    private void OnDisable () {
        CutsceneManager.CutsceneStart -= StartCutscene;
        CutsceneManager.CutsceneStop -= StopCutscene;
    }

    public void StartCutscene () {
        wasActiveBeforeCutscene = active;
        agent.enabled = false;
        if (appearsInCutscenes [CutsceneManager.CutsceneID]) {
            anim.SetLayerWeight (1, 1);
            anim.applyRootMotion = false;
        }
        active = false;
    }

    public void StopCutscene () {
        active = wasActiveBeforeCutscene;
        if (appearsInCutscenes [CutsceneManager.CutsceneID]) {
            anim.applyRootMotion = true;
            anim.SetLayerWeight (1, 0);
        }
        agent.enabled = true;
    }

    /*private void OnDrawGizmos () {
        Gizmos.color = Color.black;
        for (int i = 0; i < waypoints.Count; i++) {
            Gizmos.DrawSphere (waypoints [i], 1);
        }
        if (agent) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (agent.destination, 2);
        }
    }*/
}
