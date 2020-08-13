using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform m_OriginalParent;
    private Vector3 m_Offset;
    private Image m_Image;
    private ItemData tempData;

    public void OnBeginDrag(PointerEventData eventData)
    {
        int index = this.transform.parent.transform.GetSiblingIndex();
        //Debug.Log("index " + index);
        AllObject.instance.originDragIndex = index;
        AllObject.instance.originDragItemData = AllObject.instance.bagInfo[index];



        m_OriginalParent = this.transform.parent;

        this.transform.parent = transform.parent.parent.parent;

        m_Image.raycastTarget = false;
        UISystemManager.instance.m_CurrentDragImage = this;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (AllObject.instance.bagInfo[AllObject.instance.originDragIndex].number>0)
        {
            transform.position = Input.mousePosition;
        }
        else
        {
            return;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        //Debug.Log(m_OriginalParent.parent);
        if (this.transform.parent.parent != m_OriginalParent.parent)
        {
            //Debug.Log("Out");
            //Debug.Log(this.transform.parent.parent);
            this.transform.parent = m_OriginalParent;
            transform.localPosition = Vector3.zero;
        }

        m_Image.raycastTarget = true;
        UISystemManager.instance.m_CurrentDragImage = null;
        AllObject.instance.originDragItemData = null;
        AllObject.instance.originDragIndex = -1;

    }


    // initialization
    public void Start()
    {
        m_Image = transform.GetChild(0).GetComponent<Image>();
    }

    public Transform GetOriginalParent()
    {
        return m_OriginalParent;
    }

}
