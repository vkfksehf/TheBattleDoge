using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "ScriptableObjects/stageData", order = int.MaxValue)]
public class Stage : ScriptableObject
{
    [SerializeField]
    public int stageNumber;
    public int StageNumber { get { return stageNumber;  } }
    [SerializeField]
    public int stageLength;
    public int StageLength { get { return stageLength; } }
    [SerializeField]
    public stageChar[] stageCharList;
    public stageChar[] StageCharList { get { return stageCharList; } }
}

[System.Serializable]
public class stageChar
{
    public int[] charStat;
}