﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;

public class aSongUI_Backpack : TTUIPage
{
    GameObject propItem = null;
    Text weightText;
    aSongUI_BackpackWeapon leftWeapon;
    aSongUI_BackpackWeapon rightWeapon;


    List<aSongUI_PropListItem> propItems = new List<aSongUI_PropListItem>();
    List<aSongUI_PropListItem> propItemsPool = new List<aSongUI_PropListItem>();


    public aSongUI_Backpack() : base(UIType.PopUp, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "UIPrefab/Backpack";
    }

    public override void Awake(GameObject go)
    {
        propItem = this.transform.Find("Storehouse/Scroll View/Viewport/Content/Item").gameObject;
        propItem.SetActive(false);

        weightText = transform.Find("Storehouse/Instruction/Weight").GetComponent<Text>();
        transform.Find("Storehouse/Instruction/Back").GetComponent<Button>().onClick.AddListener(()=>{
            TTUIPage.ClosePage<aSongUI_Backpack>();
        });

        leftWeapon = transform.Find("Weapons/LeftWeapon").gameObject.AddComponent<aSongUI_BackpackWeapon>();
        rightWeapon = transform.Find("Weapons/RightWeapon").gameObject.AddComponent<aSongUI_BackpackWeapon>();
        leftWeapon.Init();
        rightWeapon.Init();
    }

    public override void Hide()
    {
        for (int i = 0; i < propItems.Count; i++)
        {
            propItems[i].gameObject.SetActive(false);
            propItemsPool.Add(propItems[i]);
        }
        propItems.Clear();
        this.gameObject.SetActive(false);
    }

    public override void Active()
    {
        base.Active();
        this.gameObject.SetActive(true);
    }

    public override void Refresh()
    {
        base.Refresh();
        ShowPage();
    }

    private void ShowPage()
    {
        aSong_PlayerData propData = this.data != null ? this.data as aSong_PlayerData : aSongUI_Controller.Instance.playerData;
        Debug.Log("propData.props.Count = " + propData.dic_bagProp.Count);
        var enumerator = propData.dic_bagProp.GetEnumerator();
        float countWeight = 0;
        while (enumerator.MoveNext())
        {
            AddPropToItem(enumerator.Current.Value.prop);
            countWeight += enumerator.Current.Value.prop.weight* enumerator.Current.Value.prop.num;
            weightText.text = countWeight+"/200";
        }
        if(propData.Guns[0])
            leftWeapon.Refresh(propData.Guns[0].prop);
        if (propData.Guns[1])
            rightWeapon.Refresh(propData.Guns[1].prop);
    }

    private void AddPropToItem(aSong_PlayerData.Prop prop)
    {
        if (propItemsPool.Count <= 0)
            CreatePropItem(prop);
        else
            GetPropItemFromPool(prop);
    }

    private void GetPropItemFromPool(aSong_PlayerData.Prop prop)
    {
        if (propItemsPool.Count <= 0)
            return;
        aSongUI_PropListItem item = propItemsPool[0];
        propItemsPool.Remove(item);
        propItems.Add(item);
        item.gameObject.SetActive(true);
        item.Refresh(prop);
        return;
    }

    private void CreatePropItem(aSong_PlayerData.Prop prop)
    {
        GameObject go = GameObject.Instantiate(propItem) as GameObject;
        go.transform.SetParent(propItem.transform.parent);
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        aSongUI_PropListItem item = go.AddComponent<aSongUI_PropListItem>();
        item.Refresh(prop);
        propItems.Add(item);
        Debug.Log("CreatePropItem");
        //add click btn
        //go.AddComponent<Button>().onClick.AddListener(aSongUI_Controller.Instance.OnClickSkillItem);
    }

   
}
