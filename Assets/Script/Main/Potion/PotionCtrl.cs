using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class PotionCtrl : MonoBehaviour {
    private List<PotionData> m_arrPotionsData;
    
    // Use this for initialization
    void Awake () {
        initPotionData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*-------------------포션 데이터---------------------*/
    public void initPotionData()
    {
        //포션 xml가져오기
        m_arrPotionsData = new List<PotionData>();
        XmlDocument potionlDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Alchemy/Potion", typeof(TextAsset)));
        XmlNodeList m_potionNodeList = potionlDoc.SelectNodes("List/Item");
        foreach (XmlNode node in m_potionNodeList)
        {
            PotionData data = new PotionData();

            //코드
            data.strCode = node.Attributes[0].InnerText;

            //이름
            data.strName = node.Attributes[1].InnerText;

            //물약 공식 모양 데이터 저장
            List<bool[,]> arrFormulaTable = new List<bool[,]>();

            string[] arrRawFormula = node.Attributes[2].InnerText.Split(';');   //공식뭉치를 ';'로 쪼개기
            for(int nFomulaCount = 0; nFomulaCount < arrRawFormula.Length; nFomulaCount++)  //공식들 동안
            {
                arrFormulaTable.Add(new bool[5,5]);
                string[] arrRawFormulaRow = arrRawFormula[nFomulaCount].Split('/'); //공식을 '/'로 쪼개기
                for (int i = 0; i < arrRawFormulaRow.Length; i++)
                {
                    for (int j = 0; j < arrRawFormulaRow[i].Length; j++)
                    {
                        if (arrRawFormulaRow[i].Substring(j, 1).Equals("1"))
                        {
                            arrFormulaTable[nFomulaCount][i, j] = true;
                        }
                        else
                        {
                            arrFormulaTable[nFomulaCount][i, j] = false;
                        }
                    }
                }
            }
            data.arrFormulaTable = arrFormulaTable;

            //필요 재료
            List<string> arrNeedMaterialCodes = new List<string>();
            string[] strRawNeedMaterialCodesData;
            strRawNeedMaterialCodesData = node.Attributes[3].InnerText.Split('/');
            foreach (string strMaterialCode in strRawNeedMaterialCodesData)
            {
                arrNeedMaterialCodes.Add(strMaterialCode);
            }
            data.arrNeedMaterialCodes = arrNeedMaterialCodes;

            //레벨
            data.nPotionLv = Convert.ToInt32(node.Attributes[4].InnerText);

            //상세 설명
            data.strDetail = node.Attributes[5].InnerText;

            //포션 값
            data.nPotionPrice = Convert.ToInt32(node.Attributes[6].InnerText);

            //타입
            data.nType = Convert.ToInt32(node.Attributes[7].InnerText);

            //모험 성공 보너스
            data.nAdventure_success_bonus = Convert.ToInt32(node.Attributes[8].InnerText);

            //
            m_arrPotionsData.Add(data);
        }
    }

    //모든 포션 데이터 가져오기
    public List<PotionData> getAllPotionData()
    {
        return m_arrPotionsData;
    }

}
