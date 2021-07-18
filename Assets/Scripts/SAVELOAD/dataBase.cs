using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataBase : MonoBehaviour
{
    public List<int[]> charList;
    public List<int[]> teamList;
    public int clearStageAmount;
    public int xp;
    public int dogeFeed;
    public int rainbowFruit;

    public int selectedStageNumber;

    public string[] stageNames;

    public string[] encoding;
    public Dictionary<string, int> decoding;
    public List<Dictionary<string, string>> alphabets;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("DataSaver"));

        stageNames = new string[] {"제주도", "전라남도", "광주 광역시",
            "경상남도", "부산 광역시", "울산 광역시",
            "대구 광역시", "전라북도", "충청남도",
            "대전 광역시", "충청북도", "경상북도",
            "울릉도", "독도", "강원도",
            "경기도", "인천 광역시", "서울 특별시"};

        alphabets = new List<Dictionary<string, string>>();
        string[] var3 = new string[20] { "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };
        for (int i = 0; i < 2; i++)
        {
            alphabets.Add(new Dictionary<string, string>());
            for (int j = 0; j < 10; j++)
            {
                alphabets[i].Add(j.ToString(), var3[10 * i + j]);
            }
        }
        encoding = new string[10] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" };
        decoding = new Dictionary<string, int>();
        for (int i = 0; i < 10; i++)
        {
            decoding.Add(encoding[i], i);
        }

        string path = Application.persistentDataPath + "/data.xml";
        if (System.IO.File.Exists(path))
        {
            TransData transData = new TransData();

            transData = XmlManager.XmlLoad<TransData>(path);

            string[] var = transData.AppData.Split('/');
            int[] var2 = new int[4];
            for (int i = 0; i < 4; i++)
            {
                var2[i] = Decoding(var[i]);
            }

            charList = new List<int[]>();
            for (int i = 0; i < transData.character.ToArray().Length; i++)
            {
                charList.Add(new int[13]);
                for (int j = 0; j < 13; j++)
                {
                    charList[i][j] = Decoding(transData.character[i][j]);
                }
            }
            teamList = new List<int[]>();
            for (int i = 0; i < transData.team.ToArray().Length; i++)
            {
                teamList.Add(new int[13]);
                for (int j = 0; j < 13; j++)
                {
                    teamList[i][j] = Decoding(transData.team[i][j]);
                }
            }
        }
        else
        {
            TransData transData = new TransData();

            string[] var2 = new string[13];
            for (int i = 0; i < 13; i++)
            {
                var2[i] = "kq!";
            }
            var2[11] = "kw!";
            var2[12] = "kw!jq)bq&";
            transData.character = new List<string[]>();
            transData.character.Add(var2);
            transData.team = new List<string[]>();
            transData.AppData = "kq!/kq!/kq!/kq!";

            charList = new List<int[]>();

            int[] var = new int[13];
            var[0] = 0;
            var[11] = 1;
            var[12] = 100;
            charList.Add(var);

            teamList = new List<int[]>();
            clearStageAmount = 0;
            xp = 0;
            dogeFeed = 0;
            rainbowFruit = 0;

            XmlManager.XmlSave<TransData>(transData, path);
        }
    }

    public void Save(int[] args, List<int[]> argChar, List<int[]> argTeam)
    {
        clearStageAmount = args[0];
        xp = args[1];
        dogeFeed = args[2];
        rainbowFruit = args[3];
        charList = argChar;
        teamList = argTeam;

        TransData transData = new TransData();

        transData.AppData = Encoding(clearStageAmount) + "/" + Encoding(xp) + "/" + Encoding(dogeFeed) + "/" + Encoding(rainbowFruit);
        transData.character = new List<string[]>();
        for (int i = 0; i < charList.ToArray().Length; i++)
        {
            transData.character.Add(new string[13]);
            for (int j = 0; j < 13; j++)
            {
                transData.character[i][j] = Encoding(charList[i][j]);
            }
        }
        transData.team = new List<string[]>();
        for (int i = 0; i < teamList.ToArray().Length; i++)
        {
            transData.team.Add(new string[13]);
            for (int j = 0; j < 13; j++)
            {
                transData.team[i][j] = Encoding(teamList[i][j]);
            }
        }

        XmlManager.XmlSave<TransData>(transData, Application.persistentDataPath + "/data.xml");
    }

    public void SelectStage(int arg)
    {
        selectedStageNumber = arg;
    }

    private string Encoding(int i)
    {
        string var = ((int)(Mathf.Pow(2, i / 10 - (int)(i / 10) + 0.1f) * Mathf.Pow(10, i.ToString().Length - 1))).ToString();
        string var2 = "";
        
        for (int j = 0; j < i.ToString().Length; j++)
        {
            var2 += alphabets[0][var[j].ToString()];
            var2 += encoding[(int)((i % Mathf.Pow(10, i.ToString().Length - 1 - j)) / Mathf.Pow(10, i.ToString().Length - 1 - j))];
            var2 += alphabets[1][var[j].ToString()];
        }

        return var2;
    }

    private int Decoding(string i)
    {
        int var = 0;
        for (int j = 0; j < (int)(i.Length/3); j++)
        {
            var += decoding[i[3 * j + 1].ToString()]*(int)Mathf.Pow(10, (int)(i.Length/3) - j);
        }

        return var;
    }
}
