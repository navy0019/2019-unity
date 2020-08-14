using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void Change();
public delegate void LocalChange();
public class AllObject : MonoBehaviour
{
    [HideInInspector]
    public Change changeBag;
    [HideInInspector]
    public LocalChange localChange;

    public static AllObject instance;

    public GameObject nameObj;
    public GameObject effectObj;
    public GameObject dataObj;
    public GameObject gridParent;
    public GameObject bagItemPrefab;
    public GameObject inventoryParent;

    public string[] StartItem;
    public int[] StartItemNum;

    //Bag Object Info
    public ItemData[] bagInfo;

    //inventory Object Info
    public ItemData[] inventoryInfo;

    //Trade Object Info
    public GameObject[] tradeListInfo;
    [HideInInspector]
    public List<Sprite> tradeItemSprites;
    [HideInInspector]
    public List<Text> tradeItemName;
    [HideInInspector]
    public List<Text> tradeItemEffect;



    [HideInInspector]
    public ItemData originDragItemData = null;
    [HideInInspector]
    public int originDragIndex = -1;

    public Dictionary<string, ItemData> itemDic;

    public void Awake()
    {
        
            //單例模式--確保遊戲中只存在一個該物件
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != null)
            {
                Destroy(gameObject);
            }
            //避免場景切換數值跑掉
            DontDestroyOnLoad(gameObject);
       
       
        itemDic = new Dictionary<string, ItemData>();

        tradeListInfo=new GameObject[nameObj.transform.childCount];
        for (int i = 0; i < dataObj.transform.childCount; i++)
        {
            itemDic.Add(dataObj.transform.GetChild(i).name, dataObj.transform.GetChild(i).GetComponent<ItemData>());
        }
        for (int i = 0; i < nameObj.transform.childCount; i++)
        {
            tradeItemSprites.Add(dataObj.transform.GetChild(i).GetComponent<Image>().sprite);
            tradeItemName.Add(nameObj.transform.GetChild(i).GetComponent<Text>());
            tradeItemEffect.Add(effectObj.transform.GetChild(i).GetComponent<Text>());
            tradeListInfo[i] = dataObj.transform.GetChild(i).gameObject;
        }


        bagInfo = new ItemData[gridParent.transform.childCount];

        for (int i = 0; i < gridParent.transform.childCount; i++)
        {
            if (gridParent.transform.GetChild(i).childCount == 0)
            {
                bagInfo[i] = null;
            }

        }

