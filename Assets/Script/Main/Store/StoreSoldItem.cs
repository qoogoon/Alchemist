using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreSoldItem : MonoBehaviour {
    public Image PotionImage;
    public Text PotionName;
    public Text PotionSoldGold;
    public Text PotionAmount;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setPotionItem(AdventurerAction action)
    {
        PotionImage.sprite = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/" + action.strPotionCode, typeof(Sprite));
        PotionData pData = new PotionData(action.strPotionCode);
        PotionName.text = pData.strName;
        PotionSoldGold.text = (action.nAmount * pData.nPotionPrice)+ "G";
        PotionAmount.text = "x"+action.nAmount.ToString();
    }
}
