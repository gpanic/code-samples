using UnityEngine;
using System.Collections;

// partial section of the whole MovementController script
public class MovementController : MonoBehaviour {

    public float crouchHeight = 1.0f; // height of controller when crouched

    private CharacterController characterController;
    private float controllerHeight;
    private float heightDifference;
    private float controllerSkinWidth = 0.08f;
    private bool crouchedWhileAirborne = false;
    private bool crouching = false;
    private float crouchCameraChange = 0;
    private bool couldNotUncrouch = false;

    // triggers are the same shape and size as the character controller, positioned so they can detect objects
    // above and below the player object
    private CollisionDetector crouchCollisionTop; // detects weather any object is blocking the uncrouch
    private CollisionDetector crouchCollisionBottom; // used to determine distance to floor

    private Vector3 currentPos;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        crouchCollisionTop = GameObject.Find("collision_detector_top").GetComponent<CollisionDetector>();
        crouchCollisionBottom = GameObject.Find("collision_detector_bottom").GetComponent<CollisionDetector>();
        controllerHeight = characterController.height;
        heightDifference = (controllerHeight - crouchHeight);
    }

    private void Crouch()
    {
        crouching = true;
        characterController.height = crouchHeight;

        if (characterController.isGrounded)
        {
            // squat by moving the controller down by half the height difference so he's at the floor
            characterController.center = new Vector3(0, -heightDifference / 2, 0);
        }
        else
        {
            // crouch in air by pulling legs up - move controller up by half of the height difference
            crouchedWhileAirborne = true;
            characterController.center = new Vector3(0, heightDifference / 2, 0);

            // since the controller is now positioned higher, move the top collision trigger up as well
            crouchCollisionTop.gameObject.transform.localPosition += new Vector3(0, heightDifference, 0);
        }
    }

    private void Uncrouch()
    {
        // remember that it wasn't possible to uncrouch so that it can be done later
        couldNotUncrouch = crouchCollisionTop.IsColliding;
        if (crouchCollisionTop.IsColliding) return;

        if (crouchedWhileAirborne)
        {
            // move the top collision trigger back
            crouchCollisionTop.gameObject.transform.localPosition += new Vector3(0, -heightDifference, 0);
            if (!characterController.isGrounded)
            {
                if (crouchCollisionBottom.IsColliding)
                {
                    // the player crouched in air, then uncrouched before fully landing, calculate remaining distance to the floor
                    // which is used to move the whole player object back up
                    float distanceToFloor = -(crouchCollisionBottom.CollidingObject.ClosestPointOnBounds(transform.position) - transform.position).y;
                    float change = heightDifference + controllerSkinWidth - distanceToFloor;
                    transform.Translate(Vector3.up * change);
                    currentPos = transform.position;

                    // move the camera back to the position before the uncrouch so it can animate smoothly
                    crouchCameraChange = -change;
                }
            }
            else
            {
                // since the controller was moved up and will be moved back down move the whole player object up by the same amount
                // (move player up to standing position then extend legs back to the ground)
                transform.Translate(Vector3.up * heightDifference);
                currentPos = transform.position;
                crouchCameraChange = -heightDifference;
            }
            crouchedWhileAirborne = false;
        }

        // uncrouch
        crouching = false;
        characterController.center = Vector3.zero;
        characterController.height = controllerHeight;
    }

}
