using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperRenderer : MonoBehaviour
{
    public Texture2D newTexture; // Imagen que asignarás en tiempo de ejecución
    private Renderer quadRenderer;

    void Start()
    {
        quadRenderer = GetComponent<Renderer>();
        //quadRenderer.enabled = false; // Inicialmente oculto

        ShowImage();
    }

    public void ShowImage()
    {
        if (newTexture != null)
        {
            quadRenderer.material.mainTexture = newTexture;
            quadRenderer.enabled = true; // Activar la superficie
        }
    }

    public void HideImage()
    {
        quadRenderer.enabled = false; // Desactivar la superficie
    }
}
