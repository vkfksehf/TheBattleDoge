using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fightManager : MonoBehaviour
{
    public List<int[]> charList;
    public List<int>[] charStat;
    public List<int[]> charAddAbility;

    // Start is called before the first frame update
    void Start()
    {
        charList = GameObject.Find("DataSaver").GetComponent<dataBase>().charList;
        charStat = new List<int>[10];
        charAddAbility = new List<int[]>();
        for (int i = 0; i < 10; i++)
        {
            Status stat = Resources.Load<Status>("char/"+charList[i][0].ToString()+"/stat");
            charStat[i] = new List<int>()
            {
                charList[i][0], (int)((charList[i][1] / 100 + 1) * stat.hp),
                (int)((charList[i][2] / 100 + 1) * stat.atk),
                (int)((charList[i][3] / 100 + 1) * stat.spd),
                stat.rng[0], stat.rng[1], stat.rng[2],
                (int)(stat.atkCool * 100), (int)(stat.dmgTime * 100),
                stat.cost, (int)(stat.reloadTime * 100), stat.barrier
            };
            /*
            캐릭터 번호0, 체력1, 공격력2, 속도3, 객체공격범위4~5,
            성공격범위6, 공격쿨탐7, 선딜8, 가격9, 재생산10, 베리어11

            숨기횟수12, 숨을시간13, 소생횟수14, 소생체력15, 소생대기시간16, 
            워프거리17, 워프확률18, 속성초뎀19, 속성맷집20, 파동레벨21,
            파동확률22, 열파레벨23, 열파확률24, 밀치기확률25, 크리확률26, 
            생존확률27, 생존횟수28, 공증가량29, 공증가발동체력30
            */
            charAddAbility[i] = stat.addAbility;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void charIcon(int num)
    {

    }
}
