using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charPowUp_IconBehaviour : MonoBehaviour
{
    Transform tr;
    float iconX;
    public int charNumber;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        iconX = tr.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaIconX = GameObject.Find("GameObject").GetComponent<charPowUpManager>().iconX;
        tr.position = new Vector2(iconX + deltaIconX, tr.position.y);
    }
}
