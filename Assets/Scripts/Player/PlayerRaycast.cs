using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public InteractuableObject interactuableObject;

    [SerializeField] LayerMask objectLayer;    

    Ray ray;
    RaycastHit hit;
    Transform myCamera;

    // GO Components
    private PlayerActions playerActions;

    private void Start()
    {
        myCamera = Camera.main.transform;

        playerActions = GetComponent<PlayerActions>();
    }
    private void Update()
    {
        ray.origin = myCamera.position;
        ray.direction = myCamera.forward;

        if (Physics.Raycast(ray, out hit, 5f, objectLayer))     
        {
            Debug.Log("Detected object");
            interactuableObject = hit.collider.GetComponent<InteractuableObject>();
            // In case if it's null we'll check also if its parent has the script.
            if (interactuableObject == null )
                interactuableObject = hit.collider.GetComponentInParent<InteractuableObject>();
            if (interactuableObject != null)
            {
                // Only if we are reading a Read Object the Object selection won't be performed
                if (interactuableObject.objectType == InteractuableObject.ObjectType.Read &&
                    interactuableObject.IsReading)
                    interactuableObject.IsObjectSelected(false);
                else
                    interactuableObject.IsObjectSelected(true);
                Debug.Log("Selected object");
            }            
        }
        else
        {
            // Assure to deselect the object the immediate frame after being not on the player view
            if(interactuableObject !=null)
            {
                interactuableObject.IsObjectSelected(false);
                interactuableObject = null;
                Debug.Log("Not selected object");
            }
        }
    }
}
