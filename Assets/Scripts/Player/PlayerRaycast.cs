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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectLayer))     
        {            
            interactuableObject = hit.collider.GetComponent<InteractuableObject>();
            if (interactuableObject != null)
            {
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
