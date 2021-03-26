using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float maxSpeed = 5f;

    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = maxSpeed * transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;

        if (transform.position.x < -15 || transform.position.x > 15) {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        }

        if (transform.position.y < -15 || transform.position.y > 15) {
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        }

        if (transform.position.z < -15 || transform.position.z > 15) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
        }
    }
}
