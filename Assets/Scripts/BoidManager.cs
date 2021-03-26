using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    private Boid[] flock;

    // Start is called before the first frame update
    void Start()
    {
        flock = FindObjectsOfType<Boid>();
        for (int i = 0; i < flock.Length; i++) {
            flock[i].Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < flock.Length; i++) {
            flock[i].UpdateBoid(flock);
        }
    }
}
