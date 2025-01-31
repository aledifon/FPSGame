using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractuableObject : Object
{
    [SerializeField] float impulse;

    // GO Components
    [SerializeField] PlayerActions playerActions;

    public void ActionOne()
    {
        Take();
    }
    public void ActionTwo()
    {
        Drop();
    }
    void Take()
    {
        // Set the current object as unselected
        IsObjectSelected(false);

        // Set rb as kinematic to avoid to be affected by gravity
        rb.isKinematic = true;

        // Set the current GO as child of the 'PosObject' GO which is a child of the 'Main Camera' GO
        //transform.SetParent(Camera.main.transform.GetChild(1).transform);
        //Transform posObjectTransform = Camera.main.transform.Find("PosObject");
        Transform posObjectTransform = playerActions.posObject;

        if (posObjectTransform != null)                    
            transform.SetParent(posObjectTransform);                    
        else
            Debug.Log("It was not found the 'PosObject' as a child of Camera.main");

        // Set the current Object Pos & Rotations respect to his father position ('PosObject')
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 107f, 0f);        
    }
    void Drop()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(transform.forward*impulse);
    }
}
