using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StorePotionItem : MonoBehaviour {
    public Image potionImage;
    public Text potionNameText;
    public Text potionPriceText;
    public Text potionAmountText;
    

    private string m_strPotionCode;
    private int m_nPotionSellAmount;
    private int m_nPotionTotalAmount;
    private int m_nPotionPrice;
    private bool m_bAmount;
    private GameObject m_storeBoardObj;
    private StoreBoardCtrl m_boardCtrl;
    private int m_nType;
	// Use this for initialization
	void Start () {
        m_bAmount = false;
        m_storeBoardObj = GameObject.Find("StoreBoard");
        m_boardCtrl = m_storeBoardObj.GetComponent<StoreBoardCtrl>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /* 포션 */
    //아이템의 데이터 설정
    public void setItemData(string strPotionCode)
    {
        m_strPotionCode = strPotionCode;
        PotionData pData = new PotionData(m_strPotionCode);

        //포션 이미지
        Sprite potionSprite = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/"+ strPotionCode, typeof(Sprite));
        potionImage.sprite = potionSprite;

        //포션 이름
        potionNameText.text = pData.strName;

        //포션 가격
        potionPriceText.text = pData.nPotionPrice + "G";

        //포션 수량
        m_nPotionPrice = pData.nPotionPrice;
        m_nPotionTotalAmount = pData.getPlayersPotionAmount();
        potionAmountText.text = m_nPotionTotalAmount.ToString();
        m_nPotionSellAmount = m_nPotionTotalAmount;

        //포션 타입
        m_nType = pData.nType;
    }

    public void addPotionAmount()
    {
        StartCoroutine(addCoroutine());
    }
    IEnumerator addCoroutine()
    {
        m_bAmount = true;
        while (m_bAmount && m_nPotionSellAmount < m_nPotionTotalAmount)
        {
            potionAmountText.text = (++m_nPotionSellAmount).ToString();
            m_boardCtrl.addTotalSellAmount(m_nPotionPrice);
            yield return new WaitForSeconds(0.1f);
        }
        m_bAmount = false;
    }

    public void reducePotionAmount()
    {
        StartCoroutine(reduceCoroutine());
    }

    IEnumerator reduceCoroutine()
    {
        m_bAmount = true;
        while (m_bAmount && m_nPotionSellAmount > 0)
        {
            potionAmountText.text = (--m_nPotionSellAmount).ToString();
            m_boardCtrl.addTotalSellAmount(-m_nPotionPrice);
            yield return new WaitForSeconds(0.1f);
        }
        m_bAmount = false;
    }

    public void stopPotionAmount()
    {
        m_bAmount = false;
    }

    //포션 코드
    public string getPotionCode()
    {
        return m_strPotionCode;
    }

    //포션 갯수
    public int getPotionSellAmount()
    {
        return m_nPotionSellAmount;
    }
    
    public void setPotionSellAmount(int nPotionAmount)
    {
        m_nPotionSellAmount = nPotionAmount;
    }

    public int getPotionType()
    {
        return m_nType;
    }
}
