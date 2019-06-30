using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CycleAndFade : MonoBehaviour
{
    Canvas parentCanvas;
    GameObject currentElement;

    [SerializeField]
    GameObject[] elements;

    MeshRenderer currentRend;

    [SerializeField]
    float fadeTime;

    [SerializeField]
    float transperentTime;

    // Start is called before the first frame update
    void Start()
    {
        parentCanvas = GetComponent<Canvas>();

        if (parentCanvas.worldCamera != Camera.main)
        {
            parentCanvas.worldCamera = Camera.main;
        }

        currentRend = currentElement.GetComponentInChildren<MeshRenderer>();
        currentElement = elements[0];

        StartCoroutine(CycleElements());
    }



    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator CycleElements()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            currentElement = elements[i];

            yield return new WaitForSeconds(transperentTime);
            yield return StartCoroutine(WaitForMouseClick());


            //Fade In 
            for (float alpha = 0; alpha < 1; alpha += Time.deltaTime / fadeTime)
            {
                Color c = currentRend.material.color;
                c.a = alpha;
                currentRend.material.color = c;
            }

            yield return StartCoroutine(WaitForMouseClick());

            //Fade Out 
            for (float alpha = 0; alpha < 1; alpha -= Time.deltaTime / fadeTime)
            {
                Color c = currentRend.material.color;
                c.a = alpha;
                currentRend.material.color = c;
            }

            yield return StartCoroutine(WaitForMouseClick());
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator WaitForMouseClick()
    {
        while (!Input.GetMouseButton(0))
            yield return null;
    }
}
