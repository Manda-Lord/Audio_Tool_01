// ***************************************************************
// Script copyright 2024 By Creepy Cat for the Showroom Vol 17
// This script change the meshe illumination color + some light
// ***************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEmissionColor : MonoBehaviour
{
    // List of emission colors
    public List<Color> colors = new List<Color>(); // Dynamic list of colors

    // Time in seconds between each color change
    public float timeBetweenColors = 2f;

    // Fade duration between colors
    public float fadeDuration = 1f;

    // Intensity of the auto-illumination (emission) specified in the inspector
    public float emissionIntensity = 1f;

    // List of MeshRenderers to apply the emission color change
    [Tooltip("Add the MeshRenderers here")]
    public List<MeshRenderer> targetMeshes;  // List of MeshRenderers, visible in the Inspector

    // List of lights to apply the color change
    [Tooltip("Add the lights here")]
    public List<Light> targetLights;  // List of Lights, visible in the Inspector

    // Checkbox to enable or disable light color modification
    [Tooltip("Check this to allow light color modification.")]
    public bool modifyLights = true;  // Option to modify light colors

    // Reference to the materials of the MeshRenderers
    private List<Material[]> materials = new List<Material[]>();
    private List<Renderer> renderers = new List<Renderer>();

    private int currentColorIndex = 0;
    private float timer;
    private float fadeTimer = 0f;
    private bool isFading = false;
    private Color startColor;
    private Color targetColor;

    void Start()
    {
        // Check if the color list is not empty
        if (colors.Count == 0)
        {
            Debug.LogWarning("The color list is empty. Please add colors in the Inspector.");
            return;
        }

        // Retrieve and store materials from all MeshRenderers in the list
        foreach (MeshRenderer meshRenderer in targetMeshes)
        {
            Material[] mats = meshRenderer.materials; // Retrieve all materials from the MeshRenderer

            // Check if emission is enabled for each material, otherwise enable it
            foreach (Material mat in mats)
            {
                if (!mat.IsKeywordEnabled("_EMISSION"))
                {
                    mat.EnableKeyword("_EMISSION");
                }
            }

            // Add the materials to the list
            materials.Add(mats);
            renderers.Add(meshRenderer);  // Store the Renderer here

            // Apply the first color with the specified intensity to each material
            foreach (Material mat in mats)
            {
                SetEmissionColor(meshRenderer, mat, colors[currentColorIndex], emissionIntensity);
            }
        }

        // Change light colors at startup only if modifyLights is true
        if (modifyLights)
        {
            foreach (Light light in targetLights)
            {
                SetLightColor(light, colors[currentColorIndex]); // Apply the initial color
            }
        }

        // Initialize the timer for the next color change
        timer = timeBetweenColors;
    }

    void Update()
    {
        // Countdown the timer
        timer -= Time.deltaTime;

        // If the timer reaches 0 and a fade is not in progress, start the fade
        if (timer <= 0 && !isFading)
        {
            StartFade();
        }

        // If a fade is in progress, continue interpolating between colors
        if (isFading)
        {
            PerformFade();
        }
    }

    // Start fading to the next color
    void StartFade()
    {
        // Reset fadeTimer for the new fade cycle
        fadeTimer = 0f;

        // Set the start and target colors for the fade
        startColor = colors[currentColorIndex];

        // Move to the next color
        currentColorIndex = (currentColorIndex + 1) % colors.Count; // Use the list size
        targetColor = colors[currentColorIndex];

        // Indicate that a fade is in progress
        isFading = true;

        // Reset the timer for the next color change after the fade
        timer = timeBetweenColors + fadeDuration;
    }

    // Perform the fade between colors
    void PerformFade()
    {
        // Increment the fade elapsed time
        fadeTimer += Time.deltaTime;

        // Interpolate between the start and target colors over the specified duration
        Color newColor = Color.Lerp(startColor, targetColor, fadeTimer / fadeDuration);

        // Apply the interpolated color with the specified intensity to all materials
        for (int i = 0; i < materials.Count; i++)
        {
            Material[] mats = materials[i];
            foreach (Material mat in mats)
            {
                SetEmissionColor(renderers[i], mat, newColor, emissionIntensity);
            }
        }

        // Apply the interpolated color to the lights only if modifyLights is true
        if (modifyLights)
        {
            foreach (Light light in targetLights)
            {
                SetLightColor(light, newColor);
            }
        }

        // If the fade time is complete, end the fade
        if (fadeTimer >= fadeDuration)
        {
            isFading = false;
        }
    }

    // Apply the emission color with the specified intensity to a material
    void SetEmissionColor(Renderer renderer, Material mat, Color color, float intensity)
    {
        // Multiply the color by the specified emission intensity
        Color emissionColor = color * intensity;

        // Apply the color with intensity to the material
        mat.SetColor("_EmissionColor", emissionColor);

        // Update global illumination for dynamic lighting
        DynamicGI.SetEmissive(renderer, emissionColor);
    }

    // Change the color of the light
    void SetLightColor(Light light, Color color)
    {
        light.color = color;  // Change the light's color
    }
}