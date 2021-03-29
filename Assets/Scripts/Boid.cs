using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings settings;

    private Vector3 velocity;

    // cache
    private Transform cacheTransform;

    public void Initialize()
    {
        cacheTransform = transform;
        velocity = settings.maxSpeed * transform.forward;
        this.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }

    public void UpdateBoid(Boid[] flock, Vector3[] sphereDirs)
    {
        Vector3 acceleration = Vector3.zero;

        Vector3 sumPos = Vector3.zero;
        int numPercieved = 0;

        Vector3 sumHeading = Vector3.zero;

        Vector3 sumAvoidance = Vector3.zero;

        for (int i = 0; i < flock.Length; i++) {
            Vector3 posOffset = flock[i].transform.position - transform.position;

            if ((posOffset).sqrMagnitude < settings.perceptionRadius * settings.perceptionRadius) {
                
                if ((posOffset).sqrMagnitude < settings.avoidanceRadius * settings.avoidanceRadius) {
                    sumAvoidance -= posOffset.normalized;
                }

                sumHeading += flock[i].transform.forward;

                sumPos += flock[i].transform.position;
                numPercieved++;
            }
        }

        if (DetectCollision()) {
            // Debug.Log("collide");
            acceleration += SteerTowards(ClearPathDir(sphereDirs)) * settings.collisionAvoidanceWeight;
        }

        acceleration += SteerTowards(sumPos/numPercieved) * settings.cohesionWeight;
        acceleration += SteerTowards(sumHeading) * settings.alignWeight;
        acceleration += SteerTowards(sumAvoidance) * settings.avoidanceWeight;

        // Debug.Log("Acceleration " + acceleration);

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cacheTransform.position += velocity * Time.deltaTime;
        cacheTransform.forward = dir;

        // TODO: collision avoidance!

        // if (cacheTransform.position.x < -15 || cacheTransform.position.x > 15) {
        //     cacheTransform.position = new Vector3(-Mathf.Clamp(cacheTransform.position.x, -15, 15), cacheTransform.position.y, cacheTransform.position.z);
        // }

        // if (cacheTransform.position.y < -15 || cacheTransform.position.y > 15) {
        //     cacheTransform.position = new Vector3(cacheTransform.position.x, -Mathf.Clamp(cacheTransform.position.y, -15, 15), cacheTransform.position.z);
        // }

        // if (cacheTransform.position.z < -15 || cacheTransform.position.z > 15) {
        //     cacheTransform.position = new Vector3(cacheTransform.position.x, cacheTransform.position.y, -Mathf.Clamp(cacheTransform.position.z, -15, 15));
        // }

        transform.position = cacheTransform.position;
        transform.forward = dir;
    }

    private Vector3 SteerTowards(Vector3 v) {
        return Vector3.ClampMagnitude(v.normalized * settings.maxSpeed - velocity, settings.maxSteerForce);
    }

    private bool DetectCollision() {
        // return Physics.Raycast(transform.position, transform.forward, settings.collisionAvoidanceRadius, settings.collisionMask);
        RaycastHit hit;
        return Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, settings.collisionAvoidanceRadius, settings.collisionMask);
    }

    private Vector3 ClearPathDir(Vector3[] sphereDirs) {
        RaycastHit hit;
        for (int i = 0; i < sphereDirs.Length; i++) {
            Vector3 worldDir = cacheTransform.TransformDirection(sphereDirs[i]);
            // if (!Physics.Raycast(transform.position, worldDir, settings.collisionAvoidanceRadius, settings.collisionMask)) {
                // return worldDir;
            // }

            if (!Physics.SphereCast(transform.position, 0.5f, worldDir, 
            out hit, settings.collisionAvoidanceRadius, settings.collisionMask)) {
                return worldDir;
            }
        }
        return transform.forward;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, settings.perceptionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.avoidanceRadius);
    }
}
