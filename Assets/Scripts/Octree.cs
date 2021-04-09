using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree<T> {

    private Node root;
    
    public Octree(Vector3 center, float radius, OctreeSettings settings, IList<Object> collection) {
        root = new Node(center, radius, null, settings);
        root.Insert(collection);
    }
}

public class Node {
    
    private Vector3 center;
    private float radius;

    private Node parent;
    private Node[] children;

    private Object[] objects;
    private int contains;
    
    private OctreeSettings settings;

    public Node(Vector3 center, float radius, Node parent, OctreeSettings settings) {
        this.center = center;
        this.radius = radius;

        this.parent = parent;

        this.contains = 0;

        this.settings = settings;
    }

    public void Insert(IList<Object> collection) {
        if (collection.Count + contains > settings.maxItemNum) {
            CreateChildren();

            for (int i = 0; i < contains; i++) {
                // TODO: insert correct items from objects into correct node
            }

            for (int i = 0; i < collection.Count; i++) {
                // TODO: insert correct items from collection into correct node
            }

            objects = null;
            contains = 0;
        } else {
            if (objects is null) {
                objects = new Object[settings.maxItemNum];
            }

            for (int i = 0; i < collection.Count; i++, contains++) {
                objects[contains] = collection[i];
            }
        }
    }

    private void CreateChildren() {
        children = new Node[8];
        children[0] = new Node(center + new Vector3(radius / 2, radius / 2, radius / 2), radius / 2, this, settings);     // +x +y +z
        children[1] = new Node(center + new Vector3(-radius / 2, radius / 2, radius / 2), radius / 2, this, settings);    // -x +y +z
        children[2] = new Node(center + new Vector3(radius / 2, -radius / 2, radius / 2), radius / 2, this, settings);    // +x -y +z
        children[3] = new Node(center + new Vector3(-radius / 2, -radius / 2, radius / 2), radius / 2, this, settings);   // -x -y +z
        children[4] = new Node(center + new Vector3(radius / 2, radius / 2, -radius / 2), radius / 2, this, settings);    // +x +y -z
        children[5] = new Node(center + new Vector3(-radius / 2, radius / 2, -radius / 2), radius / 2, this, settings);   // -x +y -z
        children[6] = new Node(center + new Vector3(radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings);   // +x -y -z
        children[7] = new Node(center + new Vector3(-radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings);  // -x -y -z
    }
}
