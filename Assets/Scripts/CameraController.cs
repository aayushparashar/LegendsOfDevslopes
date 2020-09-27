using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothing = 5f;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetCamPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPosition, smoothing * Time.deltaTime);
        
    }
}
