using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : TreeMember
{
    public BoidSettings settings;

    private Vector3 velocity;

    // cache
    private Transform cacheTransform;

    private Vector3 colDir;

    public void Initialize()
    {
        cacheTransform = transform;
        velocity = ((settings.maxSpeed + settings.minSpeed) / 2) * transform.forward;
        this.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0.669f, 0.833f, 0.9f, 1f, 0.9f, 1f);
    }

    public void UpdateBoid(Vector3[] sphereDirs, BoidManager.BoidData data)
    {
        // List<TreeMember> flock = container.GetSurroundingItems();

        Vector3 acceleration = Vector3.zero;

        // Vector3 sumPos = Vector3.zero;
        // int numPercieved = 0;

        // Vector3 sumHeading = Vector3.zero;

        // Vector3 sumAvoidance = Vector3.zero;

        // for (int i = 0; i < flock.Count; i++) {
        //     Vector3 posOffset = flock[i].transform.position - cacheTransform.position;

        //     if ((posOffset).sqrMagnitude < settings.perceptionRadius * settings.perceptionRadius) {
                
        //         if ((posOffset).sqrMagnitude < settings.avoidanceRadius * settings.avoidanceRadius) {
        //             sumAvoidance -= posOffset.normalized;
        //         }

        //         sumHeading += flock[i].transform.forward;

        //         sumPos += flock[i].transform.position;
        //         numPercieved++;
        //     }
        // }

        colDir = transform.forward;
        if (DetectCollision()) {
            colDir = ClearPathDir(sphereDirs);
            acceleration += SteerTowards(colDir) * settings.collisionAvoidanceWeight;
        }

        if (data.numPercieved > 0) {
            Vector3 center = data.sumPos / data.numPercieved;

            acceleration += SteerTowards(center - cacheTransform.position) * settings.cohesionWeight;
            acceleration += SteerTowards(data.sumHeading) * settings.alignWeight;
            acceleration += SteerTowards(data.sumAvoidance) * settings.avoidanceWeight;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cacheTransform.position += velocity * Time.deltaTime;
        cacheTransform.forward = dir;

        transform.position = cacheTransform.position;
        transform.forward = dir;

        UpdateNode();
    }

    private Vector3 SteerTowards(Vector3 v) {
        return Vector3.ClampMagnitude(v.normalized * settings.maxSpeed - velocity, settings.maxSteerForce);
    }

    private bool DetectCollision() {
        return Physics.Raycast(cacheTransform.position, cacheTransform.forward, settings.collisionAvoidanceRadius, settings.collisionMask);
        // RaycastHit hit;
        // return Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, settings.collisionAvoidanceRadius, settings.collisionMask);
    }

    private Vector3 ClearPathDir(Vector3[] sphereDirs) {
        RaycastHit hit;
        for (int i = 0; i < sphereDirs.Length; i++) {
            Vector3 worldDir = cacheTransform.TransformDirection(sphereDirs[i]);
            if (!Physics.Raycast(cacheTransform.position, worldDir, settings.collisionAvoidanceRadius, settings.collisionMask)) {
                return worldDir;
            }

            // if (!Physics.SphereCast(transform.position, 0.5f, worldDir, 
            // out hit, settings.collisionAvoidanceRadius, settings.collisionMask)) {
            //     return worldDir;
            // }
        }
        return transform.forward;
    }

    // void OnDrawGizmos() {
    //     // Gizmos.color = Color.green;
    //     // Gizmos.DrawWireSphere(transform.position, settings.perceptionRadius);

    //     // Gizmos.color = Color.red;
    //     // Gizmos.DrawWireSphere(transform.position, settings.avoidanceRadius);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, colDir * settings.collisionAvoidanceRadius);
    // }
}
