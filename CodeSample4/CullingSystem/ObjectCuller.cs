using UnityEngine;
using System.Collections;
using System;

public abstract class ObjectCuller : MonoBehaviour
{
    [Serializable]
    public class CullingGroup
    {
        public string groupName;
        public GameObject[] objectGroups;
        public float cullDistance = 350.0f;
    }

    public float updateObjectsPerFrame = 1;
    public CullingGroup[] cullingGroups;

    private Transform player;
    private int cullingGroupIndex = 0;
    private int objectGroupIndex = 0;
    private int objectIndex = 0;

    protected abstract void Activate(GameObject objectToActivate);
    protected abstract void Deactivate(GameObject objectToDeactivate);

    private void Start()
    {
        if ((player = GameObject.FindGameObjectWithTag(Tags.player).transform) == null)
        {
            Debug.LogError("Missing a player object.");
            enabled = false;
        }
        enabled = ValidateCullingGroups();
    }

    private void Update()
    {
        // we can update as many objects per frame as we want, a tradeoff between
        // activation/deactivation speed and performance
        for (int i = 0; i < updateObjectsPerFrame; ++i)
        {
            CullingGroup cullingGroup = cullingGroups[cullingGroupIndex];
            GameObject objectGroup = cullingGroup.objectGroups[objectGroupIndex];
            Transform obj = objectGroup.transform.GetChild(objectIndex);

            if (IsBeyondDrawDistance(player.position, obj.position, cullingGroup))
            {
                Deactivate(obj.gameObject);
            }
            else
            {
                Activate(obj.gameObject);
            }


            // update and wrap culling group, object group and object indices
            ++objectIndex;
            if (objectIndex >= objectGroup.transform.childCount)
            {
                objectIndex = 0;
                ++objectGroupIndex;
            }

            if (objectGroupIndex >= cullingGroup.objectGroups.Length)
            {
                objectGroupIndex = 0;
                ++cullingGroupIndex;
                cullingGroupIndex %= cullingGroups.Length;
            }
        }
    }

    private bool IsBeyondDrawDistance(Vector3 playerPosition, Vector3 objectPosition, CullingGroup cullingGroup)
    {
        return Vector3.Distance(playerPosition, objectPosition) > cullingGroup.cullDistance;

    }

    // doesn't allow empty culling groups or object groups to not waste time
    private bool ValidateCullingGroups()
    {
        if (cullingGroups.Length == 0)
        {
            Debug.LogError("Script requires at least one culling group.");
            return false;
        }
        else
        {
            foreach (CullingGroup cullingGroup in cullingGroups)
            {
                if (cullingGroup.objectGroups.Length == 0)
                {
                    Debug.LogError("A culling group contains no object groups.");
                    return false;
                }
                else
                {
                    foreach (GameObject objectGroup in cullingGroup.objectGroups)
                    {
                        if (objectGroup.transform.childCount == 0)
                        {
                            Debug.LogError("An object group contains no child objects.");
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
