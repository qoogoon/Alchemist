using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;


public class PotionData
{
    public string strCode;
    public string strName;
    public int nPotionLv;
    public string strDetail;
    public int nPotionPrice;
    public int nType;
    public List<bool[,]> arrFormulaTable;
    public List<string> arrNeedMaterialCodes;
    public int nAdventure_success_bonus;
    private Controller m_ctrl;
    public PotionData()
    {

    }

    public PotionData(string strPotionCode)
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        List<PotionData> potionList = m_ctrl.potionCtrl.getAllPotionData();
        foreach(PotionData data in potionList)
        {
            if (data.strCode.Equals(strPotionCode))
            {
                strCode = data.strCode;
                strName = data.strName;
                nPotionLv = data.nPotionLv;
                strDetail = data.strDetail;
                nPotionPrice = data.nPotionPrice;
                nType = data.nType;
                arrFormulaTable = data.arrFormulaTable;
                arrNeedMaterialCodes = data.arrNeedMaterialCodes;
                nAdventure_success_bonus = data.nAdventure_success_bonus;
            }
        }
    }

    //가지고있는 포션 갯수
    public void setPlayersPotionAmount(int nAmount)
    {
        PlayerPrefs.SetInt(strCode + "_Amount", nAmount);
    }

    public int getPlayersPotionAmount()
    {
        return PlayerPrefs.GetInt(strCode + "_Amount");
    }

}
