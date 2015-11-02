using UnityEngine;
using System.Collections;

public class TargetTrigger : HookableObject
{
    public GameObject target;
    public bool triggetStartAction = true;
    public bool triggetEndAction = true;
    private TargetAction[] targetActions = new TargetAction[0];

    void Start()
    {
        if (target)
        {
            targetActions = target.GetComponents<TargetAction>();
        }
    }

    public override void Unhhooked(HookController controller, HookState state)
    {
        if (triggetEndAction)
        {
            foreach (TargetAction ta in targetActions)
            {
                ta.EndAction();
            }
        }
    }

    public override void Hooked(HookController controller, HookState state)
    {
        if (triggetStartAction)
        {
            foreach (TargetAction ta in targetActions)
            {
                ta.StartAction();
            }
        }
    }
}
