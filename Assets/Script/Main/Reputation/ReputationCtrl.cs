using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class ReputationCtrl : MonoBehaviour {
    public Text ReputationTitleText;
    public Text ReputationValueText;
    public int nReputationLv;
    private List<ReputationRankData> m_reputationDataList;
    private ReputationRankData m_currentReputationRank;
    private ReputationRankData m_goalReputationRank;
    private Controller m_ctrl;

    // Use this for initialization
    void Awake () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        initReputationData();
        setPlayerReputationRank(getPlayerReputation());
        
       // Debug.Log(nReputationLv);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //--------------------------------명성 데이터-----------------------------//
    //명성 랭크 데이터 초기화
    private void initReputationData()
    {
        //모험가 xml가져오기
        m_reputationDataList = new List<ReputationRankData>();
        XmlDocument ReputationDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Reputation", typeof(TextAsset)));
        XmlNodeList m_reputationNodeList = ReputationDoc.SelectNodes("List/Item");
        foreach (XmlNode node in m_reputationNodeList)
        {
            ReputationRankData data = new ReputationRankData();
            data.strCode = node.Attributes[0].InnerText;
            data.strTitle = node.Attributes[1].InnerText;
            data.strContents = node.Attributes[2].InnerText;
            data.nLv = Convert.ToInt32(node.Attributes[3].InnerText);
            data.nConditionValue = Convert.ToInt32(node.Attributes[4].InnerText);
            m_reputationDataList.Add(data);
        }
    }

    //모든 명성 랭크 데이터 가져오기
    public List<ReputationRankData> getAllReputationRankData()
    {
        return m_reputationDataList;
    }

    //플레이어 명성 가져오기
    public int getPlayerReputation()
    {
        return PlayerPrefs.GetInt("PlayerReputation",0);
    }

    //플레이어 명성 설정하기
    public void setPlayerReputation(int nValue)
    {
        if(nValue >= 0)
        {
            PlayerPrefs.SetInt("PlayerReputation", nValue);
        }
    }

    //플레이어 명성 더하기
    public void addPlayerReputation(int nValue)
    {
        setPlayerReputation(getPlayerReputation() + nValue);
        setPlayerReputationRank(getPlayerReputation());
        Debug.Log("명성 증가:" + nValue + ", 총 명성:" + getPlayerReputation());

        //플레이어 명성에 따른 퀘스트 발생
        //퀘스트 검색
        foreach (QuestData qData in m_ctrl.questCtrl.getAllQuestData())
        {
            if (qData.strCharacterCode.Equals("CHAR_000"))
            {
                if (qData.nLv <= m_currentReputationRank.nLv && qData.getQuestState() == QuestData.QUEST_STATE.비활성)      //모험가 레벨이 퀘스트 충족 조건에 해당이 되고 퀘스트가 수락 되어 있지 않으면
                {
                    qData.setQuestState(QuestData.QUEST_STATE.진행준비);
                    Debug.Log("플레이어의 명성 레벨" + m_currentReputationRank.nLv+" 퀘스트 명성 레벨:"+ qData.nLv);
                    break;
                }
            }

        }
    }

    //현재 명성랭크와 목표 명성랭그 설정
    private void setPlayerReputationRank(int nReputationValue)
    {
        List<ReputationRankData> allList = getAllReputationRankData();
        ReputationRankData TmpData = new ReputationRankData();
        int nLastIndex = allList.Count - 1;
        for (int i = nLastIndex; i >= 0; i--)
        {
            if(allList[nLastIndex].nConditionValue <= nReputationValue)     //평판수치가 상한선을 넘을경우
            {
                nReputationLv = i + 1;
                ReputationTitleText.text = allList[i].strTitle;
                ReputationValueText.text = "";
                break;
            }
            else if(allList[i].nConditionValue <= nReputationValue)         //현재 평판에 맞는 평판 랭킹 찾기
            {
                nReputationLv = i + 1;
                ReputationTitleText.text = allList[i].strTitle;
                m_currentReputationRank = allList[i];
                m_goalReputationRank = TmpData;
                ReputationValueText.text = nReputationValue + "/" + m_goalReputationRank.nConditionValue;
                break;
            }
            TmpData = allList[i];
        }
        
    }
}
