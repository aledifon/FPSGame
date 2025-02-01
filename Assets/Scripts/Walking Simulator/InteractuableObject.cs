using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Object;

public class InteractuableObject : Object
{
    [SerializeField] float impulse;

    // GO Components
    [SerializeField] PlayerActions playerActions;

    // Animate Type Objects vars.
    #region Animate_Objects_Vars
    private Vector3 initPos;
    private Vector3 targetPos;
    private float openingTime = 1f;

    private Quaternion[] initRotArray;
    private Quaternion[] targetRotArray;
    #endregion

    private void Start()
    {
        initPos = transform.position;
        targetPos = new Vector3(initPos.x, initPos.y, initPos.z + 0.3f);

        //1. Get the child Transform Components
        // Transform[] transformDoor;
        //2. Set initRot rot with initRot = new Quaternion [] {child[0].transform.rotation,
        //                                                  child[1].transform.rotation}        
        //3. Set targetRot rot with targetRot = new Quaternion [] {initRot[0] * Quaternion.Euler(0, 100f, 0f),
        //                                                         initRot[0] * Quaternion.Euler(0, -100f, 0f)}
    }
    private void Update()
    {
        
    }
    public void ActionOne()
    {
        switch (objectType)
        {
            case ObjectType.TakeDrop:
                Take();
                break;
            case ObjectType.Animate:
                if (objectSubType == ObjectSubType.AnimateDrawer)
                    StartCoroutine(nameof(OpenDrawer));
                else if (objectSubType == ObjectSubType.AnimateCabinet)
                    StartCoroutine(nameof(OpenDoors));
                break;
            case ObjectType.Read:
                break;
        }        
    }
    public void ActionTwo()
    {        
        switch (objectType)
        {
            case ObjectType.TakeDrop:
                Drop();
                break;
            case ObjectType.Animate:
                if (objectSubType == ObjectSubType.AnimateDrawer)
                    StartCoroutine(nameof(CloseDrawer));
                else if (objectSubType == ObjectSubType.AnimateCabinet)
                    StartCoroutine(nameof(CloseDoors));
                break;
            case ObjectType.Read:
                break;
        }
    }
    #region TakeDrop_Methods
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
    #endregion
    #region OpenClose_Methods
    IEnumerator OpenDrawer()
    {        
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initPos, targetPos, elapsedTime / openingTime);
            yield return null;
        }
        transform.position = targetPos;
    }
    IEnumerator OpenDoors()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            for(int i=0; i<=initRotArray.Length-1, i++)
                //transformDoor[i].rotation = Quaternion.Lerp(initRotArray[i], 
                //                                        targetRotArray[i], 
                //                                        elapsedTime / openingTime);            
            yield return null;
        }
        //transformDoor[0].rotation = targetRotArray[0];
        //transformDoor[1].rotation = targetRotArray[1];        
    }
    IEnumerator CloseDrawer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPos, initPos, elapsedTime / openingTime);
            yield return null;
        }
        transform.position = initPos;

        // Set the current object as unselected
        IsObjectSelected(false);
    }
    IEnumerator CloseDoors()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i <= initRotArray.Length - 1, i++)
                //transformDoor[i].rotation = Quaternion.Lerp(targetRotArray[i], 
                //                                        initRotArray[i], 
                //                                        elapsedTime / openingTime);            
                yield return null;
        }
        //transformDoor[0].rotation = initRotArray[0];
        //transformDoor[1].rotation = initRotArray[1];     

        // Set the current object as unselected
        IsObjectSelected(false);
    }
    #endregion
    #region Reading_Methods
    void StartReading()
    {
        // Oscurecer pantalla y sacar cuadro texto
        // Mostrar texto en pantalla

        // Hacer todo por Canvas?
    }
    void StopReading()
    {

    }
    #endregion
}
