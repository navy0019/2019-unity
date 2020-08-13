using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryKey : MonoBehaviour
{
    public GameObject playerData;
    private AllObject itemInfo;
    private ItemData[] InventoryInfo;

    private Transform inventoryIndex;

    public GameObject HpEffect;
    public GameObject MpEffect;
    public GameObject healPoint;
    private GameObject HpInstance;
    private GameObject MpInstance;

    public void HealFalse()
    {
        Destroy(healPoint.transform.GetChild(0).gameObject);
    }
    private void Start()
    {
        itemInfo = AllObject.instance;

        InventoryInfo = itemInfo.inventoryInfo;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            healPoint = GameObject.Find("Character").transform.GetChild(0).GetChild(7).gameObject;
            if (InventoryInfo[0]!=null) {
                inventoryIndex = AllObject.instance.inventoryParent.transform.GetChild(0).GetChild(0).GetChild(2);
                if (InventoryInfo[0].number>0 && inventoryIndex.GetComponent<Image>().fillAmount<=0 && CharacterParameter.PlayerHP > 0)
                {

                    //AllObject.instance.updateBagInfo();
                    if (InventoryInfo[0].name == "HPpotion" && CharacterParameter.PlayerHP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[0]);
                        InventoryInfo[0].number--;

                        AllObject.instance.updateInventoryInfo();

                        HpInstance = Instantiate(HpEffect, healPoint.transform);
                        HpInstance.transform.position = healPoint.transform.position;
                        HpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                    else if (InventoryInfo[0].name == "MPpotion" && CharacterParameter.PlayerMP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[0]);
                        InventoryInfo[0].number--;

                        AllObject.instance.updateInventoryInfo();

                        MpInstance = Instantiate(MpEffect, healPoint.transform);
                        MpInstance.transform.position = healPoint.transform.position;
                        MpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                }


            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            healPoint = GameObject.Find("Character").transform.GetChild(0).GetChild(7).gameObject;
            if (InventoryInfo[1] != null)
            {
                inventoryIndex = AllObject.instance.inventoryParent.transform.GetChild(1).GetChild(0).GetChild(2);
                if (InventoryInfo[1].number > 0 && inventoryIndex.GetComponent<Image>().fillAmount <= 0 && CharacterParameter.PlayerHP > 0)
                {

                    //AllObject.instance.updateBagInfo();
                    if (InventoryInfo[1].name == "HPpotion" && CharacterParameter.PlayerHP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[1]);
                        InventoryInfo[1].number--;

                        AllObject.instance.updateInventoryInfo();

                        HpInstance = Instantiate(HpEffect, healPoint.transform);
                        HpInstance.transform.position = healPoint.transform.position;
                        HpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                    else if (InventoryInfo[1].name == "MPpotion" && CharacterParameter.PlayerMP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[1]);
                        InventoryInfo[1].number--;

                        AllObject.instance.updateInventoryInfo();

                        MpInstance = Instantiate(MpEffect, healPoint.transform);
                        MpInstance.transform.position = healPoint.transform.position;
                        MpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                }

            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            healPoint = GameObject.Find("Character").transform.GetChild(0).GetChild(7).gameObject;
            if (InventoryInfo[2] != null)
            {
                inventoryIndex = AllObject.instance.inventoryParent.transform.GetChild(2).GetChild(0).GetChild(2);
                if (InventoryInfo[2].number > 0 && inventoryIndex.GetComponent<Image>().fillAmount <= 0 && CharacterParameter.PlayerHP > 0)
                {

                    //AllObject.instance.updateBagInfo();
                    if (InventoryInfo[2].name == "HPpotion" && CharacterParameter.PlayerHP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[2]);
                        InventoryInfo[2].number--;

                        AllObject.instance.updateInventoryInfo();

                        HpInstance = Instantiate(HpEffect, healPoint.transform);
                        HpInstance.transform.position = healPoint.transform.position;
                        HpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                    else if (InventoryInfo[2].name == "MPpotion" && CharacterParameter.PlayerMP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[2]);
                        InventoryInfo[2].number--;

                        AllObject.instance.updateInventoryInfo();

                        MpInstance = Instantiate(MpEffect, healPoint.transform);
                        MpInstance.transform.position = healPoint.transform.position;
                        MpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                }

            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            healPoint = GameObject.Find("Character").transform.GetChild(0).GetChild(7).gameObject;
            if (InventoryInfo[3] != null)
            {
                inventoryIndex = AllObject.instance.inventoryParent.transform.GetChild(3).GetChild(0).GetChild(2);
                if (InventoryInfo[3].number > 0 && inventoryIndex.GetComponent<Image>().fillAmount <= 0 && CharacterParameter.PlayerHP > 0)
                {
                    //AllObject.instance.updateBagInfo();
                    if (InventoryInfo[3].name == "HPpotion" && CharacterParameter.PlayerHP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[3]);
                        InventoryInfo[3].number--;
                        AllObject.instance.updateInventoryInfo();

                        HpInstance = Instantiate(HpEffect, healPoint.transform);
                        HpInstance.transform.position = healPoint.transform.position;
                        HpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                    else if(InventoryInfo[3].name == "MPpotion" && CharacterParameter.PlayerMP < 1)
                    {
                        inventoryIndex.GetComponent<Image>().fillAmount = 1;
                        playerData.GetComponent<CharacterFakeData>().characterUpgrade(InventoryInfo[3]);
                        InventoryInfo[3].number--;
                        AllObject.instance.updateInventoryInfo();

                        MpInstance = Instantiate(MpEffect, healPoint.transform);
                        MpInstance.transform.position = healPoint.transform.position;
                        MpInstance.transform.rotation = healPoint.transform.rotation;

                        Invoke("HealFalse", 3.5f);
                    }
                }

            }
        }

        if (InventoryInfo[0] != null && AllObject.instance.inventoryParent.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount>=0)
        {
            AllObject.instance.inventoryParent.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount -= 0.02f;
            //Debug.Log("---");
        }
        if (InventoryInfo[1] != null && AllObject.instance.inventoryParent.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount >= 0)
        {
            AllObject.instance.inventoryParent.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount -= 0.02f;
            //Debug.Log("---");
        }
        if (InventoryInfo[2] != null && AllObject.instance.inventoryParent.transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount >= 0)
        {
            AllObject.instance.inventoryParent.transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount -= 0.02f;
            //Debug.Log("---");
        }
        if (InventoryInfo[3] != null && AllObject.instance.inventoryParent.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount >= 0)
        {
            AllObject.instance.inventoryParent.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount -= 0.02f;
            //Debug.Log("---");
        }


    }
}
