using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree {

    private Node root;
    
    public Octree(Vector3 center, float radius, OctreeSettings settings, IList<TreeMember> collection) {
        root = new Node(center, radius, null, settings, 1);
        root.Insert(collection);
    }
}

public class Node {
    
    public Vector3 center {get;}
    public float radius {get;}
    public int level{get;}

    private Node parent;
    private Node[] children;

    private TreeMember[] objects;
    private int contains;
    
    private OctreeSettings settings;

    public Node(Vector3 center, float radius, Node parent, OctreeSettings settings, int lvl) {
        this.center = center;
        this.radius = radius;

        this.parent = parent;

        this.contains = 0;

        this.settings = settings;
        this.level = lvl;
    }

    public void Insert(IList<TreeMember> collection) {
        if (collection.Count + contains > settings.maxItemNum) {

            if (children is null) {
                children = new Node[8];
            }

            List<TreeMember>[] partitions = new List<TreeMember>[8];
            for (int i = 0; i < partitions.Length; i++) {
                partitions[i] = new List<TreeMember>();
            }

            if (objects != null) {
                SortIntoPartition(objects, ref partitions);
            }
            SortIntoPartition(collection, ref partitions);

            for (int i = 0; i < partitions.Length; i++) {
                if (children[i] is null) {
                    CreateChildren(i);
                }
                children[i].Insert(partitions[i]);
            }

            objects = null;
            contains = 0;
        } else {
            if (objects is null) {
                objects = new TreeMember[settings.maxItemNum];
            }

            for (int i = 0; i < collection.Count; i++, contains++) {
                objects[contains] = collection[i];
                objects[contains].SetNode(this);
            }
        }
    }

    private void SortIntoPartition(IList<TreeMember> objects, ref List<TreeMember>[] partitions) {
        for (int i = 0; i < objects.Count; i++) {
            float x = objects[i].transform.position.x;
            float y = objects[i].transform.position.y;
            float z = objects[i].transform.position.z;

            if (x > center.x && y > center.y && z > center.z) {
                partitions[0].Add(objects[i]);
            } else if ( x < center.x && y > center.y && z > center.z) {
                partitions[1].Add(objects[i]);
            } else if ( x > center.x && y < center.y && z > center.z) {
                partitions[2].Add(objects[i]);
            } else if ( x < center.x && y < center.y && z > center.z) {
                partitions[3].Add(objects[i]);
            } else if ( x > center.x && y > center.y && z < center.z) {
                partitions[4].Add(objects[i]);
            } else if ( x < center.x && y > center.y && z < center.z) {
                partitions[5].Add(objects[i]);
            } else if ( x > center.x && y < center.y && z < center.z) {
                partitions[6].Add(objects[i]);
            } else if ( x < center.x && y < center.y && z < center.z) {
                partitions[7].Add(objects[i]);
            }
        }
    }

    private void CreateChildren(int i) {
        switch (i) {
            case 0:
                if (children[i] is null) {
                    children[0] = new Node(center + new Vector3(radius / 2, radius / 2, radius / 2), radius / 2, this, settings, level + 1);     // +x +y +z
                }
                break;
            case 1:
                if (children[i] is null) {
                    children[1] = new Node(center + new Vector3(-radius / 2, radius / 2, radius / 2), radius / 2, this, settings, level + 1);    // -x +y +z
                }
                break;
            case 2:
                if (children[i] is null) {
                    children[2] = new Node(center + new Vector3(radius / 2, -radius / 2, radius / 2), radius / 2, this, settings, level + 1);    // +x -y +z
                }
                break;
            case 3:
                if (children[i] is null) {
                    children[3] = new Node(center + new Vector3(-radius / 2, -radius / 2, radius / 2), radius / 2, this, settings, level + 1);   // -x -y +z
                }
                break;
            case 4:
                if (children[i] is null) {
                    children[4] = new Node(center + new Vector3(radius / 2, radius / 2, -radius / 2), radius / 2, this, settings, level + 1);    // +x +y -z
                }
                break;
            case 5:
                if (children[i] is null) {
                    children[5] = new Node(center + new Vector3(-radius / 2, radius / 2, -radius / 2), radius / 2, this, settings, level + 1);   // -x +y -z
                }
                break;
            case 6:
                if (children[i] is null) {
                    children[6] = new Node(center + new Vector3(radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings, level + 1);   // +x -y -z
                }
                break;
            case 7:
                if (children[i] is null) {
                    children[7] = new Node(center + new Vector3(-radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings, level + 1);  // -x -y -z
                }
                break;
        }
    }
}
