using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class CharacterCtrl : MonoBehaviour {
    private List<CharacterData> m_arrCharacterData;
	// Use this for initialization
	void Awake () {
        initCharacterData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*------------------------------------캐릭터 데이터---------------------------------------*/
    private void initCharacterData()
    {
        //모험가 xml가져오기
        m_arrCharacterData = new List<CharacterData>();
        XmlDocument characterDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Character", typeof(TextAsset)));
        XmlNodeList characterNodeList = characterDoc.SelectNodes("List/Item");
        foreach (XmlNode node in characterNodeList)
        {
            CharacterData data = new CharacterData();
            data.strCode = node.Attributes[0].InnerText;
            data.strName = node.Attributes[1].InnerText;
            
            m_arrCharacterData.Add(data);
        }

    }

    //모든 캐릭터 데이터
    public List<CharacterData> getAllCharacterData()
    {
        return m_arrCharacterData;
    }

   
}
