using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ïù
        float width = gameObject.GetComponent<Renderer>().bounds.size.x;
        print("width: " + width);

        // çÇÇ≥
        float height = gameObject.GetComponent<Renderer>().bounds.size.y;
        print("height: " + height);
    }
}
