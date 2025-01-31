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
    private float openingTime = 2f;
    #endregion

    private void Start()
    {
        initPos = transform.position;
        targetPos = new Vector3(initPos.x,initPos.y,initPos.z+0.3f);
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
                StartCoroutine(nameof(Open));
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
                Close();
                break;
            case ObjectType.Read:
                break;
        }
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

    IEnumerator Open()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initPos, targetPos, elapsedTime / openingTime);
            yield return null;
        }
    }
    IEnumerator Close()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openingTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initPos, targetPos, elapsedTime / openingTime);
            yield return null;
        }
    }
    void StartReading()
    {
        // Oscurecer pantalla y sacar cuadro texto
        // Mostrar texto en pantalla

        // Hacer todo por Canvas?
    }
    void StopReading()
    {

    }
}
