// -----------------------------------------
// Code by Creepy Cat, Copyright (2024/2025)
// -----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObjectFromCamera : MonoBehaviour
{
    public GameObject[] objectsToShoot; // Tableau de prefabs d'objets � tirer
    public Transform shootPoint;         // Point de tir (g�n�ralement la cam�ra)
    public float shootForce = 500f;     // Force de tir
    public float destroyAfterSeconds = 5f; // Temps avant la destruction
    private float fadeDuration = 1f;      // Dur�e du fondu avant destruction

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Tir lors du clic gauche
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Choisit un objet al�atoire � tirer
        GameObject objectToShoot = objectsToShoot[Random.Range(0, objectsToShoot.Length)];
        
        GameObject shotObject = Instantiate(objectToShoot, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = shotObject.GetComponent<Rigidbody>() ?? shotObject.AddComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * shootForce);
        
        // D�marre la coroutine pour g�rer le fondu et le contr�le de la lumi�re
        StartCoroutine(FadeOutAndDestroy(shotObject));
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) 
        {
            Debug.LogWarning("Aucun Renderer trouv� pour l'objet : " + obj.name);
            yield break; // Si aucun Renderer, sort de la coroutine
        }

        // Trouver la lumi�re enfant
        Light lightToControl = obj.GetComponentInChildren<Light>();
        float originalIntensity = lightToControl ? lightToControl.intensity : 0;
        float targetIntensity = 0; // Int�grer � 0 pendant le fondu
        float elapsedTime = 0f;

        // Dur�e du fondu d�finie par destroyAfterSeconds
        fadeDuration = destroyAfterSeconds;

        // Pour chaque Renderer, on va g�rer le fondu
        foreach (Renderer rend in renderers)
        {
            // Taille initiale
            Vector3 originalScale = obj.transform.localScale;
            Vector3 targetScale = Vector3.zero; // R�duit � z�ro pendant le fondu

            elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                // G�rer la r�duction de taille
                obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / fadeDuration);

                // G�rer l'intensit� de la lumi�re
                if (lightToControl != null)
                {
                    lightToControl.intensity = Mathf.Lerp(originalIntensity, targetIntensity, elapsedTime / fadeDuration);
                }

                // Affiche la taille actuelle dans la console
                //Debug.Log("R�duction de taille en cours pour : " + obj.name + " - Taille actuelle : " + obj.transform.localScale);
                elapsedTime += Time.deltaTime;
                yield return null; // Attend le prochain frame
            }
        }

        // Assurez-vous que l'intensit� de la lumi�re est r�gl�e sur 0 � la fin
        if (lightToControl != null)
        {
            lightToControl.intensity = targetIntensity;
        }

        Destroy(obj); // D�truit l'objet apr�s le fondu
    }
}