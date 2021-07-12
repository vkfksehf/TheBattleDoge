using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class charPowUpManager : MonoBehaviour
{
    private dataBase saveData;

    public GameObject charIconPrefab;
    public GameObject whitePanel;
    public SpriteRenderer mainIcon;

    public AudioSource[] audioSources;

    public Status mainStat;

    private Vector2 firstTouch;

    public Text[] texts;

    private bool IsIconClicked;

    public float iconX;
    private float iconXacceleration;
    private float resistAcceleration;

    public int[] needs;
    public int charSelectedNumber;
    private int charListLength;

    public int xp;
    public int dogeFeed;
    public int rainbowFruit;
    public List<int[]> charList;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.Find("DataSaver").GetComponent<dataBase>();

        xp = saveData.xp;
        dogeFeed = saveData.dogeFeed;
        rainbowFruit = saveData.rainbowFruit;
        charList = saveData.charList;

        texts[0].text = xp.ToString();
        texts[1].text = dogeFeed.ToString();
        texts[2].text = rainbowFruit.ToString();

        charListLength = charList.ToArray().Length;

        if (charListLength != 0)
        {
            for (int i = 0; i < charListLength; i++)
            {
                charIconPrefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/char/" + charList[i][0] + "/si");
                charIconPrefab.GetComponent<charPowUp_IconBehaviour>().charNumber = i;
                Instantiate(charIconPrefab, new Vector2(2.2f * i, -3), Quaternion.identity);
            }
        }

        charSelectedNumber = 0;
        mainIcon.sprite = Resources.Load<Sprite>("char/" + charList[0][0] + "/si");
        mainStat = Resources.Load<Status>("char/"+charList[0][0]+"/stat");
        texts[3].text = "체력: " + mainStat.hp.ToString() + "+" + (charList[0][1] * mainStat.hp).ToString() + "(" + charList[0][1] + "%)" +
            "\n공격력: " + mainStat.atk.ToString() + "+" + (charList[0][2] * mainStat.atk).ToString() + "(" + charList[0][2] + "%)" +
            "\n속도: " + mainStat.spd.ToString() + "+" + (charList[0][3] * mainStat.spd).ToString() + "(" + charList[0][3] + "%)" +
            "\n공격범위: " + mainStat.rng[0].ToString() + " ~ " + mainStat.rng[1].ToString() + ", " + mainStat.rng[2].ToString() +
            "\n공격쿨타임: " + mainStat.atkCool.ToString() +
            "\n코스트: " + mainStat.cost.ToString() + "+" + (charList[0][6] * mainStat.cost).ToString() + "(" + charList[0][6] + "%)" +
            "\n생산쿨타임: " + mainStat.reloadTime.ToString() + "+" + (charList[0][5] * mainStat.reloadTime).ToString() + "(" + charList[0][5] + "%)" +
            "\n베리어: " + mainStat.barrier.ToString() +
            "\n크리티컬: " + charList[0][8] + "%" +
            "\n속성: " + charList[0][10] +
            "\n배율: " + charList[0][11] +
            "\n내구성: " + charList[0][12]+"%";
        texts[4].text = mainStat.description;

        iconX = 0;
        iconXacceleration = 0;
        resistAcceleration = 1;

        IsIconClicked = false;

        needs = new int[3] { (int)(1000 + Mathf.Pow(2, charList[charSelectedNumber][11]) + Mathf.Pow(charList[charSelectedNumber][11], 2)), (int)(charList[charSelectedNumber][11] / 10), (int)(-(charList[charSelectedNumber][12]-100) / 3) };
        texts[5].text = "*" + needs[0] + "xp, 열매" + needs[1] + "개, 사료 " + needs[2] + "개 필요";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null) //만약 콜라이더가 존재할 경우
            {
                charSelectedNumber = hit.collider.GetComponent<charPowUp_IconBehaviour>().charNumber;
                mainIcon.sprite = Resources.Load<Sprite>("Images/char/" + charList[charSelectedNumber][0] + "/si");
                mainStat = Resources.Load<Status>("CharacterData/" + charList[charSelectedNumber][0]);
                texts[3].text = "체력: " + mainStat.hp.ToString() + "+" + (charList[charSelectedNumber][1] * mainStat.hp).ToString() + "(" + charList[charSelectedNumber][1] + "%)" +
                    "\n공격력: " + mainStat.atk.ToString() + "+" + (charList[charSelectedNumber][2] * mainStat.atk).ToString() + "(" + charList[charSelectedNumber][2] + "%)" +
                    "\n속도: " + mainStat.spd.ToString() + "+" + (charList[charSelectedNumber][3] * mainStat.spd).ToString() + "(" + charList[charSelectedNumber][3] + "%)" +
                    "\n공격범위: " + mainStat.rng[0].ToString() + " ~ " + mainStat.rng[1].ToString() + ", " + mainStat.rng[2].ToString() +
                    "\n공격쿨타임: " + mainStat.atkCool.ToString() +
                    "\n코스트: " + mainStat.cost.ToString() + "+" + (charList[charSelectedNumber][6] * mainStat.cost).ToString() + "(" + charList[charSelectedNumber][6] + "%)" +
                    "\n생산쿨타임: " + mainStat.reloadTime.ToString() + "+" + (charList[charSelectedNumber][5] * mainStat.reloadTime).ToString() + "(" + charList[charSelectedNumber][5] + "%)" +
                    "\n베리어: " + mainStat.barrier.ToString() +
                    "\n크리티컬: " + charList[charSelectedNumber][8] + "%" +
                    "\n속성: " + charList[charSelectedNumber][10] +
                    "\n배율: " + charList[charSelectedNumber][11] +
                    "\n내구성: " + charList[charSelectedNumber][12] + "%";
                texts[4].text = mainStat.description;
                needs = new int[3] { (int)(1000 + Mathf.Pow(2, charList[charSelectedNumber][11]) + Mathf.Pow(charList[charSelectedNumber][11], 2)), (int)(charList[charSelectedNumber][11] / 10), (int)(-(charList[charSelectedNumber][12]-100) / 3) };
                texts[5].text = "*" + needs[0] + "xp, 열매" + needs[1] + "개, 사료 " + needs[2] + "개 필요";
                IsIconClicked = true;
            }
            else
            {
                firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                IsIconClicked = false;
            }
            iconXacceleration = 0;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!IsIconClicked)
            {
                Vector2 currentTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Vector2.Distance(firstTouch, currentTouch) > 0.4f)
                {
                    if (firstTouch.x > currentTouch.x)
                    {
                        iconXacceleration -= 4;
                    }
                    else
                    {
                        iconXacceleration += 4;
                    }
                }
            }
        }
        if (iconXacceleration > 0)
        {
            iconX += iconXacceleration * Time.deltaTime;
            if (iconX > 0)
            {
                iconX = 0;
            }
            iconXacceleration -= resistAcceleration;
        }
        else if (iconXacceleration < 0)
        {
            iconX += iconXacceleration * Time.deltaTime;
            if (iconX < -2.2f * (charListLength - 1))
            {
                iconX = -2.2f * (charListLength - 1);
            }
            iconXacceleration += resistAcceleration;
        }
        if (Mathf.Abs(iconXacceleration) <= 1 && iconXacceleration != 0)
        {
            iconXacceleration = 0;
        }
    }

    public void IsCancelButtonClicked()
    {
        audioSources[0].Play();
        StartCoroutine(Exit());
    }
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("MainScene");
    }

    public void IsButtonClicked()
    {
        if (needs[0] <= xp && needs[1] <= rainbowFruit && needs[2] <= dogeFeed)
        {
            xp -= needs[0];
            rainbowFruit -= needs[1];
            dogeFeed -= needs[2];
            texts[0].text = xp.ToString();
            texts[1].text = dogeFeed.ToString();
            texts[2].text = rainbowFruit.ToString();

            charList[charSelectedNumber][1] += Random.Range(-50, 70);
            if (charList[charSelectedNumber][1] <= -100)
            {
                charList[charSelectedNumber][1] = -99;
            }
            charList[charSelectedNumber][2] += Random.Range(-50, 70);
            if (charList[charSelectedNumber][2] <= -100)
            {
                charList[charSelectedNumber][2] = -99;
            }
            charList[charSelectedNumber][3] += Random.Range(-10, 40);
            if (charList[charSelectedNumber][3] <= -100)
            {
                charList[charSelectedNumber][3] = -99;
            }
            charList[charSelectedNumber][6] += Random.Range(-20, 40);
            if (charList[charSelectedNumber][6] <= -100)
            {
                charList[charSelectedNumber][6] = -99;
            }
            charList[charSelectedNumber][5] += Random.Range(-10, 60);
            if (charList[charSelectedNumber][5] <= -100)
            {
                charList[charSelectedNumber][5] = -99;
            }
            charList[charSelectedNumber][8] += Random.Range(-15, 10);
            if (charList[charSelectedNumber][8] < 0)
            {
                charList[charSelectedNumber][8] = 0;
            }
            charList[charSelectedNumber][12] -= Random.Range(0, 10);
            if (charList[charSelectedNumber][12] <= 0)
            {
                charList[charSelectedNumber][12] = 1;
            }
            charList[charSelectedNumber][11] += 1;

            texts[3].text = "체력: " + mainStat.hp.ToString() + "+" + (charList[charSelectedNumber][1] * mainStat.hp / 100).ToString() + "(" + charList[charSelectedNumber][1] + "%)" +
                "\n공격력: " + mainStat.atk.ToString() + "+" + (charList[charSelectedNumber][2] * mainStat.atk / 100).ToString() + "(" + charList[charSelectedNumber][2] + "%)" +
                "\n속도: " + mainStat.spd.ToString() + "+" + (charList[charSelectedNumber][3] * mainStat.spd / 100).ToString() + "(" + charList[charSelectedNumber][3] + "%)" +
                "\n공격범위: " + mainStat.rng[0].ToString() + " ~ " + mainStat.rng[1].ToString() + ", " + mainStat.rng[2].ToString() +
                "\n공격쿨타임: " + mainStat.atkCool.ToString() +
                "\n코스트: " + mainStat.cost.ToString() + "+" + (charList[charSelectedNumber][6] * mainStat.cost / 100).ToString() + "(" + charList[charSelectedNumber][6] + "%)" +
                "\n생산쿨타임: " + mainStat.reloadTime.ToString() + "+" + (charList[charSelectedNumber][5] * mainStat.reloadTime / 100).ToString() + "(" + charList[charSelectedNumber][5] + "%)" +
                "\n베리어: " + mainStat.barrier.ToString() +
                "\n크리티컬: " + charList[charSelectedNumber][8] + "%" +
                "\n속성: " + charList[charSelectedNumber][10] +
                "\n배율: " + charList[charSelectedNumber][11] +
                "\n내구성: " + charList[charSelectedNumber][12] + "%";
            needs = new int[3] { (int)(1000 + Mathf.Pow(2, charList[charSelectedNumber][11]) + Mathf.Pow(charList[charSelectedNumber][11], 2)), (int)(charList[charSelectedNumber][11] / 10), (int)(-(charList[charSelectedNumber][12]-100) / 3) };
            texts[5].text = "*" + needs[0] + "xp, 열매" + needs[1] + "개, 사료 " + needs[2] + "개 필요";

            audioSources[1].Play();
            StartCoroutine(Glitter());
        }
        else
        {
            audioSources[2].Play();
        }
        saveData.Save(new int[4] {saveData.clearStageAmount, xp, dogeFeed, rainbowFruit}, charList, saveData.teamList);
    }
    IEnumerator Glitter()
    {
        whitePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whitePanel.SetActive(false);
    }
}
