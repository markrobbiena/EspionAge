﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour 
{
    public CinemachineVirtualCamera mainCamera;
    public bool isRestricted;
    
    [Header("Fog Particles")]
    public List<ParticleSystem> fogs;
    public float enabledFogAlpha = 50f / 255f;
    public float disabledFogAlpha = 0f;
    public float fadeSpeed = 0.1f;

    private void Start()
    {
        fogs.ForEach(fog =>
        {
            fog.gameObject.SetActive(GameManager.Instance.enableFog);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            if (isRestricted)
            {
                print("isrestricted");
                UIManager.Instance.staminaBar.gameObject.SetActive(true);
            }
            else
            {
                print("isunrestricted");
                UIManager.Instance.staminaBar.gameObject.SetActive(false);
            }
            // so if there is no mainCamera set, we will just keep whatever the current camera is (which may not be preferable in most cases)
            if (mainCamera)
            {
                CameraManager.Instance.BlendTo(mainCamera);
            }

            if (GameManager.Instance.enableFog)
            {
                StartCoroutine(FadeFog(enabledFogAlpha, disabledFogAlpha));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.enableFog)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                StartCoroutine(FadeFog(disabledFogAlpha, enabledFogAlpha));
            }
        }
    }

    private IEnumerator FadeFog(float startAlpha, float endAlpha)
    {
        IEnumerable<Color> startColors = fogs.Select(fog =>
            new Color(fog.main.startColor.color.r, fog.main.startColor.color.g, fog.main.startColor.color.b, startAlpha));
        IEnumerable<Color> endColors = fogs.Select(fog => 
            new Color(fog.main.startColor.color.r, fog.main.startColor.color.g, fog.main.startColor.color.b, endAlpha));

        float t = 0f;
        while (t < 1f)
        {
            t += fadeSpeed;
            
            for (int i = 0; i < fogs.Count; i++)
            {
                var main = fogs[i].main;
                main.startColor = Color.Lerp(startColors.ElementAt(i), endColors.ElementAt(i), t);

            }

            yield return null;
        }
    }
}
