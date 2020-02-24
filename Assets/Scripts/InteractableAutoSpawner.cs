﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Interactable,
    Throwable
}

public class InteractableAutoSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int spawnCount = 5;
    [SerializeField] public InteractableType interactableType;
    public string chuteSFX;

    private List<GameObject> currentInteractables;

    void Start()
    {
        currentInteractables = new List<GameObject>();
        for (int i = 0; i < spawnCount; i++)
        {
            currentInteractables.Add(SpawnInteractable());
        }

        if (interactableType == InteractableType.Throwable)
        {
            GameManager.Instance.GetPlayerManager().OnThrow += OnInteractEnd;
        }
    }

    private GameObject SpawnInteractable()
    {
        GameObject spawnedInteractable = Instantiate(prefab, transform);
        //FMODUnity.RuntimeManager.PlayOneShot(sfxPath, transform.position); this is for the pill bottle sfx
        spawnedInteractable.transform.localPosition = Vector3.zero;

        switch (interactableType)
        {
            case InteractableType.Interactable:
                {
                    Interactable interactable = Utils.GetRequiredComponent<Interactable>(spawnedInteractable);
                    interactable.OnInteractEnd += OnInteractEnd;
                }
                break;
            case InteractableType.Throwable:
                break;
            default:
                Debug.LogError($"Unknown InteractableType: {interactableType}");
                break;
        }

        return spawnedInteractable;
    }

    private void OnInteractEnd(Interactable source)
    {
        if (currentInteractables.Remove(source.gameObject))
        {
            currentInteractables.Add(SpawnInteractable());
        }
    }
}
