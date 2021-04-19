using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree {

    private Node root;
    
    public Octree(Vector3 center, float radius, OctreeSettings settings, IList<TreeMember> collection) {
        root = new Node(center, radius, null, settings, 1);
        root.BulkInsert(collection);
    }

    public void Draw() {
        root.Draw();
    }
}

public class Node {
    
    private Vector3 center {get;}
    private float radius {get;}
    private int level{get;}

    private Node parent;
    private Node[] children;

    private List<TreeMember> objects;
    private int contains {get; set;}
    
    private OctreeSettings settings;

    public Node(Vector3 center, float radius, Node parent, OctreeSettings settings, int lvl) {
        this.center = center;
        this.radius = radius;

        this.parent = parent;

        this.contains = 0;

        this.settings = settings;
        this.level = lvl;
    }

    public void BulkInsert(IList<TreeMember> collection) {
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
                if (children[i] is null && partitions[i].Count > 0) {
                    CreateChild(i);
                    children[i].BulkInsert(partitions[i]);
                }
            }

            objects = null;
            contains = 0;
        } else {
            if (objects is null) {
                objects = new List<TreeMember>(settings.maxItemNum + 1);
            }

            for (int i = 0; i < collection.Count; i++, contains++) {
                objects.Add(collection[i]);
                collection[i].SetNode(this);
            }
        }
    }

    public bool InBounds(Vector3 pos) {
        return pos.x > center.x - radius && pos.x < center.x + radius && pos.y > center.y - radius && pos.y < center.y + radius && pos.z > center.z - radius && pos.z < center.z + radius;
    }

    public void CheckNode(TreeMember item) {
        if (!InBounds(item.transform.position)) {
            objects.Remove(item);
            contains--;
            MoveItem(item);
            CheckMerge();
        }
    }

    public List<TreeMember> GetSurroundingItems() {
        if (parent != null) {
            return parent.GetSubTrees();
        } else {
            return objects;
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

    private void MoveItem(TreeMember item) {
        if (parent != null) {
            if (parent.InBounds(item.transform.position)) {
                parent.Insert(item);
            } else {
                parent.MoveItem(item);
            }
        } else {
            item.SetNode(null);
        }
    }

    private int SortIntoPartition(TreeMember item) {
        float x = item.transform.position.x;
        float y = item.transform.position.y;
        float z = item.transform.position.z;

        if (x > center.x && y > center.y && z > center.z) {
            return 0;
        } else if ( x < center.x && y > center.y && z > center.z) {
            return 1;
        } else if ( x > center.x && y < center.y && z > center.z) {
            return 2;
        } else if ( x < center.x && y < center.y && z > center.z) {
            return 3;
        } else if ( x > center.x && y > center.y && z < center.z) {
            return 4;
        } else if ( x < center.x && y > center.y && z < center.z) {
            return 5;
        } else if ( x > center.x && y < center.y && z < center.z) {
            return 6;
        } else if ( x < center.x && y < center.y && z < center.z) {
            return 7;
        }

        return 0;
    }

    private void Insert(TreeMember item) {
        if (children != null) {
            int partNum = SortIntoPartition(item);
            if (children[partNum] is null) {
                CreateChild(partNum);
            }
            children[partNum].Insert(item);
        } else {
            BulkInsert(new TreeMember[] {item});
        }
    }

    private void MergeSubTrees() {
        objects = new List<TreeMember>(GetSubTrees());
        contains = objects.Count;
        children = null;

        for (int i = 0; i < contains; i++) {
            objects[i].SetNode(this);
        }
    }

    private List<TreeMember> GetSubTrees() {
        if (children is null) {
            return objects;
        } else{
            List<TreeMember> childObjects = new List<TreeMember>();
            for (int i = 0; i < 8; i++) {
                if (children[i] != null) {
                    childObjects.AddRange(children[i].GetSubTrees());
                }
            }
            return childObjects;
        }
    }

    private void CheckMerge() {
        if (parent != null && parent.GetCount() < settings.minItemNum) {
            parent.CheckMerge();
        } else if (children != null) {
            MergeSubTrees();
        }
    }

    private int GetCount() {
        if (children is null) {
            return contains;
        } else{
            int count = 0;
            for (int i = 0; i < 8; i++) {
                if (children[i] != null) {
                    int diff = children[i].GetCount();

                    if (diff == 0) {
                        children[i] = null;
                    }

                    count += diff;
                }
            }
            return count;
        }
    }

    private void CreateChild(int i) {
        switch (i) {
            case 0:
                children[0] = new Node(center + new Vector3(radius / 2, radius / 2, radius / 2), radius / 2, this, settings, level + 1);     // +x +y +z
                break;
            case 1:
                children[1] = new Node(center + new Vector3(-radius / 2, radius / 2, radius / 2), radius / 2, this, settings, level + 1);    // -x +y +z
                break;
            case 2:
                children[2] = new Node(center + new Vector3(radius / 2, -radius / 2, radius / 2), radius / 2, this, settings, level + 1);    // +x -y +z
                break;
            case 3:
                children[3] = new Node(center + new Vector3(-radius / 2, -radius / 2, radius / 2), radius / 2, this, settings, level + 1);   // -x -y +z
                break;
            case 4:
                children[4] = new Node(center + new Vector3(radius / 2, radius / 2, -radius / 2), radius / 2, this, settings, level + 1);    // +x +y -z
                break;
            case 5:
                children[5] = new Node(center + new Vector3(-radius / 2, radius / 2, -radius / 2), radius / 2, this, settings, level + 1);   // -x +y -z
                break;
            case 6:
                children[6] = new Node(center + new Vector3(radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings, level + 1);   // +x -y -z
                break;
            case 7:
                children[7] = new Node(center + new Vector3(-radius / 2, -radius / 2, -radius / 2), radius / 2, this, settings, level + 1);  // -x -y -z
                break;
        }
    }

    public void Draw() {
        Gizmos.color = new Color(0, 1, 0, (1 - (1 / level)) * 0.25f);
        Gizmos.DrawCube(center, new Vector3(radius * 2, radius * 2, radius * 2));
        if (children != null) {
            for (int i = 0; i < 8; i++) {
                if (children[i] != null) {
                    children[i].Draw();
                }
            }
        }
    }
}
