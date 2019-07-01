using System.Collections.Generic;
using UnityEngine;

// Author: Vincent J.S. Versnel

public class ConsumedRegionResourceEventArgs : MessageEventArgs
{
    public int Amount { get; set; }
}

/// <summary>
/// Stores data about a region's resources. It updates itself when entities send a
/// 
/// </summary>
public class Region
{
    public static readonly string ARRIVED_AT_REGION = "ARRIVED_AT_REGION";
    public static readonly string FOUND_NEW_REGION = "FOUND_NEW_REGION";
    public static readonly int MIN_RESOURCE_COUNT = 0;
    public static readonly int MAX_RESOURCE_COUNT = 3000;

    public int resourceCount;
    public AABB bounding;
    private FuzzyModule fm;

    private bool isBeingUsed = false;
    private float timer;

    // Visual
    private MeshRenderer meshRenderer;
    private Gradient gradient;

    public Region(int resourceCount, AABB bounding, FuzzyModule fm)
    {
        this.resourceCount = resourceCount;
        this.bounding = bounding;
        this.fm = fm;
    }

    public double GetDesirability(float distance)
    {
        // Fuzzify antecedent 1
        fm.Fuzzify("DistToRegion", distance);
        // Fuzzify antecedent 2
        fm.Fuzzify("RegionWealth", resourceCount); 
        // Defuzzify consequence
        return fm.DeFuzzify("Desirability");
    }

    public void Update()
    {
        if (!isBeingUsed)
            return;
        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            resourceCount -= 10;
            resourceCount = Mathf.Clamp(resourceCount, 0, MAX_RESOURCE_COUNT);
            timer = 0f;

            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        if (meshRenderer == null || gradient == null)
        {
            Debug.LogError("Its null you");
            return;
        }

        float normalizedValue = (float)(resourceCount - (float)MIN_RESOURCE_COUNT) / (float)(MAX_RESOURCE_COUNT - (float)MIN_RESOURCE_COUNT);
        meshRenderer.sharedMaterial.SetColor("_Color", gradient.Evaluate(normalizedValue));
    }

    public void StartLeeching()
    {
        isBeingUsed = true;
    }

    public void SetVisual(MeshRenderer meshRenderer, Gradient gradient)
    {
        this.meshRenderer = meshRenderer;
        this.gradient = gradient;

        UpdateVisual();
    }
}
