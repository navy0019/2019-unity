using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIView : MonoBehaviour
{
    private AllObject itemInfo;
    private ItemData[] InventoryInfo;

    void Start()
    {
        itemInfo = AllObject.instance;

        InventoryInfo = itemInfo.inventoryInfo;

        itemInfo.changeBag += updateInventoryView;

        updateInventoryView();
    }


    public void updateInventoryView()
    {
        //Debug.Log("updataInventoryItem");
        for (int i = 0; i < InventoryInfo.Length; i++)
        {
            Transform InventoryChild = AllObject.instance.inventoryParent.transform.GetChild(i);
            //Debug.Log(InventoryChild.gameObject.name);
            if (InventoryInfo[i] != null)
            {
                InventoryChild.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = InventoryInfo[i].img;
                InventoryChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = InventoryInfo[i].number.ToString();

            }
        }

    }
}
