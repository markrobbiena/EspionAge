﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Awakeness")]
    public float awakenessIncrease = 0.01f;
    public float awakenessDecrease = 0.005f;
    public float dangerRadius = 1000.0f;

    [Header("Throwables")]
    public Transform throwPosition;
    public float throwMultiplier = 0.08f;
    public float angleIncreaseSpeed = 45f;
    public float minThrowAngle = 0f;
    public float maxThrowAngle = 90f;
    public float minThrowVelocity = 10f;
    public float maxThrowVelocity = 20f;

    private LaunchArcRenderer launchArcRenderer;
    private List<GameObject> currentThrowables;
    private bool isThrowing = false;
    private float startThrowTime;

    private List<Coroutine> spawnedCoroutines;

    private void Start()
    {
        launchArcRenderer = GetComponentInChildren<LaunchArcRenderer>();
        currentThrowables = new List<GameObject>();
        spawnedCoroutines = new List<Coroutine>();

        UIManager.Instance.staminaBar.OnChange += UpdateThrowVelocity;
    }

    private void Update()
    {
        HandleThrowInput();
    }

    private void FixedUpdate()
    {
        HandleDecreaseAwakeness();

        float minDistance = DistToClosestEnemy();

        if (minDistance < dangerRadius)
        {
            HandleIncreaseAwakeness((dangerRadius - minDistance) / dangerRadius);
        }
    }

    private void UpdateThrowVelocity(float fillAmount)
    {
        launchArcRenderer.velocity = Mathf.Lerp(minThrowVelocity, maxThrowVelocity, fillAmount / StaminaBar.STAMINA_MAX);
    }

    private void HandleThrowInput()
    {
        if (launchArcRenderer && currentThrowables.Count > 0)
        {
            float throwAxisValue = Input.GetAxis(Constants.INPUT_THROW_GETDOWN);

            // This means that we either just let go, or like were never pressing in the first place...
            if (Mathf.Approximately(throwAxisValue, 0f))
            {
                // So we check if we were previously throwing (so this means we just let go)
                if (isThrowing)
                {
                    // If so, then throw the object!!
                    isThrowing = false;
                    ThrowNext();
                }
            }
            else
            {
                // If we are holding the button, then ping pong the launch arc angle
                if (isThrowing)
                {
                    startThrowTime += angleIncreaseSpeed * Time.deltaTime;
                    launchArcRenderer.RenderArc(Mathf.PingPong(startThrowTime, maxThrowAngle - minThrowAngle) + minThrowAngle);
                }
                // Otherwise, we started holding just now, so let's record the start time and stuff
                else
                {
                    isThrowing = true;
                    startThrowTime = Time.time;
                }
            }
        }
    }

    public void AddThrowable(GameObject throwableObject)
    {
        throwableObject.SetActive(false);
        throwableObject.transform.parent = throwPosition;
        throwableObject.transform.localPosition = Vector3.zero;

        currentThrowables.Add(throwableObject);
    }

    // This is an unsafe function! Must check length of currentThrowables before calling it (like in HandleThrowInput() above)!
    private void ThrowNext()
    {
        GameObject current = currentThrowables[0];
        currentThrowables.RemoveAt(0);

        Rigidbody currentRigidbody = current.GetComponent<Rigidbody>();
        if (currentRigidbody)
        {
            // Display the object, center at the throw point, remove parent, then throw in the same path of the current launch arc
            // - the force angle is partly from just testing and playing with the values
            current.SetActive(true);
            current.transform.localPosition = Vector3.zero;
            current.transform.parent = null;
            currentRigidbody.AddForce(Quaternion.AngleAxis((launchArcRenderer.angle % 180f) - 90, launchArcRenderer.transform.forward) * launchArcRenderer.transform.up * launchArcRenderer.velocity * throwMultiplier, ForceMode.Impulse);
        }

        // Reset the angle to 0, which also means the line renderer will not be visible
        launchArcRenderer.RenderArc(0f);
    }

    private float DistToClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
            }
        }
        return minDistance;
    }

    // Note: Might need to do more testing if this is actually doing anything considerable...
    //  but better safe than sorry to make sure coroutines we spawn are no longer running when we enter a minigame
    void StopAllSpawnedCoroutines()
    {
        // No loose coroutines in MY house!
        foreach (Coroutine c in spawnedCoroutines)
        {
            StopCoroutine(c);
        }
        spawnedCoroutines.Clear();
    }

    private void OnDisable()
    {
        StopAllSpawnedCoroutines();

    }

    void HandleIncreaseAwakeness(float multiplier)
    {
        spawnedCoroutines.Add(StartCoroutine(UIManager.Instance.staminaBar.IncreaseStaminaBy(multiplier * awakenessIncrease)));
    }

    void HandleIncreaseAwakenessBy(float value, float speed)
    {
        spawnedCoroutines.Add(StartCoroutine(UIManager.Instance.staminaBar.IncreaseStaminaBy(value, speed)));
    }

    public void HandleDecreaseAwakeness()
    {
        spawnedCoroutines.Add(StartCoroutine(UIManager.Instance.staminaBar.DecreaseStaminaBy(awakenessDecrease)));
    }
}
