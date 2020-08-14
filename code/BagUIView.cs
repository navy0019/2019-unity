using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUIView : MonoBehaviour
{

    private AllObject itemInfo;
    private ItemData[] bagInfo;

    public GameObject playerData;
    public Text moneyText;
    public Text HPText;
    public Text MPText;
    public Text AttackText;
    public Text DefText;

    private void Start()
    {

        itemInfo = AllObject.instance;

        bagInfo = itemInfo.bagInfo;

        itemInfo.changeBag += updateBagView;

        updateBagView();


    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CheckBagTransform();
        }
    }
    //check bag item's transform is correct
    public void CheckBagTransform()
    {
        //Debug.Log("check");
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("still down");
        }
        else
        {
            for (int i = 0; i < bagInfo.Length; i++)
            {
                Transform gridChild = AllObject.instance.gridParent.transform.GetChild(i);
                if (bagInfo[i] != null && gridChild.transform.childCount == 0)
                {
                    AllObject.instance.gridParent.transform.parent.GetChild(10).transform.parent = gridChild;
                    gridChild.GetChild(0).transform.localPosition = Vector3.zero;
                    gridChild.GetChild(0).GetChild(0).GetComponent<Image>().raycastTarget = true;
                }
            }
        }

    }
    public void updateBagView()
    {
        //Debug.Log("updataItem");
        for (int i = 0; i < bagInfo.Length; i++)
        {
            Transform gridChild = AllObject.instance.gridParent.transform.GetChild(i);
            ItemData itemNow = bagInfo[i];
            if (itemNow != null && gridChild.transform.childCount != 0)
            {
                gridChild.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = bagInfo[i].img;

                if (itemNow.number > itemNow.MaxNumber)
                {
                    int tempNum = itemNow.number;


                    MakeBagItemNumber(tempNum ,itemNow);
                }
                else
                {
                    gridChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = itemNow.number.ToString();
                    //Debug.Log("small");
                }
            }
        }
        moneyText.text = AllObject.instance.itemDic["money"].number.ToString();
        HPText.text= playerData.GetComponent<CharacterFakeData>().Hp.ToString();
        MPText.text = playerData.GetComponent<CharacterFakeData>().Mp.ToString();
        AttackText.text = playerData.GetComponent<CharacterFakeData>().Attack.ToString();
        DefText.text = playerData.GetComponent<CharacterFakeData>().Defense.ToString();
    }

    public void MakeBagItemNumber(int tempNum , ItemData itemNow)
    {
        for (int k = 0; k < bagInfo.Length && tempNum != 0; k++)
        {
            Transform gridChild;

            if (AllObject.instance.gridParent.transform.childCount != 0)
            {
                 gridChild = AllObject.instance.gridParent.transform.GetChild(k);
            }
            else
            {
                continue;
            }

            if (bagInfo[k] != null && bagInfo[k].name == itemNow.name)
            {
                if (gridChild.transform.childCount == 0)
                {
                    continue;
                }
                if (tempNum > itemNow.MaxNumber)
                {
                    gridChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = itemNow.MaxNumber.ToString();
                    tempNum -= itemNow.MaxNumber;
                    //Debug.Log("max  "+ tempNum);
                }
                else
                {
                    gridChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = tempNum.ToString();
                    tempNum = 0;
                    //Debug.Log("%%%%%%  ");
                }
            }

        }
    }

}
