using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoreCtrl : MonoBehaviour {
    public GameObject StoreSettingBoardObj;
    public GameObject StoreTimeLine;
    public GameObject SelesBoard;
    public Button StoreStartBtn;
    public Button StoreCloseBtn;

    //컨트롤러
    private StoreBoardCtrl boardCtrl;
    private AdventurerCtrl adventCtrl;
    private Controller m_ctrl;

    //데이터
    public List<StorePotionItem> m_arrSellPotionData;  //판매할 포션 배열
    private ArrayList m_arrBuyMaterialData; //구매할 재료 배열
    private List<AdventurerAction> m_arrTradedItemList; //트레이드 된 아이템 리스트

    // Use this for initialization
    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        adventCtrl = GameObject.Find("AdventurerCtrl").GetComponent<AdventurerCtrl>();
        boardCtrl = StoreSettingBoardObj.GetComponent<StoreBoardCtrl>();
        m_arrTradedItemList = new List<AdventurerAction>();
        m_arrSellPotionData = new List<StorePotionItem>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
     
   

    //판매시작
    public void startSales()
    {
        //모험가 생성(행동을 하나씩 넣는다)
        List<GameObject> adventurerList = adventCtrl.createAdventurer();

        //버튼 비활성
        m_ctrl.AlchemyCtrl.OpenStoreWindowBtn.interactable = false;
        m_ctrl.newsCtrl.NewsOpenBtn.interactable = false;
        m_ctrl.storeCtrl.StoreStartBtn.interactable = false;

        //가게 시작!!!
        m_arrTradedItemList.Clear();    //구매한 품목 스택 초기화
        StoreTimeLine.SetActive(true);
        StoreTimeCtrl timeCtrl = StoreTimeLine.GetComponent<StoreTimeCtrl>();
        m_arrSellPotionData = boardCtrl.getSellPotionList();
        Debug.Log("방문 모험가 수 : "+adventurerList.Count+"명");
        timeCtrl.startTimeLine(adventurerList);
    }
    

    //트레이드 된 아이템을 리스트에 추가
    public void addTradedItemList(AdventurerAction tradedAction)
    {
        m_arrTradedItemList.Add(tradedAction);
    }

    //트레이드 된 아이템 정리해서 보내기
    private List<AdventurerAction> getTradedItemList()
    {
        List<AdventurerAction> resultActionList = new List<AdventurerAction>();
        bool bCreate = true;
        AdventurerAction searchedResultAction = new AdventurerAction();
        foreach (AdventurerAction action in m_arrTradedItemList)
        {
            foreach (AdventurerAction resultAction in resultActionList)
            {
                if (action.strPotionCode.Equals(resultAction.strPotionCode))
                {
                    bCreate = false;
                    searchedResultAction = resultAction;
                    break;
                }
            }

            if (bCreate)
            {
                AdventurerAction createAction = new AdventurerAction();
                createAction.strPotionCode = action.strPotionCode;
                createAction.nAmount = action.nAmount;
                createAction.nPotionTypeNo = action.nPotionTypeNo;
                resultActionList.Add(createAction);
            }else
            {
                bCreate = true;
                searchedResultAction.nAmount += action.nAmount;
            }
        }
        return resultActionList;
    }

    public void ClosingStore()
    {
        GameObject.Find("Windows").GetComponent<Image>().enabled = false;
        SelesBoard.SetActive(true);
        int nIncomeGold = 0;
        int nSpendGold = 0;

        StoreSoldCtrl soldCtrl = SelesBoard.GetComponent<StoreSoldCtrl>();
        soldCtrl.createItem(getTradedItemList());

        /*
        foreach (AdventurerAction action in m_arrTradedItemList)
        {
            PotionData pData = new PotionData(action.strPotionCode);
            int nPrice = pData.getPotionPrice();
            
            nIncomeGold += (action.nAmount * nPrice);
        }
        */
        //Debug.Log(nIncomeGold);
    }
}
