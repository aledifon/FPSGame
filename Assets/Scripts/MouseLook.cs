using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Camera_Rotation_Vars
    [Header ("Mouse camera Settings")]
    [SerializeField] float mouseSensitivity;
    [SerializeField] float bottomAngle;         // Limit the top and bottom camera turn angles around the X-Axis
    [SerializeField] float topAngle;
    [SerializeField] float yRotationSpeed;
    [SerializeField] float xCameraSpeed;

    private float desiredYRotation;
    private float desiredCameraXRotation;
    private float currentYRotation;
    private float currentCameraXRotation;
    private float rotationYVelocity;
    private float cameraXVelocity;

    private Camera myCamera;
    private float mouseX;
    private float mouseY;
    #endregion

    void Awake()
    {
        myCamera = Camera.main;

        // Lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }    
    void Update()
    {
        MouseInputMovement();
    }
    private void FixedUpdate()
    {
        ApplyRotation();
    }

    void MouseInputMovement()
    {
        mouseX = Input.GetAxis("Mouse X");
        //Debug.Log("Mouse X: " + mouseX);
        mouseY = Input.GetAxis("Mouse Y");
        //Debug.Log("Mouse Y: " + mouseY);

        // Desired turn for the player around the Y-axis
        desiredYRotation = desiredYRotation + (mouseX * mouseSensitivity);
        // Desired turn for the camera around the X-axis
        desiredCameraXRotation = desiredCameraXRotation - (mouseY *mouseSensitivity);
        // Limit the Camera angle rotation around the X-axis (between top and bottom angles)
        desiredCameraXRotation = Mathf.Clamp(desiredCameraXRotation,bottomAngle,topAngle);
    }

    void ApplyRotation()
    {
        // Calculate the player rotation to apply around the Y-axis

        currentYRotation = Mathf.SmoothDamp(currentYRotation, 
                                            desiredYRotation, 
                                            ref rotationYVelocity, 
                                            yRotationSpeed);
        // Calculate the camera rotation to apply around the X-axis
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, 
                                                desiredCameraXRotation,
                                                ref cameraXVelocity,
                                                xCameraSpeed);

        // APPLY THE ROTATIONS ARROUND THE GOs
        // Rotation of the Player around the Y-Axis
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        // Rotation of the Camera around the X-Axis
        myCamera.transform.localRotation  = Quaternion.Euler(currentCameraXRotation,0,0);
        
    }
}
