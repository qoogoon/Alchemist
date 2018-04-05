using UnityEngine;
using System.Collections;
using System;
using System.Xml;

//연금 재료 컨트롤러
public class MaterialData : MonoBehaviour {
    public string strCode;   //재료 코드
    public string strName;   //재료 이름
    public bool[,] arrShape; //재료 모양
    public ArrayList arrPotionCodes;  //재조할수 있는 포션들의 코드
    public int nMaterialLv;
    public int nMaterialPrice;
    private MaterialCtrl materialCtrl;

    public MaterialData()
    {

    }

    public MaterialData(string strMaterialCode)
    {
        materialCtrl = GameObject.Find("MaterialCtrl").GetComponent<MaterialCtrl>();
        foreach (MaterialData data in materialCtrl.getAllMaterialData())
        {
            if (data.strCode.Equals(strMaterialCode))
            {
                strCode = data.strCode;
                strName = data.strName;
                arrShape = data.arrShape;
                arrPotionCodes = data.arrPotionCodes;
                nMaterialLv = data.nMaterialLv;
                nMaterialPrice = data.nMaterialPrice;
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
    
    /*
    //가지고있는 연금 재료
    public void setPossesionAmount(int nAmount)
    {
        PlayerPrefs.SetInt(strCode + "_Amount", nAmount);
    }

    public int getPossesionAmount()
    {
        return PlayerPrefs.GetInt(strCode + "_Amount");
    }
    */
}
