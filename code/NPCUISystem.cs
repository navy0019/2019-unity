using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NPCUISystem : MonoBehaviour
{
    [Header("▼▼▼▼ GameObject ▼▼▼▼")]
    public GameObject Player;
    public GameObject Canvas;
    public GameObject Light;
    public GameObject Mesh;
    public List<GameObject> switchObject;

    [Header("▼▼▼▼ Transform ▼▼▼▼")]
    public Transform NPC;

    [Header("▼▼▼▼ CanvasGroup ▼▼▼▼")]
    public CanvasGroup NPCName;

    [Header("▼▼▼▼ Float ▼▼▼▼")]

    public float distance;

    [Header("▼▼▼▼ Bool ▼▼▼▼")]
    public bool enableInfoPanel;

    public GameObject mainCamera;
    public GameObject cameraTarget;
    public GameObject cameraLookTarget;

    [HideInInspector]
    public int weight;
    [HideInInspector]
    public bool UIMode;

    private Vector3 tempCameraPos;

    private void Start()
    {
        enableInfoPanel = false;
        UIMode = false;
        weight = 0;
        UISystemManager.instance.AllUICanvas.Add(Canvas, weight);
        for (int i = 0; i < 12; i++)
        {
            switchObject.Add(Mesh.transform.GetChild(i).gameObject);
        }
        switchObject.Add(GameObject.Find("UIDoNotDestroy").transform.GetChild(1).gameObject);
        switchObject.Add(GameObject.Find("UIDoNotDestroy").transform.GetChild(2).gameObject);

    }
    void Update()
    {
        Vector3 disv = NPC.position - Player.transform.position;
        float dis = disv.magnitude;

        if (dis <= distance)
        {
            NPCName.alpha = 1;
            enableInfoPanel = true;


            if (Input.GetKeyDown(KeyCode.X))
            {

                UISystemManager.instance.switchObject = switchObject;

                if (!UIMode)
                {
                    tempCameraPos = mainCamera.GetComponent<CameraConroller>().camera.transform.position;
                    setUIModeOpen();

                }
                else
                {
                    setUIModeClose();
                }

            }
        }
        else
        {
            NPCName.alpha = 0;
            enableInfoPanel = false;
        }
        if (enableInfoPanel)
        {
            //NPC name look at player
            Vector3 relativePos = Player.transform.position - NPCName.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            Quaternion RotationOnlyY = Quaternion.Euler(NPCName.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, NPCName.transform.rotation.eulerAngles.z);
            NPCName.transform.rotation = Quaternion.Lerp(NPCName.transform.rotation, RotationOnlyY, 1);

        }

    }
    public void setUIModeOpen()
    {

        UISystemManager.instance.CheckWeight(Canvas);
        if (UISystemManager.instance.canOpen)
        {

            UISystemManager.instance.canMove = true;
            UISystemManager.instance.MoveCamera(cameraTarget.transform.position, cameraLookTarget.transform.position);
            UIMode = true;
            UISystemManager.instance.OpenCanvas();
            Light.SetActive(true);
        }
    }
    public void setUIModeClose()
    {
        UISystemManager.instance.switchObject = switchObject;
        UIMode = false;

        UISystemManager.instance.CheckOpen(Canvas);

        if (UISystemManager.instance.someOneOpen)
        {
            UISystemManager.instance.canMove = true;
            UISystemManager.instance.MoveCamera(tempCameraPos, cameraLookTarget.transform.position);
        }
        else
        {
            UISystemManager.instance.canMove = false;
        }

        UISystemManager.instance.CloseCanvas();
        Light.SetActive(false);

    }

}

