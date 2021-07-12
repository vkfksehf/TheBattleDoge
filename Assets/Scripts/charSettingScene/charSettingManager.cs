using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class charSettingManager : MonoBehaviour
{
    private dataBase saveData;

    public Text[] texts;

    public GameObject charIconPrefab;

    private Vector2 firstTouch;

    private bool IsIconClicked;
    public bool IsArray;

    public float iconX;
    private float iconXacceleration;
    private float resistAcceleration;
    public int charListLength;
    public int teamListLength;

    //Select Icon
    private Transform selectedIconTr;
    private Vector2 selectedIconOriginPos;
    private int[] selectNo; // (type, Number)

    //save, load
    public List<int[]> charList;
    public List<int[]> teamList;
    public int xp;
    public int dogeFeed;
    public int rainbowFruit;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.Find("DataSaver").GetComponent<dataBase>();

        charList = saveData.charList;
        charListLength = charList.ToArray().Length;
        teamList = saveData.teamList;
        teamListLength = teamList.ToArray().Length;
        xp = saveData.xp;
        dogeFeed = saveData.dogeFeed;
        rainbowFruit = saveData.rainbowFruit;

        texts[0].text = xp.ToString();
        texts[1].text = dogeFeed.ToString();
        texts[2].text = rainbowFruit.ToString();

        StartCoroutine(array());

        iconX = 0;
        IsIconClicked = false;

        selectNo = new int[2];

        resistAcceleration = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                IsIconClicked = true;

                selectedIconTr = hit.collider.transform;
                selectedIconTr.GetComponent<SpriteRenderer>().sortingLayerName = "2";

                selectedIconOriginPos = selectedIconTr.position;
                if (selectedIconTr.position.y == -2)//char
                {
                    selectNo[0] = 0;
                    selectNo[1] = (int)((selectedIconTr.position.x - iconX) / 2.2f);
                }
                else//team
                {
                    selectNo[0] = 1;
                    selectNo[1] = (int)(-(selectedIconTr.position.y - 2.9f) / 1.48f * 5 + (selectedIconTr.position.x + 4.38f) / 2.3f);
                }
            }
            else
            {
                IsIconClicked = false;
                firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                    else if (firstTouch.x < currentTouch.x)
                    {
                        iconXacceleration += 4;
                    }
                }
            }
            else
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectedIconTr.position = mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (IsIconClicked)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool IsTeamSelected = false;
                int selectedCharSlot = -1;
                for (int i = 0; i < 10; i++)
                {
                    if (-5.2f + 2.2f * (i % 5) <= mousePos.x && mousePos.x <= -3.5f + 2.2f * (i % 5) && 2.2f - 1.5f * ((int)(i / 5)) <= mousePos.y && mousePos.y <= 3.5f - 1.5f * ((int)(i / 5)))
                    {
                        IsTeamSelected = true;
                        selectedCharSlot = i%5+5*(int)(i/5);
                    }
                }
                if (IsTeamSelected)
                {
                    if (selectNo[0] == 0) // char to team
                    {
                        selectedIconTr.GetComponent<SpriteRenderer>().sortingLayerName = "1";
                        selectedIconTr.position = new Vector2(-4.38f + 2.3f * (selectedCharSlot % 5), 2.9f - 1.48f * ((int)(selectedCharSlot / 5)));
                        if (teamListLength >= selectedCharSlot + 1) //삽입
                        {
                            selectedIconTr.GetComponent<BoxCollider2D>().size = new Vector2(0, 0);

                            Ray2D ray = new Ray2D(new Vector2(-4.38f + 2.3f * (selectedCharSlot % 5), 2.9f - 1.48f * ((int)(selectedCharSlot / 5))), Vector2.zero);
                            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                            Transform tr = hit.collider.transform;

                            tr.GetComponent<charIconMove>().iconX = 2.2f * selectNo[1];
                            tr.position = new Vector2(0, -2);

                            selectedIconTr.GetComponent<BoxCollider2D>().size = new Vector2(1.01f, 0.8f);

                            int[] var = teamList[selectedCharSlot];
                            teamList[selectedCharSlot] = charList[selectNo[1]];
                            charList[selectNo[1]] = var;
                        }
                        else // 추가
                        {
                            teamListLength++;
                            charListLength -= 1;

                            teamList.Add(charList[selectNo[1]]);
                            charList[selectNo[1]] = null;
                            charList.Remove(null);

                            StartCoroutine(array());
                        }
                    }
                    else //team to team
                    {
                        if (teamListLength >= selectedCharSlot + 1) // 교체
                        {
                            selectedIconTr.GetComponent<SpriteRenderer>().sortingLayerName = "1";
                            selectedIconTr.position = new Vector2(-4.38f + 2.3f * (selectedCharSlot % 5), 2.9f - 1.48f * ((int)(selectedCharSlot / 5)));
                            if (selectedCharSlot != selectNo[1])
                            {

                                selectedIconTr.GetComponent<BoxCollider2D>().size = new Vector2(0, 0);

                                Ray2D ray = new Ray2D(new Vector2(-4.38f + 2.3f * (selectedCharSlot % 5), 2.9f - 1.48f * ((int)(selectedCharSlot / 5))), Vector2.zero);
                                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                                Transform tr = hit.collider.transform;

                                tr.position = selectedIconOriginPos;

                                selectedIconTr.GetComponent<BoxCollider2D>().size = new Vector2(1.01f, 0.8f);

                                int[] var = teamList[selectedCharSlot];
                                teamList[selectedCharSlot] = teamList[selectNo[1]];
                                teamList[selectNo[1]] = var;
                            }
                        }
                        else
                        {
                            teamList.Add(teamList[selectNo[1]]);
                            teamList[selectNo[1]] = null;
                            teamList.Remove(null);

                            StartCoroutine(array());
                        }
                    }
                }
                else
                {
                    selectedIconTr.GetComponent<SpriteRenderer>().sortingLayerName = "1";

                    if (selectNo[0] == 0) // 변동 없음
                    {
                        selectedIconTr.position = selectedIconOriginPos;
                    }
                    else // team to char
                    {
                        charListLength++;
                        teamListLength -= 1;

                        charList.Add(teamList[selectNo[1]]);
                        teamList[selectNo[1]] = null;
                        teamList.Remove(null);

                        StartCoroutine(array());
                    }
                }
            }
        }
        if (iconXacceleration != 0)
        {
            iconX += iconXacceleration * Time.deltaTime;
            if (iconXacceleration > 0)
            {
                if (iconX > 0)
                {
                    iconX = 0;
                }
                iconXacceleration -= resistAcceleration;
            }
            else
            {
                if (iconX < -2.2f * (charListLength - 1))
                {
                    iconX = -2.2f * (charListLength - 1);
                }
                iconXacceleration += resistAcceleration;
            }
            if (Mathf.Abs(iconXacceleration) <= 1)
            {
                iconXacceleration = 0;
            }
        }
    }

    public void CancelButtonOnClick()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(LoadBackScene());
    }
    IEnumerator LoadBackScene()
    {
        yield return new WaitForSeconds(0.3f);

        saveData.Save(new int[4] { saveData.clearStageAmount, saveData.xp, saveData.dogeFeed, saveData.rainbowFruit }, charList, teamList);

        SceneManager.LoadScene("MainScene");
    }

    IEnumerator array()
    {
        IsArray = true;
        yield return new WaitForSeconds(1.1f*Time.deltaTime);
        IsArray = false;

        if (charListLength != 0)
        {
            for (int i = 0; i < charListLength; i++)
            {
                charIconPrefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("char/" + charList[i][0] + "/si");
                Instantiate(charIconPrefab, new Vector2(2.2f * i, -2), Quaternion.identity);
            }
        }
        if (teamListLength != 0)
        {
            for (int i = 0; i < teamListLength; i++)
            {
                charIconPrefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("char/" + teamList[i][0] + "/si");
                Instantiate(charIconPrefab, new Vector2(-4.38f + 2.3f * (i % 5), 2.9f - 1.48f * ((int)(i / 5))), Quaternion.identity);
            }
        }
    }
}