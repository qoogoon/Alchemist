using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DutorialCtrl : MonoBehaviour {
    public GameObject DialogObj;
    public GameObject NewsObj;
    public GameObject AlchemyObj;
    public GameObject FocusBlindObj;
    public GameObject speach_bubble01;

    //버튼
    private GameObject m_NewsBtn;
    private GameObject m_AlchemyBtn;
    private GameObject m_MaterialStoreBtn;
    private GameObject m_AlchemyStoreBtn;
    private GameObject m_QuestBtn;
    private GameObject m_InventoryBtn;
    private GameObject[] m_arrCloseBtnObj;
    private ArrayList m_arrBtnObj;

    private Controller m_ctrl;
    private Vector3 m_vInitFocusBlindScale;
    // Use this for initialization
    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    //가림막 설정
    void createFocusBlind(Vector3 vPosition, float fSize)
    {
        FocusBlindObj.SetActive(true);
        FocusBlindObj.transform.position = vPosition;
        FocusBlindObj.transform.localScale = FocusBlindObj.transform.localScale * fSize;
    }

    void destroyFocusBlind()
    {
        FocusBlindObj.SetActive(false);
        FocusBlindObj.transform.localScale = m_vInitFocusBlindScale;
    }

    //듀토리얼
    public void startTutorial()
    {
        initDutorial();
        StartCoroutine(tutorialProcess());
    }

    private void initDutorial()
    {
        

        //버튼
        m_arrBtnObj = new ArrayList();
        m_NewsBtn = GameObject.Find("NewsBtn");
        m_arrBtnObj.Add(m_NewsBtn);
        m_AlchemyBtn = GameObject.Find("AlchemyBtn");
        m_arrBtnObj.Add(m_AlchemyBtn);
        //m_MaterialStoreBtn = GameObject.Find("MaterialStoreBtn");
        //m_arrBtnObj.Add(m_MaterialStoreBtn);
        m_AlchemyStoreBtn = GameObject.Find("StoreBtn");
        m_arrBtnObj.Add(m_AlchemyStoreBtn);
        m_QuestBtn = GameObject.Find("QuestBtn");
        m_arrBtnObj.Add(m_QuestBtn);
        m_InventoryBtn = GameObject.Find("InventoryBtn");
        m_arrBtnObj.Add(m_InventoryBtn);
        m_arrCloseBtnObj = GameObject.FindGameObjectsWithTag("CloseButton");
        m_ctrl.gameData.setPlayerGold(1000);
        foreach (GameObject obj in m_arrCloseBtnObj)
        {
            m_arrBtnObj.Add(obj);
        }
        //물리제거
        GameObject[] alchemyMetarials = GameObject.FindGameObjectsWithTag("alchemyMetarial");
        foreach(GameObject obj in alchemyMetarials)
        {
            obj.GetComponent<BoxCollider>().enabled = false;
        }
        
        m_vInitFocusBlindScale = FocusBlindObj.transform.localScale;   //초기 포커스 블라인트 크기
    }

    public void setOnlyOneClickAble(GameObject clickAbleBtnObj)
    {
        bool bClickAble = false;
        foreach(GameObject btnObj in m_arrBtnObj)
        {
            if(clickAbleBtnObj != null)
            {
                if (btnObj.name.Equals(clickAbleBtnObj.name))
                {
                    bClickAble = true;
                }
                else
                {
                    bClickAble = false;
                }
            }
            btnObj.GetComponent<Button>().enabled = bClickAble;
        }
    }

    IEnumerator tutorialProcess()
    {


        m_ctrl.PhoneDebug("듀토리얼 시작");
        Debug.Log("dtd");
        //버튼 비활성
        m_ctrl.materialStoreCtrl.MaterialStoreOpenBtn.enabled = false;
        setOnlyOneClickAble(null);

        yield return new WaitForSeconds(2.75f);
        //대사1
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL,  "TUTORIAL_000"); //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())
        {
            yield return new WaitForSeconds(0.01f);
        }

        //뉴스 버튼 포커스
        createFocusBlind(m_NewsBtn.transform.position, 3.6f);

        //뉴스 버튼 활성
        setOnlyOneClickAble(m_NewsBtn);
        m_NewsBtn.GetComponent<Animator>().SetBool("blink", true);

        //뉴스 생성
        List<NewsData> arrNews = new List<NewsData>();
        arrNews.Add(new NewsData("News_000"));
        m_ctrl.newsCtrl.setTodayNews(arrNews);
        //m_ctrl.newsCtrl.createNews(arrNews);

        //뉴스 버튼 누르기를 기다리기
        while (!m_ctrl.newsCtrl.isNewsLooking()) //뉴스 보는 중
        {
            yield return new WaitForSeconds(0.5f);
        }
        m_NewsBtn.GetComponent<Animator>().SetBool("blink", false);
        destroyFocusBlind();


        //뉴스 보기를 기다리거나 닫기 버튼 기다리기
        int nTimeCount = 0;
        while (m_ctrl.newsCtrl.isNewsLooking()) //뉴스 보는 중
        {
            if (nTimeCount > 6) //시간되면
            {
                m_ctrl.newsCtrl.closeNews();    //뉴스창 끄기
                break;
            }
            yield return new WaitForSeconds(0.5f);
            nTimeCount++;
        }
        m_NewsBtn.GetComponent<Button>().enabled = false;

        //대사2
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_001");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        //연금술 버튼 누르기를 기다리기
        createFocusBlind(m_AlchemyBtn.transform.position, 4f);
        m_AlchemyBtn.GetComponent<Button>().enabled = true;
        m_AlchemyBtn.GetComponent<Animator>().SetBool("blink", true);
        while (!m_ctrl.AlchemyCtrl.isAlchemisting()) //연금술 버튼 누르길 기달
        {
            yield return new WaitForSeconds(0.5f);
        }
        m_AlchemyBtn.GetComponent<Animator>().SetBool("blink", false);
        GameObject.Find("PotionList").GetComponent<ScrollRect>().enabled = true;
        m_ctrl.InventoryCtrl.PotionInventoryBtn.interactable = false;
        destroyFocusBlind();

        //대사3
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_002");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        //아이템 누르기
        createFocusBlind(m_ctrl.InventoryCtrl.getInventoryItem()[0].transform.position, 1.4f);
        GameObject[] alchemyMetarials = GameObject.FindGameObjectsWithTag("alchemyMetarial");
        foreach (GameObject obj in alchemyMetarials)
        {
            obj.GetComponent<BoxCollider>().enabled = true;                 //물리 활성
        }
        GameObject.Find("CloseButton").GetComponent<Button>().enabled = false;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("alchemyPotionItem"))
        {
            obj.GetComponent<Button>().enabled = false;   
        }

        //아이템 누리길 기달
        while (!m_ctrl.AlchemyCtrl.dutorialMaterialSelect)
        {
            yield return new WaitForSeconds(0.5f);
        }
        destroyFocusBlind();
        //GameObject.Find("CloseButton").GetComponent<Button>().enabled = true;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("alchemyPotionItem"))
        {
            obj.GetComponent<Button>().enabled = true;
        }

        //대사4
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_003");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        //포션누르기
        GameObject potion1 = null;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("alchemyPotionItem"))
        {
            if (obj.name.Equals("potion1"))
            {
                potion1 = obj;
                createFocusBlind(potion1.transform.position, 1.4f);
            }
            else
            {
                obj.GetComponent<Button>().enabled = false;
            }
        }

        //포션 선택 대기
        AlchemyPotionObj potion_obj = potion1.GetComponent<AlchemyPotionObj>();
        while (!potion_obj.dutorialSelect)
        {
            yield return new WaitForSeconds(0.5f);
        }
        createFocusBlind(m_ctrl.AlchemyCtrl.AlchemyStartBtn.transform.position, 3f);

        //포션 제조가능
        while (!m_ctrl.AlchemyCtrl.dutorialStartFormualBaord)
        {
            yield return new WaitForSeconds(0.5f);
        }
        m_ctrl.AlchemyCtrl.FormulaBoardBackBtn.GetComponent<Button>().enabled = false;
        destroyFocusBlind();

        //대사5
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_004");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        createFocusBlind(m_ctrl.AlchemyCtrl.FormulaResetBtn.transform.position, 1.4f);
        yield return new WaitForSeconds(1.5f);
        destroyFocusBlind();

        //대사6
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_005");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        speach_bubble01.SetActive(true);

        //연금 마지막 리스너
        AlchemyNoticeWindowCtrl anwCtrl = m_ctrl.AlchemyCtrl.NoticeWindowObj.GetComponent<AlchemyNoticeWindowCtrl>();
        while (!anwCtrl.dutorialAgree)   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }
        m_ctrl.AlchemyCtrl.closeAlchemy();
        speach_bubble01.SetActive(false);

        //비활성을 다시 활성화
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("alchemyPotionItem"))
        {
            obj.GetComponent<Button>().enabled = true;
        }
        m_ctrl.AlchemyCtrl.FormulaBoardBackBtn.GetComponent<Button>().enabled = true;
        GameObject.Find("CloseButton").GetComponent<Button>().enabled = true;
        m_NewsBtn.GetComponent<Button>().enabled = false;
        m_AlchemyBtn.GetComponent<Button>().enabled = true;

        //대사7
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_006");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }
        m_ctrl.InventoryCtrl.closeInventory();
        //상점 버튼 클릭
        createFocusBlind(m_AlchemyStoreBtn.transform.position, 2.5f);
        setOnlyOneClickAble(m_AlchemyStoreBtn);
        GameObject StoreBoardObj = GameObject.Find("StoreBoard");
        
        while (!StoreBoardObj.GetComponent<StoreBoardCtrl>().bStoreOpen)   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.25f);
        }
        destroyFocusBlind();

        //대사7
        m_ctrl.dialogCtrl.createDialog(DialogCtrl.E_DIALOG_TYPE.TUTORIAL, "TUTORIAL_007");  //듀토리얼 대사 생성

        while (m_ctrl.dialogCtrl.isDialoging())   //대사 끝날때까지 기달
        {
            yield return new WaitForSeconds(0.01f);
        }

        //퀘스트
        bool bSelling = m_ctrl.storeCtrl.StoreTimeLine.GetComponent<StoreTimeCtrl>().bDutorialSelling;
        //while (!m_ctrl.storeCtrl.StoreTimeLine.GetComponent<StoreTimeCtrl>().bDutorialSelling)   //판매 끝날때 가지 기달
        while(m_ctrl.gameData.getDay() != 2)
        {
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2.75f);

        m_ctrl.questCtrl.setQuestState(m_ctrl.questCtrl.getAllQuestData()[0], QuestData.QUEST_STATE.진행준비);
        m_ctrl.questCtrl.startQuest();

        //버튼 정상화
        m_ctrl.storeCtrl.StoreCloseBtn.enabled = true;
        m_ctrl.InventoryCtrl.closeInventoryBtn.enabled = true;
        m_ctrl.InventoryCtrl.MaterialInventoryBtn.enabled = true;
        m_ctrl.InventoryCtrl.PotionInventoryBtn.enabled = true;
        m_ctrl.AlchemyCtrl.FormulaBoardBackBtn.GetComponent<Button>().enabled = true;
        m_ctrl.InventoryCtrl.PotionInventoryBtn.interactable = true;

        //듀토리얼 비활성
        m_ctrl.gameData.setDutorial(false);

        //인벤토리 물리 활성
        foreach (GameObject obj in alchemyMetarials)
        {
            obj.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
