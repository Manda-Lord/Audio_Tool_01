using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log($"{gameObject.name} was clicked");
    }
}
