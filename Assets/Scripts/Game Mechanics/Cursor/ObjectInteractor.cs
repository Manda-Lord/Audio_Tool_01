using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                // Check ifthe object has an Interactable component
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null) {
                    interactable.OnClick(); // Trigger interaction
                }
            }
        }
    }
}