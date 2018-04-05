using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdventurerObj : MonoBehaviour {
    //public resource
    public Image StateIcon;
    public GameObject SpeachBubble;

    //public value
    public int walkSpeed = 8;
    public bool Pause = false;

    //private
    AdventurerData m_aData;
    PotionData m_potionData;
    private Vector3 m_vDoorPosition;
    private Vector3 m_vTablePositionStart;
    private Vector3 m_vTablePositionEnd;
    private int m_nAmount;

    private StoreCtrl m_storeCtrl;
    private ReputationCtrl m_reputCtrl;
    private Controller m_ctrl;

    public enum ADVENTURE_RESULT
    {
        FAIL, NONE, SUCCESS
    }
	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    //데이터 설정
    public void setAdventurerData(AdventurerData adventurerData, PotionData potionData)
    {
        m_nAmount = 10;
        m_aData = adventurerData;
        m_potionData = potionData;
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_storeCtrl = GameObject.Find("StoreCtrl").GetComponent<StoreCtrl>();
        m_reputCtrl = GameObject.Find("ReputationCtrl").GetComponent<ReputationCtrl>();
    }


    //행동하기
    public void startAction()
    {
        //문 위치
        m_vDoorPosition = GameObject.Find("DoorPositon").transform.position;
        //테이블 위치
        m_vTablePositionStart = GameObject.Find("TablePositionFirst").transform.position;
        m_vTablePositionEnd = GameObject.Find("TablePositionEnd").transform.position;

        //애니메이션
        StartCoroutine(AnimationCoroutine());
    }

    IEnumerator AnimationCoroutine()
    {
        bool bTrade = true; //트레이드 하기(퀘스트 부여 하면 트레이트 하지 않고 그냥 감)

        //말풍선 활성
        SpeachBubble.SetActive(true);

        //상태 아이콘
        
        string strCode = m_potionData.strCode;
        Sprite stateIconSprite;
        stateIconSprite = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/" + strCode, typeof(Sprite));
        StateIcon.sprite = stateIconSprite;

        //애니메이션 켜기
        gameObject.GetComponent<Animator>().enabled = true;

        //테이블로 이동
        float fX = UnityEngine.Random.Range(m_vTablePositionStart.x, m_vTablePositionEnd.x);
        while (transform.position.x < fX)
        {
            if (!Pause)
            {
                transform.Translate(Vector3.right * walkSpeed / 100 * Time.deltaTime, Space.World);
            }
            yield return new WaitForSeconds(0.01f);
        }
        //퀘스트 
        //퀘스트 검색
        Debug.Log(m_aData.strName);
        int nAdventurerLv = m_aData.getAdventurerLv();
        foreach (QuestData qData in m_ctrl.questCtrl.getAllQuestData())
        {
            if (qData.strCharacterCode.Equals(m_aData.strCharCode))
            {
                if (qData.nLv <= nAdventurerLv && qData.getQuestState() == QuestData.QUEST_STATE.비활성)      //모험가 레벨이 퀘스트 충족 조건에 해당이 되고 퀘스트가 수락 되어 있지 않으면
                {
                    qData.setQuestState(QuestData.QUEST_STATE.진행준비);
                    m_ctrl.questCtrl.startQuest(qData);                     //퀘스트 시작하기
                    bTrade = false;
                    SpeachBubble.SetActive(false);
                    qData.nLv = nAdventurerLv;
                    Debug.Log(m_aData.strCharCode + "가 " + qData.strCharacterCode + "의 " + qData.strTitle + " 퀘스트를 얻음");

                    GameObject[] arrAdventurerObj = GameObject.FindGameObjectsWithTag("adventurerObj");
                    List<float> arrAniSpeed = new List<float>();
                    foreach (GameObject obj in arrAdventurerObj)
                    {
                        Debug.Log(obj.name);
                        arrAniSpeed.Add(obj.GetComponent<Animator>().speed);
                        obj.GetComponent<Animator>().speed = 0;
                        obj.GetComponent<AdventurerObj>().Pause = true;
                    }
                    m_ctrl.storeCtrl.StoreTimeLine.GetComponent<StoreTimeCtrl>().Pause = true;
                    while (m_ctrl.dialogCtrl.m_bDialoging)      //대사 중이면
                    {
                        yield return new WaitForSeconds(0.1f);  //기달
                    }
                    for (int i = 0; i < arrAdventurerObj.Length; i++)   //모든 애니메이션 일시정지
                    {
                        arrAdventurerObj[i].GetComponent<Animator>().speed = arrAniSpeed[i];
                        arrAdventurerObj[i].GetComponent<AdventurerObj>().Pause = false;
                    }
                    m_ctrl.storeCtrl.StoreTimeLine.GetComponent<StoreTimeCtrl>().Pause = false;
                    break;
                }
            }

        }
        //트레이드
        if (bTrade)
        {
            

            //서있기
            gameObject.GetComponent<Animator>().SetBool("idle", true);
            yield return new WaitForSeconds(3f);
            while (Pause)
            {
                yield return new WaitForSeconds(0.1f);
            }

            //구매하기
            gameObject.GetComponent<Animator>().SetBool("trade", true);
            gameObject.GetComponent<Animator>().SetBool("idle", false);


            bool bNothing = true;
            foreach (StorePotionItem potionItem in m_storeCtrl.m_arrSellPotionData)      //플레이어가 판매하는 물약 중
            {
                if (potionItem.getPotionType() == m_potionData.nType && potionItem.getPotionSellAmount() > 0)  //모험가가 구매할 물약을 찾아 처리
                {

                    m_nAmount = m_nAmount / 10;        //모험가 레벨에 따라 구매 양 조절(10랩 이하 : 1, 20랩 이하 :2,~~~)
                    int nTotalAmount = potionItem.getPotionSellAmount() - m_nAmount;
                    if (nTotalAmount < 1)
                    {
                        m_nAmount = potionItem.getPotionSellAmount();
                        nTotalAmount = 0;
                    }
                    potionItem.setPotionSellAmount(nTotalAmount);
                    PotionData sellPotionData = new PotionData(potionItem.getPotionCode());
                    sellPotionData.setPlayersPotionAmount(sellPotionData.getPlayersPotionAmount() - m_nAmount);
                    AdventurerAction action = new AdventurerAction();
                    action.nAmount = m_nAmount;
                    action.nPotionTypeNo = potionItem.getPotionType();
                    action.strPotionCode = potionItem.getPotionCode();
                    m_storeCtrl.addTradedItemList(action);
                    //Debug.Log(action.strPotionCode + ":" + action.nAmount);

                    //골드 변경
                    GameData gData = GameObject.Find("GameData").GetComponent<GameData>();
                    gData.addPlayerGold(sellPotionData.nPotionPrice);
                    bNothing = false;
                    break;
                }
            }

            if (!bNothing)  //구매
            {
                if (!m_aData.strCode.Equals("ADVENTURER_DUMMY"))    //더미가 아니면
                {

                    ADVENTURE_RESULT result = getAdventureResult(m_potionData);         //모험 결과
                    Debug.Log(m_aData.strName + "의 모험 : " + result);

                    NewsData newsData;


                    //Debug.Log("Lv:"+nAdventurerLv+", Exp:"+m_aData.getAdventurerExp());
                    int nAddExp = 0;
                    foreach (AdventurerExpData expData in m_ctrl.adventurerCtrl.getAllAdventurerExpTable())
                    {
                        if (nAdventurerLv == expData.nLv - 1)    //레벨 1이면 2레벨 경험치 테이블에서 가져옴. 레벨 100이면 false. 
                        {
                            nAddExp = expData.nAddExp;
                            break;
                        }
                    }

                    switch (result)             //결과에 따라 다음 뉴스에 등장
                    {
                        case ADVENTURE_RESULT.SUCCESS:
                            newsData = m_ctrl.newsCtrl.getCustomNews(NewsData.E_TYPE.모험가, NewsData.E_EFFECT.호감도, NewsData.E_VALUE.증가, m_aData.strCode);
                            m_ctrl.newsCtrl.addNextNews(newsData);
                            m_aData.addAdventurerExp(nAddExp * 2);  //경험치의 두배
                            break;
                        case ADVENTURE_RESULT.NONE:
                            m_aData.addAdventurerExp(nAddExp);  //경험치 상승
                            break;
                        case ADVENTURE_RESULT.FAIL:
                            newsData = m_ctrl.newsCtrl.getCustomNews(NewsData.E_TYPE.모험가, NewsData.E_EFFECT.호감도, NewsData.E_VALUE.감소, m_aData.strCode);
                            m_ctrl.newsCtrl.addNextNews(newsData);
                            break;
                    }

                    if (nAdventurerLv < m_aData.getAdventurerLv())   //경험치 상승으로 레벨이 올랐다면        레벨 증가 감지
                    {
                        Debug.Log(m_aData.strName + "가 lv" + m_aData.getAdventurerLv() + "이 됨.");
                        //뉴스 발행
                        nAdventurerLv = m_aData.getAdventurerLv();
                        newsData = m_ctrl.newsCtrl.getCustomNews(NewsData.E_TYPE.모험가, NewsData.E_EFFECT.레벨, NewsData.E_VALUE.증가, m_aData.strCode);  //레벨 상승 뉴스
                        newsData.nValue = nAdventurerLv;
                        m_ctrl.newsCtrl.addNextNews(newsData);


                    }
                }
                double dAddReputation = (float)m_aData.getAdventurerLv() / 10f;
                m_reputCtrl.addPlayerReputation((int)Math.Ceiling(dAddReputation));     //모험가 레벨에 따라 평판 상승
                stateIconSprite = (Sprite)Resources.Load("Image/Main/Adventurer/Icon/tradeIcon", typeof(Sprite));
            }
            else           //구매 못함.
            {
                stateIconSprite = (Sprite)Resources.Load("Image/Main/Adventurer/Icon/nothingIcon", typeof(Sprite));
            }

            StateIcon.sprite = stateIconSprite;
            yield return new WaitForSeconds(2.5f);
            while (Pause)
            {
                yield return new WaitForSeconds(0.1f);
            }

            //말풍선 비활성
            if (!bNothing)
            {
                SpeachBubble.SetActive(false);
            }
        }
        
            

        //걷기
        gameObject.GetComponent<Animator>().SetBool("walk", true);
        gameObject.GetComponent<Animator>().SetBool("trade", false);
        transform.Rotate(new Vector3(0f, 180f, 0f));

        while (transform.position.x > m_vDoorPosition.x)
        {
            if (!Pause)
            {
                transform.Translate(Vector3.left * walkSpeed / 100 * Time.deltaTime, Space.World);
            }
            
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
        
    }

    
    //물약성능과 모험과 레벨에 따른 모험 결과
    private ADVENTURE_RESULT getAdventureResult(PotionData potion)
    {
        int nAdventurerLv = m_aData.getAdventurerLv();  //모험가 레벨
        int nFailValue = nAdventurerLv;                 //실패 값
        int nNoneValue = nAdventurerLv;                 //중간 값
        int nSuccessValue = nAdventurerLv + potion.nAdventure_success_bonus;    //성공 값
        //Debug.Log("fail:" + nFailValue + ",none:" + nNoneValue + ",success:" + nSuccessValue);
        List<int> arrResultRange = new List<int>();     //결과 범위 리스트
        arrResultRange.Add(nFailValue);
        arrResultRange.Add(nFailValue + nNoneValue);
        arrResultRange.Add(nFailValue + nNoneValue + nSuccessValue);

        int nRandom = UnityEngine.Random.Range(0, arrResultRange[arrResultRange.Count - 1] + 1);
        //Debug.Log("random:" + nRandom);
        foreach(int nRange in arrResultRange)
        {
            if(nRandom <= arrResultRange[nRange])
            {
                switch (nRange)
                {
                    case 0:
                        //실패
                        return ADVENTURE_RESULT.FAIL;
                    case 1:
                        //중간
                        return ADVENTURE_RESULT.NONE;
                    case 2:
                        //성공
                        return ADVENTURE_RESULT.SUCCESS;
                }
            }
        }
        return ADVENTURE_RESULT.NONE;
    }
    

}
