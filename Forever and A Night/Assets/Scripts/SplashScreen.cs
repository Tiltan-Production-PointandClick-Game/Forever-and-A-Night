using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    CanvasGroup currentUiElement;
    Canvas parentCanvas;

    [SerializeField]
    CanvasGroup[] cgElements; // Pictures we want to cycle through

    [SerializeField]
    float fadeTime; // amount of time it takes to fade an image


    void Start()
    {
        parentCanvas = GetComponent<Canvas>();

        if (parentCanvas.worldCamera != Camera.main)
            parentCanvas.worldCamera = Camera.main;

        currentUiElement = GetComponentInChildren<CanvasGroup>();

        currentUiElement = cgElements[0];

        for (int i = 0; i < cgElements.Length; i++)
        {
            currentUiElement.alpha = 0;
        }

        WaitForMouseClick();
        StartCoroutine(CycleImages());

    }

    void Update()
    {

    }

    IEnumerator CycleImages()
    {

        for (int i = 0; i < cgElements.Length; i++)
        {
            currentUiElement = cgElements[i];

            WaitForMouseClick();

            //Fade in for loop
            for (float a = 0; a < 1; a += Time.deltaTime / fadeTime)
            {
                currentUiElement.alpha = a;

                yield return null; // Wait for frame then return to execution
            }

            yield return StartCoroutine(WaitForMouseClick());

            //Fade out for loop
            for (float a = 1; a > 0; a -= Time.deltaTime / fadeTime)
            {
                currentUiElement.alpha = a;
                yield return null; // Wait for frame then return to execution
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    IEnumerator WaitForMouseClick()
    {
        while (!Input.GetMouseButton(0))
            yield return null;
    }
}
