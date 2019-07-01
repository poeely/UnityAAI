using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainClickEventArgs : EventArgs
{
    public Vector3 Position { get; set; }
}

public class TerrainClickHandler : MonoBehaviour
{
    public EventHandler<TerrainClickEventArgs> Clicked;

    public TerrainCollider terrainCollider;

    private void Update()
    {
        if (terrainCollider == null)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(terrainCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("Clicked terrain!");
                if(Clicked != null)
                {
                    TerrainClickEventArgs e = new TerrainClickEventArgs();
                    e.Position = hit.point;
                    Clicked.Invoke(this, e);
                }
            }
        }
    }
}
