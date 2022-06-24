using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    [SerializeField]
    float m_rotateSpeed = 50.0f;
    [SerializeField]
    bool m_enableRotate = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();


        return;
    }

    void Rotate()
    {
        if (m_enableRotate != true)
        {
            return;
        }

        this.transform.Rotate(new Vector3(0.0f, m_rotateSpeed * Time.deltaTime, 0.0f));

        return;
    }
}
