using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dropper : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        DragItem d = UISystemManager.instance.m_CurrentDragImage;
        ItemData item = AllObject.instance.originDragItemData;
        int originIndex = AllObject.instance.originDragIndex;
        if (this.transform.childCount == 0)
        {
            if (this.transform.gameObject.name == "inventory")
            {
                int nowIndex = this.gameObject.transform.GetSiblingIndex();
                int overlappingIndex = 0;
                //Debug.Log("true");
                if (AllObject.instance.inventoryCheck(item, ref overlappingIndex))
                {
                    Instantiate(d, this.transform);
                    AllObject.instance.inventoryParent.transform.GetChild(nowIndex).transform.GetChild(0).localPosition = Vector3.zero;
                    AllObject.instance.inventoryInfo[nowIndex] = item;
                    AllObject.instance.updateBagInfo();
                    return;
                }
                else
                {
                    AllObject.instance.inventoryMove(overlappingIndex, nowIndex);
                    return;
                }

            }
            else
            {
                if (item.number>0) {
                    int nowIndex = this.gameObject.transform.GetSiblingIndex();
                    AllObject.instance.bagInfo[nowIndex] = item;
                    AllObject.instance.bagInfo[originIndex] = null;

                    d.transform.parent = this.transform;
                    d.transform.localPosition = Vector3.zero;

                    return;
                }


            }


        }

        //change
        Transform t = this.transform.GetChild(0);
        if (t == null)
        {
            int nowIndex = this.gameObject.transform.GetSiblingIndex();
            AllObject.instance.bagInfo[nowIndex] = item;
            AllObject.instance.bagInfo[originIndex] = null;

            d.transform.parent = this.transform;
            d.transform.localPosition = Vector3.zero;

        }
        else if (t != null && this.transform.gameObject.name == "inventory")
        {

            int dragIndex = AllObject.instance.inventoryFindPos(item);
            int targetIndex = t.transform.parent.transform.GetSiblingIndex();
            if (dragIndex != targetIndex)
            {
                //Debug.Log(dragIndex + "   "+ targetIndex);
                AllObject.instance.inventoryExchange(dragIndex, targetIndex);
            }
            else
            {
                //Debug.Log(dragIndex + "   " + targetIndex);
            }


        }
        else
        {
            //change two item position
            int nowIndex = this.transform.GetSiblingIndex();
            //Debug.Log(originIndex+"  "+nowIndex);
            ItemData tempData = AllObject.instance.bagInfo[nowIndex];

            AllObject.instance.bagInfo[nowIndex] = item;
            AllObject.instance.bagInfo[originIndex] = tempData;

            t.transform.parent = d.GetOriginalParent();
            t.transform.localPosition = Vector3.zero;
            d.transform.parent = this.transform;
            d.transform.localPosition = Vector3.zero;
            //Debug.Log("aa");





        }
    }

}
