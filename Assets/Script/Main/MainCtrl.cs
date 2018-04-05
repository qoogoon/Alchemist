using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class MainCtrl : MonoBehaviour {
    public GameObject TestBtn;

    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        init();

        //듀토리얼 조건 검사
        if (m_ctrl.gameData.isDutorial())
        {
            m_ctrl.dutorialCtrl.startTutorial();
        }
    }

    void init()
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
    }
	
	// Update is called once per frame
	void Update () {
        
    }


    public void initGame()
    {
        Debug.Log("초기화");
        //게임 첫시작으로 설정
        Constant.g_ctrl.gameData.setFirstGame(true);

        //시나리오 초기화
        Constant.g_ctrl.gameData.setActNum(0);

        //포션 초기화
        foreach (PotionData data in m_ctrl.potionCtrl.getAllPotionData())
        {
            new PotionData(data.strCode).setPlayersPotionAmount(0);
        }

        //환경 초기화
        PlayerPrefs.SetString("GameEnvironmentCode", "NONE");

        //플레이어 레벨 초기화
        m_ctrl.gameData.setPlayerLv(1);

        //골드 초기화
        m_ctrl.gameData.setPlayerGold(100);

        //날짜 초기화
        m_ctrl.gameData.setDay(1);

        //뉴스 만들기
        NewsCtrl m_nCtrl = m_ctrl.newsCtrl;
        List<NewsData> arrNews = new List<NewsData>();
        arrNews.Add(new NewsData("News_000"));
        arrNews.Add(new NewsData("News_001"));
        m_nCtrl.createNews(arrNews);
        m_nCtrl.clearNextNews();    //다음 뉴스 초기화
        m_nCtrl.clearTodayNews();

        //모험가 호감도 & 경험치 초기화(레벨은 자동으로 경험치에 따라 초기화됨)
        foreach (AdventurerData data in m_ctrl.adventurerCtrl.getAllAdventurerData())
        {
            data.setLikeAbility(0);
            data.setAdventurerExp(0);
        }

        //평판 초기화
        ReputationCtrl rCtrl = GameObject.Find("ReputationCtrl").GetComponent<ReputationCtrl>();
        rCtrl.setPlayerReputation(0);

        //듀토리얼
        m_ctrl.gameData.setDutorial(false);
        /*-----------------------------------추후에 유지할지 말지 결정 해야할 요소들-------------------------------------*/
        //진행 중인 퀘스트 초기화
        foreach (QuestData data in m_ctrl.questCtrl.getAllQuestData())
        {
            data.setQuestState(QuestData.QUEST_STATE.비활성);  
             
        }
    }
}