using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AlchemyNoticeWindowCtrl : MonoBehaviour {
    //public resource
    public GameObject PotionNameText;
    public Text ButtonText;
    public Text CreateAmountText;
    public Text PharmacyGoldText;
    public Text PlayerGoldText;
    public Image PotionImage;
    public GameObject NoticeTextObj;
    public GameObject AmountUI;
    public GameObject ButtonObj;

    //public value
    public bool dutorialAgree = false;
    
    //private
    private int m_nCreateAmount;    //제작 수량
    private string m_strPotionCode;
    private bool m_bComplete;   //완성
    private bool m_bAmount;
    private int m_nPharmacyMaterialGold;    //제조재료값
    private int m_nPlayerGoldTmp;   //플레이어 골드 임시 저장(확정된 값은 아님. 제조를 완료 할 경우 플레이어 골드를 변수 값만큼 차감)

    //컨트롤러
    private Controller m_ctrl;
    // Use this for initialization
    void Start () {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //포션 수량 창 열기
    public void createPotionAmountWindow(string strCode)
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_ctrl.AlchemyCtrl.FormulaResetBtn.GetComponent<Button>().enabled = false;
        
        //Data 초기화
        m_nCreateAmount = 0;
        m_strPotionCode = strCode;
        m_bComplete = false;
        PotionData pData = new PotionData(strCode);
        PotionNameText.GetComponent<Text>().text = pData.strName;
        m_nPharmacyMaterialGold = m_ctrl.AlchemyCtrl.m_nPharmacyMaterialsGoldAmount;    //제조 재료 값
        m_nPlayerGoldTmp = m_ctrl.gameData.getPlayerGold(); //플레이어 소지금 임시 저장
        


        //UI
        if (m_nPharmacyMaterialGold <= m_nPlayerGoldTmp)     //소지금이 있으면 
        {
            m_nPlayerGoldTmp -= m_nPharmacyMaterialGold * m_nCreateAmount;
            NoticeTextObj.SetActive(false);
            AmountUI.SetActive(true);
            //ButtonObj.GetComponent<Button>().interactable = true;
        }
        else           //소지금이 부족하면
        {
            AmountUI.SetActive(false);
            NoticeTextObj.SetActive(true);
            NoticeTextObj.GetComponent<Text>().text = "소지금 부족";
            ButtonObj.GetComponent<Button>().interactable = false;
        }

        gameObject.SetActive(true);
        PotionImage.sprite = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/" + m_strPotionCode, typeof(Sprite));
        CreateAmountText.text = m_nCreateAmount.ToString();
        PlayerGoldText.text = m_nPlayerGoldTmp.ToString();
        PharmacyGoldText.text = (m_nCreateAmount * m_nPharmacyMaterialGold).ToString();

        
    }


    //제조버튼 클릭시
    public void createPotion()
    {
        if (!m_bComplete && m_nCreateAmount > 0)   //제조하기
        {
            StartCoroutine(clickAnimationCoroutine());
        }
        else                //확인하기
        {
            closeWindow();
        }        
    }

    //포션 생성 버튼 클릭 애니메이션
    IEnumerator clickAnimationCoroutine()
    {
        //애니메이션
        gameObject.GetComponent<Animator>().SetBool("click", true);
        ButtonObj.GetComponent<Button>().enabled = false;

        yield return new WaitForSeconds(1f);

        m_bComplete = true;
        AmountUI.SetActive(false);
        gameObject.GetComponent<Animator>().SetBool("click", false);
        ButtonObj.GetComponent<Button>().enabled = true;
        ButtonText.text = "확인";
        NoticeTextObj.SetActive(true);
        NoticeTextObj.GetComponent<Text>().text = m_nCreateAmount+ "개 완성.";
        potionAmountProcess();
    }

    //포션 갯수 처리
    private void potionAmountProcess()
    {
        MaterialData mData;
        string strDuplicateObjCode;
        string strInvetoryObjCode;
        
        //포션 수 조절
        PotionData pData = new PotionData(m_strPotionCode);
        pData.setPlayersPotionAmount(pData.getPlayersPotionAmount() + m_nCreateAmount);

        //골드 소비
        m_ctrl.gameData.addPlayerGold(-m_nCreateAmount * m_nPharmacyMaterialGold);
        
    }

    public void closeWindow()
    {
        m_ctrl.AlchemyCtrl.FormulaResetBtn.GetComponent<Button>().enabled = true;
        gameObject.SetActive(false);        //창 끄기
        m_ctrl.AlchemyCtrl.closeFormulaBoard();  //공식판 끄기
        dutorialAgree = true;
    }

    //제작할 수량 조절
    public void addCreateAmount()
    {
        

        StartCoroutine(addCoroutine());
    }

    IEnumerator addCoroutine()
    {

        m_bAmount = true;

        
        while (m_bAmount)
        {
            if(m_nPharmacyMaterialGold <= m_nPlayerGoldTmp)
            {
                m_nCreateAmount++;
                m_nPlayerGoldTmp -= m_nPharmacyMaterialGold;
                CreateAmountText.text = m_nCreateAmount.ToString();
                PharmacyGoldText.text = (m_nCreateAmount * m_nPharmacyMaterialGold).ToString();
                PlayerGoldText.text = m_nPlayerGoldTmp.ToString();
            }
            
            yield return new WaitForSeconds(0.1f);
        }

        if(m_nCreateAmount > 0)
        {
            ButtonObj.GetComponent<Button>().interactable = true;
        }
        m_bAmount = false;
    }
    public void stopMaterialAmount()
    {
        m_bAmount = false;
    }

    //
    public void reduceCreateAmount()
    {
        StartCoroutine(reduceCoroutine());
    }

    IEnumerator reduceCoroutine()
    {
        m_bAmount = true;
        while (m_bAmount)
        {
            if (m_nCreateAmount > 1)
            {
                m_nCreateAmount--;
                m_nPlayerGoldTmp += m_nPharmacyMaterialGold;
                CreateAmountText.text = m_nCreateAmount.ToString();
                PharmacyGoldText.text = (m_nCreateAmount * m_nPharmacyMaterialGold).ToString();
                PlayerGoldText.text = m_nPlayerGoldTmp.ToString();
            }

            yield return new WaitForSeconds(0.1f);
        }
        if (m_nCreateAmount == 0)
        {
            ButtonObj.GetComponent<Button>().interactable = false;
        }
        m_bAmount = false;
    }

}
