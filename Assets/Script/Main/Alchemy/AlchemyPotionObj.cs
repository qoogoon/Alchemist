using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlchemyPotionObj : MonoBehaviour {
    public Text potionNameText;
    public GameObject PotionDetailObj;
    public GameObject SelectBoxObj;
    public GameObject materialLackText;
    public Text AmountTxt;
    private string m_strPotionCode;
    private Controller m_ctrl;
    private Text m_StateText;

    public bool dutorialSelect = false; //듀토리얼에서만 씀. 선택 여부

    public enum E_STATE_MODE
    {
        SATISFY, MATERIAL_UNSATISFY, LEVEL_UNSATISFY
    }
    E_STATE_MODE m_mode;
    // Use this for initialization
    void Start () {
        m_mode = E_STATE_MODE.LEVEL_UNSATISFY;
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_strPotionCode = "NONE";
        m_StateText = GameObject.Find("AlchemyStateText").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public E_STATE_MODE getPotionStateMode()
    {
        return m_mode;
    }
    public void setPotionStateMode(E_STATE_MODE mode)
    {
        m_mode = mode;
        Color32 nameTextColor = potionNameText.color;
        Color32 potionImageColor = gameObject.GetComponent<Image>().color;

        PotionData pData = new PotionData(m_strPotionCode);
        int nAmount = pData.getPlayersPotionAmount();

        switch (mode)
        {
            case E_STATE_MODE.SATISFY:
                potionNameText.color = new Color32(nameTextColor.r, nameTextColor.g, nameTextColor.b, 255);
                gameObject.GetComponent<Image>().color = new Color32(potionImageColor.r, potionImageColor.g, potionImageColor.b, 255);
                materialLackText.SetActive(false);
                AmountTxt.text = nAmount.ToString();
                break;

            case E_STATE_MODE.MATERIAL_UNSATISFY:
                potionNameText.color = new Color32(nameTextColor.r, nameTextColor.g, nameTextColor.b, 150);
                gameObject.GetComponent<Image>().color = new Color32(potionImageColor.r, potionImageColor.g, potionImageColor.b, 150);
                materialLackText.SetActive(true);
                AmountTxt.text = nAmount.ToString();
                break;

            case E_STATE_MODE.LEVEL_UNSATISFY:
                potionNameText.color = new Color32(nameTextColor.r, nameTextColor.g, nameTextColor.b, 255);
                gameObject.GetComponent<Image>().color = new Color32(potionImageColor.r, potionImageColor.g, potionImageColor.b, 255);
                potionNameText.text = "";
                materialLackText.SetActive(false);
                AmountTxt.text = "";
                break;
        }
    }

    public string getPotionCode()
    {
        return m_strPotionCode;
    }

    public void setPotionName(string strName)
    {
        potionNameText.text = strName;
    }

    public void setPotionImage(Sprite potionSprite)
    {
        gameObject.GetComponent<Image>().sprite = potionSprite;
    }

    public void setPotionCode(string strCode)
    {
        m_strPotionCode = strCode;
        
    }
    public GameObject getSelectBoxImageObj()
    {
        return SelectBoxObj;
    }
    public void selectPotion(GameObject item)
    {
        dutorialSelect = true;
        AlchemyPotionObj potionObjScript = item.GetComponent<AlchemyPotionObj>();
        string strPotionCode = potionObjScript.getPotionCode();
        GameObject[] arrPotionList = GameObject.FindGameObjectsWithTag("alchemyPotionItem");
        foreach(GameObject potionItem in arrPotionList)
        {
            if(potionItem.name.Equals(item.name))
            {
                potionItem.GetComponent<AlchemyPotionObj>().SelectBoxObj.SetActive(true);
            }else
            {
                potionItem.GetComponent<AlchemyPotionObj>().SelectBoxObj.SetActive(false);
            }
        }

        //재료 선택끄기
        PotionData pData = new PotionData(strPotionCode);
        MaterialObj mObj;
        foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
        {
            mObj = obj.GetComponent<MaterialObj>();
            mObj.setVisiablePointer(false);
        }

        //재료 선택 켜기
        switch (potionObjScript.getPotionStateMode())
        {
            case E_STATE_MODE.SATISFY:
                //시작 버튼 활성화
                m_ctrl.AlchemyCtrl.setStartBtnText("물약 제조하기");
                m_ctrl.AlchemyCtrl.startBtnActive(true);

                //디테일 정보
                PotionDetailObj.SetActive(true);
                PotionDetailObj.GetComponent<AlchemyPotionDetialObj>().showPotionDetialData(strPotionCode);

                m_ctrl.AlchemyCtrl.setMakePotion(true);    //포션을 만들 수 있음
                m_ctrl.AlchemyCtrl.setSelectPotionCode(strPotionCode);

                //포인터 이미지 설정
                foreach(string strNeedMaterialCode in pData.arrNeedMaterialCodes)
                {
                    foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
                    {
                        mObj = obj.GetComponent<MaterialObj>();
                        if (mObj.getCode().Equals(strNeedMaterialCode))
                        {
                            mObj.setVisiablePointer(true);
                        }
                    }
                }
                break;

            case E_STATE_MODE.MATERIAL_UNSATISFY:
                //시작 버튼 비활성화
                m_ctrl.AlchemyCtrl.setStartBtnText("재료 부족");
                m_ctrl.AlchemyCtrl.startBtnActive(false);

                //
                PotionDetailObj.SetActive(true);
                PotionDetailObj.GetComponent<AlchemyPotionDetialObj>().showPotionDetialData(strPotionCode);

                m_ctrl.AlchemyCtrl.setMakePotion(false);    //포션을 만들 수 없음
                m_ctrl.AlchemyCtrl.setSelectPotionCode(strPotionCode);

                //포인터 이미지 설정
                foreach (string strNeedMaterialCode in pData.arrNeedMaterialCodes)
                {
                    foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
                    {
                        mObj = obj.GetComponent<MaterialObj>();
                        if (mObj.getCode().Equals(strNeedMaterialCode))
                        {
                            mObj.setVisiablePointer(true);
                        }
                    }
                }
                break;

            case E_STATE_MODE.LEVEL_UNSATISFY:
                //시작 버튼 비활성화
                m_ctrl.AlchemyCtrl.setStartBtnText("잠김");
                m_ctrl.AlchemyCtrl.startBtnActive(false);

                //
                m_ctrl.AlchemyCtrl.setMakePotion(false);
                PotionDetailObj.SetActive(false);
                m_StateText.text = "Locking";
                break;
        }        
    }
}
