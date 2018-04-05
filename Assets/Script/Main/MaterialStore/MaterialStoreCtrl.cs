using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MaterialStoreCtrl : MonoBehaviour {
    public GameObject BuyScrollViewContentObj;      //구매 스크롤뷰 컨텐트
    public GameObject BuyItemPrefeb;               //판매 아이템 프리펩
    public GameObject MaterialStoreWindow;
    public Button MaterialStoreOpenBtn;
    public Text BuyGoldText;
    private int m_nBuyItemCount;
    private int m_nBuyTotalGold;                    //구매할 총 금액
    private int m_nPlayerGold;
    private List<GameObject> arrItems;
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        m_nBuyItemCount = 0;
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        arrItems = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void openStore()
    {
        //초기화
        arrItems.Clear();
        m_nBuyItemCount = 0;

        RectTransform rectTrans = MaterialStoreWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(rectTrans.rect.width / 2, 0);
        GameData gData = GameObject.Find("GameData").GetComponent<GameData>();
        MaterialCtrl mCtrl = GameObject.Find("MaterialCtrl").GetComponent<MaterialCtrl>();
        //소지금 표기
        m_nPlayerGold = gData.getPlayerGold();
        BuyGoldText.text = 0 + "G";

        //아이템에 넣을 포션
        int nPlayerLv = gData.getPlayerLv();
        List<string> arrAddMaterialCode = new List<string>();
        foreach(MaterialData mData in mCtrl.getAllMaterialData())
        {
            if(mData.nMaterialLv <= nPlayerLv)
            {
                arrAddMaterialCode.Add(mData.strCode);
            }
        }

        setBuyScrollViewContentSize(arrAddMaterialCode.Count);
        //Debug.Log(arrAddMaterialCode.Count);
        foreach(string strCode in arrAddMaterialCode)
        {
            addBuyItem(strCode);
        }
    }
    /* 구매 */
    //구매 아이템 추가하기
    private void addBuyItem(string strCode)
    {
        GameObject obj = (GameObject)Instantiate(BuyItemPrefeb, Vector3.zero, BuyItemPrefeb.transform.rotation);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        obj.transform.parent = BuyScrollViewContentObj.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);

        //content 크기 얻기
        RectTransform content = BuyScrollViewContentObj.GetComponent<RectTransform>();
        float contentHeight = content.rect.height;
        float firstItemY = objRect.rect.height / -2;
        float firstItemX = objRect.rect.width / 2;
        //아이템 위치 조정        
        obj.transform.localPosition = new Vector3(firstItemX, firstItemY - (objRect.rect.height * m_nBuyItemCount++), 0f);

        //아이템 데이터 넣기
        obj.GetComponent<StoreMaterialItem>().setItemData(strCode);
        arrItems.Add(obj);
    }

    //구매 스크롤뷰 컨텐트 사이즈 설정
    private void setBuyScrollViewContentSize(int nItemAmount)
    {
        //아이템 height구하기
        RectTransform objRect = BuyItemPrefeb.GetComponent<RectTransform>();

        //content 크기 설정
        RectTransform content = BuyScrollViewContentObj.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(0f, objRect.rect.height * nItemAmount);
    }

    //총 구매 가격
    public void addTotalBuyAmountSum(int nBuyPrice)
    {
        m_nBuyTotalGold += nBuyPrice;
        BuyGoldText.text = m_nBuyTotalGold + "G";
        //Debug.Log(m_nBuyTotalGold + "G");
    }

    public int getTotalBuyAmountSum()
    {
        return m_nBuyTotalGold;
    }

    //아이템 구매
    public void buyMaterialItem()
    {
        foreach(GameObject obj in arrItems)
        {
            StoreMaterialItem item = obj.GetComponent<StoreMaterialItem>();

            MaterialData data = new MaterialData(item.getMaterialCode());
            //data.setPossesionAmount(data.getPossesionAmount() + item.getMaterialSellAmount());
        }
        m_ctrl.gameData.addPlayerGold(-m_nBuyTotalGold);

        //초기화
        m_nBuyTotalGold = 0;
        BuyGoldText.text = "0G";
        foreach(GameObject obj in arrItems)
        {
            obj.GetComponent<StoreMaterialItem>().setMaterialSellAmount(0);
            obj.GetComponent<StoreMaterialItem>().MaterialAmountText.text = "0";
        }
    }

    public void closeStore()
    {
        RectTransform rectTrans = MaterialStoreWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.left * 900f;
        foreach (GameObject ItemObj in arrItems)
        {
            Destroy(ItemObj);
        }
    }
}
