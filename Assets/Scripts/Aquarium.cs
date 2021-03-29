using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquarium : MonoBehaviour
{
    public int aquariumRadius;

    private readonly int colliderHalfWidth = 1;

    void Awake() {
        BoxCollider pX = gameObject.AddComponent<BoxCollider>();
        pX.center = new Vector3(aquariumRadius + colliderHalfWidth, 0, 0);
        pX.size = new Vector3(colliderHalfWidth * 2, aquariumRadius * 2 + 4, aquariumRadius * 2 + 4);

        BoxCollider nX = gameObject.AddComponent<BoxCollider>();
        nX.center = new Vector3(-(aquariumRadius + colliderHalfWidth), 0, 0);
        nX.size = new Vector3(colliderHalfWidth * 2, aquariumRadius * 2 + 4, aquariumRadius * 2 + 4);

        BoxCollider pY = gameObject.AddComponent<BoxCollider>();
        pY.center = new Vector3(0, aquariumRadius + colliderHalfWidth, 0);
        pY.size = new Vector3(aquariumRadius * 2 + 4, colliderHalfWidth * 2, aquariumRadius * 2 + 4);

        BoxCollider nY = gameObject.AddComponent<BoxCollider>();
        nY.center = new Vector3(0, -(aquariumRadius + colliderHalfWidth), 0);
        nY.size = new Vector3(aquariumRadius * 2 + 4, colliderHalfWidth * 2, aquariumRadius * 2 + 4);

        BoxCollider pZ = gameObject.AddComponent<BoxCollider>();
        pZ.center = new Vector3(0, 0, aquariumRadius + colliderHalfWidth);
        pZ.size = new Vector3(aquariumRadius * 2 + 4, aquariumRadius * 2 + 4, colliderHalfWidth * 2);

        BoxCollider nZ = gameObject.AddComponent<BoxCollider>();
        nZ.center = new Vector3(0, 0, -(aquariumRadius + colliderHalfWidth));
        nZ.size = new Vector3(aquariumRadius * 2 + 4, aquariumRadius * 2 + 4, colliderHalfWidth * 2);
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position, new Vector3(30, 30, 30));
    // }
}
