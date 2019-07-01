using System.Collections.Generic;

/// <summary>
/// Registeres entities for easier look-ups.
/// 
/// Author: Vincent Versnel
/// </summary>
public static class EntityManager
{
    private static int nextValidID = 0;
    private static int GetNextValidID { get { return nextValidID++; } }

    private static List<Entity> entities;
    public static int EntityCount
    {
        get { return entities.Count; }
    }

    public static void Register(Entity entityToRegister)
    {
        if (entities == null)
            entities = new List<Entity>();

        entityToRegister.SetID(GetNextValidID);

        entities.Add(entityToRegister);
    }

    public static Entity GetEntity(int id)
    {
        if (id > entities.Count)
            return null;
        return entities[id];
    }

    public static void Clear()
    {
        entities.Clear();
        entities = null;
    }
}