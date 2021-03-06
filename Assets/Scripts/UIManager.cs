﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public StaminaBar staminaBar;

    [Header("Fading Settings")]
    public Image fader;
    public float fadeSpeed = 2f;

    public delegate void FadingComplete();
    public event FadingComplete OnFadingComplete;

    private void Start()
    {
        fader.gameObject.SetActive(true);
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
}
