using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoreSoldCtrl : MonoBehaviour {
    public GameObject Content;
    public GameObject ScrollView;
    public GameObject itemPrefeb;
    public Text TotalSoldGoldText;
    private Controller m_ctrl;
    private List<GameObject> m_arrItemObj;
	// Use this for initialization
	void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void createItem(List<AdventurerAction> actionList)
    {
        setScrollViewContentSize(actionList.Count);

        int nCount = 0;
        int nTotalSoldGold = 0;
        m_arrItemObj = new List<GameObject>();
        foreach (AdventurerAction action in actionList)
        {
            GameObject obj = (GameObject)Instantiate(itemPrefeb, Vector3.zero, itemPrefeb.transform.rotation);
            RectTransform objRect = obj.GetComponent<RectTransform>();
            obj.transform.parent = Content.transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = new Vector3((objRect.rect.width / 2 + 5) +(objRect.rect.width +5) * nCount, 0, 0f);
            nCount++;
            Debug.Log(obj.transform.localPosition);
            obj.GetComponent<StoreSoldItem>().setPotionItem(action);
            PotionData pData = new PotionData(action.strPotionCode);
            nTotalSoldGold += action.nAmount * pData.nPotionPrice;
            m_arrItemObj.Add(obj);
        }
        TotalSoldGoldText.text = nTotalSoldGold.ToString() + "G";
       
    }

    //판매 스크롤뷰 컨텐트 사이즈 설정
    private void setScrollViewContentSize(int nItemAmount)
    {
        //아이템 height구하기
        RectTransform objRect = itemPrefeb.GetComponent<RectTransform>();

        //스크롤 뷰 width구하기
        RectTransform scrollviewRect = ScrollView.GetComponent<RectTransform>();
        float fScrollViewWidth = scrollviewRect.rect.width;

        //content 크기 설정
        RectTransform content = Content.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2((objRect.rect.width + 5) * nItemAmount, scrollviewRect.rect.height);
       
    }

    public void close()
    {
        //창닫기
        gameObject.SetActive(false);
        //버튼 활성화
        m_ctrl.setOpenBtnEnable(true);
        //초기화
        foreach(GameObject obj in m_arrItemObj)
        {
            Destroy(obj);
        }
        m_arrItemObj.Clear();
        //버튼 활성
        m_ctrl.AlchemyCtrl.OpenStoreWindowBtn.interactable = true;
        m_ctrl.newsCtrl.NewsOpenBtn.interactable = true;
        m_ctrl.storeCtrl.StoreStartBtn.interactable = true;

    }
}
