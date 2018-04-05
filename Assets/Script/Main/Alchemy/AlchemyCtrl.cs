using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class AlchemyCtrl : MonoBehaviour
{
    public GameObject AlchemyWindow;
    public GameObject InvetoryUI;

    public GameObject AlchemyStartBtn;
    public GameObject PotionDetailObj;

    public GameObject PotionSelectBoard;            //포션 선택 판
    public GameObject PotionFomulaBoard;            //포션 공식 판
    public Text AlchemyStartBtnText;                //연금술 시작 버튼 텍스트
    public ScrollRect InventoryScrollView;     //재료 인벤토리의 스크롤뷰
    public GameObject FormulaBoardBackBtn;      //공식 판 뒤로가기 버튼
    public GameObject FormulaResetBtn;          //공식 판 리셋 버튼
    public Text AlchemyCreateAmountText;        //연금 생성 수량 텍스트
    public Button OpenStoreWindowBtn;

    public Text m_StateText;
    private GameObject[,] FormulaTile;
    private bool[,] selectItemFormula;
    private GameData m_gameData;
    private PlayerData m_playerData;
    private Controller m_ctrl;
    private GameObject[] m_inventoryItemObj;

    //연금술 중
    private bool m_bAlchemisting;

    //드래그, 메인제료 삽입시
    private GameObject m_dragObj;
    private GameObject m_mainMaterialObj;
    private bool m_bDrag;
    private Vector3 m_vMousePosition;
    private int m_nDragMaterialGold; //드래그 중인 재료 골드
    public int m_nPharmacyMaterialsGoldAmount;   //조제 중인 모든 재료의 골드 값

    //모양 만들기
    private bool m_bAlchemyStarting = false;
    private bool m_bShapeDrop;  //모양을 드랍해도 되는지 여부
    private float m_fClickTimeCount = 0f;
    private bool m_bClickCount = false;
    private GameObject m_clickTmpObj;   //클릭, 더블클릭을 구별하기위한 임시 저장
    private List<GameObject> m_arrDuplicateShapeObj;  //드랍된 복제 모양 오브젝트들
    private AlchemyNoticeWindowCtrl m_noticeCtrl;

    //물약
    public GameObject[] PotionListObjs;

    //선택 포션
    private string m_strSelectPotionCode; //선택 포션 코드
    private bool m_bMakePotion = false;   //포션 만들수 있는지
    public bool dutorialStartFormualBaord = false;

    //알림창
    public GameObject NoticeWindowObj;

    //듀토리얼에서 사용
    public bool dutorialMaterialSelect = false;
    // Use this for initialization
    void Start()
    {
        m_ctrl = GameObject.Find("Controller").GetComponent<Controller>();
        m_inventoryItemObj = m_ctrl.InventoryCtrl.getInventoryItem();
        init();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bAlchemyStarting && m_bAlchemisting)     //주재료 선택시에
        {
            if (Input.GetMouseButtonDown(0) && m_ctrl.InventoryCtrl.getInventoryMode() == InventoryCtrl.MODE.MATERIAL)    //마우스 클릭
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("click " + hit.collider.tag);
                    if (hit.collider.tag.Equals("alchemyMetarial"))
                    {
                        Debug.Log("click");
                        //이전에 선택된 메인 재료 삭제
                        if (m_mainMaterialObj != null)
                        {
                            Destroy(m_mainMaterialObj);
                        }
                        hit.collider.gameObject.GetComponent<MaterialObj>().setSelectMaterial();

                        if (m_mainMaterialObj != hit.collider.gameObject) //메인 재료창의 재료를 누른 게 아닐 경우 && 재료량이 0 이상이면
                        {
                            dutorialMaterialSelect = true;

                            //상태 표시
                            m_StateText.text = "Select Potion";

                            //시작 버튼 활성화
                            setStartBtnText("포션 선택 중");
                            startBtnActive(false);

                            //포션 리스트 선택 박스 비활성
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("alchemyPotionItem"))
                            {
                                obj.GetComponent<AlchemyPotionObj>().SelectBoxObj.SetActive(false);
                            }

                            //보조재료 포인터 비활성
                            foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
                            {
                                obj.GetComponent<MaterialObj>().selectPointerImage.SetActive(false);
                            }

                            //메인 재료 창에 재료 삽입
                            GameObject MainMeterialWindowObj = GameObject.Find("mainMaterial");
                            m_mainMaterialObj = duplicateMaterialObj(hit.collider.gameObject, MainMeterialWindowObj.transform.position);  //재료 오브젝트 복제
                            m_mainMaterialObj.transform.parent = MainMeterialWindowObj.transform;

                            //물약 리스트에 물약 삽입
                            createPotionList(hit.collider.gameObject.GetComponent<MaterialObj>().getCode());

                            //물약 상세 정보 숨기기
                            PotionDetailObj.SetActive(false);
                        }
                    }
                }
            }
        }
        else if (m_bAlchemyStarting)//연금 시작
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag.Equals("alchemyMetarial"))     //오브젝트를 클릭한 경우
                    {
                        m_clickTmpObj = hit.collider.gameObject;    //임시저장
                        m_bClickCount = true;     //클릭 카운트 시작
                        m_bDrag = true;
                        m_fClickTimeCount = 0;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0) && m_bDrag)     //마우스를 내려놧고 이전 상황이 드래그 상황이면
            {
                m_bDrag = false;
                if (m_fClickTimeCount < 0.2f)
                {
                    //Debug.Log("DoubleClick");
                    //회전 시키키
                    m_clickTmpObj.GetComponent<MaterialObj>().rotateCell();
                    m_fClickTimeCount = 0;
                    m_bClickCount = false;
                }
                if (m_dragObj != null)
                {
                    ArrayList arrActiveCell = new ArrayList();
                    foreach (Transform obj in m_dragObj.GetComponentsInChildren<Transform>())    //드래그 되고 있는 재료의 모양 셀들중에
                    {
                        if (obj.tag.Equals("alchemyShapeCell") && obj.gameObject.active) //태그가 alchemyShapeCell이고 셀이 일치하고 활성화 되어있는 쎌이면
                        {
                            if (obj.GetComponent<AlchemyShapeCellObj>().isAccord())
                            {
                                arrActiveCell.Add(obj);
                                m_bShapeDrop = true;
                            }
                            else
                            {
                                arrActiveCell.Clear();
                                m_bShapeDrop = false;
                                break;
                            }
                        }
                    }
                    if (m_bShapeDrop)   //드랍해도 되는 상황이면
                    {
                        foreach (Transform cellObj in arrActiveCell)
                        {
                            if (cellObj.GetComponent<AlchemyShapeCellObj>().CenterCell)
                            {
                                m_dragObj.transform.position = cellObj.GetComponent<AlchemyShapeCellObj>().getDropPosition();
                            }
                        }
                        //드랍된 복제 모양들 저장
                        m_arrDuplicateShapeObj.Add(m_dragObj);

                        //공식타일 overlap=true 인 상태이면 sticky = true
                        for (int i = 0; i < FormulaTile.GetLength(0); i++)
                        {
                            for (int j = 0; j < FormulaTile.GetLength(1); j++)
                            {
                                AlchemyFormulaCellObj obj = FormulaTile[i, j].GetComponent<AlchemyFormulaCellObj>();
                                if (obj.m_Overlap)
                                {
                                    obj.m_stickObj = true;
                                }
                            }
                        }
                        m_dragObj = null;
                        m_bShapeDrop = false;
                    }
                    else
                    {
                        MaterialObj invetoryMaterialObj = m_clickTmpObj.GetComponent<MaterialObj>();
                        invetoryMaterialObj.setMaterialActive(true);
                        Destroy(m_dragObj);
                        
                        m_nPharmacyMaterialsGoldAmount -= m_nDragMaterialGold;
                    }
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                
                Destroy(m_dragObj);
            }

            if (m_bClickCount)
            {
                MaterialObj invetoryMaterialObj = m_clickTmpObj.GetComponent<MaterialObj>();
                m_nDragMaterialGold = invetoryMaterialObj.getGold();
                m_fClickTimeCount += Time.deltaTime;
                if (m_fClickTimeCount > 0.2f && m_bDrag) //드래그 상황이면서 재료 수량이 0 이상일 경우
                {
                    //Debug.Log("Drag");
                    m_bClickCount = false;
                    m_dragObj = duplicateMaterialObj(m_clickTmpObj, m_clickTmpObj.transform.position);

                    MaterialObj dragMaterialObj = m_dragObj.GetComponent<MaterialObj>();
                    dragMaterialObj.setDuplicateImage();   //붙이는 이미지로 변경

                    //골드 소비
                    
                    m_nPharmacyMaterialsGoldAmount += m_nDragMaterialGold;
                    //invetoryMaterialObj.setMaterialAmount(invetoryMaterialObj.getMaterialAmount() - 1);

                }
            }
        }

        if (m_bDrag && m_dragObj != null)
        {
            m_clickTmpObj.GetComponent<MaterialObj>().setMaterialActive(false);
            m_vMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_dragObj.transform.position = new Vector3(m_vMousePosition.x - 1f, m_vMousePosition.y + 1f, 0f);
            m_dragObj.transform.localPosition = new Vector3(m_dragObj.transform.localPosition.x, m_dragObj.transform.localPosition.y, 0f);
        }
    }

    //연금술 생성하기
    public void createAlchemy()
    {
        AlchemyWindow.SetActive(true);
        startBtnActive(false);
    }

    //포션 리스트 생성하기
    public void createPotionList(string strMaterialCode)
    {
        MaterialData selectMaterialData = new MaterialData(strMaterialCode);
        ArrayList arrPotionCode = selectMaterialData.arrPotionCodes;
        setStartBtnText("주재료 선택 중");
        PotionDetailObj.SetActive(false);
        startBtnActive(false);

        if (!strMaterialCode.Equals(""))
        {
            for (int i = 0; i < arrPotionCode.Count; i++)
            {
                PotionData potionData = new PotionData((string)arrPotionCode[i]);
                PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionName(potionData.strName);   //포션 이름 삽입
                PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionCode(potionData.strCode);  //포션 코드 넣기
                                                                                                         //Debug.Log(m_gameData.getMaterialCollectLv());
                if (m_gameData.getPlayerLv() >= potionData.nPotionLv)       //포션 수집 레벨 까지 충족하면!(최종)
                {
                    Sprite potionImage = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/" + (string)arrPotionCode[i], typeof(Sprite));
                    //Debug.Log("Image/Main/Alchemy/Potion/" + (string)arrPotionCode[i]);
                    PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionImage(potionImage); //포션 리스트 창에 포션 이미지 삽입
                    PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionStateMode(AlchemyPotionObj.E_STATE_MODE.SATISFY);
                }
                else
                {
                    //레벨 부족
                    //Debug.Log("렙부족");
                    Sprite potionImage = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/POTION_CLOSE", typeof(Sprite));
                    PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionImage(potionImage); //포션 리스트 창에 포션 이미지 삽입
                    PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionStateMode(AlchemyPotionObj.E_STATE_MODE.LEVEL_UNSATISFY);
                }
            }
        }
        else
        {
            for (int i = 0; i < PotionListObjs.Length; i++)
            {
                Sprite potionImage = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/POTION_CLOSE", typeof(Sprite));
                PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionImage(potionImage); //포션 리스트 창에 포션 이미지 삽입
                PotionListObjs[i].GetComponent<AlchemyPotionObj>().setPotionStateMode(AlchemyPotionObj.E_STATE_MODE.LEVEL_UNSATISFY);
            }
        }


    }
    //포션 선택 창 정리
    public void clearPotionSelectBoard()
    {
        //상세 정보 창 비활성
        PotionDetailObj.SetActive(false);

        //주재료 칸의 복제된 재료 오브젝트 삭제
        Destroy(m_mainMaterialObj);

        //포션 리스트 초기화
        foreach (GameObject potionItemObj in PotionListObjs)
        {
            Sprite potionImage = (Sprite)Resources.Load("Image/Main/Alchemy/Potion/POTION_CLOSE", typeof(Sprite));
            potionItemObj.GetComponent<AlchemyPotionObj>().setPotionImage(potionImage); //포션 리스트 창에 포션 이미지 삽입
            potionItemObj.GetComponent<AlchemyPotionObj>().setPotionStateMode(AlchemyPotionObj.E_STATE_MODE.LEVEL_UNSATISFY);
            potionItemObj.GetComponent<AlchemyPotionObj>().getSelectBoxImageObj().SetActive(false);
        }

        //선택된 포션 코드 초기화
        m_strSelectPotionCode = "";
    }

    //재료 오브젝트 복제
    public GameObject duplicateMaterialObj(GameObject obj, Vector3 vPosition)
    {
        string strCode = obj.GetComponent<MaterialObj>().getCode(); //복제 대상 코드
        GameObject dragObj = (GameObject)Instantiate(obj.gameObject, vPosition, obj.transform.rotation);   //복제하기
        dragObj.GetComponent<MaterialObj>().setCode(strCode);  //복제obj에 코드 넣기
        dragObj.transform.parent = obj.transform.parent.parent.parent.parent;    //부모 찾기
        dragObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);    //크기 조절
        dragObj.GetComponent<MaterialObj>().setTitle("");    //제목 조절
        dragObj.GetComponent<MaterialObj>().setGoldText("");

        return dragObj;
    }

    //연금 공식 생성
    public void createAlchemyFormulaBoard(string strPotionCode)
    {
        m_ctrl.InventoryCtrl.setPotionInventoryEnabled(false);
        m_ctrl.InventoryCtrl.setMaterialInventoryEnabled(false);
        m_ctrl.InventoryCtrl.setInventoryImageNone();
        PotionData pData = new PotionData(strPotionCode);
        
        bool[,] arrPotionFormulaTable = pData.arrFormulaTable[UnityEngine.Random.RandomRange(0,3)];
        
        for (int i = 0; i < arrPotionFormulaTable.GetLength(0); i++)
        {
            for (int j = 0; j < arrPotionFormulaTable.GetLength(1); j++)
            {
                selectItemFormula[i, j] = arrPotionFormulaTable[i, j];
                FormulaTile[i, j].SetActive(arrPotionFormulaTable[i, j]);
            }
        }
    }

    public void resetFomulaBoard()
    {
        m_nPharmacyMaterialsGoldAmount = 0; //끌어다 놓은 제조 재료 합계

        string strDuplicateObjCode;
        string strInvetoryObjCode;
        MaterialData mData;
        foreach (GameObject itemObj in m_ctrl.InventoryCtrl.getInventoryItem())
        {
            foreach (GameObject duplicateObj in m_arrDuplicateShapeObj)
            {
                if (duplicateObj != null)
                {
                    strDuplicateObjCode = duplicateObj.GetComponent<MaterialObj>().getCode();
                    strInvetoryObjCode = itemObj.GetComponent<MaterialObj>().getCode();
                    if (strDuplicateObjCode.Equals(strInvetoryObjCode))
                    {
                        //itemObj.GetComponent<MaterialObj>().setMaterialAmount(itemObj.GetComponent<MaterialObj>().getMaterialAmount() + 1);
                        itemObj.GetComponent<MaterialObj>().setMaterialActive(true);
                    }
                }

            }
        }
        clearFormulaBoard();
    }

    //공식판 닫기
    public void closeFormulaBoard()
    {
        m_ctrl.InventoryCtrl.setPotionInventoryEnabled(true);
        m_ctrl.InventoryCtrl.setMaterialInventoryEnabled(true);
        NoticeWindowObj.SetActive(false);
        FormulaBoardBackBtn.SetActive(false);    //공식판 뒤로가기 버튼 비활성화
        FormulaResetBtn.SetActive(false);
        InventoryScrollView.vertical = true;    //인벤토리 스크롤 고정 & 이동

        //오브젝트 활성화, 비활성화
        PotionSelectBoard.SetActive(true);
        PotionFomulaBoard.SetActive(false);

        //연금 버튼 설정 변경
        startBtnActive(false);
        setStartBtnText("주재료 선택 중");

        //인벤토리 안의 재료 이미지를 퍼즐이미지로 변경
        for (int i = 0; i < m_ctrl.InventoryCtrl.getInventoryItem().Length; i++)
        {
            m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().setItemImageVisiable(true);
            m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().setShapeImageVisiable(false);
            m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().setMaterialActive(true);
        }

        //재료 선택 단계로
        m_bAlchemyStarting = false;

        //포션 선택 창 정리
        clearPotionSelectBoard();

        //공식판 정리
        resetFomulaBoard();

        //인벤토리 생성
        m_ctrl.InventoryCtrl.createMaterialInventory();
    }


    //모양 맞추기 게임 시작
    public void startShapePuzzle()
    {
        if (m_bMakePotion)
        {
            dutorialStartFormualBaord = true;
            FormulaBoardBackBtn.SetActive(true);    //공식판 뒤로가기 버튼 활성화
            FormulaResetBtn.SetActive(true);        //공식판 재설정 버튼
            InventoryScrollView.vertical = false;   //인벤토리 스크롤 고정 & 이동

            MaterialObj mObj;
            //인벤토리 초기화
            foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
            {
                mObj = obj.GetComponent<MaterialObj>();
                mObj.selectMaterialObj.SetActive(false);
                mObj.selectPointerImage.SetActive(false);
            }

            //오브젝트 활성화, 비활성화
            startBtnActive(true);
            PotionSelectBoard.SetActive(false);
            PotionFomulaBoard.SetActive(true);

            //연금 공식 생성하기
            createAlchemyFormulaBoard(m_strSelectPotionCode);

            //필요한 재료로 인벤토리 설정
            PotionData selectPotionData = new PotionData(m_strSelectPotionCode);
            List<MaterialData> arrMaterialData = new List<MaterialData>();
            foreach (string strMaterialCode in selectPotionData.arrNeedMaterialCodes)
            {
                arrMaterialData.Add(new MaterialData(strMaterialCode));
            }
            m_ctrl.InventoryCtrl.setMaterialInventory(arrMaterialData);
            m_ctrl.InventoryCtrl.setInventoryImageNone();

            //인벤토리 안의 재료 이미지를 퍼즐이미지로 변경
            for (int i = 0; i < m_ctrl.InventoryCtrl.getInventoryItem().Length; i++)
            {
                m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().setItemImageVisiable(false);
                m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().setShapeImageVisiable(true);
                m_ctrl.InventoryCtrl.getInventoryItem()[i].GetComponent<MaterialObj>().GoldTxt.gameObject.SetActive(true);
            }
            m_bAlchemyStarting = true;
            //연금 버튼 설정 변경
            startBtnActive(false);
            setStartBtnText("제조 중...");

            //퍼즐 게임 리스너
            StartCoroutine(ShapePuzzleListener());

        }
    }

    //퍼즐 게임 리스너
    IEnumerator ShapePuzzleListener()
    {
        bool bSuccess = false;  //성공여붕

        //for(int i = 0; i< FormulaTile)
        while (!bSuccess)
        {
            bSuccess = true;
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < selectItemFormula.GetLength(0); i++)  //selectItemFormula 논리적인 공식, FormulaTile 물리적인 공식
            {
                for (int j = 0; j < selectItemFormula.GetLength(0); j++)
                {
                    if (selectItemFormula[i, j])    //논리적인 공식 판에서 참인 것이
                    {
                        //Debug.Log("[" + i + "," + j + "]");
                        bool bStickObj = FormulaTile[i, j].GetComponent<AlchemyFormulaCellObj>().isStickObj();
                        if (bStickObj)   //재료 모양에 붙어 있다면
                        {
                            bSuccess = true;
                        }
                        else
                        {
                            bSuccess = false;
                            break;
                        }
                    }
                }
                if (!bSuccess)
                {
                    break;
                }
            }
        }
        if (bSuccess)   //성공했다면
        {
            //알림 창(최종 제조)
            m_noticeCtrl.createPotionAmountWindow(m_strSelectPotionCode);


        }
    }

    //연금 시작 버튼 활성화
    public void startBtnActive(bool bActive)
    {
        AlchemyStartBtn.GetComponent<Button>().enabled = bActive;
        AlchemyStartBtn.GetComponent<Animator>().SetBool("blink", bActive);
    }

    //연금 시작 버튼 텍스트
    public void setStartBtnText(string strBtnTitle)
    {
        AlchemyStartBtnText.text = strBtnTitle;
    }

    //연금을 하는 중인지
    public bool isAlchemyStarting()
    {
        return m_bAlchemyStarting;
    }

    //포션 만들 수 있는지
    public void setMakePotion(bool bAble)
    {
        m_bMakePotion = bAble;

    }

    //선택 포션
    public void setSelectPotionCode(string strCode)
    {
        m_strSelectPotionCode = strCode;
    }

    //마우스 드래그 중인지
    public bool isMouseDrag()
    {
        return m_bDrag;
    }

    //공식판 초기화
    public void clearFormulaBoard()
    {
        //위에 덧 씌워진 재료 모양 오브젝트 지우기
        foreach (GameObject duplicateObj in m_arrDuplicateShapeObj)
        {
            Destroy(duplicateObj);
        }
        m_arrDuplicateShapeObj.Clear();
        //공식 판 셀들 초기화
        AlchemyFormulaCellObj cellObj;
        for (int i = 0; i < FormulaTile.GetLength(0); i++)
        {
            for (int j = 0; j < FormulaTile.GetLength(1); j++)
            {
                cellObj = FormulaTile[i, j].GetComponent<AlchemyFormulaCellObj>();
                cellObj.clear();
            }
        }
    }

    //초기화
    void init()
    {
        m_noticeCtrl = NoticeWindowObj.GetComponent<AlchemyNoticeWindowCtrl>();
        m_bAlchemisting = false;
        m_arrDuplicateShapeObj = new List<GameObject>();
        m_bShapeDrop = false;
        m_gameData = new GameData();
        m_playerData = new PlayerData();
        selectItemFormula = new bool[5, 5];
        
        //공식판 가져오기

        FormulaTile = new GameObject[5, 5];
        FormulaTile[0, 0] = GameObject.Find("(0,0)");
        FormulaTile[0, 1] = GameObject.Find("(0,1)");
        FormulaTile[0, 2] = GameObject.Find("(0,2)");
        FormulaTile[0, 3] = GameObject.Find("(0,3)");
        FormulaTile[0, 4] = GameObject.Find("(0,4)");

        FormulaTile[1, 0] = GameObject.Find("(1,0)");
        FormulaTile[1, 1] = GameObject.Find("(1,1)");
        FormulaTile[1, 2] = GameObject.Find("(1,2)");
        FormulaTile[1, 3] = GameObject.Find("(1,3)");
        FormulaTile[1, 4] = GameObject.Find("(1,4)");

        FormulaTile[2, 0] = GameObject.Find("(2,0)");
        FormulaTile[2, 1] = GameObject.Find("(2,1)");
        FormulaTile[2, 2] = GameObject.Find("(2,2)");
        FormulaTile[2, 3] = GameObject.Find("(2,3)");
        FormulaTile[2, 4] = GameObject.Find("(2,4)");

        FormulaTile[3, 0] = GameObject.Find("(3,0)");
        FormulaTile[3, 1] = GameObject.Find("(3,1)");
        FormulaTile[3, 2] = GameObject.Find("(3,2)");
        FormulaTile[3, 3] = GameObject.Find("(3,3)");
        FormulaTile[3, 4] = GameObject.Find("(3,4)");

        FormulaTile[4, 0] = GameObject.Find("(4,0)");
        FormulaTile[4, 1] = GameObject.Find("(4,1)");
        FormulaTile[4, 2] = GameObject.Find("(4,2)");
        FormulaTile[4, 3] = GameObject.Find("(4,3)");
        FormulaTile[4, 4] = GameObject.Find("(4,4)");

        //공식판 안보이게 하기
        PotionFomulaBoard.SetActive(false);
    }

    //연금술 닫기
    public void closeAlchemy()
    {
        RectTransform rectTrans = AlchemyWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.left * 1000f;

        m_bAlchemisting = false;
        //InvetoryUI.GetComponent<InventoryCtrl>().openBtn.SetActive(true);

        //보조재료 포인터 비활성
        foreach (GameObject obj in m_ctrl.InventoryCtrl.getInventoryItem())
        {
            obj.GetComponent<MaterialObj>().selectPointerImage.SetActive(false);
        }

        //공식판 끄기
        closeFormulaBoard();
    }

    //연금술 열기
    public void openAlchemy()
    {
        RectTransform rectTrans = AlchemyWindow.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(rectTrans.rect.width / 2, 0);
        
        //AlchemyWindow.transform.localPosition = new Vector3(rectTrans.rect.width / 2, 0, 0);
        Debug.Log(AlchemyWindow.transform.localPosition);
        m_bAlchemisting = true;
        m_ctrl.InventoryCtrl.createMaterialInventory();
    }

    //연금술 중(연금술 창 활성화중)
    public bool isAlchemisting()
    {
        return m_bAlchemisting;
    }

    //드레그로 복제된 재료 이미지
    public List<GameObject> getDuplicateShapeObjs()
    {
        return m_arrDuplicateShapeObj;
    }

}