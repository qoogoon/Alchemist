using UnityEngine;
using System.Collections;
using System.Xml;

public class AdventurerData : MonoBehaviour {
    public string strName;
    public string strCode;
    public string strJob;
    public string strCharCode;
    private AdventurerCtrl aCtrl;
    public AdventurerData()
    {

    }
    public AdventurerData(string strAdventurerCode)
    {
        aCtrl = GameObject.Find("AdventurerCtrl").GetComponent<AdventurerCtrl>();
        foreach(AdventurerData data in aCtrl.getAllAdventurerData())
        {
            if (data.strCode.Equals(strAdventurerCode))
            {
                strName = data.strName;
                strCode = data.strCode;
                strJob = data.strJob;
                strCharCode = data.strCharCode;
                break;
            }
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //호감도
    public void setLikeAbility(int nAbility)
    {
        if(nAbility < 0)
        {
            nAbility = 0;
        }
        PlayerPrefs.SetInt(strCode + "_LikeAbility", nAbility);
    }

    public int getLikeAbility()
    {
        return PlayerPrefs.GetInt(strCode + "_LikeAbility", 10);
    }

    public void addLikeAbility(int nAddAbility)
    {
        setLikeAbility(getLikeAbility() + nAddAbility);
    }

    //레벨 최대 100lv
    public int getAdventurerLv()
    {
        aCtrl = GameObject.Find("AdventurerCtrl").GetComponent<AdventurerCtrl>();
        foreach (AdventurerExpData expData in aCtrl.getAllAdventurerExpTable())
        {
            if(expData.nTotalRequiredExp <= getAdventurerExp())
            {
                return expData.nLv;
            }
        }
        return 1;
    }

   

    //경험치
    public int getAdventurerExp()
    {
        return PlayerPrefs.GetInt(strCode + "_Exp", 0);
    }

    public void setAdventurerExp(int nExp)
    {
        PlayerPrefs.SetInt(strCode + "_Exp", nExp);
    }

    public void addAdventurerExp(int nAddExp)
    {
        if (nAddExp > 0)
        {
            setAdventurerExp(getAdventurerExp() + nAddExp);
        }
    }

}
