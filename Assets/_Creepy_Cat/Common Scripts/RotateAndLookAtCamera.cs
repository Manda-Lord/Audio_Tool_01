using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndLookAtCamera : MonoBehaviour
{
    public List<Transform> objectsToRotate; // Liste des objets à faire tourner
    public Transform cameraTransform;       // La caméra à pointer
    public float rotationSpeed = 5f;        // Vitesse de rotation sur l'axe Y

   // public bool rotateOnX = false;
   // public bool rotateOnY = true;
   // public bool rotateOnZ = false;


    void Start()
    {
        // Si aucune caméra n'est assignée, on utilise la caméra principale
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        foreach (Transform obj in objectsToRotate)
        {
            if (obj == null) continue; // S'assurer que l'objet est valide

            // Calcul de la direction vers la caméra
            Vector3 directionToCamera = cameraTransform.position - obj.position;

            directionToCamera.y = 0;

        //   if (!rotateOnX) directionToCamera.x = 0; // On ignore l'axe x pour garder l'alignement horizontal
        //   if (!rotateOnY) directionToCamera.y = 0; // On ignore l'axe Y pour garder l'alignement horizontal

            // Rotation actuelle vers la caméra
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

            // Rotation lissée sur l'axe Y
            obj.rotation = Quaternion.Slerp(obj.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}