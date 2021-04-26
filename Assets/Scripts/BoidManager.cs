using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    private Octree ot;
    public OctreeSettings octreeSettings;
    public float aquaRadius;

    private List<Boid> flock;
    public BoidSettings settings;

    public int collisionRayDensity = 300;
    private Vector3[] sphereDirs;

    public ComputeShader compute;

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

        ot = new Octree(transform.position, aquaRadius, octreeSettings, flock);
    }

    // Update is called once per frame
    void Update()
    {
        // Update boid ids
        for (int i = 0; i < flock.Count; i++) {
            flock[i].id = i;
        }

        // Generate nodeIndexData and memberIndexData buffers
        List<int> nodeIndexData;
        int[] memberIndexData;

        ot.BuildBuffer(out nodeIndexData, out memberIndexData, flock.Count);

        // Generate memberData buffer
        BoidData[] boidData = new BoidData[flock.Count];
        for (int i = 0; i < flock.Count; i++) {
            boidData[i].position = flock[i].transform.position;
            boidData[i].forward = flock[i].transform.forward;
            boidData[i].nodeId = flock[i].nodeId;
        }

        // Run shader
        ComputeBuffer nodeIndexBuffer = new ComputeBuffer(nodeIndexData.Count, sizeof(int)); // (count, stride)
        ComputeBuffer memberIndexBuffer = new ComputeBuffer(memberIndexData.Length, sizeof(int));
        ComputeBuffer boidBuffer = new ComputeBuffer(flock.Count, BoidData.Size());

        nodeIndexBuffer.SetData(nodeIndexData);
        memberIndexBuffer.SetData(memberIndexData);
        boidBuffer.SetData(boidData);

        compute.SetBuffer(0, "nodeIndexBuffer", nodeIndexBuffer);
        compute.SetBuffer(0, "memberIndexBuffer", memberIndexBuffer);
        compute.SetBuffer(0, "boidBuffer", boidBuffer);

        compute.SetFloat("percepRad", settings.perceptionRadius);
        compute.SetFloat("avoidRad", settings.avoidanceRadius);

        compute.Dispatch(0, Mathf.CeilToInt(flock.Count / 512f), 1, 1);

        boidBuffer.GetData(boidData);


        // Update boids and prune escaped boids
        for (int i = flock.Count - 1; i >= 0; i--) {
            flock[i].UpdateBoid(sphereDirs, boidData[i]);
            
            if (flock[i].OutOfTree()) {
                Boid b = flock[i];
                flock.RemoveAt(i);
                Object.Destroy(b.gameObject);
            }
        }

        nodeIndexBuffer.Release();
        memberIndexBuffer.Release();
        boidBuffer.Release();
    }

    void OnDrawGizmos() {
        if (ot != null) {
            ot.Draw();
        }
    }

    public struct BoidData {
        public int nodeId;

        public Vector3 position;
        public Vector3 forward;

        public Vector3 sumPos;
        public Vector3 sumHeading;
        public Vector3 sumAvoidance;

        public int numPercieved;

        public static int Size() {
            return sizeof(float) * 3 * 5 + sizeof(int) * 2;
        }
    }
}
