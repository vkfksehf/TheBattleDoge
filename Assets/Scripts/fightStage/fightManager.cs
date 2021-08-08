using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class fightManager : MonoBehaviour
{
    public AudioClip[] soundEffects;
    public AudioSource audioSource;

    Transform camTr;
    Vector2 firstTouch;

    float camAcceleration;
    float money;
    float time;

    int stageLength;
    int[] reinforceLevel; // walletReinforceLevel 0, productReinforceLevel 1
    int[] CH; //OCH0, MOCH1, ECH2, MECH3

    bool IsUIClicked;

    float[] summonCoolTime;
    public Image[] summonIconsImage;
    public GameObject[] gaugeBar;
    public Image[] gaugeFill;
    public GameObject[] summonPrices;

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
        summonCoolTime = new float[10];
        for (int i = 0; i < charLen; i++)
        {
            Status stat = Resources.Load<Status>("char/"+charList[i][0].ToString()+"/stat");

            summonCoolTime[i] = stat.reloadTime;
            summonPrices[i].SetActive(true);
            summonPrices[i].GetComponent<Text>().text = stat.cost.ToString();

            charStat.Add(new int[14]
            {
                (int)((charList[i][1] / 100 + 1) * stat.hp),
                (int)((charList[i][2] / 100 + 1) * stat.atk),
                (int)((charList[i][3] / 100 + 1) * stat.spd),
                stat.rng[0], stat.rng[1], stat.rng[2],
                (int)(stat.atkCool * 100), (int)(stat.dmgTime * 100), 
                stat.barrier, stat.hitback, charList[i][0], stat.isObjInt,
                stat.cost, (int)(stat.reloadTime * 100)
            });
            /*
            체력0, 공격력1, 속도2, 객체공격범위3~4,
            성.공격범위5, 공격쿨탐6, 선딜7, 베리어8, 히트백9, 
            식별번호10, 단일/전체11, 가격12, 재생산13

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
            enemyStat.Add(new int[12]
            {
                (int)(stat.hp * (1+stageNum/50)),
                (int)(stat.atk * (1+stageNum/50)),
                (int)(stat.spd * (1+stageNum/100)),
                stat.rng[0], stat.rng[1], stat.rng[2],
                (int)(stat.atkCool*100), (int)(stat.dmgTime*100),
                stat.barrier, stat.hitback, 
                stageData.stageCharList[i].charNumber,
                stat.isObjInt
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

        textUI[0].text = ((int)money).ToString() + "/" + (reinforceLevel[0] * 2000 + 2500).ToString();
        textUI[1].text = "1";
        textUI[2].text = "1";
        textUI[3].text = saveData.stageNames[stageNum];
        textUI[4].text = CH[0].ToString() + "/" + CH[1].ToString();
        textUI[5].text = CH[2].ToString() + "/" + CH[3].ToString();
        textUI[6].text = "700"; //walletReinforce * 200 + 500
        textUI[7].text = "650"; //productReinforce * 400 + 250

        colors = new Color[2]
        {
            new Color(126 / 255f, 126 / 255f, 126 / 255f, 255 / 255f), new Color(255, 255, 255)
        };
        IsAbleReinforce = new int[2]
        {
            0, 0
        };

        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)(time * 100) % 1500 == 0)
        {
            Summon(charStat[0], charAddAbility[0], 1);
        }
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
                else if (firstTouch.x != currentTouch.x)
                {
                    camAcceleration = 0.5f * (firstTouch.x - currentTouch.x) / Mathf.Abs(firstTouch.x - currentTouch.x);
                }
            }
        }
        if (camAcceleration != 0)
        {
            if (camTr.position.x + camAcceleration * Time.deltaTime > 0)
            {
                camTr.position = new Vector3(0, 0, -10);
            }
            else if (camTr.position.x + camAcceleration * Time.deltaTime < 10-stageLength)
            {
                camTr.position = new Vector3(10-stageLength, 0, -10);
            }
            else
            {
                camTr.position += Vector3.right * camAcceleration * Time.deltaTime;
            }
            camAcceleration -= 0.5f * Mathf.Abs(camAcceleration) / camAcceleration;
        }

        if (entity.Count > 0)
        {
            int delete = 0;
            for (int i = 0; i < entity.Count; i++)
            {
                Character e = entity[i - delete];

                if (e.hp > 0) // live
                {
                    if (e.hp <= e.hitback * e.maxHp / e.maxHitback && e.motion != 3) // knockback distinct
                    {
                        e.anim.SetInteger("type", 3);
                        e.motion = 3;
                        e.time = e.anim.GetCurrentAnimatorStateInfo(0).length + Time.deltaTime;
                        e.hitback--;
                    }
                    else if (e.motion <= 1) // wait or move
                    {
                        // IsTarget
                        bool IsTarget = false;
                        for (int j = 0; j < entity.Count; j++)
                        {
                            if (!IsTarget)
                            {
                                if (e.tag + entity[j].tag == 1 && !(3 <= entity[j].motion && entity[j].motion <= 5))
                                {
                                    if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= entity[j].tr.position.x && 
                                        entity[j].tr.position.x <= e.tr.position.x + e.e_range[1]) || 
                                        (e.tag == 0 && e.tr.position.x - e.e_range[1] <= entity[j].tr.position.x && 
                                        entity[j].tr.position.x <= e.tr.position.x - e.e_range[0]))
                                    {
                                        IsTarget = true;
                                    }
                                }
                            }
                        }
                        if (!IsTarget)
                        {
                            if ((e.tag == 1 && e.tr.position.x <= 5 && 5 <= e.tr.position.x + e.c_range) || 
                                (e.tag == 0 && e.tr.position.x - e.c_range <= 5 - stageLength && 5 - stageLength <= e.tr.position.x))
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
                            else if (e.time <= 0)
                            {
                                e.anim.SetInteger("type", 2);
                                e.motion = 2;
                                e.time = e.delayTime;
                            }
                        }
                        else if (e.motion == 1) //move
                        {
                            if (IsTarget)
                            {
                                e.anim.SetInteger("type", 2);
                                e.motion = 2;
                            }
                            else
                            {
                                e.self.transform.position += Vector3.right * (2 * e.tag - 1) * e.spd;
                            }
                        }
                    }
                    else if (e.motion == 2) //attack
                    {
                        if (e.time <= 0)
                        {
                            bool IsTarget = false;
                            for (int j = 0; j < entity.Count; j++)
                            {
                                if (!IsTarget)
                                {
                                    if (e.tag + entity[j].tag == 1 && !(3 <= entity[j].motion && entity[j].motion <= 5))
                                    {
                                        if ((e.tag == 1 && e.tr.position.x + e.e_range[0] <= entity[j].tr.position.x && 
                                            entity[j].tr.position.x <= e.tr.position.x + e.e_range[1]) || 
                                            (e.tag == 0 && e.tr.position.x - e.e_range[1] <= entity[j].tr.position.x && 
                                            entity[j].tr.position.x <= e.tr.position.x - e.e_range[0]))
                                        {
                                            entity[j].hp -= e.atk;
                                            if (e.IsObj)
                                            {
                                                IsTarget = true;
                                                audioSource.clip = soundEffects[Random.Range(1, 2)];
                                                audioSource.Play();
                                            }
                                        }
                                    }
                                }
                            }
                            if (!IsTarget)
                            {
                                if ((e.tag == 1 && e.tr.position.x <= 5 && 5 <= e.tr.position.x + e.c_range) || 
                                    (e.tag == 0 && e.tr.position.x - e.c_range <= 5 - stageLength && 5 - stageLength <= e.tr.position.x))
                                {
                                    CH[2 - 2 * e.tag] -= e.atk;
                                    audioSource.clip = soundEffects[3];
                                    audioSource.Play();
                                }
                            }
                            e.motion = 0;
                            e.anim.SetInteger("type", 0);
                            e.time = e.atkCoolTime + e.anim.GetCurrentAnimatorStateInfo(0).length - e.delayTime;
                        }
                    }
                    else if (e.motion == 3) // knockback
                    {
                        if (e.time <= 0)
                        {
                            e.motion = 0;
                            e.anim.SetInteger("type", 0);
                            Debug.Log(e.tr.position.x);
                            e.tr.position -= Vector3.right * (2 * e.tag - 1) * 0.04f;
                        }
                        else
                        {
                            Debug.Log(e.tr.position.x);
                            e.tr.position -= Vector3.right * (2 * e.tag - 1) * 0.02f;
                        }
                    }

                    if (e.time < 0)
                    {
                        e.time = 0;
                    }
                    else
                    {
                        e.time -= Time.deltaTime;
                    }
                }
                else // die
                {
                    if (e.motion >= 0)
                    {
                        e.anim.SetInteger("type", 3);
                        e.time = e.anim.GetCurrentAnimatorStateInfo(0).length + Time.deltaTime;
                        e.motion = -1;
                    }
                    if (e.motion == -1)
                    {
                        e.tr.position -= Vector3.right * (2 * e.tag - 1) * 0.02f;
                    }
                    if (e.time <= 0)
                    {
                        if (e.motion == -1)
                        {
                            e.anim.SetInteger("type", -1);
                            e.time = e.anim.GetCurrentAnimatorStateInfo(0).length + Time.deltaTime;
                            e.motion = -2;

                            audioSource.PlayOneShot(soundEffects[4]);
                        }
                        else
                        {
                            Destroy(e.self);
                            entity.RemoveAt(i - delete);
                            delete++;
                        }
                    }
                    e.time -= Time.deltaTime;
                }
            }
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
        textUI[0].text = ((int)money).ToString() + "/" + (reinforceLevel[0] * 2000 + 2500).ToString();

        for (int i = 0; i < charLen; i++)
        {
            if (summonCoolTime[i] < charStat[i][13] / 100)
            {
                summonCoolTime[i] += reinforceLevel[1] * Time.deltaTime;
                if (summonCoolTime[i] >= charStat[i][13] / 100)
                {
                    summonCoolTime[i] = charStat[i][13] / 100;
                    gaugeBar[i].SetActive(false);
                    summonIconsImage[i].color = colors[1];
                    summonPrices[i].SetActive(true);

                    audioSource.PlayOneShot(soundEffects[5]);
                }
                else
                {
                    gaugeFill[i].fillAmount = summonCoolTime[i] / charStat[i][13] * 100;
                }
            }
        }

        time += Time.deltaTime;
    }
    public void IsReinforceButtonClicked(int var)
    {
        if (money >= reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1) &&
            reinforceLevel[var] < 8)
        {
            audioSource.PlayOneShot(soundEffects[0]);

            money -= reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1);
            reinforceLevel[var] += 1;
            textUI[var + 1].text = reinforceLevel[var].ToString();
            textUI[var + 6].text = (reinforceLevel[var] * 200 * (var + 1) + 500 / (var + 1)).ToString();
        }
        else
        {
            audioSource.PlayOneShot(soundEffects[6]);
        }
    }
    public void IsCharIconButtonClicked(int var)
    {
        if (charLen > var)
        {
            if (money >= charStat[var][12] && summonCoolTime[var] == charStat[var][13] / 100)
            {
                audioSource.PlayOneShot(soundEffects[0]);

                money -= charStat[var][12];

                summonCoolTime[var] = 0;
                summonIconsImage[var].color = colors[0];
                gaugeBar[var].SetActive(true);
                gaugeFill[var].fillAmount = 0;
                summonPrices[var].SetActive(false);

                Summon(charStat[var], charAddAbility[var], 0);
            }
            else
            {
                audioSource.PlayOneShot(soundEffects[6]);
            }
        }
    }
    void Summon(int[] stat, int[] ability, int tag)
    {
        Character c = new Character();

        GameObject obj = Resources.Load<GameObject>("char/" + stat[10].ToString() + "/mob");
        Instantiate(obj, new Vector2(1000, entity.Count+1), Quaternion.identity);

        Ray2D ray = new Ray2D(new Vector2(1000, entity.Count+1), Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        c.self = hit.collider.gameObject;
        c.tr = c.self.transform;
        c.tr.localScale = new Vector3(3 * (1 - 2 * tag), 3, 1);
        c.anim = c.self.GetComponent<Animator>();
        c.e_range = new float[2]
        {
            stat[3] * 0.01f, stat[4] * 0.01f
        };
        c.addAbility = ability;
        c.spd = stat[2] * 0.002f;
        c.time = 0;
        c.atkCoolTime = stat[6] * 0.01f;
        c.delayTime = stat[7] * 0.01f;
        c.c_range = stat[5] * 0.01f;
        c.motion = 0;
        c.tag = tag;
        c.maxHp = stat[0];
        c.hp = stat[0];
        c.atk = stat[1];
        c.barrier = stat[8];
        c.maxHitback = stat[9];
        c.hitback = stat[9] - 1;
        c.IsObj = new bool[2] { true, false }[stat[11]];

        if (tag == 0)
        {
            c.tr.position = new Vector2(5, -2);
        }
        else
        {
            c.tr.position = new Vector2(5 - stageLength, -2);
        }
        entity.Add(c);
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
    //0wait, 1move, 2attack, 3knockback, 4burrow, 5emerge, 6warpIn, 7warpOut
    public int tag;
    //0 our, 1 enem
    public int maxHp;
    public int hp;
    public int atk;
    public int barrier;
    public int maxHitback;
    public int hitback;
    public bool IsObj; //t:객체공격, f:범위공격
}
