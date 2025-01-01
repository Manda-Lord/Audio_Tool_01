using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            audioSource.Play(); // Play audio when 'P' is pressed
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            audioSource.Play(); // Stop audio when 'O' is pressed
        }
    }
}
