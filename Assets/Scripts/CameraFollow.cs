using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform followTransform;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        this.mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float camY = followTransform.position.y;
        float camX = followTransform.position.x;
        this.transform.position = new UnityEngine.Vector3(camX, camY, transform.position.z);
    }
}
