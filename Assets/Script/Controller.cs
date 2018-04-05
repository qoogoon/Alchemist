using UnityEngine;
using UnityEngine.UI;

using System.Collections;

//모든 컨트롤러를 모음.
public class Controller : MonoBehaviour {
    public Text DebugText;
    public PotionCtrl potionCtrl;
    public MaterialCtrl materialCtrl;
    public MainCtrl mainCtrl;
    public DutorialCtrl dutorialCtrl;
    public StoreCtrl storeCtrl;
    public AdventurerCtrl adventurerCtrl;
    public GameData gameData;
    public DateCtrl dateCtrl;
    public QuestCtrl questCtrl;
    public ReputationCtrl reputationCtrl;
    public NewsCtrl newsCtrl;
    public MaterialStoreCtrl materialStoreCtrl;
    public AlchemyCtrl AlchemyCtrl;
    public CharacterCtrl CharacterCtrl;
    public DialogCtrl dialogCtrl;
    public InventoryCtrl InventoryCtrl;
    public EnvironmentCtrl environmentCtrl;
    // Use this for initialization
    void Awake () {
        Constant.g_ctrl = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setOpenBtnEnable(bool enable)
    {
        AlchemyCtrl.OpenStoreWindowBtn.enabled = enable;                //연금
        newsCtrl.NewsOpenBtn.enabled = enable;                          //뉴스
        storeCtrl.StoreStartBtn.enabled = enable;                       //가게
        mainCtrl.TestBtn.GetComponent<Button>().enabled = enable;       //테스트
        materialStoreCtrl.MaterialStoreOpenBtn.enabled = enable;        //재료가게
        questCtrl.QuestBtn.enabled = enable;                            //퀘스트
        InventoryCtrl.openInventoryBtn.enabled = enabled;               //인벤토리
    }

    public void PhoneDebug(string strLog)
    {
        DebugText.text = strLog;
    }
}
