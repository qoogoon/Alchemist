using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreMaterialItem : MonoBehaviour {
    public Image MaterialImage;
    public Text MaterialNameText;
    public Text MaterialPriceText;
    public Text MaterialAmountText;
    private string m_strMaterialCode;
    private int m_nMaterialPrice;
    private bool m_bAmount;
    private int m_nMaterialSellAmount;
    private MaterialStoreCtrl m_MaterialStoreCtrl;
    private GameData m_gamdData;
    // Use this for initialization
    void Start () {
        //init
        m_nMaterialSellAmount = 0;
        m_gamdData = GameObject.Find("GameData").GetComponent<GameData>();
        m_MaterialStoreCtrl = GameObject.Find("MaterialStoreCtrl").GetComponent<MaterialStoreCtrl>(); 
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //아이템의 데이터 설정
    public void setItemData(string strMaterialCode)
    {
        m_strMaterialCode = strMaterialCode;
        MaterialData mData = new MaterialData(m_strMaterialCode);

        //재료 이미지
        Sprite materialSprite = (Sprite)Resources.Load("Image/Main/Alchemy/Item/" + m_strMaterialCode, typeof(Sprite));
        MaterialImage.sprite = materialSprite;

        //재료 이름
        MaterialNameText.text = mData.strName;

        //재료 가격
        MaterialPriceText.text = mData.nMaterialPrice + "G";
        m_nMaterialPrice = mData.nMaterialPrice;

        //재료 수량
        MaterialAmountText.text = "0";
    }

    public void addMaterialAmount()
    {
        //Debug.Log(m_strMaterialCode);
        StartCoroutine(addCoroutine());
    }

    IEnumerator addCoroutine()
    {
        
        m_bAmount = true;
        while (m_bAmount && (m_MaterialStoreCtrl.getTotalBuyAmountSum() + m_nMaterialPrice) <= m_gamdData.getPlayerGold())
        {
            MaterialAmountText.text = (++m_nMaterialSellAmount).ToString();
            m_MaterialStoreCtrl.addTotalBuyAmountSum(m_nMaterialPrice);
            yield return new WaitForSeconds(0.1f);
        }
        m_bAmount = false;
    }

    public void reduceMaterialAmount()
    {
        StartCoroutine(reduceCoroutine());
    }

    IEnumerator reduceCoroutine()
    {
        m_bAmount = true;
        while (m_bAmount && m_MaterialStoreCtrl.getTotalBuyAmountSum() > 0 && m_nMaterialSellAmount > 0)
        {
            MaterialAmountText.text = (--m_nMaterialSellAmount).ToString();
            m_MaterialStoreCtrl.addTotalBuyAmountSum(-m_nMaterialPrice);
            yield return new WaitForSeconds(0.1f);
        }
        m_bAmount = false;
    }

    public void stopMaterialAmount()
    {
        m_bAmount = false;
    }

    //포션 코드
    public string getMaterialCode()
    {
        return m_strMaterialCode;
    }

    //포션 갯수
    public int getMaterialSellAmount()
    {
        return m_nMaterialSellAmount;
    }

    public void setMaterialSellAmount(int nAmount)
    {
        m_nMaterialSellAmount = nAmount;
    }
}
