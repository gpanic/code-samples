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
        public float horizontalCullDistance = 350.0f;
        public float verticalUpwardsCullDistance = 25.0f;
        public float verticalDownwardsCullDistance = 25.0f;
        public bool verticalUpwardsCulling = true;
        public bool verticalDownwardsCulling = true;
        public bool horizontalCulling = true;
    }

    public float updateObjectsPerFrame = 1;
    public CullingGroup[] cullingGroups;

    private GameObject player;
    private int cullingGroupIndex = 0;
    private int objectGroupIndex = 0;
    private int objectIndex = 0;

    protected abstract void Activate(GameObject objectToActivate);
    protected abstract void Deactivate(GameObject objectToDeactivate);

    private void Start()
    {
        if ((player = GameObject.FindGameObjectWithTag(Tags.player)) == null)
        {
            Debug.LogError("Missing a player object.");
            enabled = false;
        }
        else
        {
            enabled = ValidateCullingGroups();
        }
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

            if (IsBeyondDrawDistance(player.transform.position, obj.position, cullingGroup))
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

    // we use the horizontal distance and the vertical difference between the y components to determine when to draw an object
    private bool IsBeyondDrawDistance(Vector3 playerPosition, Vector3 objectPosition, CullingGroup cullingGroup)
    {
        Vector3 playerHorizontalPosition = playerPosition;
        playerHorizontalPosition.y = 0;
        Vector3 objectHorizontalPosition = objectPosition;
        objectHorizontalPosition.y = 0;

        return cullingGroup.horizontalCulling && (Vector3.Distance(playerHorizontalPosition, objectHorizontalPosition) > cullingGroup.horizontalCullDistance)
            || cullingGroup.verticalUpwardsCulling && ((playerPosition.y - objectPosition.y) < -cullingGroup.verticalUpwardsCullDistance)
            || cullingGroup.verticalDownwardsCulling && ((playerPosition.y - objectPosition.y) > cullingGroup.verticalDownwardsCullDistance);
    }

    // doesn't allow empty culling groups or object groups so as to not waste time
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
