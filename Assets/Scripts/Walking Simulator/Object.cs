using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Object : MonoBehaviour
{
    public enum ObjectType {TakeDrop, Read, Animate}
    public ObjectType objectType;

    public enum ObjectSubType { None, AnimateDrawer, AnimateDoors, ClosedDoor}
    public ObjectSubType objectSubType;

    [SerializeField] Material mat;      //Original Object material
    [SerializeField] Material newMat;   //New Object material when the object is selected
    [SerializeField] string textToShow;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] List<Renderer> rendList;
    
    public void IsObjectSelected(bool selected)
    {
        if (selected)
        {   
            foreach(Renderer rend in rendList)
                rend.material = newMat;
            textUI.text = textToShow;
        }
        else
        {
            foreach (Renderer rend in rendList)
                rend.material = mat;
            textUI.text = "";
        }
    }
}
