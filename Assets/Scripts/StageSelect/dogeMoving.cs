using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dogeMoving : MonoBehaviour
{
    Transform mainCameraTr;
    Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        mainCameraTr = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        tr.position += (new Vector3(mainCameraTr.position.x, mainCameraTr.position.y + 0.4f, 0) - tr.position) * Time.deltaTime * 2;
    }
}
