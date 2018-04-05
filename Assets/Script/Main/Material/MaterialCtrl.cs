using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

public class MaterialCtrl : MonoBehaviour {
    private List<MaterialData> m_arrMaterialsData;

    // Use this for initialization
    void Awake () {
        initData();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*---------------------------데이터-----------------------*/
    private void initData()
    {
        //연금 재료 xml가져오기
        m_arrMaterialsData = new List<MaterialData>();
        XmlDocument materialDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Alchemy/M​aterial", typeof(TextAsset)));
        XmlNodeList m_materialNodeList = materialDoc.SelectNodes("List/Item");
        string strRawShapeData;
        string strRawPotionsCodeData;
        
        foreach (XmlNode node in m_materialNodeList)
        {
            MaterialData mData = new MaterialData();
            //코드
            mData.strCode = node.Attributes[0].InnerText;

            //이름
            mData.strName = node.Attributes[1].InnerText;

            //재료 모양 데이터 저장
            bool[,] m_arrShape = new bool[3, 3];
            strRawShapeData = node.Attributes[2].InnerText;
            string[] arrRawShapeRows = strRawShapeData.Split('/');
            for (int i = 0; i < arrRawShapeRows.Length; i++)
            {
                for (int j = 0; j < arrRawShapeRows[i].Length; j++)
                {
                    if (arrRawShapeRows[i].Substring(j, 1).Equals("1"))
                    {
                        m_arrShape[i, j] = true;
                    }
                    else
                    {
                        m_arrShape[i, j] = false;
                    }
                }
            }
            mData.arrShape = m_arrShape;

            //만들 수 있는 포션 리스트
            strRawPotionsCodeData = node.Attributes[3].InnerText;
            string[] arrRawPotionsCodeRow = strRawPotionsCodeData.Split('/');
            ArrayList arrPotionCodes = new ArrayList();
            foreach (string strPotionCode in arrRawPotionsCodeRow)
            {
                arrPotionCodes.Add(strPotionCode);
            }
            mData.arrPotionCodes = arrPotionCodes;

            mData.nMaterialLv = Convert.ToInt32(node.Attributes[4].InnerText);
            mData.nMaterialPrice = Convert.ToInt32(node.Attributes[5].InnerText);
            
            m_arrMaterialsData.Add(mData);
        }
    }

    //모든 재료 데이터
    public List<MaterialData> getAllMaterialData()
    {
        return m_arrMaterialsData;
    }
}
