using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charIconMove : MonoBehaviour
{
    Transform tr;
    public float iconX;
    float deltaIconX;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        iconX = tr.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (tr.position.y == -2)
        {
            deltaIconX = GameObject.Find("GameObject").GetComponent<charSettingManager>().iconX;
            tr.position = new Vector2(iconX + deltaIconX, tr.position.y);
        }
        if (GameObject.Find("GameObject").GetComponent<charSettingManager>().IsArray)
        {
            Destroy(this.gameObject);
        }
    }
}
