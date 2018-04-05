using UnityEngine;
using System.Collections.Generic;

public class QuestData  {
    private QuestCtrl m_questCtrl;
    public string strCode;          //퀘스트 코드
    public string strTitle;         //퀘스트 제목
    public string strContents;      //퀘스트 내용
    public string strCharacterCode; //퀘스트 대상 캐릭터 코드
    public int nNo;                 //퀘스트 번호
    public QUEST_TYPE eType;        //퀘스트 타입
    public int nCompliteValue;              //퀘스트 충족 값
    public string strTargetCode;    //퀘스트 대상 코드(물약, 재료~~)
    public int nLv;                 //퀘스트 수락 충족 레벨
    private QUEST_STATE quest_state;          //퀘스트 완료
    public List<string> arrRewardCodes;
    public List<int> arrRewardValues;

    public enum QUEST_STATE
    {
        비활성, 진행준비, 진행중, 완료
    }
    public enum QUEST_TYPE
    {
        평판, 골드, 물약
    }
    public QuestData()
    {

    }

    public QuestData(string strQuestCode)
    {
        m_questCtrl = GameObject.Find("QuestCtrl").GetComponent<QuestCtrl>();
        foreach (QuestData data in m_questCtrl.getAllQuestData())
        {
            if (data.strCode.Equals(strQuestCode))
            { 
                strCode = data.strCode;
                strTitle = data.strTitle;
                strContents = data.strContents;
                strCharacterCode = data.strCharacterCode;
                nNo = data.nNo;
                eType = data.eType;
                nCompliteValue = data.nCompliteValue;
                strTargetCode = data.strTargetCode;
                nLv = data.nLv;
                arrRewardCodes = data.arrRewardCodes;
                arrRewardValues = data.arrRewardValues;
                break;
            }
        }
    }

    public QUEST_STATE getQuestState()
    {
        string strState = PlayerPrefs.GetString("QuestState_" + strCode, "비활성");

        switch (strState)
        {
            case "비활성":
                return QUEST_STATE.비활성;
            case "진행준비":
                return QUEST_STATE.진행준비;
            case "진행중":
                return QUEST_STATE.진행중;
            default:
                return QUEST_STATE.완료;
        }
        
    }

    public void setQuestState(QUEST_STATE state)
    {
        Debug.Log(strTitle + ":" + state);
        switch (state)
        {
            case QUEST_STATE.비활성:
                PlayerPrefs.SetString("QuestState_" + strCode, "비활성");
                break;
            case QUEST_STATE.진행준비:
                PlayerPrefs.SetString("QuestState_" + strCode, "진행준비");
                break;
            case QUEST_STATE.진행중:
                PlayerPrefs.SetString("QuestState_" + strCode, "진행중");
                break;
            default:
                PlayerPrefs.SetString("QuestState_" + strCode, "완료");
                break;
        }
    }

    
}
