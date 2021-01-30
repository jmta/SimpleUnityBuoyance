using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Buoyancy : MonoBehaviour
{
    /**
     * The BoxCollider is used to determine various points around the object where the object will float from.
     * A box should provide a reasonable enough estimation to be convincing
     */
    private BoxCollider collider;

    /**
     * This rigid body is what the floatation force is applied to
     */
    private Rigidbody rigidBody;

    /**
     * The total floatation the object would produce if the whole object was submerged
     */
    public float totalFloatationForce = 500000;

    /**
     * The box collider is assessed at various different points.
     * A uniform point cloud is generated for the Box and each point assessed for the force it should be applying
     */
    //How many points to generate in the X Plane
    public int floatationPointsX = 10;

    //How many points to generate in the Y Plane
    public int floatationPointsY = 10;

    //How many points to generate in the Z Plane
    public int floatationPointsZ = 10;

    /**
     * Force each point produces once in water
     */
    private float floatationForcePerPoint;

    /**
     * The points we will be accessing for buoyancy
     */
    private Vector3[] floatationPoints;

    void Start()
    {
        // Get the collider and rigid body
        collider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

        // Generate the floatation point cloud
        floatationPoints = calculateBoxColliderCorners(collider);

        // Calculate the force per point (if submerged to 1 m)
        floatationForcePerPoint = totalFloatationForce / (floatationPointsX * floatationPointsY * floatationPointsZ);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Get every single point which is deemed to be underwater and store in local co-ordinates
        List<Vector3> underwaterCorners = fromGlobal(WaterManager.instance.getPointsInWater(toGlobal(floatationPoints)));

        //Only need to calculate force and location if we are actually underwater
        if(underwaterCorners.Count > 0)
        {
            //When applying the force it should be applied at the central point of all the underwater points
            Vector3 center = calculateCenterOfUnderwater(underwaterCorners);
            float totalForce = calculateTotalFloatationForce(underwaterCorners);

            //The force will just be acting upwards, this is buoyancy, we will deal with drag elsewhere
            rigidBody.AddForceAtPosition(new Vector3(0, totalForce, 0), center);

            Debug.DrawRay(center, Vector3.up, Color.green, totalForce/1000);
        }
    }

    /**
     * This calculates our 3d array of points to be considered for buoyancy
     */
    private Vector3[] calculateBoxColliderCorners(BoxCollider coll) {
        Vector3[] ret = new Vector3[floatationPointsX * floatationPointsY * floatationPointsZ];
        Vector3 center = coll.center;
        Vector3 size = coll.size;
        int count = 0;
        for(int i = 0; i < floatationPointsX; i++)
        {
            for(int j = 0; j < floatationPointsY; j++)
            {
                for(int k = 0; k < floatationPointsZ; k++)
                {
                    ret[count] = new Vector3(center.x - (size.x / 2) + ((size.x / floatationPointsX) * i), center.y - (size.y / 2) + ((size.y / floatationPointsY) * j), center.z - (size.z / 2) + ((size.z / floatationPointsZ) * k));
                    count++;
                }
            }
        }
        return ret;
    }

    private Vector3 calculateCenterOfUnderwater(List<Vector3> underwaterCorners)
    {
        //To find the center of the vector you can add them all together then divide by the total points considered
        Vector3 sum = Vector3.zero;
        foreach (Vector3 underwaterPoint in underwaterCorners)
        {
            sum += underwaterPoint;
        }
        return sum / underwaterCorners.Count;
    }

    /**
     * This calculates the total force all the underwater points will be generating
     */
    private float calculateTotalFloatationForce(List<Vector3> underwaterCorners)
    {
        float sum = 0.0f;
        foreach (Vector3 underwaterPoint in underwaterCorners){
            //We clamp the depth between 0 and 1 to ensure each point can't provide more
            sum += floatationForcePerPoint * WaterManager.instance.depthUnderWater(transform.TransformPoint(underwaterPoint));
        }
        return sum;
    }

    /**
     * Convert the vector provided to global co-ordinates
     */
    private Vector3[] toGlobal(Vector3[] points)
    {
        Vector3[] ret = new Vector3[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            ret[i] = transform.TransformPoint(points[i]);
        }
        return ret;
    }

    /**
     * Convert the List of vector3 provided to local co-ordinates
     */
    private List<Vector3> fromGlobal(List<Vector3> points)
    {
        List<Vector3> ret = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            ret.Add(transform.InverseTransformPoint(points[i]));
        }
        return ret;
    }
}
