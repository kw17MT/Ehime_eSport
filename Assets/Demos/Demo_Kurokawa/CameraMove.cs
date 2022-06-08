using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject ownPlayer = null;
    private bool isGetOwnPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGetOwnPlayer)
		{
            ownPlayer = GameObject.Find("OwnPlayer");
            if(ownPlayer != null)
			{
                isGetOwnPlayer = true;
            }
        }
      

        Vector3 cameraPos = ownPlayer.transform.position + (ownPlayer.transform.forward * -8.0f);
        cameraPos.y += 5.0f;

        Camera camera = Camera.main;
        camera.gameObject.transform.position = cameraPos;
        camera.gameObject.transform.LookAt(ownPlayer.transform);
    }
}
