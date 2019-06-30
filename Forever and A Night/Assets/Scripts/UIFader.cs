using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    public CanvasGroup uiElement;

    void FadeIn()
    {
        StartCoroutine(WaitForMouseClick());
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1));
        StartCoroutine(WaitForMouseClick());
    }

    void FadeOut()
    {
        StartCoroutine(WaitForMouseClick());
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0));
        StartCoroutine(WaitForMouseClick());
    }


    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentComplete);

            cg.alpha = currentValue;

            if (percentComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator WaitForMouseClick()
    {
        while (!Input.GetMouseButton(0))
            yield return null;
    }
}
