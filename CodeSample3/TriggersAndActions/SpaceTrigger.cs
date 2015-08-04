using UnityEngine;
using System.Collections;

public class SpaceTrigger : MonoBehaviour
{
    public GameObject target;
    public bool triggerStartAction = true;
    public bool triggerEndAction = true;
    private TargetAction[] targetActions = new TargetAction[0];

    private void Start()
    {
        if (target)
        {
            targetActions = target.GetComponents<TargetAction>();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == Tags.player && triggerStartAction)
        {
            foreach (TargetAction ta in targetActions)
            {
                ta.StartAction();
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == Tags.player && triggerEndAction)
        {
            foreach (TargetAction ta in targetActions)
            {
                ta.EndAction();
            }
        }
    }
}
