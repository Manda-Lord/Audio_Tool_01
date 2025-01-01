using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTiledTexture : MonoBehaviour {
    // Variables for the whole sheet
    public int colCount = 4;
    public int rowCount = 4;

    // Variables for animation
    public int rowNumber = 0; // Zero Indexed
    public int colNumber = 0; // Zero Indexed
    public int totalCells = 4;
    public int fps = 10;

    private Vector2 offset;
    private Vector2 size;
    private int index;
    private Renderer rend;
    private float timeCounter;

    void Start() {
        // Cache the Renderer component
        rend = GetComponent<Renderer>();

        // Calculate the size of each cell
        size = new Vector2(1.0f / colCount, 1.0f / rowCount);
    }

    void Update() {
        SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps);
    }

    void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps) {
        // Update time counter based on fps
        timeCounter += Time.deltaTime;
        index = (int)(timeCounter * fps) % totalCells;

        // Split into horizontal and vertical index
        int uIndex = index % colCount;
        int vIndex = index / colCount;

        // Build offset (v coordinate is the bottom of the image in OpenGL so we need to invert)
        offset = new Vector2((uIndex + colNumber) * size.x, (1.0f - size.y) - (vIndex + rowNumber) * size.y);

        // Update material properties
        rend.material.SetTextureOffset("_BaseMap", offset);
        rend.material.SetTextureScale("_BaseMap", size);
    }
}