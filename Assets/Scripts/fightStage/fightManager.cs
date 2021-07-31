using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class fightManager : MonoBehaviour
{
    Transform camTr;
    Vector2 firstTouch;

    float camAcceleration;
    float money;

    int stageLength;
    int[] reinforceLevel; // walletReinforceLevel 0, productReinforceLevel 1
    int[] CH; //OCH0, MOCH1, ECH2, MECH3

    bool IsUIClicked;

    public Image[] reinforceUI; // w0, p1
    public Sprite[] reinforceSprites; // wOn 0, wOff 1, pOn 2, pOff 3
    public Image[] reinforceImageUI; // w0, w1, p2, p3
    public Text[] reinforceTextUI; // w0, w1, p2, p3
    public int[] IsAbleReinforce; // w0, p1
    Color[] colors; //black0, gray1

    public Text[] textUI;
    //money0, left1, right2, stageName3, OCH4, ECH5
    //WRFC6, PRFC7

    List<Character> entity;

    public List<int[]> charList;
    public List<int[]> charStat;
    public List<int[]> charAddAbility;
    int charLen;

    public List<int[]> enemyStat;
    public List<int[]> enemyAddAbility;
    int enemyLen;

    // Start is called before the first frame update
    void Start()
    {
        entity = new List<Character>();

        dataBase saveData = GameObject.Find("DataSaver").GetComponent<dataBase>();

        charList = saveData.teamList;
        charLen = charList.ToArray().Length;
        charStat = new List<int[]>();
        charAddAbility = new List<int[]>();
        for (int i = 0; i < charLen; i++)
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
            성.공격범위5, 공격쿨탐6, 선딜7, 가격8, 재생산9, 베리어10

            숨기횟수0, 숨을시간1, 소생횟수2, 소생체력3, 소생대기시간4, 
            워프거리5, 워프확률6, 속성초뎀7, 속성맷집8, 파동레벨9,
            파동확률10, 열파레벨11, 열파확률12, 밀치기확률13, 크리확률14, 
            생존확률15, 생존횟수16, 공증가량17, 공증가발동체력18
            */
            charAddAbility.Add(stat.addAbility);
            GameObject.Find("icon ("+i.ToString()+")").GetComponent<Image>().sprite = Resources.Load<Sprite>("char/" + charList[i][0].ToString() + "/si");
        }
        int stageNum = saveData.selectedStageNumber;
        Stage stageData = Resources.Load<Stage>("StageData/" + stageNum.ToString());
        stageLength = stageData.stageLength;

        GameObject bg = GameObject.Find("stage_backGround").gameObject;
        bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/bg/stageBg (" + stageNum.ToString() + ")");
        for (int i = 1; i < (int)(stageLength / 17.78f) + 2; i++)
        {
            Instantiate(bg, Vector3.left * 17.78f * i, Quaternion.identity);
        }

        GameObject.Find("enemyCastle").transform.position = new Vector2(5 - stageData.stageLength, -0.6f);

        enemyLen = stageData.stageCharList.Length;
        enemyStat = new List<int[]>();
        enemyAddAbility = new List<int[]>();
        for (int i = 0; i < enemyLen; i++)
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

        money = 0;
        reinforceLevel = new int[2] { 1, 1 };
        CH = new int[4] {
            2500*(stageNum + 1), 2500*(stageNum + 1), 6250*(stageNum + 1), 6250*(stageNum + 1)
        };

        textUI[0].text = ((int)money).ToString() + "/" + (reinforceLevel[0] * 1000 + 2500).ToString();
        textUI[1].text = "1";
        textUI[2].text = "1";
        textUI[3].text = saveData.stageNames[stageNum];
        textUI[4].text = CH[0].ToString() + "/" + CH[1].ToString();
        textUI[5].text = CH[2].ToString() + "/" + CH[3].ToString();
        textUI[6].text = "700"; //walletReinforce * 200 + 500
        textUI[7].text = "650"; //productReinforce * 400 + 250

        colors = new Color[2]
        {
            new Color(126, 126, 126), new Color(255, 255, 255)
        };
        IsAbleReinforce = new int[2]
        {
            0, 0
        };
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
                    camAcceleration += 2.5f * (firstTouch.x - currentTouch.x) / Mathf.Abs(firstTouch.x - currentTouch.x);
                }
            }
        }
        if (entity.Count > 0)
        {
            int delete = 0;
            for (int i = 0; i < entity.Count; i++)
            {
                Character e = entity[i - delete];
                if (e.hp > 0)
                {
                    if (e.motion <= 1)
                    {
                        //공격 판정
                        bool IsTarget = false;
                        for (int j = 0; j < entity.Count; j++)
                        {
                            if (!IsTarget)
                            {
                                if (e.tag + entity[j].tag == 1)
                                {
                                    if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= entity[j].tr.position.x && entity[j].tr.position.x <= e.tr.position.x + e.e_range[1]) || (e.tag == 0 && e.tr.position.x - e.e_range[1] <= entity[j].tr.position.x && entity[j].tr.position.x <= e.tr.position.x - e.e_range[0]))
                                    {
                                        IsTarget = true;
                                    }
                                }
                            }
                        }
                        if (!IsTarget)
                        {
                            if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= 5 && 5 <= e.tr.position.x + e.e_range[1]) || (e.tag == 0 && e.tr.position.x - e.e_range[1] <= 5 - stageLength && 5 - stageLength <= e.tr.position.x - e.e_range[0]))
                            {
                                IsTarget = true;
                            }
                        }
                        if (e.motion == 0) //wait
                        {
                            if (!IsTarget)
                            {
                                e.anim.SetInteger("type", 1);
                                e.motion = 1;
                            }
                            else if (e.time >= e.atkCoolTime)
                            {
                                e.anim.SetInteger("type", 2);
                                e.motion = 2;
                                e.time = 0;
                            }
                            e.time += Time.deltaTime;
                        }
                        else if (e.motion == 1) //move
                        {
                            e.tr.position += Vector3.right * (2 * e.tag - 1) * e.spd;
                        }
                    }
                    else if (e.motion == 2) //attack
                    {
                        if (e.time >= e.delayTime)
                        {
                            bool IsTarget = false;
                            for (int j = 0; j < entity.Count; j++)
                            {
                                if (!IsTarget)
                                {
                                    if (e.tag + entity[j].tag == 1)
                                    {
                                        if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= entity[j].tr.position.x && entity[j].tr.position.x <= e.tr.position.x + e.e_range[1]) || (e.tag == 0 && e.tr.position.x - e.e_range[1] <= entity[j].tr.position.x && entity[j].tr.position.x <= e.tr.position.x - e.e_range[0]))
                                        {
                                            entity[j].hp -= e.atk;
                                            if (e.IsObj)
                                            {
                                                IsTarget = true;
                                            }
                                        }
                                    }
                                    if (!IsTarget)
                                    {
                                        if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= 5 && 5 <= e.tr.position.x + e.e_range[1]) || (e.tag == 0 && e.tr.position.x - e.e_range[1] <= 5 - stageLength && 5 - stageLength <= e.tr.position.x - e.e_range[0]))
                                        {
                                            CH[2 - 2 * e.tag] -= e.atk;
                                        }
                                    }
                                }
                            }
                            e.motion = 0;
                            e.time = 0;
                        }
                        e.time += Time.deltaTime;
                    }
                }
                else
                {

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
            camAcceleration -= 0.5f * Mathf.Abs(camAcceleration) / camAcceleration;
        }
        if (CH[0].ToString() + "/" + CH[1].ToString() != textUI[4].text)
        {
            textUI[4].text = CH[0].ToString() + "/" + CH[1].ToString();
        }
        if (CH[2].ToString() + "/" + CH[3].ToString() != textUI[5].text)
        {
            textUI[5].text = CH[2].ToString() + "/" + CH[3].ToString();
        }
        if (money < reinforceLevel[0] * 2000 + 2500)
        {
            money += 25 * reinforceLevel[0] * Time.deltaTime;
        }
        else
        {
            money = reinforceLevel[0] * 2000 + 2500;
        }
        for (int i = 0; i < 2; i++)
        {
            if (money >= reinforceLevel[i] * 200 * (i + 1) + 500 / (i + 1))
            {
                if (IsAbleReinforce[i] == 0)
                {
                    reinforceUI[i].sprite = reinforceSprites[2 * i];
                    reinforceImageUI[2 * i].color = colors[1];
                    reinforceImageUI[2 * i + 1].color = colors[1];
                    reinforceTextUI[2 * i].color = colors[1];
                    reinforceTextUI[2 * i + 1].color = colors[1];

                    IsAbleReinforce[i] = 1;
                }
            }
            else
            {
                if (IsAbleReinforce[i] == 1)
                {
                    reinforceUI[i].sprite = reinforceSprites[2 * i + 1];
                    reinforceImageUI[2 * i].color = colors[0];
                    reinforceImageUI[2 * i + 1].color = colors[0];
                    reinforceTextUI[2 * i].color = colors[0];
                    reinforceTextUI[2 * i + 1].color = colors[0];

                    IsAbleReinforce[i] = 0;
                }
            }
        }
        textUI[0].text = ((int)money).ToString() + "/" + (reinforceLevel[0] * 1000 + 2500).ToString();
    }
    public void IsReinforceButtonClicked(int var)
    {
        if (money >= reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1))
        {
            money -= reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1);
            reinforceLevel[var] += 1;
            textUI[var + 1].text = reinforceLevel[var].ToString();
            textUI[var + 6].text = (reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1)).ToString();
        }
    }
}

public class Character
{
    public GameObject self;
    public Transform tr;
    public Animator anim;
    public float[] e_range;
    public int[] addAbility;
    public float spd;
    public float time;
    public float atkCoolTime;
    public float delayTime;
    public float c_range;
    public int motion;
    //0wait, 1move, 2attack, 3knockback, 4die, 5burrow, 6emerge
    public int tag;
    public int hp;
    public int atk;
    public int barrier;
    public bool IsObj;
}