using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    private List<Boid> flock;

    public int collisionRayDensity = 300;
    private Vector3[] sphereDirs;

    void Awake() {
        sphereDirs = new Vector3[collisionRayDensity];

        for (int i = 0; i < collisionRayDensity; i++) {
            float phi = Mathf.Acos(1 - ((i + 0.5f) / 150));
            float theta = Mathf.PI * i * (1 + Mathf.Sqrt(5));

            float x = Mathf.Cos(theta) * Mathf.Sin(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(phi);

            sphereDirs[i] = new Vector3(x, y, z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        flock = new List<Boid>(FindObjectsOfType<Boid>());
        for (int i = 0; i < flock.Count; i++) {
            flock[i].Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = flock.Count - 1; i >= 0; i--) {
            flock[i].UpdateBoid(flock, sphereDirs);
            Vector3 pos = flock[i].transform.position;
            if (pos.x < -150 || pos.x > 150 || pos.y < -150 || pos.y > 150) {
                Boid boyd = flock[i];
                flock.RemoveAt(i);
                Object.Destroy(boyd);
                Debug.Log("Destroyed boid #" + i);
            }
        }
    }
}
