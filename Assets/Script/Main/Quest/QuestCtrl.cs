using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;

public class QuestCtrl : MonoBehaviour {
    public GameObject QuestItemPrefeb;
    public GameObject ContentObj;
    public GameObject QuestWindow;
    public GameObject QuestDetailWindow;
    public Button QuestBtn;
    public bool bListening;
    public bool bNextDayOpenWindow = false;

    private int m_QuestItemCount;
    private List<GameObject> m_arrItem;
    private GameData m_gameData;
    private List<QuestData> m_playerQuestList;
    private List<QuestData> m_arrQuestData;
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        initQuestData();
        bListening = true;
        m_gameData = GameObject.Find("GameData").GetComponent<GameData>();
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        StartCoroutine(CompliteConditionListener());
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    /*--------------------------------퀘스트 완료 조건------------------------------*/
    //퀘스트 완료 조건 리스너
    IEnumerator CompliteConditionListener()
    {
        bool bBlinkBtn = false;
        Animator BtnAni = QuestBtn.GetComponent<Animator>();
        m_playerQuestList = getProceedingQuest();
        while (true)
        {
            if (!QuestWindow.active) //퀘스트 창이 오픈되어 있지 않으면
            {
                bBlinkBtn = false;
                m_playerQuestList = getProceedingQuest();
                foreach (QuestData data in m_playerQuestList)
                {
                    if (isCompliteCondition(data))
                    {
                        bBlinkBtn = true;
                        break;
                    }
                }
                
                if (bBlinkBtn)
                {
                    if (!BtnAni.GetBool("blink"))
                    {
                        BtnAni.SetBool("blink", true);
                    }
                }
                else
                {
                    if (BtnAni.GetBool("blink"))
                    {
                        BtnAni.SetBool("blink", false);
                    }
                }
            }else
            {
                BtnAni.SetBool("blink", false);
            }
   
            yield return new WaitForSeconds(1f);
        }
    }

    //퀘스트 완료 조건
    public bool isCompliteCondition(QuestData data)
    {
        bool bComplite = false;
        switch (data.eType)
        {
            case QuestData.QUEST_TYPE.골드:
                if (m_gameData.getPlayerGold() >= data.nCompliteValue)
                {
                    bComplite = true;
                }
                else
                {
                    bComplite = false;
                }
                break;

            case QuestData.QUEST_TYPE.평판:
                if (m_ctrl.reputationCtrl.getPlayerReputation() >= data.nCompliteValue)
                {
                    bComplite = true;
                }
                else
                {
                    bComplite = false;
                }
                break;
            case QuestData.QUEST_TYPE.물약:
                PotionData pData = new PotionData(data.strTargetCode);
                if (pData.getPlayersPotionAmount() >= data.nCompliteValue)
                {
                    bComplite = true;
                }
                else
                {
                    bComplite = false;
                }
                break;
        }
        return bComplite;
    }

    /*---------------------------------퀘스트 데이터--------------------------------*/
    //데이터 초기화
    private void initQuestData()
    {
        //퀘스트 xml가져오기
        m_arrQuestData = new List<QuestData>();
        XmlDocument QuestDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Quest", typeof(TextAsset)));
        XmlNodeList m_questNodeList = QuestDoc.SelectNodes("List/Item");
        foreach (XmlNode node in m_questNodeList)
        {
            QuestData data = new QuestData();
            data.strCode = node.Attributes[0].InnerText;
            data.strTitle = node.Attributes[1].InnerText;
            data.strContents = node.Attributes[2].InnerText;
            data.strCharacterCode = node.Attributes[3].InnerText;
            data.nNo = Convert.ToInt32(node.Attributes[4].InnerText);
            QuestData.QUEST_TYPE type;
            switch (node.Attributes[5].InnerText)
            {
                case "평판":
                    type = QuestData.QUEST_TYPE.평판;
                    break;
                case "골드":
                    type = QuestData.QUEST_TYPE.골드;
                    break;
                default:
                    type = QuestData.QUEST_TYPE.물약;
                    break;
            }
            data.eType = type;

            //목표 
            data.strTargetCode = node.Attributes[6].InnerText;

            //완료 값
            data.nCompliteValue = Convert.ToInt32(node.Attributes[7].InnerText);

            //
            data.nLv = Convert.ToInt32(node.Attributes[8].InnerText);

            //보상 코드들
            string strRawRewardCodes = node.Attributes[9].InnerText;
            string[] arrRawRewardCodes = strRawRewardCodes.Split('#');
            List<string> arrRewardCodes = new List<string>();
            foreach(string strRewardCode in arrRawRewardCodes)
            {
                arrRewardCodes.Add(strRewardCode);
            }
            data.arrRewardCodes = arrRewardCodes;
            

            //보상 값들
            string strRawRewardValues = node.Attributes[10].InnerText;
            string[] arrRawRewardValues = strRawRewardValues.Split('#');
            List<int> arrRewardValues = new List<int>();
            foreach(string strRewardValue in arrRawRewardValues)
            {
                arrRewardValues.Add(Convert.ToInt32(strRewardValue));
            }
            data.arrRewardValues = arrRewardValues;

            //데이터 넣기
            m_arrQuestData.Add(data);
        }
    }

