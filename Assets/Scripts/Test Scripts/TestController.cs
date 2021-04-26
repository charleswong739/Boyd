using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    private Octree ot;
    public OctreeSettings settings;

    public ComputeShader compute;
    private const int numthread = 64;

    public TreeMember thing;
    private TreeMember[] o;

    void Awake() {
        o = new TreeMember[7];

        for (int i = 0; i < o.Length; i++) {
            o[i] = Object.Instantiate(thing, transform);
            o[i].id = i;
            o[i].transform.position = transform.position + (Random.insideUnitSphere * 30);
        }

        ot = new Octree(transform.position, 30, settings, o);
    }

    void Update() {
        for (int i = 0; i < o.Length; i++) {
            o[i].UpdateNode();
        }
    }

    public void RunShader() {
        // int size = 80;
        // int[] data = new int[size];
        // ComputeBuffer buffer = new ComputeBuffer(size, sizeof(int));
        // buffer.SetData(data);
        // compute.SetBuffer(0, "buffer", buffer);

        // compute.Dispatch(0, Mathf.CeilToInt(size / 64f), 1, 1);

        // buffer.GetData(data);

        // for (int i = 0; i < data.Length; i++) {
        //     if (data[i] != 1) {
        //         Debug.Log("Index " + i + " failed");
        //     } else {
        //         Debug.Log("Index " + i + " passed");
        //     }
        // }
        // Debug.Log(ts(data));

        // buffer.Release();
        List<int> nodeIndexData;
        int[] memberIndexData;

        ot.BuildBuffer(out nodeIndexData, out memberIndexData, o.Length);

        Debug.Log("Node Index: " + ts(nodeIndexData));
        Debug.Log("Member Index: " + ts(memberIndexData));

        (int, int)[] stuff = new (int, int)[o.Length];

        for (int i = 0; i < o.Length; i++) {
            stuff[i] = (o[i].nodeId, o[i].id);
        }

        Debug.Log("Item Stats:" + ts(stuff));
    }

    private string ts(IList l) {
        string s = "[";
        for (int i = 0; i < l.Count; i++) {
            s += l[i].ToString() + ", ";
        }

        return s.Substring(0, s.Length - 2) + "]";
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(60, 60, 60));

        if (ot != null) {
            ot.Draw();
        }
    }
}
