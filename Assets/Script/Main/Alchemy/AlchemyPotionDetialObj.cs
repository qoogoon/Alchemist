using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

public class AlchemyPotionDetialObj : MonoBehaviour {
    public Image PotionImage;
    public Text PotionName;
    public Text PotionDetailText;
    public Image[] NeedMaterialImage;
    public Text StateText;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    //포션 상세 정보 디스플레이
    public void showPotionDetialData(string strCode)
    {
        if (!strCode.Equals(""))
        {
            //포션 이미지
            Sprite potionImage = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/" + strCode, typeof(Sprite));
            PotionImage.sprite = potionImage;

            //포션 정보 가져오기
            XmlDocument potionDoc = XmlCtrl.Load((TextAsset)Resources.Load("Xml/Alchemy/Potion", typeof(TextAsset)));      //포션 xml
            XmlNodeList potionXmlList = potionDoc.SelectNodes("List/Item");
            string[] arrStrNeedMaterialCodes = null;
            foreach (XmlNode potionNode in potionXmlList)
            {
                if (potionNode.Attributes[0].InnerText.Equals(strCode))    //해당 포션 코드를 찾아
                {
                    PotionName.text = potionNode.Attributes[1].InnerText;   //포션이름 삽입.
                    PotionDetailText.text = potionNode.Attributes[5].InnerText;  //포션 정보 삽입.
                    arrStrNeedMaterialCodes = potionNode.Attributes[3].InnerText.Split('/');    //필요 재료 코드들
                }
            }

            //필요 재료 이미지 넣기
            if (arrStrNeedMaterialCodes != null) //필요 재료 코드들이 없지 않다면
            {
                for (int i = 0; i < NeedMaterialImage.Length; i++)
                {
                    if (i < arrStrNeedMaterialCodes.Length)  //재료 이미지 넣기
                    {
                        NeedMaterialImage[i].sprite = (Sprite)Resources.Load("Image/Main/Alchemy/Item/" + arrStrNeedMaterialCodes[i], typeof(Sprite));
                    }
                    else   //빈 이미지 넣기
                    {
                        NeedMaterialImage[i].sprite = null;
                    }
                }
            }
        }else
        {
            StateText.text = "Locking";
            gameObject.SetActive(false);
        }
        
    }

}
