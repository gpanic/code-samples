using UnityEngine;
using System.Collections;

public class RotateBeamAction : TargetAction
{
    private enum State { RotatingForward, RotatingBack, Stationary };

    public Vector3 rotationAxis = new Vector3(1, 0, 0);
    public float rotationAmount = 90;
    public float rotationForwardTime = 4.0f;
    public float rotationBackwardsTime = 4.0f;

    private Transform pivot;
    private float rotationForwardTimer = 0.0f;
    private float rotationBackwardsTimer = 0.0f;
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private Quaternion currentRotation;
    private Vector3 originalPivotPosition;
    private State state = State.Stationary;

    // we want to always rotate in a single direction with the same speed
    // we use rotationPercent to mark rotation progress and to translate
    // between two possibly different rotation times
    private float rotationPercent = 0.0f;

    private void Start()
    {
        pivot = transform.FindChild("pivot").GetComponent<Transform>();
        fromRotation = transform.localRotation;
        toRotation = fromRotation * Quaternion.AngleAxis(rotationAmount, transform.InverseTransformDirection(rotationAxis));
        originalPivotPosition = pivot.position;
    }

    private void Update()
    {
        switch (state)
        {
            case State.RotatingForward:
                RotateForwardAroundPivot();
                break;
            case State.RotatingBack:
                RotateBackAroundPivot();
                break;
        }
    }

    private void RotateForwardAroundPivot()
    {
        RotateAroundPivot();
        rotationForwardTimer += Time.deltaTime;
        rotationPercent = rotationForwardTimer / rotationForwardTime;
        if (rotationForwardTimer >= rotationForwardTime)
        {
            rotationForwardTimer = 0.0f;
            state = State.Stationary;
        }
    }

    private void RotateBackAroundPivot()
    {
        RotateAroundPivot();
        rotationBackwardsTimer -= Time.deltaTime;
        rotationPercent = rotationBackwardsTimer / rotationBackwardsTime;
        if (rotationBackwardsTimer <= 0)
        {
            rotationBackwardsTimer = rotationBackwardsTime;
            state = State.Stationary;
        }
    }

    private void RotateAroundPivot()
    {
        transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, Mathf.Clamp01(rotationPercent));
        transform.position += originalPivotPosition - pivot.position;
    }


    public override void StartAction()
    {
        state = State.RotatingForward;
        rotationForwardTimer = rotationPercent * rotationForwardTime;
    }

    public override void EndAction()
    {
        state = State.RotatingBack;
        rotationBackwardsTimer = rotationPercent * rotationBackwardsTime;
    }
}
