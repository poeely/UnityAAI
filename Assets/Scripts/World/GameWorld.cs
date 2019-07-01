using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides access to various world functionality such as NavGraph, Octree, and Region info. Author: Vincent J.S. Versnel
/// </summary>
public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance;

    [Header("Navigation Graph")]
    public LayerMask blockingLayer;
    public LayerMask walkableLayer;
    public int width, height;
    public float heightThreshold;

    public bool showNavGraphInEditor;

    private NavGraph navGraph;

    [Header("Region Settings")]
    public bool showRegionsInEditor;
    public int regionCount;
    public MeshRenderer regionVisualPrefab;
    public Gradient regionGradient;

    private Region[,] regions;
    private GameObject regionsVisualParent;

    [Header("Item Spawning")]
    public WeaponSpawner weaponSpawnerPrefab;

    [Header("Oct Tree")]
    public int size = 240;
    public bool showOctreeInEditor;

    private Octree<Entity> octree;

    private FuzzyModule promisingRegionFM;

    // Initialization
    private void Awake()
    {
        // Assure only one instance
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        InitPromisingRegionFM();

        CreateNavGraph();
        CreateRegions();
        CreateSpawners();
        CreateOctTree();
    }

    private void Update()
    {
        for (int x = 0; x < regions.GetLength(0); x++)
            for (int y = 0; y < regions.GetLength(1); y++)
                regions[x, y].Update();

        if(Input.GetKeyDown(KeyCode.Y))
            regionsVisualParent.SetActive(!regionsVisualParent.activeSelf);
    }

    private void FixedUpdate()
    {
        // Clear & Refill Octree
        octree.Clear();
        for (int i = 0; i < EntityManager.EntityCount; i++)
            octree.Insert(EntityManager.GetEntity(i));
    }

    // Creates the Navigation Graph
    private void CreateNavGraph()
    {
        Debug.Log("[WORLD]: Re-creating NavGraph...");

        if (navGraph == null)
            navGraph = new NavGraph();
        navGraph.blockingLayer = blockingLayer;
        navGraph.terrainLayer = walkableLayer;

        navGraph.GenerateFloodfill(width, height, 0.1f);
        Debug.Log("[WORLD]: NavGraph Generated.");
    }

    private void CreateRegions()
    {
        Debug.Log("[WORLD]: Creating regions.");
        if (regionCount > 0)
        {
            int regionWidth = Mathf.FloorToInt(width / regionCount);
            int regionDepth = Mathf.FloorToInt(height / regionCount);

            Vector3 offset = new Vector3(-(width / 2) + (regionWidth / 2), 1f, -(height / 2) + (regionDepth / 2));
            Vector3 size = new Vector3(regionWidth, regionWidth, regionDepth);

            regions = new Region[regionCount, regionCount];

            for (int x = 0; x < regionCount; x++)
            {
                for (int z = 0; z < regionCount; z++)
                {
                    Vector3 center = new Vector3(x * regionWidth, 0f, z * regionDepth) + offset;
                    AABB aabb = new AABB(center, size);
                    regions[x, z] = new Region(Random.Range(Region.MIN_RESOURCE_COUNT, Region.MAX_RESOURCE_COUNT), aabb, promisingRegionFM);
                }
            }
        }
    }

    private void InitPromisingRegionFM()
    {
        // Promising Region FuzzyModule
        promisingRegionFM = new FuzzyModule();
        FuzzyVariable DistToRegion = promisingRegionFM.CreateFLV("DistToRegion");

        FzSet Dist_Close = DistToRegion.AddLeftShoulderSet("Dist_Close", 0, 50, 150);
        FzSet Dist_Medium = DistToRegion.AddTriangleSet("Dist_Medium", 50, 150, 200);
        FzSet Dist_Far = DistToRegion.AddRightShoulderSet("Dist_Far", 150, 250, 300);

        FuzzyVariable RegionWealth = promisingRegionFM.CreateFLV("RegionWealth");

        FzSet Region_Poor = RegionWealth.AddLeftShoulderSet("Region_Poor", 0, 300, 750);
        FzSet Region_Decent = RegionWealth.AddTriangleSet("Region_Decent", 750, 1500, 2700);
        FzSet Region_Rich = RegionWealth.AddRightShoulderSet("Region_Rich", 2250, 2700, Region.MAX_RESOURCE_COUNT);

        FuzzyVariable Desirability = promisingRegionFM.CreateFLV("Desirability");

        FzSet Undersirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);
        FzSet Desirable = Desirability.AddTriangleSet("Desirable", 25, 50, 75);
        FzSet Very_Desirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);

        promisingRegionFM.AddRule(new FzAND(Dist_Close, Region_Poor), Undersirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Close, Region_Decent), Desirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Close, Region_Rich), Very_Desirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Medium, Region_Poor), Undersirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Medium, Region_Decent), Undersirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Medium, Region_Rich), Very_Desirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Far, Region_Poor), Undersirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Far, Region_Decent), Undersirable);
        promisingRegionFM.AddRule(new FzAND(Dist_Far, Region_Rich), Very_Desirable);
    }

    private void CreateSpawners()
    {
        // Get normalize values for resourceCount of regions
        float minResources = Region.MIN_RESOURCE_COUNT;
        float maxResources = Region.MAX_RESOURCE_COUNT;

        regionsVisualParent = new GameObject();
        regionsVisualParent.transform.SetParent(transform);

        for (int x = 0; x < regions.GetLength(0); x++)
        {
            for (int y = 0; y < regions.GetLength(1); y++)
            {
                Region current = regions[x, y];

                // Spawn weapon on random location in region
                Vector3 min = current.bounding.center - (current.bounding.size / 2);
                Vector3 max = current.bounding.center + (current.bounding.size / 2);

                Vector3 position = VectorExtensions.RandomRange(min, max);

                WeaponSpawner spawner = Instantiate(weaponSpawnerPrefab, position, Quaternion.identity);

                // Create visual feedback for this region
                Vector3 newPosition = new Vector3(current.bounding.center.x, 1f, current.bounding.center.z);      
                MeshRenderer newMeshRenderer = Instantiate(regionVisualPrefab, newPosition, Quaternion.identity);

                current.SetVisual(newMeshRenderer, regionGradient);

                Vector3 newScale = new Vector3(current.bounding.size.x, newMeshRenderer.transform.localScale.y, current.bounding.size.z) - new Vector3(1f, 0f, 1f);
                newMeshRenderer.transform.localScale = newScale;

                newMeshRenderer.transform.SetParent(regionsVisualParent.transform);
            }
        }

        regionsVisualParent.SetActive(false);
    }

    private void CreateOctTree()
    {
        AABB bound = new AABB(transform.position, Vector3.one * size);
        octree = new Octree<Entity>(bound, 1);
    }

    private void DrawOctTree(Octree<Entity> octTree)
    {
        Gizmos.DrawWireCube(octree.boundary.center, octree.boundary.size);
        foreach (var tree in octTree.subtrees)
            DrawOctTree(tree);
    }

    // Gizmos to provide relevant information in scene view
    private void OnDrawGizmos()
    {
        if(regionCount > 0 && showRegionsInEditor)
        {
            CreateRegions();

            for (int i = 0; i < regionCount; i++)
                for (int j = 0; j < regionCount; j++)
                    regions[i, j].bounding.DrawGizmos(Color.blue);
        }

        if(showOctreeInEditor)
        {
            // Oct tree
            if (octree == null)
                CreateOctTree();

            DrawOctTree(octree);
        }

        if (!showNavGraphInEditor)
            return;

        if (navGraph == null || width != navGraph.Width || height != navGraph.Depth)
            CreateNavGraph();

        for (int x = 0; x < navGraph.Width; x++)
        {
            for (int z = 0; z < navGraph.Depth; z++)
            {
                Node current = navGraph.GetNode(x, z);

                foreach (Node neighbour in current.GetNeighbours())
                {
                    Gizmos.DrawLine(current.worldPosition, neighbour.worldPosition);
                }
            }
        }
    }

    public NavGraph NavGraph { get { return navGraph; } }
    public Region[,] Regions { get { return regions; } }
    public Octree<Entity> Octree { get { return octree; } }
    public FuzzyModule GetPromisingRegionFM { get { return promisingRegionFM; } }
}
