using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    Rigidbody rb = null;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate(string dir)
    {
        switch (dir)
        {
            case "right":
                rb.angularVelocity = new Vector3(0.0f, -1.0f, 0.0f);
                break;
            case "left":
                rb.angularVelocity = new Vector3(0.0f, 1.0f, 0.0f);
                break;
            case "touch":
                rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            default:
                break;
        }
    }
}