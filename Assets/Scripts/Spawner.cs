using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    public int numBoids = 10;
    public float spawnRadius = 10f;

    public Boid boid;

    // Spawn
    void Awake()
    {
        for (int i = 0; i < numBoids; i++) {
            Boid b = Object.Instantiate(boid, transform);
            b.transform.position = transform.position + (Random.insideUnitSphere * spawnRadius);;
            b.transform.forward = Random.insideUnitSphere;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(30, 30, 30));
    }
}
