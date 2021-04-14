using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreeMember : MonoBehaviour
{
    private Node container;

    public void SetNode(Node node) {
        container = node;
    }

    protected void UpdateNode() {
        container.CheckNode(this);
    }

    void OnDrawGizmos() {
        if (container != null) {
            Gizmos.color = new Color(0, 0, 1, (1 - (1 / container.level)) * 0.25f);
            Gizmos.DrawCube(container.center, new Vector3(container.radius * 2, container.radius * 2, container.radius * 2));
        }
    }
}
