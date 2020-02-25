using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaFillImage;
    public Image greyOutlineImage;
    private Image staminaBarImage;
    public float changePerSecond = 0.1f;

    public delegate void FadingComplete();
    public event FadingComplete OnFadingComplete;
    public float fadeSpeed = 2f;

    // Events for others to subscribe to OnChange events
    public delegate void ChangedAction(float fillAmount);
    public event ChangedAction OnChange;

    [HideInInspector] 
    public const float STAMINA_MAX = 1f;

    void Start()
    {
        staminaBarImage = GetComponent<Image>();
        print(staminaBarImage);
    }

    private void Awake() 
    {
        staminaFillImage.fillAmount = 0.0f;
    }

    // No custom decrease function needed so it was no implemented (like below)
    public IEnumerator DecreaseStaminaBy(float percent)
    {
        float percentClamped = Mathf.Clamp(percent, 0f, STAMINA_MAX);
        float decreaseBy = percentClamped * STAMINA_MAX;

        float goalFillAmount = Mathf.Max(0f, staminaFillImage.fillAmount - decreaseBy);
        float toDecrease = staminaFillImage.fillAmount - goalFillAmount;

        return ChangeStaminaGeneral(toDecrease, -changePerSecond);
    }

    // Default IncreaseStaminaBy function, using the public changePerSecond field
    public IEnumerator IncreaseStaminaBy(float percent)
    {
        return IncreaseStaminaBy(percent, changePerSecond);
    }

    // Allowing custom speed (specifically) for minigames, due to large increases being common
    public IEnumerator IncreaseStaminaBy(float percent, float speed) 
    {
        float percentClamped = Mathf.Clamp(percent, 0f, STAMINA_MAX);
        float increaseBy = percentClamped * STAMINA_MAX;

        float goalFillAmount = Mathf.Min(STAMINA_MAX, staminaFillImage.fillAmount + increaseBy);
        float toAdd = goalFillAmount - staminaFillImage.fillAmount;

        return ChangeStaminaGeneral(toAdd, speed);
    }

    // Shortens the code, and keeps the main Coroutine loop logic in one place for updating the fillAmount
    IEnumerator ChangeStaminaGeneral(float difference, float speed)
    {
        float currentSum = 0f;
        while (currentSum <= difference)
        {
            float change = speed * Time.fixedDeltaTime;
            UpdateFillAmount(staminaFillImage.fillAmount + change);
            currentSum += Mathf.Abs(change);  // because the change can be negative
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    void UpdateFillAmount(float newFill)
    {
        // Update the fill amount
        staminaFillImage.fillAmount = newFill;

        OnChange?.Invoke(newFill);
    }

    //Fading in and out
    public void FadeIn()
    {
        // full black --> invisible
        StartCoroutine(FadeCoroutine(1f, 0f));
    }

    public void FadeOut()
    {
        print("fading out");
        // invisible --> full black
        StartCoroutine(FadeCoroutine(0f, 1f));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        float currentAlpha = startAlpha;
        while (Mathf.Abs(currentAlpha - startAlpha) < Mathf.Abs(startAlpha - endAlpha))
        {
            staminaFillImage.color = new Color(staminaFillImage.color.r, staminaFillImage.color.g, staminaFillImage.color.b, currentAlpha);
            staminaBarImage.color = new Color(staminaBarImage.color.r, staminaBarImage.color.g, staminaBarImage.color.b, currentAlpha);
            greyOutlineImage.color = new Color(greyOutlineImage.color.r, greyOutlineImage.color.g, greyOutlineImage.color.b, currentAlpha);
            currentAlpha += fadeSpeed * Time.deltaTime * Mathf.Sign(endAlpha - startAlpha);
            yield return null;
        }

        OnFadingComplete?.Invoke();

        yield return null;
    }
}
