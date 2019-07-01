using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHerdeEntity
{
    void Init(Herde herde);
}

/// <summary>
/// Spawns a herde of sheep and cowboys at initialization.
/// </summary>
public class Herde : MonoBehaviour
{
    // Settings
    public int sheepCount;
    public int ogreCount;
    public int wolfCount;

    // Prefabs
    public Ogre ogrePrefab;
    public Sheep sheepPrefab;
    public Wolf wolfPrefab;
    public CameraMovement cameraPrefab;

    // References
    private Sheep[] sheep;
    private Ogre ogre;
    private Wolf wolf;
    

    // Fuzzy Logic
    private FuzzyModule sheepDesireFM;


    public float MaxHerdeRadius = 50.0f;

    private void Awake()
    {
        InitSheepDesireFM();


        SpawnHerde();
    }

    bool gizmoBool = false;
    void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (gizmoBool)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(HerdeCentroid(),MaxHerdeRadius);
        }

#endif
    }

    private void SpawnHerde()
    {
        sheep = new Sheep[sheepCount];

        //gizmo
        gizmoBool = true;

        for (int i = 0; i < sheep.Length; i++)
        {
            Vector3 sheepPos = transform.position + new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
            sheepPos.y = 1;
            sheep[i] = Instantiate(sheepPrefab, sheepPos, Quaternion.identity);
        }

        Vector3 position = Random.insideUnitSphere;
        position.y = 1;
        ogre = Instantiate(ogrePrefab, position, Quaternion.identity);

        CameraMovement camera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity);
        camera.target = ogre.transform;

        position = Random.insideUnitSphere;
        position.y = 1;
        wolf = Instantiate(wolfPrefab, position * 50.0f, Quaternion.identity);

        ogre.Init(this);
        wolf.Init(this);

        for (int i = 0; i < sheep.Length; i++)
        {
            sheep[i].Init(this);
        }
    }

    public Vector3 HerdeCentroid()
    {
        Vector3 centroid = Vector3.zero;

        foreach(Sheep s in sheep)
        {
            centroid += s.transform.position;
        }

        centroid /= sheepCount;
        return centroid;
    }

    public Sheep CheckHerdCohesion()
    {
        Vector3 centroid = HerdeCentroid();

        Sheep furthest = null;

        foreach(Sheep s in sheep)
        {
            if(furthest == null)
                furthest = s;
            float current = Vector3.Distance(centroid, furthest.Position);
            float temp = Vector3.Distance(centroid, s.transform.position);
            if (temp > current)
                furthest = s;
        }

        if(Vector3.Distance(furthest.Position, centroid) > MaxHerdeRadius)
            return furthest;
        
        return null;
    }

    private void InitSheepDesireFM()
    {
        sheepDesireFM = new FuzzyModule();
        FuzzyVariable DistToTarget = sheepDesireFM.CreateFLV("DistToOgre");

        FzSet Dist_Close = DistToTarget.AddLeftShoulderSet("Dist_Close", 0, 25, 150);
        FzSet Dist_Medium = DistToTarget.AddTriangleSet("Dist_Medium", 25, 50, 300);
        FzSet Dist_Far = DistToTarget.AddRightShoulderSet("Dist_Far", 150, 300, 500);

        FuzzyVariable AmmoStatus = sheepDesireFM.CreateFLV("Hunger");

        FzSet Hunger_Low = AmmoStatus.AddLeftShoulderSet("Hunger_Low", 0, 0, 10);
        FzSet Hunger_Okay = AmmoStatus.AddTriangleSet("Hunger_Okay", 0, 10, 30);
        FzSet Hunger_Intense = AmmoStatus.AddRightShoulderSet("Hunger_Intense", 10, 30, 40);

        FuzzyVariable Desirability = sheepDesireFM.CreateFLV("Desirability");

        FzSet Undersirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);
        FzSet Desirable = Desirability.AddTriangleSet("Desirable", 25, 50, 75);
        FzSet VeryDesirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);

        sheepDesireFM.AddRule(new FzAND(Dist_Close, Hunger_Low), Undersirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Close, Hunger_Okay), Undersirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Close, Hunger_Intense), Undersirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Medium, Hunger_Low), Desirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Medium, Hunger_Okay), VeryDesirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Medium, Hunger_Intense), VeryDesirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Far, Hunger_Low), Undersirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Far, Hunger_Okay), Desirable);
        sheepDesireFM.AddRule(new FzAND(Dist_Far, Hunger_Intense), Desirable);
    }

    public Sheep[] Sheep { get { return sheep; } }
    public Ogre Ogre { get { return ogre; } }
    public Wolf Wolf { get { return wolf; } }

   	public FuzzyModule GetSheepDesireFM { get { return sheepDesireFM; } }

}
