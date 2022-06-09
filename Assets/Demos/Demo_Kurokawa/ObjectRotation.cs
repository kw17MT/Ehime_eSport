using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用感確認のテストオブジェクト用
public class ObjectRotation : MonoBehaviour
{
    Rigidbody rb = null;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    public void Rotate(string dir)
    {
        //どちらにフリックされたかでオブジェクトをその方向に回す
        switch (dir)
        {
            case "right":
                rb.angularVelocity = new Vector3(0.0f, -1.0f, 0.0f);
                break;
            case "left":
                rb.angularVelocity = new Vector3(0.0f, 1.0f, 0.0f);
                break;
            //タッチで止める
            case "touch":
                rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            default:
                break;
        }
    }
}