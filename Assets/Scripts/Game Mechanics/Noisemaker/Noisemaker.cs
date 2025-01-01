using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noisemaker : MonoBehaviour
{
    public GameObject uiPrefab;     // UI Prefab to spawn
    public AudioSource audioSource; // Unique audiosource for this Noisemaker
    
    private void OnMouseDown()
    {
        // Spawn UI
        GameObject newUI = Instantiate(uiPrefab, transform.position + new Vector3(1.05f, 0.0f, 1.0f), Quaternion.identity);
        newUI.SetActive(true);

        // Set Noisemaker as parent to keep UI posiitoned realtive to it
        newUI.transform.SetParent(transform);

        // Get the UI_Controller component form the spawned UI and assign Noisemaker's audiosource
        UI_Controller uiController = newUI.GetComponent<UI_Controller>();

        if (uiController != null) {
            uiController.Initialize(audioSource);
        }
    }
}