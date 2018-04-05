using UnityEngine;
using System.Collections;

public class TestCtrl : MonoBehaviour {
    public TestPart testPart;
    public enum TestPart
    {
        Dutorial, Quest1, Quest2
    }
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TestBtn()
    {
        
        switch (testPart)
        {
            case TestPart.Dutorial:
                m_ctrl.mainCtrl.initGame();
                //듀토리얼 활성
                m_ctrl.gameData.setDutorial(true);
                break;
            case TestPart.Quest1:
                setQuest1();
                break;

            case TestPart.Quest2:
                setQuest2();
                break;
        }
    }

    private void setQuest1()
    {
        //초기화
        m_ctrl.mainCtrl.initGame();

        //퀘스트 설정
        QuestData quest1Data = new QuestData("QUEST_000");
        m_ctrl.questCtrl.setQuestState(quest1Data, QuestData.QUEST_STATE.진행중);

        //평판 설정
        m_ctrl.reputationCtrl.setPlayerReputation(12);
    }

    private void setQuest2()
    {
        //초기화
        m_ctrl.mainCtrl.initGame();

        //퀘스트 설정
        QuestData quest1Data = new QuestData("QUEST_000");
        m_ctrl.questCtrl.setQuestState(quest1Data, QuestData.QUEST_STATE.완료);
        m_ctrl.gameData.setPlayerLv(2);

        QuestData quest2Data = new QuestData("QUEST_001");
        m_ctrl.questCtrl.setQuestState(quest2Data, QuestData.QUEST_STATE.진행중);


        //값 설정
        m_ctrl.reputationCtrl.setPlayerReputation(120);
        m_ctrl.gameData.setPlayerGold(2000);
    }
}
