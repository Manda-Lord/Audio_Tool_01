using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReflection : MonoBehaviour
{
    public Transform listener;          // Assign in the Inspector
    public LayerMask reflectionLayer;   // Assign layers in Inspector
    public int maxReflections    = 6;   // Maximum reflections per ray
    public int maxSuccessfulRays = 6;   // Maximum rays that need to reach Robot Kyle
    public int spreadRays        = 30;  // Number of initial spread rays
    public float spreadAngle     = 15f; // Spread angle in degrees

    // Variables for sound effects (adjust as needed)
    public float baseReverb = 0.5f;
    public float minReverb  = 0.0f;
    public float maxReverb  = 1.0f;
    public float reflectionReverbIncrement = 0.2f;
    public AudioSource audioSource;

    private int successfulRayCount = 0; // Counter for successful rays reaching Robot Kyle

    void Update()
    {
        successfulRayCount = 0; // Reset successful hits count each frame
        // Cast spherical spread of rays around the main direction
        CastSphericalSpreadRays();
    }

    void CastSphericalSpreadRays()
    {
        Vector3 mainDirection = (listener.position - transform.position).normalized;

        for (int i = 0; i < spreadRays; i++)
        {
            if (successfulRayCount >= maxSuccessfulRays) {
                // Debug.Log("Max successful rays reached. Stopping further raycasts.");
                break;
            }

            Vector3 randomDirection = GetRandomDirectionInSphere(mainDirection, spreadAngle);
            CastReflectiveRay(randomDirection);
        }
    }

    Vector3 GetRandomDirectionInSphere(Vector3 mainDirection, float angle)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        float theta = Random.Range(0f, 2 * Mathf.PI);
        float phi   = Random.Range(0f, angleInRadians);
        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = Mathf.Cos(phi);

        Vector3 localDirection = new Vector3(x, y, z);
        Quaternion rotationToMainDirection = Quaternion.FromToRotation(Vector3.forward, mainDirection);
        return rotationToMainDirection * localDirection;
    }

    void CastReflectiveRay(Vector3 direction)
{
    float remainingDistance  = 100f;
    float proximityThreshold = 0.5f;

    int  reflections     = 0;
    bool listenerReached = false;

    Vector3 currentPosition  = transform.position;
    Vector3 currentDirection = direction;

    while (reflections < maxReflections && !listenerReached)
    {
        if (Physics.Raycast(currentPosition, currentDirection, out RaycastHit hit, remainingDistance, reflectionLayer))
        {
            // Check if the hit object is Robot Kyle by layer or tag
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Listener") || hit.transform.CompareTag("Player"))
            {
                // Debug.Log("Ray hit Robot Kyle!");

                listenerReached = true;
                successfulRayCount++;

                if (successfulRayCount <= maxSuccessfulRays) {
                    ApplySoundEffects(reflections);
                }

                Debug.DrawRay(currentPosition, currentDirection * hit.distance, Color.green, 1.0f);
                break;
            }

            // Check if the ray is within proximity of the listener
            if (hit.transform == listener || Vector3.Distance(hit.point, listener.position) <= proximityThreshold)
            {
                // Debug.Log("Ray reached the listener or is within proximity threshold!");

                listenerReached = true;
                successfulRayCount++;

                if (successfulRayCount <= maxSuccessfulRays) {
                    ApplySoundEffects(reflections);
                }

                Debug.DrawRay(currentPosition, currentDirection * hit.distance, Color.green, 1.0f);
                break;
            }

            // Draw the reflection path and log what was hit
            // Debug.Log($"Hit object: {hit.collider.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            Debug.DrawRay(currentPosition, currentDirection * hit.distance, Color.blue, 1.0f);

            // Reflect off the surface and update position for the next reflection
            currentDirection = Vector3.Reflect(currentDirection, hit.normal);
            currentPosition = hit.point;
            reflections++;
        }
        else
        {
            // Debug.Log("No further hits detected, stopping reflection.");
            break;
        }
    }

    if (!listenerReached && successfulRayCount < maxSuccessfulRays)
    {
        // Debug.Log("Listener not reached by this ray. Resetting reverb.");
        audioSource.reverbZoneMix = baseReverb;
    }
}


    void ApplySoundEffects(int reflectionCount)
    {
        // Debug.Log("ApplySoundEffects called");

        float reverbIntensity = baseReverb + (reflectionReverbIncrement * reflectionCount);
        audioSource.reverbZoneMix = Mathf.Clamp(reverbIntensity, minReverb, maxReverb);

        // Debug.Log($"Applying reverb: {reverbIntensity} for reflection count: {reflectionCount}");
    }
}