// -----------------------------------------
// Code by Creepy Cat, Copyright (2024/2025)
// -----------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObjectFromCamera : MonoBehaviour
{
    public GameObject[] objectsToShoot; // Tableau de prefabs d'objets à tirer
    public Transform shootPoint;         // Point de tir (généralement la caméra)
    public float shootForce = 500f;     // Force de tir
    public float destroyAfterSeconds = 5f; // Temps avant la destruction
    private float fadeDuration = 1f;      // Durée du fondu avant destruction

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Tir lors du clic gauche
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Choisit un objet aléatoire à tirer
        GameObject objectToShoot = objectsToShoot[Random.Range(0, objectsToShoot.Length)];
        
        GameObject shotObject = Instantiate(objectToShoot, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = shotObject.GetComponent<Rigidbody>() ?? shotObject.AddComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * shootForce);
        
        // Démarre la coroutine pour gérer le fondu et le contrôle de la lumière
        StartCoroutine(FadeOutAndDestroy(shotObject));
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) 
        {
            Debug.LogWarning("Aucun Renderer trouvé pour l'objet : " + obj.name);
            yield break; // Si aucun Renderer, sort de la coroutine
        }

        // Trouver la lumière enfant
        Light lightToControl = obj.GetComponentInChildren<Light>();
        float originalIntensity = lightToControl ? lightToControl.intensity : 0;
        float targetIntensity = 0; // Intégrer à 0 pendant le fondu
        float elapsedTime = 0f;

        // Durée du fondu définie par destroyAfterSeconds
        fadeDuration = destroyAfterSeconds;

        // Pour chaque Renderer, on va gérer le fondu
        foreach (Renderer rend in renderers)
        {
            // Taille initiale
            Vector3 originalScale = obj.transform.localScale;
            Vector3 targetScale = Vector3.zero; // Réduit à zéro pendant le fondu

            elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                // Gérer la réduction de taille
                obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / fadeDuration);

                // Gérer l'intensité de la lumière
                if (lightToControl != null)
                {
                    lightToControl.intensity = Mathf.Lerp(originalIntensity, targetIntensity, elapsedTime / fadeDuration);
                }

                // Affiche la taille actuelle dans la console
                //Debug.Log("Réduction de taille en cours pour : " + obj.name + " - Taille actuelle : " + obj.transform.localScale);
                elapsedTime += Time.deltaTime;
                yield return null; // Attend le prochain frame
            }
        }

        // Assurez-vous que l'intensité de la lumière est réglée sur 0 à la fin
        if (lightToControl != null)
        {
            lightToControl.intensity = targetIntensity;
        }

        Destroy(obj); // Détruit l'objet après le fondu
    }
}