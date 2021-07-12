using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stageSelectManager : MonoBehaviour
{
    public GameObject[] stageButtons;
    public GameObject[] Arrows;
    public GameObject touchBlockPanel;

    public Animator startAnim;

    public Transform cameraTransform;
    public RectTransform stageButtonEdgeTr;

    public Vector2[] stagePos;
    private Vector3 cameraMovementDirection;

    public Text[] texts;
    public Text[] stageButtonTexts;

    private int page;
    private int UtmostPage;
    private int selectedStageNumber;

    private string[] stageNames;

    //save, load
    public int clearStageAmount;
    public int xp;
    public int dogeFeed;
    public int rainbowFruit;

    private dataBase saveData;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.Find("DataSaver").GetComponent<dataBase>();

        clearStageAmount = saveData.clearStageAmount;
        xp = saveData.xp;
        dogeFeed = saveData.dogeFeed;
        rainbowFruit = saveData.rainbowFruit;

        texts[0].text = xp.ToString();
        texts[1].text = dogeFeed.ToString();
        texts[2].text = rainbowFruit.ToString();

        stageNames = new string[] {"제주도", "전라남도", "광주 광역시",  
            "경상남도", "부산 광역시", "울산 광역시", 
            "대구 광역시", "전라북도", "충청남도",
            "대전 광역시", "충청북도", "경상북도",
            "울릉도", "독도", "강원도",
            "경기도", "인천 광역시", "서울 특별시"};

        if (clearStageAmount == 18)//utmost stageNumber
        {
            selectedStageNumber = clearStageAmount - 1;
        }
        else
        {
            selectedStageNumber = clearStageAmount;
        }
        page = (int)(selectedStageNumber / 3);
        UtmostPage = page;
        if (page == 0)
        {
            Arrows[0].SetActive(false);
        }

        stageButtonEdgeTr.anchoredPosition = new Vector2(-210, 110 - 95 * (selectedStageNumber % 3));
        for (int i = 0; i < 3; i++)
        {
            if (selectedStageNumber % 3 >= i)
            {
                stageButtonTexts[i].text = stageNames[page * 3 + i];
            }
            else
            {
                stageButtons[i].SetActive(false);
            }
        }
        
        cameraMovementDirection = new Vector3(0, 0, 0);
        cameraTransform.position = new Vector3(stagePos[selectedStageNumber].x, stagePos[selectedStageNumber].y, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Magnitude(cameraMovementDirection) != 0)
        {
            cameraTransform.position += cameraMovementDirection/10;
            if (cameraTransform.position == new Vector3(stagePos[selectedStageNumber].x, stagePos[selectedStageNumber].y, -10))
            {
                cameraMovementDirection = Vector3.zero;
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
        SceneManager.LoadScene("MainScene");
    }
    void stageArray()
    {
        stageButtonEdgeTr.anchoredPosition = new Vector2(-210, 110 - 95 * (selectedStageNumber % 3));
        for (int i = 0; i < 3; i++)
        {
            if (clearStageAmount >= page * 3 + i)
            {
                stageButtons[i].SetActive(true);
                stageButtonTexts[i].text = stageNames[page * 3 + i];
            }
            else
            {
                stageButtons[i].SetActive(false);
            }
        }
    }
    public void OnArrowClicked(int var)
    {
        if (var == 0)
        {
            if (page == UtmostPage)
            {
                Arrows[1].SetActive(true);
            }
            page -= 1;
            selectedStageNumber = page * 3;
            if (page == 0)
            {
                Arrows[0].SetActive(false);
            }
        }
        else
        {
            if (page == 0)
            {
                Arrows[0].SetActive(true);
            }
            page += 1;
            selectedStageNumber = page * 3;
            if (page == UtmostPage)
            {
                Arrows[1].SetActive(false);
            }
        }
        stageArray();
        Vector2 vec2 = cameraTransform.position;
        cameraMovementDirection = stagePos[selectedStageNumber] - vec2;
    }
    public void OnButtonClicked(int var)
    {
        stageButtonEdgeTr.anchoredPosition = new Vector2(-210, 110 - 95 * var);
        selectedStageNumber = page * 3 + var;
        Vector2 vec2 = cameraTransform.position;
        cameraMovementDirection = stagePos[selectedStageNumber] - vec2;
    }
    public void OnFightButtonClicked()
    {
        startAnim.SetBool("fight", true);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(LoadFightScene());
    }
    IEnumerator LoadFightScene()
    {
        yield return new WaitForSeconds(0.3f);

        AudioSource MainAudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        MainAudioSource.loop = false;
        MainAudioSource.clip = Resources.Load<AudioClip>("Sound/snd007");
        MainAudioSource.Play();

        saveData.SelectStage(selectedStageNumber);

        yield return new WaitForSeconds(4.5f);
        SceneManager.LoadScene("fightScene");
    }
}