    //모든 퀘스트 가져오기
    public List<QuestData> getAllQuestData()
    {
        return m_arrQuestData;
    }

    //진행중인 퀘스트 가져져오기
    public List<QuestData> getProceedingQuest()
    {
        List<QuestData> PlayerQuestList = new List<QuestData>();
        foreach(QuestData data in getAllQuestData())
        {
            if (data.getQuestState() == QuestData.QUEST_STATE.진행중)
            {
                PlayerQuestList.Add(data);
            }
        }
        return PlayerQuestList;
    }

    //진행중인 퀘스트에 추가
    /*
    public bool addProceedingQuest(QuestData questData)
    {
        if(questData.getQuestState() == QuestData.QUEST_STATE.진행준비)
        {
            questData.setQuestComplite(QuestData.QUEST_STATE.진행중);
            return true;
        }
        else
        {
            Debug.Log("추가 실패");
            return false;
        }
    }
    */

    //진행중인 퀘스트 완료
    public bool compliteProceedingQuest(QuestData questData)
    {
        //첫 메인 퀘스트 후에 모험가 호감도 조절
        if (questData.strCode.Equals("QUEST_000"))
        {
            foreach (AdventurerData data in m_ctrl.adventurerCtrl.getAllAdventurerData())
            {
                data.setLikeAbility(10);
            }
            Debug.Log("모험가 호감도 활성");
        }
        if (questData.getQuestState() == QuestData.QUEST_STATE.진행중)
        {
            
            questData.setQuestState(QuestData.QUEST_STATE.완료);          //완료 처리
            for(int i = 0; i< questData.arrRewardCodes.Count; i++)           //보상 처리
            {
                Debug.Log(questData.arrRewardCodes[i]);
                if (questData.arrRewardCodes[i].Contains("REPUTATION"))
                {
                    m_ctrl.reputationCtrl.addPlayerReputation(questData.arrRewardValues[i]);   //평판 보상
                }
                else if (questData.arrRewardCodes[i].Contains("GOLD"))
                {
                    m_ctrl.gameData.addPlayerGold(questData.arrRewardValues[i]);
                }
                else if (questData.arrRewardCodes[i].Contains("POTION"))
                {
                    //포션 레벨 제한 풀기                    
                    m_ctrl.gameData.setPlayerLv(questData.arrRewardValues[i]);
                    m_ctrl.InventoryCtrl.setNewPotionAndMaterialNoticeActive(); //새 포션 표기
                }
                else if (questData.arrRewardCodes[i].Contains("MATERIAL"))
                {
                    //재료 레벨 제한 풀기
                    m_ctrl.gameData.setPlayerLv(questData.arrRewardValues[i]);
                    m_ctrl.InventoryCtrl.setNewPotionAndMaterialNoticeActive(); //새 재료 표기
                }
            }
            
            switch (questData.eType)        //완료 값에 따른 감소
            {
                case QuestData.QUEST_TYPE.평판:
                    
                    break;
                case QuestData.QUEST_TYPE.골드:
                    m_ctrl.gameData.addPlayerGold(-questData.nCompliteValue);
                    break;
                case QuestData.QUEST_TYPE.물약:
                    PotionData potion = new PotionData(questData.strTargetCode);
                    potion.setPlayersPotionAmount(potion.getPlayersPotionAmount() - questData.nCompliteValue);
                    break;
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public void setQuestState(string strQuestCode, QuestData.QUEST_STATE state)
    {
        foreach(QuestData data in getAllQuestData())
        {
            if (data.strCode.Equals(strQuestCode))
            {
                data.setQuestState(state);
                break;
            }
        }
    }

    public void setQuestState(QuestData questData, QuestData.QUEST_STATE state)
    {
        foreach (QuestData data in getAllQuestData())
        {
            if (data.strCode.Equals(questData.strCode))
            {
                data.setQuestState(state);
                break;
            }
        }
    }

    //퀘스트 시작 (진행 준비 중인 퀘스트 하나를 진행)
    public void startQuest()
    {
        Debug.Log("startQuest");
        //closeWindow();   //퀘스트 창 닫기
        bool bNothingQuest = true;  //퀘스트가 없는지
        QuestData startQuestData = new QuestData();
        foreach(QuestData qData in getAllQuestData())
        {
            if(qData.getQuestState() == QuestData.QUEST_STATE.진행준비)
            {
                startQuestData = qData;
                qData.setQuestState(QuestData.QUEST_STATE.진행중);
                m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.QUEST_START, qData.strCode);
                StartCoroutine(DialogingCoroutine(startQuestData));
                bNothingQuest = false;
                break;
            }
        }
       
    }

    //퀘스트 시작 (진행 준비 중인 퀘스트 하나를 진행)
    public void startQuest(QuestData data)
    {
        Debug.Log("startQuest");
        //closeWindow();   //퀘스트 창 닫기
        data.setQuestState(QuestData.QUEST_STATE.진행중);
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.QUEST_START, data.strCode);
        StartCoroutine(DialogingCoroutine(data));
        
    }


    IEnumerator DialogingCoroutine(QuestData startQuestData)
    {
        while (m_ctrl.dialogCtrl.m_bDialoging)
        {
            yield return new WaitForSeconds(0.5f);
        }
        QuestDetailWindow.GetComponent<QuestDetailObj>().setQuestData(startQuestData);  //퀘스트 상세 내용 띄우기
        QuestDetailWindow.SetActive(true);
        openWindow();
    }

    /*----------------------------------퀘스트 창-----------------------------------*/
    //퀘스트 창 열기
    public void openWindow()
    {
        /*test start*/
        foreach (QuestData data in getAllQuestData())
        {
            //data.setQuestState(QuestData.QUEST_STATE.진행중);
        }
        /*test end*/
        QuestWindow.SetActive(true);
        m_QuestItemCount = 0;
        m_arrItem = new List<GameObject>();

        setScrollContentSize(getProceedingQuest().Count);
        foreach (QuestData data in getProceedingQuest())
        {
            addScrollItem(data);
        }
        RectTransform rectTrans = QuestWindow.GetComponent<RectTransform>();
        float fX = rectTrans.rect.width/2;
        
        //rectTrans.rect.Set(fX, 0, rectTrans.rect.width, rectTrans.rect.height);
        
        //QuestWindow.transform.localPosition = QuestWindow.transform.localPosition + Vector3.right * fWidth/2;
        Debug.Log(QuestWindow.transform.localPosition);
    }

    //퀘스트 창 닫기
    public void closeWindow()
    {
        QuestWindow.SetActive(false);
        //QuestWindow.transform.localPosition = Vector3.right * 1000f;
        clearScrollItem();
    }

    //스크롤에 아이템 추가
    private void addScrollItem(QuestData data)
    {
        GameObject obj = (GameObject)Instantiate(QuestItemPrefeb, Vector3.zero, QuestItemPrefeb.transform.rotation);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        obj.transform.parent = ContentObj.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);

        //content 크기 얻기
        RectTransform content = ContentObj.GetComponent<RectTransform>();
        float contentHeight = content.rect.height;
        float firstItemY = objRect.rect.height / -2;

        //아이템 위치 조정        
        obj.transform.localPosition = new Vector3(objRect.rect.width/2, firstItemY - (objRect.rect.height * m_QuestItemCount++), 0f);

        //아이템 데이터 넣기
        obj.GetComponent<QuestItem>().setItemData(data.strCode);
        m_arrItem.Add(obj);

        //완료 상태
        if (isCompliteCondition(data))
        {
            obj.GetComponent<QuestItem>().setComplite();
        }
    }

    //스크롤 아이템 정리
    private void clearScrollItem()
    {
        m_QuestItemCount = 0;
        if(m_arrItem != null)
        {
            foreach (GameObject obj in m_arrItem)
            {
                Destroy(obj);
            }
            m_arrItem.Clear();
        }
        
    }

    //스크롤뷰 컨텐트 사이즈 설정
    private void setScrollContentSize(int nItemAmount)
    {
        //아이템 height구하기
        RectTransform objRect = QuestItemPrefeb.GetComponent<RectTransform>();

        //content 크기 설정
        RectTransform content = ContentObj.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(0f, objRect.rect.height * nItemAmount);
    }
}
