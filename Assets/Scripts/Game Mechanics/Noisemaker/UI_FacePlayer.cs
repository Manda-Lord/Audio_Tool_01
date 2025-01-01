using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        // Make the Canvas fully face the player's camera
        transform.LookAt(playerCamera);
        
        // Adjust rotation to face directly toward the camera
        transform.Rotate(0, 180, 0); // Flips the Canvas if it’s backwards
    }
}