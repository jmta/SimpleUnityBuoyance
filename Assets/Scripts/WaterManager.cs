using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WaterManager : MonoBehaviour
{

    public static WaterManager instance;

    public float length = 2f;

    public float amplitude = 1f;

    public float speed = 1f;

    private float offset = 0f;

    private MeshFilter meshFilter;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(this);
        }

        meshFilter = GetComponent<MeshFilter>();
    }
    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * speed;

        Vector3[] vertices = meshFilter.mesh.vertices;
        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = this.getWaveHeight(transform.TransformPoint(vertices[i]));
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }

    public float getWaveHeight(Vector3 point)
    {
        return amplitude * Mathf.Sin(point.x / length  + offset);
    }

    public float depthUnderWater(Vector3 point)
    {
        if (pointUnderwater(point)) 
        {
            return getWaveHeight(point) - point.y;
        }
        else
        {
            return 0f;
        }    
    }

    public bool pointUnderwater(Vector3 point)
    {
        return point.y < getWaveHeight(point);
    }

    public List<Vector3> getPointsInWater(Vector3[] points)
    {
        List<Vector3> underwaterCorners = new List<Vector3>();
        foreach (Vector3 corner in points)
        {

            if (pointUnderwater(transform.TransformPoint(corner)))
            {
                underwaterCorners.Add(corner);
            }
        }
        return underwaterCorners;
    }
}
