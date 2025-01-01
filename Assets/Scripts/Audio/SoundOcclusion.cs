using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundOcclusion : MonoBehaviour
{
    public Transform listener;
    public AudioLowPassFilter lowPassFilter;
    public AudioSource audioSource; // Reference to the AudioSource on the Noisemaker
    public float maxCutoffFrequency = 22000f; // Maximum frequency (no occlusion)
    public float minCutoffFrequency = 500f; // Minimim frequency (full occlusion)
    public float maxVolume = 1f;
    public float minVolume = 0.5f;
    public float transitionSpeed = 2f; // Speed of the transition between the occluded and unoccluded sound
    public LayerMask occlusionLayer; // Objects that should block sound

    private float _targetCutoffFrequency; // Desired cutoff frequency based on occlusion
    private float _targetVolume; // Desired volume based on occlusion
    private int wallCount;

    void Start()
    {
        // Start withj the max cutoff frequency to ensure no Low Pass active initially
        _targetCutoffFrequency = maxCutoffFrequency;
        lowPassFilter.cutoffFrequency = maxCutoffFrequency;
        _targetVolume = maxVolume;
        audioSource.volume = maxVolume;
    }

    void Update()
    {
        wallCount = 0;

        // Calculate direction to the listener
        Vector3 direction = listener.position - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, occlusionLayer);

        foreach (RaycastHit wallHit in hits)
        {
            if (wallHit.collider.gameObject.layer == LayerMask.NameToLayer("Occluder")) {
                wallCount++;
            }
        }

        Debug.Log($"Wall Count: {wallCount}");

        if (wallCount >= 4)
        {
            _targetCutoffFrequency = minCutoffFrequency;
            _targetVolume = 0f;
            Debug.Log($"Wall Count >= 4. Volume set to 0. Current Volume: {audioSource.volume}");
        }
        else if (wallCount > 0)
        {
            float occlusionFactor = wallCount / 3f; // Scale from 0 to 1
            _targetCutoffFrequency = Mathf.Lerp(maxCutoffFrequency, minCutoffFrequency, occlusionFactor);
            _targetVolume = Mathf.Lerp(maxVolume, 0, occlusionFactor);
        }
        else
        {
            // No walls between, so reset to max values
            _targetCutoffFrequency = maxCutoffFrequency;
            _targetVolume = maxVolume;
            Debug.Log("No occlusion. Resetting to max values.");
        }

        // Smoothly transition the current cutoff frequency and volume to the target values
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, _targetCutoffFrequency, Time.deltaTime * transitionSpeed);
        audioSource.volume = Mathf.Lerp(audioSource.volume, _targetVolume, Time.deltaTime * transitionSpeed);

        // Ensures volume is exactly 0 when fully occluded
        if (wallCount < 5)
        {
            // Raycast to check for obstacles between the Noisemaker and the listener
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, occlusionLayer))
            {
                float listenerDistanceFromCenter = Vector3.Distance(listener.position, hit.point);
                float wallWidth = hit.collider.bounds.size.x;

                // Calculate how far the listener is from the center line of the wall
                float proximityFactor = Mathf.Clamp01(listenerDistanceFromCenter / (wallWidth / 2));

                // Set target values based on proximity
                _targetCutoffFrequency = Mathf.Lerp(minCutoffFrequency, maxCutoffFrequency, proximityFactor);
                _targetVolume = Mathf.Lerp(minVolume, maxVolume, proximityFactor);
            }
            else
            {
                // No obstacle detected, reset to max values
                _targetCutoffFrequency = maxCutoffFrequency;
                _targetVolume = maxVolume;
            }
        }

        Debug.Log($"Wall Count: {wallCount} | Target Volume: {_targetVolume} | Target Cutoff: {_targetCutoffFrequency}");
    }
}