using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour {
    public float mass;                  // [kg] 
    public Vector3 velocityVector;      // Average velocityVector this FixedUpdate() [m s^-1]
    public Vector3 netForceVector;      // N [kg m s^-2]
    public bool showTrails = true;

    

    private List<Vector3> forceVectorList = new List<Vector3>();
    private LineRenderer lineRenderer;
    private int numberOfForces;
    private Vector3 deltaS;            // [m]
    private PhysicsEngine[] physicsEngineArray;
    private Vector3 gravitationalForcevector;
    // Use this for initialization
    void Start() {
        deltaS = new Vector3(0, 0, 0);
        StartDrawForces();
        physicsEngineArray = FindObjectsOfType<PhysicsEngine>();
    }

	// Update is called once per frame
	void FixedUpdate () {
        UpdateDrawForces();
        CalculateGravity();
        UpdatePosition();
    }

    public void AddForce(Vector3 forceVector)
    {
        forceVectorList.Add(forceVector);
    }

    void CalculateGravity()
    {
        float G = .000006674f;
        gravitationalForcevector = Vector3.zero; 

        foreach (PhysicsEngine body in physicsEngineArray) {
            if (!body.Equals(this) && (gameObject.transform.position - body.transform.position).magnitude >= 1f) {
                Vector3 r = gameObject.transform.position - body.transform.position;

                //Debug.Log((G * mass * body.mass / Mathf.Pow(r.magnitude, 2)) * r.normalized);
                gravitationalForcevector += -(G * mass * body.mass / Mathf.Pow(r.magnitude, 2)) * r.normalized;

            }

        }
        //Debug.Log(gravitationalForcevector);
    }

    void UpdatePosition() {
        // Sum forces and clear list
        netForceVector = Vector3.zero;

        foreach (Vector3 forceVector in forceVectorList)
        {
            netForceVector += forceVector;
        }
        forceVectorList.Clear();

        // Update velocity
        Vector3 accelerationVector = (netForceVector + gravitationalForcevector) / mass;
        Debug.Log(accelerationVector);
        velocityVector += accelerationVector * Time.deltaTime;
        // Update Position
        deltaS = velocityVector * Time.deltaTime;
        gameObject.transform.position += deltaS;
    }

    void StartDrawForces() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.startWidth = 0.2F;
        lineRenderer.endWidth = 0.2F;
        lineRenderer.useWorldSpace = false;
    }

    void UpdateDrawForces() {
        if (showTrails)
        {
            lineRenderer.enabled = true;
            numberOfForces = forceVectorList.Count;
            lineRenderer.positionCount = numberOfForces * 2;
            int i = 0;
            foreach (Vector3 forceVector in forceVectorList)
            {
                lineRenderer.SetPosition(i, Vector3.zero);
                lineRenderer.SetPosition(i + 1, -forceVector);
                i = i + 2;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
