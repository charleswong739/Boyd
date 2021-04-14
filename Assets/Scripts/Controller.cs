using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Octree ot;
    public OctreeSettings settings;

    public TreeMember thing;

    void Awake() {
        TreeMember[] o = new TreeMember[50];

        for (int i = 0; i < o.Length; i++) {
            o[i] = Object.Instantiate(thing, transform);
            o[i].transform.position = transform.position + (Random.insideUnitSphere * 30);
        }

        ot = new Octree(transform.position, 30, settings, o);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(60, 60, 60));
    }
}
