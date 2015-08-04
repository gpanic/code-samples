using UnityEngine;
using System.Collections;

public class MoveAction : TargetAction
{
    private enum State { MovingForward, MovingBack, Waiting, Stationary };

    public Transform moveTo;
    public bool moveBack = true;
    public float moveTime = 3.0f;
    public float waitTime = 2.5f;

    private Vector3 originalPosition;
    private float moveTimer = 0.0f;
    private float waitTimer = 0.0f;
    private int connectedHooks;
    private Vector3 currentPosition;
    private Vector3 oldPosition;
    private State state = State.Stationary;

    private void Start()
    {
        GameObject.Find("killplanes").GetComponent<Killplane>().OnTeleport += OnTeleport;
        originalPosition = transform.position;
        currentPosition = originalPosition;
    }

    // interpolate position between fixed updates
    private void Update()
    {
        float t = ((Time.time - Time.fixedTime) / Time.fixedDeltaTime);
        Vector3 pos = Vector3.Lerp(oldPosition, currentPosition, t);
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        oldPosition = currentPosition;
        transform.position = currentPosition;

        switch (state)
        {
            case State.MovingForward:
                moveTimer += Time.deltaTime;
                Move();
                if (moveTimer >= moveTime)
                {
                    state = State.Stationary;
                    moveTimer = moveTime;
                }
                break;
            case State.Waiting:
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    state = State.MovingBack;
                    waitTimer = 0.0f;
                }
                break;
            case State.MovingBack:
                moveTimer -= Time.deltaTime;
                Move();
                if (moveTimer <= 0.0f)
                {
                    state = State.Stationary;
                    moveTimer = 0.0f;
                }
                break;
        }


        // do not actually move the object on FixedUpdate
        // only calculate next position for Update to interpolate
        // the position to
        currentPosition = transform.position;
        transform.position = oldPosition;
    }

    // when the player respawns, reset
    private void OnTeleport()
    {
        currentPosition = originalPosition;
        transform.position = originalPosition;
        state = State.Stationary;
        moveTimer = 0;
        waitTimer = 0;
    }

    private void Move()
    {
        transform.position = Vector3.Lerp(originalPosition, moveTo.position, Mathf.Clamp01(moveTimer / moveTime));
    }

    public override void StartAction()
    {
        ++connectedHooks;

        // do not react to a second hook being connected
        if (transform.position == originalPosition && connectedHooks == 1)
        {
            state = State.MovingForward;
        }
    }

    public override void EndAction()
    {
        --connectedHooks;

        // only react when the last hook has been disconnected
        if (moveBack && connectedHooks == 0)
        {
            if (moveTimer >= moveTime)
            {
                state = State.Waiting;
            }
            else
            {
                state = State.MovingBack;
            }
        }
    }
}
