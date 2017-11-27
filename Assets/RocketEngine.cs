using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsEngine))]
public class RocketEngine : MonoBehaviour {

    public float fuelMass;             // [kg]
    public float maxThrust;            // kN [kg*m*s^-2]

    [Range(0, 1f)]
    public float thrustPercent;        // [none]
    public Vector3 thrustUnitVector;   // [none] -- a direction 

    private PhysicsEngine physicsEngine;
    private float currentThrust;       // N
    private float netMassInitial;
    private float netMassFinal;

    void Start()
    {
        physicsEngine = gameObject.GetComponent<PhysicsEngine>();
        netMassFinal = physicsEngine.mass;
        physicsEngine.mass += fuelMass;
        netMassInitial = physicsEngine.mass;
    }

    void FixedUpdate()
    {
        float fuelMassThisUpdate = FuelThisUpdate();
        if (fuelMass >= fuelMassThisUpdate)
        {
            fuelMass -= fuelMassThisUpdate;
            physicsEngine.mass -= fuelMassThisUpdate;
            ExertForce();
        }
        else {
            Debug.LogWarning("Out of rocket fuel.");
        }
    }

    float FuelThisUpdate() {
        float exhaustMassFlow;                     // [kg s^-1]
        float effectiveExhaustVelocity;            // [m s^-1]

        effectiveExhaustVelocity = 4464f;          // [m s^-1] for a liquid Oxygen/Liquid Hydrogen Engine
        exhaustMassFlow = currentThrust / effectiveExhaustVelocity; 
        return exhaustMassFlow * Time.deltaTime;   // [kg]
    }

    void ExertForce() {
        currentThrust = thrustPercent * maxThrust * 1000f;
        Vector3 thrustVector = thrustUnitVector.normalized * currentThrust; // N
        physicsEngine.AddForce(thrustVector);
    }
}
