using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradUIView : MonoBehaviour
{
    public GameObject player;
    public GameObject item;
    public GameObject needItem;
    public GameObject buttonTrue;
    public GameObject buttonFalse;
    public Text itemName;
    public Text itemEffect;
    public GameObject playerData;

    public Sprite Exchange;
    public Sprite Upgrade;

    private GameObject[] itemList;
    private List<GameObject> needList;
    private AllObject itemInfo;

    private int index;
    private int indexL;
    private int indexR;

    private int nowActive;
    private void Start()
    {
        itemInfo = AllObject.instance;
        itemList = new GameObject[3];
        needList = new List<GameObject>();
        if (GameObject.Find("UIDoNotDestroy").transform.gameObject)
        {
            //Debug.Log("find");
            playerData = GameObject.Find("UIDoNotDestroy");
        }
        else
        {
            Debug.Log("Can't find playerData");
        }

        for (int i = 0; i < item.transform.childCount; i++)
        {
            itemList[i] = item.transform.GetChild(i).gameObject;
            //Debug.Log(items.transform.GetChild(i).GetComponent<Image>().sprite.name);
        }
        for (int i = 0; i < needItem.transform.childCount; i++)
        {
            for (int j = 0; j < needItem.transform.GetChild(i).childCount; j++)
            {
                needList.Add(needItem.transform.GetChild(i).GetChild(j).gameObject);

            }
        }

        index = 2;
        indexL = index - 1;
        indexR = index + 1;

        //可創造物件
        updateItemInfo();
        
        //需要物件
        updateNeedItem();


        itemInfo.localChange += updateNeedItem;


    }
    public void updateItemInfo()
    {
        itemList[1].transform.GetChild(0).GetComponent<Image>().sprite = itemInfo.tradeItemSprites[index];
        itemList[0].transform.GetChild(0).GetComponent<Image>().sprite = itemInfo.tradeItemSprites[indexL];
        itemList[2].transform.GetChild(0).GetComponent<Image>().sprite = itemInfo.tradeItemSprites[indexR];
        itemName.text = itemInfo.tradeItemName[index].text;
        itemEffect.text = itemInfo.tradeItemEffect[index].text;

        string nowKey = itemInfo.dataObj.transform.GetChild(index).name;
        if (itemInfo.itemDic[nowKey].onlyOne)
        {
            buttonTrue.transform.GetChild(0).GetComponent<Image>().sprite = Upgrade;
        }
        else
        {
            buttonTrue.transform.GetChild(0).GetComponent<Image>().sprite = Exchange;
        }

    }
    public void updateNeedItem()
    {
        string nowKey = itemInfo.dataObj.transform.GetChild(index).name;
        nowActive = 0;
        int trueActive = 0;
        int needActive = itemInfo.itemDic[nowKey].needItemNum.Length;
        for (int j=0;j< needItem.transform.childCount;j++)
        {
            for (int i = 0; i < needItem.transform.GetChild(j).childCount; i++)
            {
                if (needItem.transform.GetChild(j).GetChild(i).gameObject.activeSelf)
                {
                    nowActive++;
                }

            }
        }

        //Debug.Log("active " + nowActive);
        //Debug.Log("need Active " + needActive);


        if (nowActive < needActive)
        {
            for (int j = 0; j < needItem.transform.childCount; j++)
            {
                for (int i = 0; i <= needItem.transform.GetChild(j).childCount - 1 && nowActive < needActive; i++)
                {
                    if (!needItem.transform.GetChild(j).GetChild(i).gameObject.activeSelf)
                    {
                        needItem.transform.GetChild(j).GetChild(i).gameObject.SetActive(true);
                        ++nowActive;
                    }

                }
            }


        }else if (nowActive > needActive)
        {
            for (int j = needItem.transform.childCount-1; j >=0 ; j--)
            {
                for (int i = needItem.transform.GetChild(j).childCount - 1; i >= 0 && nowActive > needActive; i--)
                {
                    if (needItem.transform.GetChild(j).GetChild(i).gameObject.activeSelf)
                    {
                        //Debug.Log("--");
                        needItem.transform.GetChild(j).GetChild(i).gameObject.SetActive(false);
                        --nowActive;
                    }

                }
            }

        }

        for (int i = 0; i < needActive; i++)
        {
            //Debug.Log(itemInfo.itemDic[nowKey].needItem[i].name+ "  " +itemInfo.itemDic[nowKey].number);

            Sprite sourceImg = itemInfo.itemDic[nowKey].needItem[i].GetComponent<Image>().sprite;
            needList[i].transform.GetChild(0).GetComponent<Image>().sprite = sourceImg;
            needList[i].transform.GetChild(1).GetComponent<Text>().text = itemInfo.itemDic[nowKey].needItem[i].GetComponent<ItemData>().number.ToString();
            //Debug.Log(itemInfo.itemDic[nowKey].name+"  "+ itemInfo.itemDic[nowKey].number);
            needList[i].transform.GetChild(3).GetComponent<Text>().text = itemInfo.itemDic[nowKey].needItemNum[i].ToString();
            if (itemInfo.itemDic[nowKey].needItem[i].GetComponent<ItemData>().number < int.Parse(needList[i].transform.GetChild(3).GetComponent<Text>().text))
            {
                needList[i].transform.GetChild(1).GetComponent<Text>().color = Color.gray;
                needList[i].transform.GetChild(3).GetComponent<Text>().color = Color.gray;
            }
            else
            {
                ++trueActive;
                needList[i].transform.GetChild(1).GetComponent<Text>().color = Color.white;
                needList[i].transform.GetChild(3).GetComponent<Text>().color = Color.white;
                if (trueActive>= needActive) {
                    buttonTrue.SetActive(true);
                    buttonFalse.SetActive(false);
                }
                else
                {
                    buttonTrue.SetActive(false);
                    buttonFalse.SetActive(true);
                }
            }
        }
    }
    public void TradeButtonClick()
    {
        string[] items = new string[] { AllObject.instance.tradeListInfo[index].name };
        int[] nums = new int[] {1};
        ItemData now = AllObject.instance.tradeListInfo[index].GetComponent<ItemData>();
        for (int i=0;i< now.needItemNum.Length;i++)
        {
            now.needItem[i].GetComponent<ItemData>().number -= now.needItemNum[i];
        }
        if (!AllObject.instance.tradeListInfo[index].GetComponent<ItemData>().onlyOne) {
            AllObject.instance.AddItem(items, nums);
        }
        else
        {
            playerData.GetComponent<CharacterFakeData>().characterUpgrade(AllObject.instance.tradeListInfo[index].GetComponent<ItemData>());
        }

        AllObject.instance.updateBagInfo();
        updateNeedItem();

    }
    public void indexIncrease()
    {
        if (index + 1 > itemInfo.tradeItemSprites.Count - 1)
        {
            index = 0;
        }
        else
        {
            ++index;
        }

        if (index - 1 < 0)
        {
            indexL = itemInfo.tradeItemSprites.Count - 1;
        }
        else
        {
            indexL = index - 1;
        }

        if (index + 1 > itemInfo.tradeItemSprites.Count - 1)
        {
            indexR = 0;
        }
        else
        {
            indexR = index + 1;
        }
        updateItemInfo();
        updateNeedItem();

    }
    public void indexDecrease()
    {
        if (index - 1 < 0)
        {
            index = itemInfo.tradeItemSprites.Count - 1;
        }
        else
        {
            --index;
        }

        if (index - 1 < 0)
        {
            indexL = itemInfo.tradeItemSprites.Count - 1;
        }
        else
        {
            indexL = index - 1;
        }

        if (index + 1 > itemInfo.tradeItemSprites.Count - 1)
        {
            indexR = 0;
        }
        else
        {
            indexR = index + 1;
        }
        updateItemInfo();
        updateNeedItem();
    }
    public void ClickDeBug()
    {
        itemInfo.itemDic["money"].number += 500;
        updateNeedItem();
        //Debug.Log("click!!");
    }
}
