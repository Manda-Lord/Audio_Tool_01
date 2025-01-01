using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public GameObject uiPanel; // Ref to the Canvas
    public Button playButton;
    public Button stopButton;
    public AudioSource noisemakerAudio;

    public void Initialize(AudioSource audioSource)
    {
        noisemakerAudio = audioSource;

        // Assign the Main Camera to the Canvas' Event Camera field
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && Camera.main != null) {
            canvas.worldCamera = Camera.main;
        }
    }

    private void Start()
    {

        playButton.onClick.AddListener(PlayAudio);
        stopButton.onClick.AddListener(StopAudio);
    }

    /* private void OnMouseDown()
    {
        uiPanel.SetActive(true); // Shows UI when Noisemaker is clicked
    } */

    private void PlayAudio()
    {
        uiPanel.SetActive(false); // Hide the UI when the Play button is clicked

        if (noisemakerAudio != null) {
            noisemakerAudio.loop = false;
            noisemakerAudio.Play();
        }
    }

    private void StopAudio()
    {
        uiPanel.SetActive(false);

        if (noisemakerAudio.isPlaying) {
            noisemakerAudio.Stop();
        }
    }
}
