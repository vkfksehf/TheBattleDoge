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
    public Status[] stageCharList;
    public Status[] StageCharList { get { return stageCharList; } }
}