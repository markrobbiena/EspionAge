using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public StaminaBar staminaBar;
    private Image staminaBarImage;

    [Header("Fading Settings")]
    public Image fader;
    public float fadeSpeed = 2f;

    public delegate void FadingComplete();
    public event FadingComplete OnFadingComplete;

    

    private void Start()
    {
        fader.gameObject.SetActive(true);
        staminaBarImage = staminaBar.GetComponent<Image>();
    }

    private Color GetFaderColorWithAlpha(float alpha)
    {
        return new Color(fader.color.r, fader.color.g, fader.color.b, alpha);
    }

    public void FadeIn()
    {
        // full black --> invisible
        StartCoroutine(FadeCoroutine(1f, 0f));
    }

    public void FadeOut()
    {
        // invisible --> full black
        StartCoroutine(FadeCoroutine(0f, 1f));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        float currentAlpha = startAlpha;
        while (Mathf.Abs(currentAlpha - startAlpha) < Mathf.Abs(startAlpha - endAlpha))
        {
            fader.color = GetFaderColorWithAlpha(currentAlpha);
            currentAlpha += fadeSpeed * Time.deltaTime * Mathf.Sign(endAlpha - startAlpha);
            yield return null;
        }

        OnFadingComplete?.Invoke();

        yield return null;
    }

    public void InstantFadeIn()
    {
        fader.color = GetFaderColorWithAlpha(0f);
    }

    public void InstantFadeOut()
    {
        fader.color = GetFaderColorWithAlpha(1f);
    }

    //duplicate code to fade stamina bar


    private Color SBGetFaderColorWithAlpha(float alpha)
    {
        return new Color(staminaBarImage.color.r, staminaBarImage.color.g, staminaBarImage.color.b, alpha);
    }

    public void SBFadeIn()
    {
        // full black --> invisible
        StartCoroutine(SBFadeCoroutine(1f, 0f));
    }

    public void SBFadeOut()
    {
        // invisible --> full black
        StartCoroutine(SBFadeCoroutine(0f, 1f));
    }

    private IEnumerator SBFadeCoroutine(float startAlpha, float endAlpha)
    {
        float currentAlpha = startAlpha;
        while (Mathf.Abs(currentAlpha - startAlpha) < Mathf.Abs(startAlpha - endAlpha))
        {
            staminaBarImage.color = SBGetFaderColorWithAlpha(currentAlpha);
            currentAlpha += fadeSpeed * Time.deltaTime * Mathf.Sign(endAlpha - startAlpha);
            yield return null;
        }

        OnFadingComplete?.Invoke();

        yield return null;
    }

    public void SBInstantFadeIn()
    {
        staminaBarImage.color = SBGetFaderColorWithAlpha(0f);
    }

    public void SBInstantFadeOut()
    {
        staminaBarImage.color = SBGetFaderColorWithAlpha(1f);
    }
}
