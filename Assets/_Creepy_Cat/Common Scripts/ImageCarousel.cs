using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageCarousel: MonoBehaviour
{
    public List<Texture> imageList; // La liste des textures pour le carrousel
    public Renderer quadRenderer; // Le Renderer du quad sur lequel afficher les textures
    public float fadeDuration = 1f; // Dur�e du fondu encha�n�
    public float displayTime = 2f; // Temps d'affichage de chaque image
    public Color emissionColor = Color.white; // Couleur de l'auto-illumination fixe
    public float emissionPower = 1f; // Puissance de l'illumination

    private int currentImageIndex = 0;
    private Material quadMaterial;
    private Material secondQuadMaterial;

    private void Start()
    {
        if (imageList.Count > 0 && quadRenderer != null)
        {
            // Duplique le mat�riau pour g�rer le fondu entre deux images
            quadMaterial = quadRenderer.material;
            secondQuadMaterial = new Material(quadMaterial);

            // Active l'auto-illumination dans le shader du mat�riau (URP)
            quadMaterial.EnableKeyword("_EMISSION");
            secondQuadMaterial.EnableKeyword("_EMISSION");

            // D�finit l'image initiale avec alpha � 1
            quadMaterial.mainTexture = imageList[currentImageIndex];
            quadMaterial.color = new Color(quadMaterial.color.r, quadMaterial.color.g, quadMaterial.color.b, 1f);
            quadRenderer.material = quadMaterial;

            StartCoroutine(StartCarousel());
        }
        else
        {
            Debug.LogError("Assurez-vous d'avoir ajout� des images et assign� un Renderer.");
        }
    }

    private IEnumerator StartCarousel()
    {
        while (true)
        {
            int nextImageIndex = (currentImageIndex + 1) % imageList.Count;
            Texture nextTexture = imageList[nextImageIndex];

            // Applique la texture de la prochaine image au second mat�riau
            secondQuadMaterial.mainTexture = nextTexture;
            secondQuadMaterial.color = new Color(secondQuadMaterial.color.r, secondQuadMaterial.color.g, secondQuadMaterial.color.b, 0f);

            // Applique la texture d'�mission avec la puissance sp�cifi�e (URP)
            quadMaterial.SetTexture("_EmissionMap", imageList[currentImageIndex]);
            quadMaterial.SetColor("_EmissionColor", emissionColor * emissionPower);
            secondQuadMaterial.SetTexture("_EmissionMap", nextTexture);
            secondQuadMaterial.SetColor("_EmissionColor", emissionColor * emissionPower);

            // Applique les deux mat�riaux au Renderer
            quadRenderer.materials = new Material[] { quadMaterial, secondQuadMaterial };

            // D�marre la transition de fondu encha�n� (crossfade)
            yield return StartCoroutine(CrossfadeImages());

            // Passe � l'image suivante
            currentImageIndex = nextImageIndex;

            // Attend avant de recommencer
            yield return new WaitForSeconds(displayTime);
        }
    }

    private IEnumerator CrossfadeImages()
    {
        float elapsedTime = 0f;

        // On garde la puissance de l'auto-illumination constante
        Color originalEmissionColor = emissionColor * emissionPower;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Diminue l'alpha de l'image actuelle (de 1 � 0)
            quadMaterial.color = new Color(quadMaterial.color.r, quadMaterial.color.g, quadMaterial.color.b, 1 - alpha);

            // Augmente l'alpha de l'image suivante (de 0 � 1)
            secondQuadMaterial.color = new Color(secondQuadMaterial.color.r, secondQuadMaterial.color.g, secondQuadMaterial.color.b, alpha);

            // Garder la couleur d'�mission constante avec la puissance d�finie
            quadMaterial.SetColor("_EmissionColor", originalEmissionColor);
            secondQuadMaterial.SetColor("_EmissionColor", originalEmissionColor);

            // Applique les deux mat�riaux simultan�ment
            quadRenderer.materials = new Material[] { quadMaterial, secondQuadMaterial };

            yield return null;
        }

        // Fin de la transition, on garde uniquement le nouveau mat�riau
        quadRenderer.material = secondQuadMaterial;
    }
}