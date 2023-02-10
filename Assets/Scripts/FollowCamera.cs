using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class FollowCamera : NetworkBehaviour
{
    private Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
         cameraTransform = gameObject.transform;   
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTransform.position;
        transform.rotation = cameraTransform.rotation;
    }
}
