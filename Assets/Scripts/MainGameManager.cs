using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    public Animator[] animators;

    public AudioClip[] audioClips;
    private AudioSource audioSource;

    public Text[] texts; //textBalloon, screenName, xp, feed, fruit

    public float t;
    public float selected;
    public int turnOut;

    private bool[] turnRequire;

    //save, load
    public List<int[]> charList;
    public List<int[]> teamList;
    public int clearStageAmount;
    public int xp;
    public int dogeFeed;
    public int rainbowFruit;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        t = -1;
        turnOut = 0;

        texts[0].text = "...";

        dataBase GetData;
        GetData = GameObject.Find("DataSaver").GetComponent<dataBase>();

        texts[2].text = GetData.xp.ToString();
        texts[3].text = GetData.dogeFeed.ToString();
        texts[4].text = GetData.rainbowFruit.ToString();

        turnRequire = new bool[2] { true, true };
        if (GetData.charList.ToArray().Length == 0)
        {
            turnRequire[1] = false;
        }
        if (GetData.teamList.ToArray().Length == 0)
        {
            turnRequire[0] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            texts[0].text = "...";
        }
        if (t >= 0)
        {
            if (10*t >= 5)
            {
                if (turnOut == 3) //exit
                {
                    SceneManager.LoadScene("StartScene");
                }
                else
                {
                    animators[0].SetBool("main", false);
                    animators[1].SetBool("main", false);
                    animators[2].SetBool("main", false);
                    animators[3].SetBool("main", false);
                }
                if (10 * t >= 10)
                {
                    if (turnOut == 0)
                    {
                        SceneManager.LoadScene("chapterSelect");
                    }
                    else if (turnOut == 2)
                    {
                        SceneManager.LoadScene("charSetting");
                    }
                    else if (turnOut == 1)
                    {
                        SceneManager.LoadScene("charPowUp");
                    }
                }
            }
            t += Time.deltaTime;
        }
    }

    public void IsButtonClicked(int var)
    {
        if (var == 0 || var == 1)
        {
            if (turnRequire[var])
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
                t = 0;
                turnOut = var;
            }
            else
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
                texts[0].text = new string[2] {"전투할 캐릭터를 선택하세요!", "강화할 캐릭터가 없습니다!"}[var];
            }
        }
        else
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();
            t = 0;
            turnOut = var;
        }
    }

    private void OnApplicationQuit()
    {
        SceneManager.LoadScene("StartScene");
    }
}