        inventoryInfo = new ItemData[inventoryParent.transform.childCount];
        for (int i = 0; i < inventoryParent.transform.childCount; i++)
        {
            if (inventoryParent.transform.GetChild(i).childCount == 0)
            {
                inventoryInfo[i] = null;
            }

        }

    }
    public void Start()
    {
        //GameObject item1 = dataObj.transform.Find("HPpotion").gameObject;
        //GameObject item2 = dataObj.transform.Find("MPpotion").gameObject;
        //GameObject[] items = new GameObject[] { item1, item2 };
        //int[] nums = new int[] { 5, 5 };
        AddItem(StartItem, StartItemNum);

    }

    public void AddItem(string[] items, int[] itemNum)
    {
        Transform gridChild;
        for (int i = 0; i < items.Length; i++)
        {
            //int needNum = 0;
            int checkNum = 0;
            string now = items[i];
            itemDic[now].number += itemNum[i];

            //Debug.Log(items[i].name);
            if (now != "money")
            {
                if (itemDic[now].number >= itemDic[now].MaxNumber)
                {
                    itemDic[now].nowNeedInstance = itemDic[now].number / itemDic[now].MaxNumber;
                    if (itemDic[now].number % itemDic[now].MaxNumber!=0)
                    {
                        itemDic[now].nowNeedInstance += 1;
                    }
                    //Debug.Log(items[i].name);
                    //Debug.Log(" nowNum "+ itemDic[items[i].name].number+" MaxNum "+ itemDic[items[i].name].MaxNumber+" needNum " + needNum);
                }
                else
                {
                    itemDic[now].nowNeedInstance = 1;
                }

                for (int j = 0; j < bagInfo.Length; j++)
                {
                    gridChild = gridParent.transform.GetChild(j);
                    if (gridChild.transform.childCount != 0 && bagInfo[j]!=null)
                    {
                        if (bagInfo[j].name == now)
                        {
                            checkNum++;
                        }
                    }

                }
                for (int k = checkNum; checkNum < itemDic[now].nowNeedInstance; k++)
                {
                    for (int kk = 0; kk < bagInfo.Length; kk++)
                    {
                        gridChild = gridParent.transform.GetChild(kk);
                        if (gridChild.transform.childCount == 0)
                        {
                            Instantiate(bagItemPrefab, gridParent.transform.GetChild(kk).transform);
                            gridChild.transform.GetChild(0).transform.localPosition = Vector3.zero;

                            bagInfo[kk] = itemDic[now];
                            checkNum++;
                            break;
                        }
                    }
                }

            }
        }

        changeBag();
        if (UISystemManager.instance.AllUICanvas.Count>0)
        {
            GameObject f = GameObject.Find("UI/TradeCanvas");
            if (f !=null&&UISystemManager.instance.AllUICanvas.ContainsKey(f) && f.activeSelf) {
                Debug.Log("local change");
                localChange();
            }
        }
    }
    public void updateBagInfo()
    {
        //Debug.Log("updateBagInfo");
        int checkNum = 0;
        for (int j=0;j< dataObj.transform.childCount;j++) {

            int dataNum= dataObj.transform.GetChild(j).GetComponent<ItemData>().number;
            int dataMaxNum = dataObj.transform.GetChild(j).GetComponent<ItemData>().MaxNumber;
            int instanceNum = dataObj.transform.GetChild(j).GetComponent<ItemData>().nowNeedInstance;
            instanceNum = dataNum / dataMaxNum;
            if (dataNum % dataMaxNum!=0)
            {
                instanceNum++;
            }
            //Debug.Log(dataObj.transform.GetChild(j).name+"  instance Num"+ instanceNum);
            for (int i = 0; i < bagInfo.Length; i++)
            {
                if (bagInfo[i]!=null&&bagInfo[i].name== dataObj.transform.GetChild(j).name) {
                    checkNum++;
                }
                if (checkNum > instanceNum)
                {
                    //Debug.Log("destroy "+ bagInfo[i].name );
                    if (gridParent.transform.GetChild(i).childCount>0)
                    {
                        Destroy(gridParent.transform.GetChild(i).GetChild(0).gameObject);
                    }
                    else
                    {
                        Destroy(gridParent.transform.parent.GetChild(10).gameObject);
                        

                    }
                    //Destroy(gridParent.transform.GetChild(i).GetChild(0).gameObject);

                    bagInfo[i] = null;
                    checkNum--;
                }
                //if (bagInfo[i] != null && bagInfo[i].number <=0)
                //{
                //    bagInfo[i] = null;
                //}
            }
            checkNum = 0;
        }
        changeBag();
        if (UISystemManager.instance.AllUICanvas.Count > 0)
        {
            GameObject f = GameObject.Find("UI/TradeCanvas");
            if (f != null && UISystemManager.instance.AllUICanvas.ContainsKey(f) && f.activeSelf)
            {
                Debug.Log("local change");
                localChange();
            }
        }

    }
    public void updateInventoryInfo()
    {
        for (int i=0;i<inventoryInfo.Length;i++)
        {
            if (inventoryInfo[i]!=null)
            {
                if (inventoryInfo[i].number==0)
                {
                    Destroy(inventoryParent.transform.GetChild(i).GetChild(0).gameObject);
                    inventoryInfo[i] = null;
                }
            }
        }
        updateBagInfo();
    }
    public int inventoryFindPos(ItemData item)
    {
        int nowIndex;
        for (int i = 0; i < inventoryInfo.Length; i++)
        {
            if (inventoryInfo[i]!=null) {
                if (inventoryInfo[i].name == item.name)
                {
                    nowIndex = i;

                    return nowIndex;
                }
            }


        }
        Debug.Log("can't find");
        return -1;
    }
    public bool inventoryCheck(ItemData self,ref int index)
    {
        for (int i = 0; i < inventoryInfo.Length; i++)
        {
            if (inventoryInfo[i] != null)
            {
                if (inventoryInfo[i].name == self.name)
                {
                    index = i;
                    //Debug.Log(i + "  overLapping");
                    return false;
                }

            }
        }
        return true;
    }
    public void inventoryMove(int overlappingIndex,int nowIndex)
    {
        //Debug.Log(inventoryParent.transform.GetChild(overlappingIndex).transform.GetChild(0).name);
        ItemData tempData = inventoryInfo[overlappingIndex];

        inventoryInfo[nowIndex] = tempData;
        inventoryInfo[overlappingIndex] = null;

        inventoryParent.transform.GetChild(overlappingIndex).transform.GetChild(0).transform.parent = inventoryParent.transform.GetChild(nowIndex).transform;
        inventoryParent.transform.GetChild(nowIndex).transform.GetChild(0).transform.localPosition = Vector3.zero;


    }
    public void inventoryExchange(int dragIndex, int targetIndex)
    {
        ItemData tempData = inventoryInfo[dragIndex];

        inventoryInfo[dragIndex] = inventoryInfo[targetIndex];
        inventoryInfo[targetIndex] = tempData;

        inventoryParent.transform.GetChild(dragIndex).transform.GetChild(0).transform.parent = inventoryParent.transform.GetChild(targetIndex).transform;
        inventoryParent.transform.GetChild(targetIndex).transform.GetChild(0).transform.parent= inventoryParent.transform.GetChild(dragIndex).transform;

        inventoryParent.transform.GetChild(dragIndex).transform.GetChild(0).transform.localPosition = Vector3.zero;
        inventoryParent.transform.GetChild(targetIndex).transform.GetChild(0).transform.localPosition = Vector3.zero;


    }

}
