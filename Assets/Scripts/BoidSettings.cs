using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float minSpeed = 5f;
    public float maxSpeed = 10f;

    public float perceptionRadius = 3f;
    public float avoidanceRadius = 1f;

    public float collisionAvoidanceRadius = 3f;
    public LayerMask collisionMask;

    public float maxSteerForce = 3f;

    public float avoidanceWeight = 1;
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float collisionAvoidanceWeight = 1;
}
