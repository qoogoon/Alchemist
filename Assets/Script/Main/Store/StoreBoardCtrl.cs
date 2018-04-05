using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoreBoardCtrl : MonoBehaviour {
    //public resource
    public GameObject SellScrollViewContentObj;     //판매 스크롤뷰 컨텐트
    public GameObject SellItemPrefeb;               //판매 아이템 프리펩
    public Text SellTotalGoldText;
    public Text PlayerGoldText;
    public Text NothingInventoryText;               //재고 없음 텍스트
    public Button OpenButton;                       //개점 버튼

    //private resource
    private Controller m_ctrl;

    //private value
    private int m_nSellItemCount;                   //판매 아이템 카운터
    private int m_nSellTotalGold;
    private int m_nPlayerGold;                      //플레이어 돈
    private List<GameObject> arrItems;
    public bool bStoreOpen;

    // Use this for initialization
    void Start () {
        //init
        bStoreOpen = false;
        m_nSellItemCount = 0;
        m_nSellTotalGold = 0;
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //상점판 생성
    public void openStoreBoard()
    {
        GameObject.Find("Windows").GetComponent<Image>().enabled = true;
        bStoreOpen = true;
        RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0, 0);
        //초기화
        GameData gameData = GameObject.Find("GameData").GetComponent<GameData>();
        arrItems = new List<GameObject>();
        arrItems.Clear();

        //포션 값 초기화
        foreach (PotionData potionData in m_ctrl.potionCtrl.getAllPotionData())
        {
            m_nSellTotalGold += (potionData.getPlayersPotionAmount() * potionData.nPotionPrice);
        }
        SellTotalGoldText.text = m_nSellTotalGold + "G";

        //버튼 비활성
        m_ctrl.AlchemyCtrl.AlchemyStartBtn.GetComponent<Button>().enabled = false;
        m_ctrl.newsCtrl.NewsOpenBtn.enabled = false;
        m_ctrl.storeCtrl.StoreStartBtn.enabled = false;
        m_ctrl.mainCtrl.TestBtn.GetComponent<Button>().enabled = false;
        m_ctrl.materialStoreCtrl.MaterialStoreOpenBtn.enabled = false;
        m_ctrl.questCtrl.QuestBtn.enabled = false;
        

        //보유 하고 있는 포션을 스크롤 뷰에 추가
        ArrayList arrHavePotionCodes = new ArrayList();
        int nPotionCount = 0;
        foreach(PotionData potionData in m_ctrl.potionCtrl.getAllPotionData())       //모든 포션들 중
        {
            if(potionData.getPlayersPotionAmount() > 0)                     //가지고 있는 포션이 한개 이상 있다면
            {
                nPotionCount++;
                arrHavePotionCodes.Add(potionData.strCode);                      //아이템에 추가될 코드 저장
            }
        }
        setSellScrollViewContentSize(arrHavePotionCodes.Count);             //content obj 크기 조절

        foreach(string strCode in arrHavePotionCodes)
        {
            addSellItem(strCode);
        }

        //판매할 포션이 없으면
        
        if(nPotionCount == 0)
        {
            NothingInventoryText.enabled = true;
            OpenButton.interactable = false;

        }
        else
        {
            NothingInventoryText.enabled = false;
            OpenButton.interactable = true;
        }

        
    }
    /* 판매 */
    //판매 아이템 추가하기
    private void addSellItem(string strCode)
    {
        GameObject obj = (GameObject)Instantiate(SellItemPrefeb, Vector3.zero, SellItemPrefeb.transform.rotation);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        obj.transform.parent = SellScrollViewContentObj.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);

        //content 크기 얻기
        RectTransform content = SellScrollViewContentObj.GetComponent<RectTransform>();
        float contentHeight = content.rect.height;
        float firstItemY = objRect.rect.height / -2;
        float firstItemX = objRect.rect.width / 2;

        //아이템 위치 조정        
        obj.transform.localPosition = new Vector3(firstItemX, firstItemY - (objRect.rect.height * m_nSellItemCount++), 0f);

        //아이템 데이터 넣기
        obj.GetComponent<StorePotionItem>().setItemData(strCode);
        arrItems.Add(obj);
    }

    //판매 스크롤뷰 컨텐트 사이즈 설정
    private void setSellScrollViewContentSize(int nItemAmount)
    {
        //아이템 height구하기
        RectTransform objRect = SellItemPrefeb.GetComponent<RectTransform>();

        //content 크기 설정
        RectTransform content = SellScrollViewContentObj.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(0f, objRect.rect.height * nItemAmount);
    }

    //총 판매 가격
    public void addTotalSellAmount(int nSellPrice)
    {
        m_nSellTotalGold += nSellPrice;
        SellTotalGoldText.text = m_nSellTotalGold + "G";
    }


    //총 판매할 포션 품목
    public List<StorePotionItem> getSellPotionList()
    {
        //판매할 포션 데이터
        List<StorePotionItem> arrSellPotionData = new List<StorePotionItem>();
        StorePotionItem potionItem;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("storeSellItem"))
        {
            potionItem = obj.GetComponent<StorePotionItem>();
            if(potionItem.getPotionSellAmount() > 0)
            {
                arrSellPotionData.Add(potionItem);
            }
            
        }
        return arrSellPotionData;
    }

    //총 구매할 재료 품목
    public List<StoreMaterialItem> getBuyMaterialList()
    {
        //구매할 포션 데이터
        List<StoreMaterialItem> arrBuyMaterialData = new List<StoreMaterialItem>();
        StoreMaterialItem materialItem;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("storeBuyItem"))
        {
            materialItem = obj.GetComponent<StoreMaterialItem>();
            if (materialItem.getMaterialSellAmount() > 0)
            {
                arrBuyMaterialData.Add(materialItem);
            }
            
        }
        return arrBuyMaterialData;
    }

    public void closeStoreBoard()
    {
        GameObject.Find("Windows").GetComponent<Image>().enabled = false;
        bStoreOpen = false;
        m_nSellItemCount = 0;
        m_nSellTotalGold = 0;
        RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.right * 1200f;
        m_ctrl.setOpenBtnEnable(true);
        foreach (GameObject ItemObj in arrItems)
        {
            Destroy(ItemObj);
        }
    }
}
