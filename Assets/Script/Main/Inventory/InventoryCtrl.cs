using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryCtrl : MonoBehaviour {
    public GameObject InventoryObj;
    public GameObject[] InventoryItemObj;
    //public Text PlayerGoldTxt;
    public Button PotionInventoryBtn;
    public Button MaterialInventoryBtn;
    public Button openInventoryBtn;
    public Button closeInventoryBtn;
    private GameData m_gameData;
    private MODE m_inventoryMode;

    //컨트롤
    private AlchemyCtrl m_alchemyCtrl;
    private MaterialCtrl m_materialCtrl;
    private Controller m_ctrl;
    public enum MODE
    {
        MATERIAL, POTION
    }

    // Use this for initialization
    void Start () {
        m_inventoryMode = MODE.MATERIAL;
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_gameData = m_ctrl.gameData;
        m_alchemyCtrl = m_ctrl.AlchemyCtrl;
        m_materialCtrl = m_ctrl.materialCtrl;
        createMaterialInventory();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //연금 인벤토리 생성
    public void createMaterialInventory()
    {
       
        //PlayerGoldTxt.text = m_ctrl.gameData.getPlayerGold() + "G";
        int nMaterialAmount;
        List<MaterialData> arrInvenMaterialData = new List<MaterialData>();

        //가지고 있는 재료 처리
        
        foreach (MaterialData mData in m_materialCtrl.getAllMaterialData()) //재료
        {
           
            //Debug.Log(strCode + ":" + nMaterialAmount);
            if (mData.nMaterialLv <= m_gameData.getPlayerLv())  //재료 레벨
            {
                arrInvenMaterialData.Add(mData);
            }
        }

        
        setMaterialInventory(arrInvenMaterialData);
    }

    //연금 재료 인벤토리 설정
    public void setMaterialInventory(List<MaterialData> mData)
    {
        m_inventoryMode = MODE.MATERIAL;
        Sprite potionInventory = (Sprite)Resources.Load("Image/Main/Inventory/alchemyResource", typeof(Sprite));
        InventoryObj.GetComponent<Image>().sprite = potionInventory;
        MaterialObj invenItemObj;
        for (int i = 0; i < InventoryItemObj.Length; i++)    //디스플레이에 있는 인벤토리 오브젝트들 동안
        {
            InventoryItemObj[i].GetComponent<MaterialObj>().selectMaterialObj.SetActive(false);     //선택 박스 초기화
            if (i < mData.Count)  //채워질 인벤토리이면
            {
                InventoryItemObj[i].SetActive(true);
                invenItemObj = InventoryItemObj[i].GetComponent<MaterialObj>();  //인벤토리 오브젝트에
                invenItemObj.setTitle(mData[i].strName); //이름 달고
                invenItemObj.setGold(mData[i].nMaterialPrice); //골드 달고
                invenItemObj.GoldTxt.gameObject.SetActive(false);   //골드 비활성
                //invenItemObj.setMaterialAmount(mData[i].getPossesionAmount());
                invenItemObj.setItemImage(mData[i].strCode); //이미지 바꾸기
                invenItemObj.setShape(mData[i].arrShape);    //재료 모양 넣기
                invenItemObj.setCode(mData[i].strCode);

            }
            else   //비어있는 인벤토리 자리이면
            {
                InventoryItemObj[i].SetActive(false);
            }
        }
    }

    //연금 포션 인벤토리 설정
    public void createPotionInventory()
    {
        Sprite materialInventory = (Sprite)Resources.Load("Image/Main/Inventory/alchemyPotion", typeof(Sprite));
        m_inventoryMode = MODE.POTION;
        InventoryObj.GetComponent<Image>().sprite = materialInventory;

        int nPotionAmount;
        List<PotionData> arrInvenPotionData = new List<PotionData>();

        //가지고 있는 물약 처리
        foreach (PotionData pData in m_ctrl.potionCtrl.getAllPotionData()) //재료
        {
            nPotionAmount = pData.getPlayersPotionAmount();    //플레이어가 가진재료 수량
            //Debug.Log(strCode + ":" + nMaterialAmount);
            if (nPotionAmount != 0)    //재료 수량이 0이 아니면
            {
                arrInvenPotionData.Add(pData);
            }
        }

        //
        PotionObj invenItemObj;
        for (int i = 0; i < InventoryItemObj.Length; i++)    //디스플레이에 있는 인벤토리 오브젝트들 동안
        {
            InventoryItemObj[i].GetComponent<MaterialObj>().selectMaterialObj.SetActive(false);     //선택 박스 초기화
            if (i < arrInvenPotionData.Count)  //채워질 인벤토리이면
            {
                InventoryItemObj[i].SetActive(true);
                invenItemObj = InventoryItemObj[i].GetComponent<PotionObj>();  //인벤토리 오브젝트에
                invenItemObj.setTitle(arrInvenPotionData[i].strName); //이름 달고
                invenItemObj.setMaterialAmount(arrInvenPotionData[i].getPlayersPotionAmount());
                invenItemObj.setItemImage(arrInvenPotionData[i].strCode); //이미지 바꾸기
            }
            else   //비어있는 인벤토리 자리이면
            {
                InventoryItemObj[i].SetActive(false);
            }
        }

        //보조재료 포인터 비활성
        foreach (GameObject obj in getInventoryItem())
        {
            obj.GetComponent<MaterialObj>().selectPointerImage.SetActive(false);
        }

        //포션 리스트 초기화
        m_alchemyCtrl.createPotionList("");
    }

    public GameObject[] getInventoryItem()
    {
        return InventoryItemObj;
    }

    public MODE getInventoryMode()
    {
        return m_inventoryMode;
    }

    public void setInventoryImageNone()
    {
        Sprite noneInventory = (Sprite)Resources.Load("Image/Main/Inventory/alchemyNone", typeof(Sprite));
        InventoryObj.GetComponent<Image>().sprite = noneInventory;
    }

    public void setPotionInventoryEnabled(bool bEnabled)
    {
        PotionInventoryBtn.enabled = bEnabled;
    }

    public void setMaterialInventoryEnabled(bool bEnabled)
    {
        MaterialInventoryBtn.enabled = bEnabled;
    }

    public void openInventory()
    {
        GameObject.Find("Windows").GetComponent<Image>().enabled = true;
        createMaterialInventory();
        RectTransform rectTrans = InventoryObj.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(-(rectTrans.rect.width / 2) - 25, 0);
        //openBtn.SetActive(false);
    }

    public void closeInventory()
    {
        GameObject.Find("Windows").GetComponent<Image>().enabled = false;
        RectTransform rectTrans = InventoryObj.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.right* 1100f;
        m_ctrl.AlchemyCtrl.closeAlchemy();
        m_ctrl.materialStoreCtrl.closeStore();
    }

    //새 재료, 새 물약 표기
    public void setNewPotionAndMaterialNoticeActive()
    {
        MaterialObj mObj;
        
        foreach (GameObject MaterialItem in InventoryItemObj)
        {
            mObj = MaterialItem.GetComponent<MaterialObj>();
            if (MaterialItem.active)    //활성화된 아이템 중
            {
                if (mObj.m_MaterialData.nMaterialLv == m_ctrl.gameData.getPlayerLv())
                {
                    mObj.NewMaterialTxt.enabled = true;
                }
            }
            
        }
        StartCoroutine(ListnerForNotice());
    }

    //하루가 지나면 새 재료 표시 끄기
    IEnumerator ListnerForNotice()
    {
        int nCurrentDay = m_ctrl.dateCtrl.m_nDate;
        while (nCurrentDay == m_ctrl.dateCtrl.m_nDate)  //하루가 가길 기달
        {
            yield return new WaitForSeconds(0.5f);
        }
        MaterialObj mObj;
        //새 재료 표시 끄기
        foreach (GameObject MaterialItem in InventoryItemObj)
        {
            mObj = MaterialItem.GetComponent<MaterialObj>();
            if (MaterialItem.active)    //활성화된 아이템 중
            {
                if (mObj.m_MaterialData.nMaterialLv == m_ctrl.gameData.getPlayerLv())
                {
                    mObj.NewMaterialTxt.enabled = false;
                }
            }
        }
    }

}
