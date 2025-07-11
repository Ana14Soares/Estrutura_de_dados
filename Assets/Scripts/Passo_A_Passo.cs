using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Referência ao GameObject que contém o Canvas
    public GameObject canvasObject;

    // Método para alternar a visibilidade do Canvas
    public void ToggleCanvas()
    {
        if (canvasObject != null)
        {
            bool isActive = canvasObject.activeSelf;
            canvasObject.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("CanvasObject não está atribuído.");
        }
    }
}
