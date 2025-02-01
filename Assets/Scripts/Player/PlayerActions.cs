using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] float mouseSensitivity;    // It will control the rotation sensitivity of the Object
    [SerializeField] float zObject;             // We'll place the object a bit on the Z-Axis to be
                                                // placed in front of the camera.
    
    public Transform posObject;                 // The empty GO will be as child of the camera.

    public GameObject objectSelected;          // Ref. to the Selected GO

    private PlayerRaycast playerRaycast;        // Ref. to the Player Raycast
    private MouseLook mouseLook;                // Ref. to the 'MouseLook' script

    private Vector3 posObjectInit;              // Initial position
    private Vector3 posObjectToRotate;          // Pos. where I'll place the object to rotate it.
    private float angleY;
    private float angleX;

    private bool IsRotObjectEnabled;

    private void Awake()
    {
        playerRaycast = GetComponent<PlayerRaycast>();
        mouseLook = GetComponent<MouseLook>();
    }
    private void Start()
    {
        posObjectInit = posObject.localPosition;
        posObjectToRotate = new Vector3(posObject.localPosition.x,
                                        posObject.localPosition.y,
                                        posObject.localPosition.z+zObject);
    }
    private void Update()
    {
        ActionPlayer();
        rotateObject();
    }
    void ActionPlayer()
    {
        // I'm clicking and pointing (with the Raycast) to an Interactuable Object
        if (Input.GetMouseButtonDown(0) && playerRaycast.interactuableObject != null
            && objectSelected == null)
        {
            playerRaycast.interactuableObject.ActionOne();
            objectSelected = playerRaycast.interactuableObject.gameObject;
            playerRaycast.interactuableObject = null;
        }
        else if (Input.GetMouseButtonDown(0) && objectSelected != null && !IsRotObjectEnabled) 
        { 
            objectSelected.GetComponent<InteractuableObject>().ActionTwo();
            objectSelected = null;
        }
    }
    void rotateObject()
    {
        if (objectSelected != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Disable temporary the MouseLook script in order to disable the camera movement
                mouseLook.enabled = false;
                // Set the Object in the Rotation Position (to make it more visible)
                posObject.localPosition = posObjectToRotate;

                // Indicates the player is on Rotation Object Mode
                IsRotObjectEnabled = true;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                // Disable temporary the MouseLook script in order to disable the camera movement
                //mouseLook.enabled = false;

                // Get the Mouse Input Axis
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Calculate the Mouse deplacement along X&Y-Axis in func. of its Sensitivity
                angleY += (mouseX * mouseSensitivity);
                angleX += -(mouseY * mouseSensitivity);

                // Set the Object in the Rotation Position (to make it more visible)
                //posObject.localPosition = posObjectToRotate;

                // Update the Object selected Rotation
                objectSelected.transform.rotation = Quaternion.Euler(angleX, angleY, 0);

            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                // Replace again the object in its Init Position
                posObject.localPosition = posObjectInit;

                //Re-enable the 'MouseLook' script in order to enable the camera movement again
                mouseLook.enabled = true;

                // Disable the Rotation Object Mode
                IsRotObjectEnabled = false;
            }
        }
    }
}
