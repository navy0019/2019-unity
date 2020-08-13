using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUISystem : MonoBehaviour
{
    [Header("▼▼▼▼ GameObject ▼▼▼▼")]
    public GameObject Canvas;
    public List<GameObject> switchObject;
    public GameObject Button;

    public int weight;
    public KeyCode key;
    [HideInInspector]
    public bool UIMode;

    AudioSource ButtonA;

    void Start()
    {
        UISystemManager.instance.AllUICanvas.Add(Canvas, weight);
        ButtonA = Button.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(key))
        {
            ButtonA.Play();
            if (!UISystemManager.instance.AllUICanvas.ContainsKey(Canvas))
            {
                UISystemManager.instance.AllUICanvas.Add(Canvas, weight);
            }
            //Debug.Log("click  "+ key);
            UISystemManager.instance.switchObject = switchObject;

            if (!UIMode)
            {
                setUIModeOpen();

            }
            else
            {
                setUIModeClose();
            }

        }
    }
    public void setUIModeOpen()
    {
        UISystemManager.instance.CheckWeight(Canvas);
        if (UISystemManager.instance.canOpen)
        {
            UISystemManager.instance.canMove = false;
            UIMode = true;
            UISystemManager.instance.OpenCanvas();
        }
    }
    public void setUIModeClose()
    {
        UISystemManager.instance.switchObject = switchObject;
        UIMode = false;

        UISystemManager.instance.CheckOpen(Canvas);
        UISystemManager.instance.canMove = false;
        UISystemManager.instance.CloseCanvas();

    }
}
