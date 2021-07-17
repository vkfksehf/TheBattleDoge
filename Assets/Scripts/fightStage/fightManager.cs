using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class fightManager : MonoBehaviour
{
    Transform camTr;
    Vector2 firstTouch;

    float camAcceleration;
    int selectedStageNumber;
    int stageLength;
    bool IsUIClicked;

    public List<int[]> charList;
    public List<int[]> charStat;
    public List<int[]> charAddAbility;

    public List<int[]> enemyStat;
    public List<int[]> enemyAddAbility;

    // Start is called before the first frame update
    void Start()
    {
        charList = GameObject.Find("DataSaver").GetComponent<dataBase>().charList;
        charStat = new List<int[]>();
        charAddAbility = new List<int[]>();
        for (int i = 0; i < charList.ToArray().Length; i++)
        {
            Status stat = Resources.Load<Status>("char/"+charList[i][0].ToString()+"/stat");
            charStat.Add(new int[11]
            {
                (int)((charList[i][1] / 100 + 1) * stat.hp),
                (int)((charList[i][2] / 100 + 1) * stat.atk),
                (int)((charList[i][3] / 100 + 1) * stat.spd),
                stat.rng[0], stat.rng[1], stat.rng[2],
                (int)(stat.atkCool * 100), (int)(stat.dmgTime * 100), 
                stat.barrier, stat.cost, (int)(stat.reloadTime * 100), 
            });
            /*
            체력0, 공격력1, 속도2, 객체공격범위3~4,
            성공격범위5, 공격쿨탐6, 선딜7, 베리어8, 가격9, 재생산10

            숨기횟수12, 숨을시간13, 소생횟수14, 소생체력15, 소생대기시간16, 
            워프거리17, 워프확률18, 속성초뎀19, 속성맷집20, 파동레벨21,
            파동확률22, 열파레벨23, 열파확률24, 밀치기확률25, 크리확률26, 
            생존확률27, 생존횟수28, 공증가량29, 공증가발동체력30
            */
            charAddAbility.Add(stat.addAbility);
        }
        int stageNum = GameObject.Find("DataSaver").GetComponent<dataBase>().selectedStageNumber;
        Stage stageData = Resources.Load<Stage>("StageData/" + stageNum.ToString());
        stageLength = stageData.stageLength;

        GameObject bg = GameObject.Find("stage_backGround").gameObject;
        bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/bg/stageBg (" + stageNum.ToString() + ")");
        for (int i = 1; i < (int)(stageLength / 17.78f) + 2; i++)
        {
            Instantiate(bg, Vector3.left * 17.78f * i, Quaternion.identity);
        }

        GameObject.Find("enemyCastle").transform.position = new Vector2(5 - stageData.stageLength, -0.6f);

        enemyStat = new List<int[]>();
        enemyAddAbility = new List<int[]>();
        for (int i = 0; i < stageData.stageCharList.Length; i++)
        {
            Status stat = stageData.stageCharList[i];
            enemyStat.Add(new int[9]
            {
                (int)(stat.hp * (1+stageNum/50)),
                (int)(stat.atk * (1+stageNum/50)),
                (int)(stat.spd * (1+stageNum/100)),
                stat.rng[0], stat.rng[1], stat.rng[2],
                (int)(stat.atkCool*100), (int)(stat.dmgTime*100),
                stat.barrier
            });
            enemyAddAbility.Add(stat.addAbility);
        }

        camTr = GameObject.Find("Main Camera").GetComponent<Transform>();
        IsUIClicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            camAcceleration = 0;
            IsUIClicked = EventSystem.current.IsPointerOverGameObject();
            if (!IsUIClicked)
            {
                firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (!IsUIClicked)
            {
                Vector2 currentTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Mathf.Abs(firstTouch.x - currentTouch.x) > 0.4f)
                {
                    camAcceleration += 4.5f * (firstTouch.x - currentTouch.x) / Mathf.Abs(firstTouch.x - currentTouch.x);
                }
            }
        }
        if (camAcceleration != 0)
        {
            if (camTr.position.x + camAcceleration * Time.deltaTime > 0)
            {
                camTr.position = new Vector3(0, 0, -10);
            }
            else if (camTr.position.x + camAcceleration * Time.deltaTime < -stageLength)
            {
                camTr.position = new Vector3(-stageLength, 0, -10);
            }
            else
            {
                camTr.position += Vector3.right * camAcceleration * Time.deltaTime;
            }
            camAcceleration -= Mathf.Abs(camAcceleration) / camAcceleration;
        }
    }
}
