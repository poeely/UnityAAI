using UnityEngine;


public class Entity : MonoBehaviour, IPoint
{
    private int id = -1;

    protected virtual void Awake()
    {
        EntityManager.Register(this);
    }

    public virtual void HandleMessage(MessageEventArgs e)
    {
        
    }

    public int ID { get { return id; } }

    public void SetID(int id)
    {
        if (this.id >= 0)
            return;
        this.id = id;
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }
}
