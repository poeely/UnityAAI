using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MessageEventArgs : EventArgs
{
    public Entity Sender    { get; set; }
    public string Message   { get; set; }
}

/// <summary>
/// A messenger class that allows entities to listen to other specific entities.
/// Eg. A cowboy could only listens to the sheep of his herde.
/// </summary>
public static class EntityMessenger
{
    public static EventHandler<MessageEventArgs> MessageSend;

    private static Dictionary<int, List<Entity>> registeredListeners;

    public static void RegisterEntity(int entityID)
    {
        if (registeredListeners == null)
            registeredListeners = new Dictionary<int, List<Entity>>();

        Debug.Log("Registered entity: " + EntityManager.GetEntity(entityID).name);

        registeredListeners.Add(entityID, new List<Entity>());
    }

    public static void SubscribeToEntity(Entity entity, Entity subscriber)
    {
        if (!registeredListeners.ContainsKey(entity.ID))
            throw new Exception("[SMSG]: Entity to subscribe to is not registered.");

        registeredListeners[entity.ID].Add(subscriber);
    }

    public static void SendMessage(MessageEventArgs e)
    {
        if (e.Sender == null)
            throw new Exception("[EMSG]: A sender has to be supplied to send a message.");
        List<Entity> listeners = registeredListeners[e.Sender.ID];

        Debug.Log("Message was send: " + e.Message + ". Listeners: " + listeners.Count);

        for (int i = 0; i < listeners.Count; i++)
            listeners[i].HandleMessage(e);
    }

    // Super expensive operation when lots of callbacks are registered.
    public static void SendGlobalMessage(Entity sender, MessageEventArgs e)
    {
        if(MessageSend != null)
        {
            MessageSend.Invoke(sender, e);
        }
    }

    public static void Clear()
    {
        registeredListeners.Clear();
    }
}

