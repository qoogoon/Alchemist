using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestDetailObj : MonoBehaviour {
    //public resource
    public GameObject QuestStoryContent;
    public GameObject QuestRewardCotent;
    public Text QuestTitle;
    public Text QuestStoryText;
    public Text QuestStoryReward;
    public Font font;
    public GameObject closeBtn;

    //private resource
    private Controller m_ctrl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setQuestData(QuestData data)
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        //타이틀
        QuestTitle.text = data.strTitle;

        //스토리
        QuestStoryText.text = data.strContents.Replace("\\n", "\n");
        float fY = QuestStoryText.preferredHeight ;
        QuestStoryText.transform.localPosition = new Vector3(QuestStoryText.transform.localPosition.x, -(fY / 2 + 5));

        RectTransform contentsRect = QuestStoryContent.GetComponent<RectTransform>();
        contentsRect.sizeDelta = new Vector2(0, fY + 10);

        //보상
        string strReward = "";
        
        for(int i = 0; i<data.arrRewardCodes.Count; i++)
        {
            Debug.Log(data.arrRewardCodes[i]);
            if (data.arrRewardCodes[i].Contains("REPUTATION"))
            {
                strReward = strReward + "* 평판 : " + data.arrRewardValues[i] + '\n';
            }
            else if (data.arrRewardCodes[i].Contains("GOLD"))
            {
                strReward = strReward + "* 골드 : " + data.arrRewardValues[i] + '\n';
            }
            else if (data.arrRewardCodes[i].Contains("POTION"))
            {
                foreach(PotionData pData in m_ctrl.potionCtrl.getAllPotionData())
                {
                    
                    if(pData.nPotionLv == data.arrRewardValues[i])
                    {
                        strReward = strReward + "* " + pData.strName + " 제조법" + '\n';
                    }
                }                
            }
            else if (data.arrRewardCodes[i].Contains("MATERIAL"))
            {
                foreach (MaterialData mData in m_ctrl.materialCtrl.getAllMaterialData())
                {
                    if (mData.nMaterialLv == data.arrRewardValues[i])
                    {
                        strReward = strReward + "* " + mData.strName + " 획득" + '\n';
                    }
                }
                
            }
        }
        

        QuestStoryReward.text = strReward;
        float fYRewardScroll = QuestStoryReward.preferredHeight;
        QuestStoryReward.transform.localPosition = new Vector3(QuestStoryReward.transform.localPosition.x, -(fYRewardScroll / 2 + 5));

        RectTransform contentsRewardRect = QuestRewardCotent.GetComponent<RectTransform>();
        contentsRewardRect.sizeDelta = new Vector2(0, fYRewardScroll);
    }

    //닫기
    public void closeDetailWindow()
    {
        gameObject.SetActive(false);
    }
}