using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterMove : MonoBehaviour
{
    GameObject characterSelf;
    Transform characterTrSelf;
    Animator animatorSelf;
    Status charStatSelf;
    BoxCollider2D attackRange;

    public int charNumberSelf;

    int direc;
    int type;

    // Start is called before the first frame update
    void Start()
    {
        characterSelf = this.gameObject;
        characterTrSelf = GetComponent<Transform>();
        animatorSelf = GetComponent<Animator>();
        attackRange = GetComponent<BoxCollider2D>();
        charStatSelf = Resources.Load<Status>("char/" + charNumberSelf.ToString() + "/stat");
        if (characterTrSelf.rotation.y == 0)
        {
            direc = -1;
            characterSelf.tag = "our";
        }
        else
        {
            direc = 1;
            characterSelf.tag = "enemy";
        }
        attackRange.offset = new Vector2(-(charStatSelf.rng[0] + charStatSelf.rng[1]) / 400, 0);
        attackRange.size = new Vector2((charStatSelf.rng[1] - charStatSelf.rng[0]) / 200, 0.5f);
        animatorSelf.SetInteger("type", 1);
        type = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //type 0(wait), 1(move), 2(attack), 3(knockback)
        if (type == 1)
        {
            characterTrSelf.position += Vector3.right * direc * charStatSelf.spd * Time.deltaTime;
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
