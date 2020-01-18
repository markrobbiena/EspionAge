﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public const float STAMINA_MAX = 1f;

    private float staminaAmount;
    private Image staminaBarImage;
    // private Stamina stamina;
    private void Awake() {
        staminaBarImage = transform.Find("stamina").GetComponent<Image>();
        staminaBarImage.fillAmount = 1f;
   

    }
    void Update(){
        //Testing stuff
        // if (Input.GetKeyDown("space")){
        //     print("pressed space");
        //     IEnumerator decreaseCoroutine = decreaseStaminaBy(.1f);
        //     StartCoroutine(decreaseCoroutine);
        // } if (Input.GetKeyDown("up")){
        //     print("pressed f");
        //     IEnumerator increaseCoroutine = increaseStaminaBy(.5f);
        //     StartCoroutine(increaseCoroutine);
        // }
    }

    public IEnumerator decreaseStaminaBy(float percent) {
        float percentClamped = Mathf.Clamp(percent, 0, 1);
        float decreaseBy = percentClamped * STAMINA_MAX;
        
        float fillAmount = staminaBarImage.fillAmount - decreaseBy; 
        if (fillAmount <= 0){ 
            fillAmount = staminaBarImage.fillAmount;
        }
        while (fillAmount <= staminaBarImage.fillAmount){
            staminaBarImage.fillAmount -= 0.001f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator increaseStaminaBy(float percent) {
        float percentClamped = Mathf.Clamp(percent, 0, 1);
        float increaseBy = percentClamped * STAMINA_MAX;
        
        float fillAmount = staminaBarImage.fillAmount + increaseBy; 
        if (fillAmount >= STAMINA_MAX){ 
            fillAmount = STAMINA_MAX;
        }
        while (fillAmount >= staminaBarImage.fillAmount){
            staminaBarImage.fillAmount += 0.001f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    
}
