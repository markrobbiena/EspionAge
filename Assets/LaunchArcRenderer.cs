﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    public float velocity;
    public float angle;
    public int resolution = 10;

    private float g;  // force of gravity on the y-axis
    private float radianAngle;

    private LineRenderer lr;

    private void Awake()
    {
        lr = Utils.GetRequiredComponent<LineRenderer>(this);
        g = Mathf.Abs(Physics.gravity.y);
    }

    private void OnValidate()
    {
        if (lr != null && Application.isPlaying)
        {
            RenderArc();
        }
    }

    void Start()
    {
        RenderArc();
    }

    // populating the line renderer withthe appropriate settings
    public void RenderArc()
    {
        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());
    }
    
    public void RenderArc(float newAngle)
    {
        angle = newAngle;
        RenderArc();
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;
        
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}
