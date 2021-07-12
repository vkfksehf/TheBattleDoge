using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cameraMove : MonoBehaviour
{
    public GameObject bg;
    Transform camTr;
    Stage stageData;
    Vector2 firstTouch;

    float camAcceleration;
    int selectedStageNumber;
    bool IsUIClicked;

    // Start is called before the first frame update
    void Start()
    {
        camTr = GetComponent<Transform>();
        IsUIClicked = false;

        selectedStageNumber = GameObject.Find("DataSaver").GetComponent<dataBase>().selectedStageNumber;

        bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/bg/stageBg (" + selectedStageNumber.ToString() + ")");
        stageData = Resources.Load<Stage>("StageData/" + selectedStageNumber.ToString());
        for (int i = 1; i < (int)(stageData.stageLength / 17.78f) + 2; i++)
        {
            Instantiate(bg, Vector3.left * 17.78f * i, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                    camAcceleration += 4.5f * (firstTouch.x - currentTouch.x) / Mathf.Abs(firstTouch.x - currentTouch.x);
                }
            }
        }
        if (camAcceleration != 0)
        {
            if (camTr.position.x + camAcceleration * Time.deltaTime > 0)
            {
                camTr.position = new Vector3(0, 0, -10);
            }
            else if (camTr.position.x + camAcceleration * Time.deltaTime < -stageData.stageLength)
            {
                camTr.position = new Vector3(-stageData.stageLength, 0, -10);
            }
            else
            {
                camTr.position += Vector3.right * camAcceleration * Time.deltaTime;
            }
            camAcceleration -= Mathf.Abs(camAcceleration) / camAcceleration;
        }
    }
}
