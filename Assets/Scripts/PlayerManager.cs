﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Awakeness")]
    public float awakenessIncrease = 0.01f;
    public float awakenessDecrease = 0.005f;
    public float dangerRadius = 1000.0f;

    private List<Coroutine> spawnedCoroutines;

    private void Start()
    {
        spawnedCoroutines = new List<Coroutine>();
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
