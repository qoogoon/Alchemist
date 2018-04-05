using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PotionObj : MonoBehaviour {
    public Text titleText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setTitle(string strTitle)
    {
        titleText.text = strTitle;
    }

    public void setMaterialAmount(int nAmount)
    {
        titleText.text += "x"+nAmount;
    }

    public void setItemImage(string strCode)
    {
        Sprite potionItem = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/"+ strCode, typeof(Sprite));
        gameObject.GetComponent<Image>().sprite = potionItem;
    }
}
