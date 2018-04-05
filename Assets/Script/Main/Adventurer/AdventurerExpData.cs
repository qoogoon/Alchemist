using UnityEngine;
using System.Collections;

public class AdventurerExpData {
    public int nLv;
    public int nRequiredExp;
    public int nTotalRequiredExp;
    public int nAddExp;

    public AdventurerExpData()
    {
    }

    public AdventurerExpData(int nAdventurerLv)
    {
        AdventurerCtrl aCtrl = GameObject.Find("AdventurerCtrl").GetComponent<AdventurerCtrl>();
        foreach (AdventurerExpData data in aCtrl.getAllAdventurerExpTable())
        {
            if (data.nLv== nAdventurerLv)
            {
                nLv = data.nLv;
                nRequiredExp = data.nRequiredExp;
                nTotalRequiredExp = data.nTotalRequiredExp;
                nAddExp = data.nAddExp;
                break;
            }
        }
    }
}
