using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Object : MonoBehaviour
{
    public enum ObjectType {TakeDrop, Read, Animate}
    public ObjectType objectType;

    [SerializeField] Material mat;      //Original Object material
    [SerializeField] Material newMat;   //New Object material when the object is selected
    [SerializeField] string textToShow;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] Renderer rend;
    
    public void IsObjectSelected(bool selected)
    {
        if (selected)
        {
            rend.material = newMat;
            textUI.text = textToShow;
        }
        else
        {
            rend.material = mat;
            textUI.text = "";
        }
    }
}
