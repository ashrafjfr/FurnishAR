using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private float rotateX = -90.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(rotateX, 0.0f, 0.0f);
    }
}
