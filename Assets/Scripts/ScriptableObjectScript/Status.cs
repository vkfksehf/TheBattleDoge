using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "ScriptableObjects/CharacterData", order = int.MaxValue)]
public class Status : ScriptableObject
{
    [SerializeField]
    public int charNumber = 0;
    public int CharNumber {  get { return charNumber; } }
    [SerializeField]
    public int hp = 0;
    public int Hp { get { return hp; } }
    [SerializeField]
    public int atk = 0;
    public int Atk { get { return atk; } }
    [SerializeField]
    public int spd = 0;
    public int Spd { get { return spd; } }
    [SerializeField]
    public int[] rng = new int[3]; // 객체 공격 범위 0 ~ 1, 성 공격 범위 2
    public int[] Rng { get { return rng; } }
    [SerializeField]
    public float atkCool = 0;
    public float AtkCool { get { return atkCool; } }
    [SerializeField]
    public float dmgTime = 0; // 공격이 시작된지 {dmgTime}초 후 공격 데미지 입힘
    public float DmgTime { get { return dmgTime; } }
    [SerializeField]
    public int cost = 0;
    public int Cost { get { return cost; } }
    [SerializeField]
    public float reloadTime = 0; // 소환 {reloadTime}초 후 다시 소환이 가능해짐
    public float ReloadTime { get { return reloadTime; } }
    [SerializeField]
    public int barrier = 0;
    public int Barrier { get { return barrier; } }
    [SerializeField]
    public int hitback = 0;
    public int Hitback { get { return hitback; } }
    [SerializeField]
    public int isObjInt = 0;
    public int IsObjInt { get { return isObjInt; } }
    [SerializeField]
    public int[] addAbility = new int[19];
    /*
    숨기횟수0, 숨을시간1, 소생횟수2, 소생체력3, 소생대기시간4, 워프거리5,
    워프확률6, 속성초뎀7, 속성맷집8, 파동레벨9, 파동확률10, 열파레벨11,
    열파확률12, 밀치기확률13, 크리확률14, 생존확률15, 생존횟수16, 
    공증가량17, 공증가발동체력18
    */
    public int[] AddAbility { get { return addAbility; } }
    [SerializeField]
    public string description = "";
    public string Description { get { return description; } }
}
