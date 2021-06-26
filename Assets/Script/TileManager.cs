using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public GameObject OrigiTile;     //要被複製的地格
    Quaternion tileRota;
    public GameObject OrigiRole;     //要被複製的角色棋
    public GameObject MingCard;
    public GameObject TileCard;

    public GameObject Camera;
    bool camerable;

    public Canvas canvas;
    Image curtain;

    [HideInInspector]
    public Library library;

    [HideInInspector]
    public GameObject tileCard;
    GameObject[] actionCardList;
    CardDisplay[] actionCardDisplay;

    //public Light Sun;
    Color sunColor = Color.clear;
    int sunStatus = 0;
    bool sunset = false;

    //Tile[] tilesList;    //儲存tile的component

    public Color openTileColor;
    public Color coverTileColor;

    public GameObject ExtentFrame;  //行動範圍框
    public Color extentFrameColor;
    GameObject[] extentFrame;       //儲存複製的行動範圍框
    Renderer[] extentFrameRender;
    Color closeFrameColor = new Color(0, 0, 0, 0);

    public float edge;
    public float CmrMvSp;
    float cmrMoveSpeed;

    GameObject role;    //複製的角色棋
    bool[] operaRange;

    bool moverole = false;
    int moveTarget;
    public float roleMoveSpeed;

    bool flash;
    int flashstatus;
    int flashTimes;
    int flashMaxTimes = 3;
    float flashTimer;
    int flashMark;
    Renderer[] flashFrame;

    public GameObject Hint;
    Text HintText;
    Image HintBoard;
    string hintText;
    bool hintWithBg = false;
    float hintTimer = 0;
    int hintStatus;
    int hintPri = -1;
    public Color HintColor;
    public Color HintBoardColor;
    Color hintBoardColor;
    Color hintColor;
    public float hintKeepTime;
    bool activeHint;

    public struct Tiles
    {
        public int[] type;  //儲存地格種類
        public int[] status;    //用以儲存地格狀態的陣列, 0:未翻開, 1:已翻開, 2:被佔領
        public bool[] locked;
        public bool[] s;
        public bool[] m;
        public bool[] o;
    }

    public Tiles tile;

    [HideInInspector]
    public GameObject[] tileMap;   //儲存複製出的地格
    public int[] tileTypeNum;      //儲存此關卡內會出現的地格種類的個數
    public int[] runTile;      //儲存需要被執行的地格編號
    public int runTilemark = 0;
    Renderer[] tileRender;
    int roleNo;  //角色棋編號

    Quaternion flopTileRotate;
    int flopCount;
    int flopCount_s;
    int[] flopTilesList;
    int flopTilesLengh;
    bool flopTiles = false;

    int delay;

    [HideInInspector]
    public int choseTileNo;    //被選取的地格編號

    struct Operable
    {
        public bool[] s;
        public bool[] m;
        public bool[] o;
    }
    Operable operable;    //用以儲存地格是否可被操作


    public int length;  //地格圖的長
    public int width;   //地格圖的寬
    int area;
    float timeDelta;
    float timeDelta_half;

    public Text powerText;
    public Text timesText;
    public Text daysText;

    int power;
    int time;
    int days;

    public int fullpower;
    public int fulltime;
    public int fulldays;

    int scoutPowerCost;    //探看體力消耗
    int movePowerCost;    //移動體力消耗
    int occupyPowerCost;    //佔領體力消耗

    int scoutTimeCost;    //探看時間消耗
    int moveTimeCost;    //移動時間消耗
    int occupyTimeCost;    //佔領時間消耗

    int[] acode;     //控制碼
    int[] temp;

    [HideInInspector]
    public bool canAction = false;
    [HideInInspector]
    public int activeCard;   //儲存下一個要觸發的行動, 0:Scout, 1:Move, 2:Occupy

    bool backTolobby = false;

    void Start()
    {
        SetInitialValue();

        SetBasicActionCard();
        CreatTileCard();

        CreatTileMap();//製作地格
        CreatRole();//製作角色棋

        valueUpdate();

        CreatExtentFrame();
    }

    public void Update()
    {
        /*for (int i = 0; i < tilesList.Length; i ++)
        {
            tilesList[i].UpdateMe();
        }*/
        CameraCtrl();
        BackToLobby();

        for (int i = 0; i < actionCardDisplay.Length; i++)
        {
            actionCardDisplay[i].UpdateMe();
        }

        if (activeHint) ActiveHint();
        if (flash) Flash();
        if (sunset) SunSet();
        if (moverole) MoveRole();
        if (flopTiles) FlopTiles();
    }

    void SetInitialValue()
    {
        area = length * width;

        timeDelta = Time.deltaTime;
        timeDelta_half = timeDelta * 0.5f;

        tileRota = OrigiTile.transform.localRotation;

        flashFrame = new Renderer[8];
        operaRange = new bool[8];
        runTile = new int[area];
        acode = new int[area];
        temp = new int[area + 2];

        curtain = canvas.GetComponent<Image>();
        curtain.color = Color.clear;

        HintText = Hint.transform.GetChild(0).GetComponent<Text>();
        HintBoard = Hint.GetComponent<Image>();
        hintKeepTime = Mathf.Abs(hintKeepTime);

        roleMoveSpeed = Mathf.Abs(roleMoveSpeed);
        cmrMoveSpeed = timeDelta * CmrMvSp;

        library = GetComponent<Library>();
        ExtentFrame.GetComponent<CombineChildrenLate>().combine = true;

        activeCard = -1;

        power = fullpower;
        time = fulltime;
        days = fulldays;
    }

    void CreatTileMap()//製作地格地圖
    {
        tileMap = new GameObject[area];//依照地格數設定陣列長度
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                tileMap[length * i + j] = Instantiate(OrigiTile, new Vector3(i, j, 0), tileRota);
                tileMap[length * i + j].SetActive(true);
            }
        }
        GetTilesRender();
        SetTileType();
        SetTileStatus();
        SetOperable();
        SetLocked();
        //GetTileComponent();
    }

    void GetTilesRender()
    {
        tileRender = new Renderer[area];
        for (int i = 0; i < area; i++)
        {
            tileRender[i] = tileMap[i].GetComponent<Renderer>();
        }
    }

    void CreatRole()//製作角色棋
    {
        role = Instantiate(OrigiRole, new Vector3(Random.Range(0, width), Random.Range(0, length), -0.1f), Quaternion.Euler(-90, 0, 0));
        role.SetActive(true);
        roleNo = (length * (int)role.transform.localPosition.x) + (int)role.transform.localPosition.y;
        tile.type[roleNo] = 1;
        tile.locked[roleNo] = true;
        BeChose(roleNo);

        //tileRender[roleNo].material.color = openTileColor;
        openTile(roleNo);

        costUpdate();
        ChangeTileStatus(roleNo, 2);
        operable.s[roleNo] = false;
        SetExtent(roleNo, true);   //設定可操作的範圍
        SetRange(new bool[8] { false, true, false, true, false, true, false, true });
        SetCamera();    //設定攝影機位置
    }

    void SetBasicActionCard()
    {
        actionCardList = new GameObject[3];
        actionCardDisplay = new CardDisplay[3];
        RectTransform rectransform;

        for (int i = 0; i < 3; i++)
        {
            actionCardList[i] = CreatCard(i);
            actionCardDisplay[i] = actionCardList[i].GetComponent<CardDisplay>();
            rectransform = actionCardList[i].transform as RectTransform;
            rectransform.anchorMax = new Vector2(0, 0);
            rectransform.anchorMin = new Vector2(0, 0);
            switch (i)
            {
                case 0:
                    rectransform.anchoredPosition3D = new Vector3(75, 75, 0);
                    break;
                case 1:
                    rectransform.anchoredPosition3D = new Vector3(160, 75, 0);
                    break;
                case 2:
                    rectransform.anchoredPosition3D = new Vector3(245, 75, 0);
                    break;
            }
        }
        scoutPowerCost = actionCardDisplay[0].card.powerCost;
        scoutTimeCost = actionCardDisplay[0].card.timeCost;

        movePowerCost = actionCardDisplay[1].card.powerCost;
        moveTimeCost = actionCardDisplay[1].card.timeCost;

        occupyPowerCost = actionCardDisplay[2].card.powerCost;
        occupyTimeCost = actionCardDisplay[2].card.timeCost;
    }

    GameObject CreatCard(int No)
    {
        Card carddata = library.cardLibrary[No];
        GameObject card = Instantiate(MingCard, canvas.transform);
        card.transform.SetSiblingIndex(0);
        card.GetComponent<CardDisplay>().card = carddata;
        card.SetActive(true);

        return card;
    }

    void CreatTileCard()
    {
        tileCard = Instantiate(TileCard, canvas.transform);
    }

    void CreatExtentFrame()
    {
        extentFrame = new GameObject[8];
        extentFrameRender = new Renderer[8];

        for (int i = 0; i < 8; i++)
        {
            extentFrame[i] = Instantiate(ExtentFrame, new Vector3(0, 0, 0), Quaternion.identity);
        }

        Vector3 roleposi = role.transform.localPosition;
        extentFrame[0].transform.localPosition = new Vector3(roleposi.x - 1, roleposi.y + 1, 0);
        extentFrame[1].transform.localPosition = new Vector3(roleposi.x, roleposi.y + 1, 0);
        extentFrame[2].transform.localPosition = new Vector3(roleposi.x + 1, roleposi.y + 1, 0);
        extentFrame[3].transform.localPosition = new Vector3(roleposi.x + 1, roleposi.y, 0);
        extentFrame[4].transform.localPosition = new Vector3(roleposi.x + 1, roleposi.y - 1, 0);
        extentFrame[5].transform.localPosition = new Vector3(roleposi.x, roleposi.y - 1, 0);
        extentFrame[6].transform.localPosition = new Vector3(roleposi.x - 1, roleposi.y - 1, 0);
        extentFrame[7].transform.localPosition = new Vector3(roleposi.x - 1, roleposi.y, 0);

        for (int i = 0; i < 8; i++)
        {
            extentFrame[i].transform.SetParent(role.transform);
            extentFrameRender[i] = extentFrame[i].GetComponent<Renderer>();
        }
    }
    void SetTileType()
    {
        tile.type = new int[area];
        int count = 0;
        for (int i = 0; i < tileTypeNum.Length; i++)
            count += tileTypeNum[i];
        if (count > tile.type.Length)
        {
            Debug.Log("Wrong tileTypeNum!");
            return;
        }
        if (count < tile.type.Length)
            tileTypeNum[1] += (tile.type.Length - count);
        count = 0;
        for (int i = 0; i < tileTypeNum.Length; i++)
        {
            for (int j = 0; j < tileTypeNum[i]; j++)
            {
                tile.type[j + count] = i;
            }
            count += tileTypeNum[i];
        }
        count = 0;
        int temp, mark;
        for (int i = 0; i < tile.type.Length; i++)
        {
            temp = tile.type[i];
            mark = Random.Range(count, tile.type.Length);
            tile.type[i] = tile.type[mark];
            tile.type[mark] = temp;
            count++;
        }
    }

    void SetTileStatus()
    {
        tile.status = new int[area];
        for (int i = 0; i < area; i++)
            tile.status[i] = 0;
    }

    void SetOperable()
    {
        operable.s = new bool[area];
        operable.m = new bool[area];
        operable.o = new bool[area];
        tile.s = new bool[area];
        tile.m = new bool[area];
        tile.o = new bool[area];
        for (int i = 0; i < area; i++)
        {
            operable.s[i] = false;
            operable.m[i] = false;
            operable.o[i] = false;
            tile.s[i] = true;
            tile.m[i] = true;
            tile.o[i] = true;
        }
    }

    void SetLocked()
    {
        tile.locked = new bool[area];
        for (int i = 0; i < area; i++)
            tile.locked[i] = false;
    }

    /*void GetTileComponent()
    {
        tilesList = new Tile[area];
        for (int i = 0; i < area; i++)
        {
            tilesList[i] = tileMap[i].GetComponent<Tile>();
        }
    }*/

    void ChangeTileStatus(int which, int status)
    {
        tile.status[which] = status;
    }

    public void BeChose(int tileNo)
    {
        choseTileNo = tileNo;
    }

    void SetRange(bool[] range)
    {
        operaRange = range;
    }

    void SetExtent(int which, bool op)
    {
        bool[] exist = CheckTile("cross", which);
        if (exist[0])
        {
            operable.s[which + 1] = op;
            operable.m[which + 1] = op;
            operable.o[which + 1] = op;
        }
        if (exist[1])
        {
            operable.s[which + length] = op;
            operable.m[which + length] = op;
            operable.o[which + length] = op;
        }
        if (exist[2])
        {
            operable.s[which - 1] = op;
            operable.m[which - 1] = op;
            operable.o[which - 1] = op;
        }
        if (exist[3])
        {
            operable.s[which - length] = op;
            operable.m[which - length] = op;
            operable.o[which - length] = op;
        }
    }

    int[] CheckAround(int which)  //確認四周地格狀態
    {
        bool[] exist = CheckTile("cross", which);
        int[] status = new int[4];
        status[0] = exist[0] ? tile.status[which + 1] : -1;
        status[1] = exist[1] ? tile.status[which + length] : -1;
        status[2] = exist[2] ? tile.status[which - 1] : -1;
        status[3] = exist[3] ? tile.status[which - length] : -1;
        return status;
    }

    void MoveRole()
    {
        Vector3 target = tileMap[moveTarget].transform.localPosition;
        Vector3 start = role.transform.localPosition;
        float speed = timeDelta * roleMoveSpeed;
        if (target.x > start.x)
            start.x += speed;
        if (target.x < start.x)
            start.x -= speed;
        if (target.y > start.y)
            start.y += speed;
        if (target.y < start.y)
            start.y -= speed;
        if (Mathf.Abs(target.x - start.x) < 0.05f && Mathf.Abs(target.y - start.y) < 0.05f)
        {
            start.x = target.x;
            start.y = target.y;
            moverole = false;
        }
        role.transform.localPosition = start;
    }

    void ShowFrame()
    {
        if (flashMark != 0)
            return;
        bool[] exist = CheckTile("around", roleNo);
        for (int i = 0; i < 8; i++)
        {
            if (exist[i] && operaRange[i])
                PushFlash(extentFrameRender[i]);
        }
        flash = true;
        //pulsate = true;
    }

    void CloseFrame()
    {
        for (int i = 0; i < extentFrameRender.Length; i++)
            extentFrameRender[i].material.color = closeFrameColor;
        CloseFlash();

    }

    void CloseFlash()
    {
        flash = false;
        flashMark = 0;
        flashMaxTimes = 3;
        flashstatus = 0;
        flashTimer = 0;
        flashTimes = 0;
    }

    void PushFlash(Renderer which)
    {
        flashFrame[flashMark] = which;
        flashMark++;
    }

    void Flash()
    {
        switch (flashstatus)
        {
            case 0:
                if (flashTimes == 0)
                {
                    flashstatus = 1;
                    if (flashFrame[0].material.color == extentFrameColor)
                        flashMaxTimes += 2;
                    for (int i = 0; i < flashMark; i++)
                        flashFrame[i].material.color = extentFrameColor;
                    return;
                }
                for (int i = 0; i < flashMark; i++)
                {
                    flashFrame[i].material.color = (flashTimes % 2) == 1 ? closeFrameColor : extentFrameColor;
                }
                flashstatus = 1;
                break;
            case 1:
                flashTimer += timeDelta;
                if (flashTimer > 0.175f)
                {
                    flashTimer = 0;
                    flashstatus = 0;
                    flashTimes++;
                    if(flashTimes == flashMaxTimes)
                    {
                        flashMaxTimes = 3;
                        flashTimes = 0;
                        flash = false;
                        flashMark = 0;
                    }
                }
                break;
        }
    }

    bool[] CheckTile(string kind, int which)
    {
        bool[] exist;
        exist = new bool[0];
        switch (kind)
        {
            case "cross":
                exist = new bool[4];
                exist[0] = (which % length != (length - 1)) ? true : false;//判斷角色棋是否位於上方邊界
                exist[1] = (which < area - length) ? true : false; //判斷角色棋是否位於右方邊界
                exist[2] = (which % length != 0) ? true : false; //判斷角色棋是否位於下方邊界
                exist[3] = (which > length - 1) ? true : false;//判斷角色棋是否位於左方邊界
                break;

            case "around":
                exist = new bool[8];
                exist[0] = ((which % length != (length - 1)) && (which > length - 1)) ? true : false;//判斷角色棋是否位於左上方邊界
                exist[1] = (which % length != (length - 1)) ? true : false;//判斷角色棋是否位於上方邊界
                exist[2] = ((which % length != (length - 1)) && (which < area - length)) ? true : false; //判斷角色棋是否位於右上方邊界
                exist[3] = (which < area - length) ? true : false; //判斷角色棋是否位於右方邊界
                exist[4] = ((which < area - length) && (which % length != 0)) ? true : false; //判斷角色棋是否位於右下方邊界
                exist[5] = (which % length != 0) ? true : false; //判斷角色棋是否位於下方邊界
                exist[6] = ((which > length - 1) && (which % length != 0)) ? true : false;//判斷角色棋是否位於左下方邊界
                exist[7] = (which > length - 1) ? true : false;//判斷角色棋是否位於左方邊界
                break;
        }
        return exist;
    }

    void SetCamera()
    {
        Vector3 mp = role.transform.localPosition;
        mp.y -= 3.0f;
        mp.z = Camera.transform.localPosition.z;
        Camera.transform.localPosition = mp;
    }

    void CameraCtrl()
    {
        if (camerable)
        {
            Vector3 mp = Camera.transform.localPosition;
            if (Input.mousePosition.x < edge && mp.x > -3.0f)
                mp.x -= cmrMoveSpeed;
            if (Input.mousePosition.x > Screen.width - edge && mp.x < width + 3.0f)
                mp.x += cmrMoveSpeed;
            if (Input.mousePosition.y < edge && mp.y > -3.0f)
                mp.y -= cmrMoveSpeed;
            if (Input.mousePosition.y > Screen.height - edge && mp.y < length + 1.0f)
                mp.y += cmrMoveSpeed;
            Camera.transform.localPosition = mp;
        }
    }

    public void SetCamerable(bool able)
    {
        camerable = !able;
    }

    void StillRunning()
    {
        delay++;
    }

    void FinishedRun()
    {
        delay--;
    }

    void valueUpdate()
    {
        daysText.text = "DAYS    " + days;
        powerText.text = "POWER    " + power;
        timesText.text = "TIME    " + time;
        CheckMission();
    }

    void CheckMission()
    {
        if (power <= 0 && delay == 0)
        {
            ShowHint("Mission Failed!", true, 2);
            backTolobby = true;
            return;
        }
        if (time <= 0 && delay == 0)
        {
            days--;
            daysText.text = "DAYS " + days;
            TurnTilesOver();
            sunset = true;
            ShowHint("~Day pass by~", true, 2);
            if (days == 0)
            {
                ShowHint("Mission Failed!", true, 2);
                backTolobby = true;
                return;
            }
            power = fullpower;
            time = fulltime;
            powerText.text = "POWER " + power;
            timesText.text = "TIME " + time;
        }
        for (int i = 0; i < area; i++)
        {
            if (tile.status[i] == 0) return;
        }
        ShowHint("Mission Completed!", true, 2);
        backTolobby = true;
    }

    void BackToLobby()
    {
        if (!backTolobby) return;

        if(hintStatus == 2 && hintTimer > 1)
            SceneManager.LoadScene(0);
    }

    void TurnTilesOver()
    {
        int[] tilebuffer = new int[area];
        int bufferCount = 0;
        for (int i = 0; i < area; i++)
        {
            if (tile.status[i] == 1 && !tile.locked[i] && i != roleNo)
            {
                for(int j = 0; j < runTilemark; j++)
                {
                    if(runTile[j] == i)
                    {
                        PopTile(j);
                        break;
                    }
                }
                flopTilesLengh ++;
                tilebuffer[bufferCount] = i;
                bufferCount += 1;
            }
        }

        flopTilesList = new int[flopTilesLengh];
        for(int i = 0; i < flopTilesLengh; i ++)
        {
            flopTilesList[i] = tilebuffer[i];
        }
        flopTiles = true;
    }

    void FlopTiles()
    {
        
        if (flopCount % 3 == 0)
        {

            switch (flopCount_s)
            {
                case 0:
                    flopTileRotate = Quaternion.Euler(-180 + Random.Range(-5, 5), Random.Range(-5, 5), 0);
                    if (flopCount > 9)
                        flopCount_s = 1;
                    break;
                case 1:
                    flopTileRotate = Quaternion.Euler(-180 + Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    if (flopCount > 18)
                        flopCount_s = 2;
                    break;
                case 2:
                    flopTileRotate = Quaternion.Euler(-180 + Random.Range(-1, 1), Random.Range(-1, 1), 0);
                    if (flopCount > 27)
                    {
                        flopTileRotate = Quaternion.Euler(-180, 0, 0);
                        flopCount_s = 3;
                    }
                    break;
                case 3:
                    if(flopCount - 27 < 180)
                    {
                        flopTileRotate = (flopCount - 27 < 90) ? Quaternion.Euler(-180, flopCount - 27, 0) : Quaternion.Euler(0, flopCount - 27, 180);

                        flopCount += 5;
                        break;
                    }
                    flopTileRotate = Quaternion.Euler(-180, 0, 0);
                    flopCount_s = 0;
                    flopTiles = false;
                    break;
            }

            for (int i = 0; i < flopTilesLengh; i++)
            {
                tileMap[flopTilesList[i]].transform.localRotation = flopTileRotate;
                if (flopCount - 27 == 89)
                    CloseTile(flopTilesList[i]);
                if (flopCount - 27 >= 180)
                {
                    
                    //tileRender[flopTilesList[i]].material.color = coverTileColor;
                    tile.status[flopTilesList[i]] = 0;
                    if (i == flopTilesLengh - 1)
                    {
                        flopCount = -1;
                        flopTilesLengh = 0;
                    }
                }
            }
        }

        flopCount += 1;
    }

    void SunSet()
    {
        curtain.color = sunColor;
        switch (sunStatus)
        {
            case 0:
                if (sunColor.a > 0.85f)
                {
                    sunStatus = 1;
                    return;
                }
                sunColor.a += timeDelta_half;
                break;
            case 1:
                if(sunColor.a < 0.1f)
                {
                    sunStatus = 0;
                    sunset = false;
                    return;
                }
                sunColor.a -= timeDelta_half;
                break;
        }

        /*sunColor = Sun.color;
        switch (sunStatus)
        {
            case 0:
                if (sunColor.r < 0.1f)
                {
                    sunStatus = 1;
                    return;
                }
                Sun.color = new Color(sunColor.r - timeDelta, sunColor.g - timeDelta, sunColor.b - timeDelta);
                break;
            case 1:
                if (sunColor.r > 0.9f)
                {
                    Sun.color = Color.white;
                    sunStatus = 0;
                    sunset = false;
                    return;
                }
                Sun.color = new Color(sunColor.r + timeDelta, sunColor.g + timeDelta, sunColor.b + timeDelta);
                break;
        }*/
    }

    void openTile(int which)
    {
        tileMap[which].GetComponent<MeshFilter>().mesh = library.tilebase[tile.type[which]].GetComponent<MeshFilter>().sharedMesh;
        tileMap[which].GetComponent<MeshRenderer>().sharedMaterials = library.tilebase[tile.type[which]].GetComponent<MeshRenderer>().sharedMaterials;

        switch (tile.type[which])
        {
            case 2:
                RevisePosi(which, -0.035f);
                break;
            case 3:
                RevisePosi(which, -0.15f);
                break;
            case 4:
                RevisePosi(which, -0.05f);
                break;
            case 5:
                RevisePosi(which, -0.2f);
                break;
            case 6:
                RevisePosi(which, -0.18f);
                break;
            case 7:
                RevisePosi(which, -0.165f);
                break;
            case 8:
                RevisePosi(which, -0.185f);
                break;
            case 9:
                RevisePosi(which, 0.02f);
                break;
            case 10:
                RevisePosi(which, -0.135f);
                break;
            case 11:
                RevisePosi(which, -0.05f);
                break;

        }
    }

    void RevisePosi(int which, float value)
    {
        Vector3 size = tileMap[which].transform.position;
        size.z += value;
        tileMap[which].transform.position = size;
    }

    void CloseTile(int which)
    {
        tileMap[which].GetComponent<MeshFilter>().mesh = OrigiTile.GetComponent<MeshFilter>().sharedMesh;
        tileMap[which].GetComponent<MeshRenderer>().sharedMaterials = OrigiTile.GetComponent<MeshRenderer>().sharedMaterials;

        Vector3 posi = tileMap[which].transform.localPosition;
        posi.z = 0;
        tileMap[which].transform.localPosition = posi;
    }

    void costUpdate()
    {
        actionCardDisplay[0].mainCostText.text = "" + scoutPowerCost;
        actionCardDisplay[0].secondCostText.text = "" + scoutTimeCost;

        actionCardDisplay[1].mainCostText.text = "" + movePowerCost;
        actionCardDisplay[1].secondCostText.text = "" + moveTimeCost;

        actionCardDisplay[2].mainCostText.text = "" + occupyPowerCost;
        actionCardDisplay[2].secondCostText.text = "" + occupyTimeCost;
    }

    public void GoAction()
    {
        canAction = false;
        switch (activeCard)
        {
            case 0:
                Scout();
                break;
            case 1:
                Move();
                break;
            case 2:
                Occupy();
                break;

            case 1002:
                Grassland();
                break;
        }
    }

    void ShowHint(string what, bool withbg, int priority)
    {
        if(priority > hintPri)
        {
            hintStatus = 0;
            hintText = what;
            hintWithBg = withbg;
            hintPri = priority;
            activeHint = true;
        }
    }

    void ActiveHint()
    {
        switch (hintStatus)
        {
            case 0:
                HintText.text = hintText;
                HintText.color = HintColor; 
                HintText.fontSize = 25;
                if (hintWithBg)
                {
                    HintBoard.color = HintBoardColor;
                    HintText.fontSize = 35;
                }
                hintStatus = 1;
                break;
            case 1:
                hintTimer += timeDelta;
                if (hintTimer > hintKeepTime)
                {
                    hintTimer = 0;
                    hintStatus = 2;
                    if (hintWithBg)
                        hintBoardColor = HintBoardColor;
                    hintColor = HintColor;
                }
                break;
            case 2:
                if (hintTimer > 1)
                {
                    activeHint = false;
                    hintTimer = 0;
                    hintStatus = 0;
                    hintPri = -1;
                    return;
                }
                hintTimer += timeDelta;
                HintText.color = hintColor;
                hintColor.a = 1 - hintTimer;
                if (hintWithBg)
                {
                    HintBoard.color = hintBoardColor;
                    hintBoardColor.a = 1 - hintTimer;
                }
                break;
        }
    }

    public void StartCard(int which)
    {
        canAction = true;
        activeCard = which;
        if (which == 0 || which == 1 || which == 2)
        {
            ShowFrame();
        }
    }

    void EnableActionCard()
    {
        for (int i = 0; i < actionCardList.Length; i++)
            actionCardList[i].SetActive(true);
    }

    void DisableActionCard()
    {
        for (int i = 0; i < actionCardList.Length; i++)
            actionCardList[i].SetActive(false);
    }

    bool CanScout(int which)
    {
        return (tile.status[which] == 0 && operable.s[which] && tile.s[which]);
    }

    bool CanMove(int which)
    {
        return (tile.status[which] != 0 && operable.m[which] && tile.m[which]);
    }

    bool CanOccupy(int which)
    {
        return (tile.status[which] == 1 && operable.o[which] && tile.o[which]);
    }

    private void Scout()
    {
        if (CanScout(choseTileNo))
        {
            //tileRender[choseTileNo].material.color = openTileColor;
            openTile(choseTileNo);
            ChangeTileStatus(choseTileNo, 1);

            PushTile();
            activeCard = -1;

            StartTile();

            CloseFrame();

            power -= scoutPowerCost;
            time -= scoutTimeCost;
            valueUpdate();
            return;
        }
        ShowHint("Wrong Target!", false, 0);
    }

    private void Move()
    {
        if (CanMove(choseTileNo))
        {
            moveTarget = choseTileNo;
            moverole = true;

            SetExtent(roleNo, false);
            roleNo = choseTileNo;
            SetExtent(roleNo, true);

            activeCard = -1;

            StartTile();

            CloseFrame();

            power -= movePowerCost;
            time -= moveTimeCost;
            valueUpdate();
            //SetCamera();
            return;
        }
        ShowHint("Wrong Target!", false, 0);
    }

    private void Occupy()
    {
        if (CanOccupy(choseTileNo))
        {
            moveTarget = choseTileNo;
            moverole = true;

            ChangeTileStatus(choseTileNo, 2);

            SetExtent(roleNo, false);
            roleNo = choseTileNo;
            SetExtent(roleNo, true);

            activeCard = -1;

            StartTile();

            CloseFrame();

            power -= occupyPowerCost;
            time -= occupyTimeCost;
            valueUpdate();
            //SetCamera();
            return;
        }
        ShowHint("Wrong Target!", false, 0);
    }

    void PushTile()
    {
        runTile[runTilemark] = choseTileNo;
        runTilemark += 1;
    }

    void PopTile(int which)
    {
        if (which < runTilemark)
        {
            for (int i = which; i < runTilemark; i++)
            {
                runTile[i] = runTile[i + 1];
            }
            runTilemark -= 1;
            return;
        }
        runTile[which] = 0;
        runTilemark -= 1;
    }

    void StartTile()    //執行地格效果
    {
        for (int i = 0; i < runTilemark; i++)
        {
            switch (tile.type[runTile[i]])
            {
                case 0:
                    break;
                case 1:
                    PopTile(i);
                    break;
                case 2:     //佔領:探看此格四方一格地格
                    switch (acode[runTile[i]])
                    {
                        case 0:
                            int[] ts = CheckAround(runTile[i]);
                            if (tile.status[runTile[i]] == 2 && ((ts[0] == 0) || (ts[1] == 0) || (ts[2] == 0) || (ts[3] == 0)))
                            {
                                StillRunning();
                                ShowHint("Scout a tile", false, 1);
                                SetExtent(roleNo, false);  //關閉角色棋可行動範圍
                                SetExtent(runTile[i], true);   //開啟此地格四方行動權
                                DisableActionCard();
                                acode[runTile[i]] = 1;
                                StartCard(1002);
                            }
                            else if (tile.status[runTile[i]] == 2)
                            {
                                acode[runTile[i]] = 0;
                                PopTile(i);
                            }
                            break;
                        case 1:
                            SetExtent(runTile[i], false);
                            SetExtent(roleNo, true);
                            EnableActionCard();
                            acode[runTile[i]] = 0;
                            PopTile(i);
                            break;
                    }
                    break;
                case 3:     //探看:立即佔領此格
                    tile.status[runTile[i]] = 2;
                    PopTile(i);
                    break;
                case 4:     //佔領:你下次的行動不會消耗體力
                    switch (acode[runTile[i]])
                    {
                        case 0:
                            if (tile.status[runTile[i]] == 2)
                            {
                                temp[runTile[i]] = scoutPowerCost;
                                scoutPowerCost = 0;
                                temp[area] = movePowerCost;
                                movePowerCost = 0;
                                temp[area + 1] = occupyPowerCost;
                                occupyPowerCost = 0;
                                costUpdate();
                                acode[runTile[i]] = 1;
                            }
                            break;
                        case 1:
                            scoutPowerCost = temp[runTile[i]];
                            movePowerCost = temp[area];
                            occupyPowerCost = temp[area + 1];
                            costUpdate();
                            temp[runTile[i]] = 0;
                            temp[area] = 0;
                            temp[area + 1] = 0;
                            PopTile(i);
                            break;
                    }
                    break;
                case 5:     //此處:你的探看消耗+1/+0 
                    switch (acode[runTile[i]])
                    {
                        case 0:
                            if (runTile[i] == roleNo)
                            {
                                scoutPowerCost += 1;
                                costUpdate();
                                acode[runTile[i]] = 1;
                            }
                            break;
                        case 1:
                            if (runTile[i] != roleNo)
                            {
                                scoutPowerCost -= 1;
                                costUpdate();
                                acode[runTile[i]] = 0;
                            }
                            break;
                    }
                    break;
                case 6:     //此處:你的行動消耗+1/+0
                    switch (acode[runTile[i]])
                    {
                        case 0:
                            if (runTile[i] == roleNo)
                            {
                                scoutPowerCost += 1;
                                movePowerCost += 1;
                                occupyPowerCost += 1;
                                costUpdate();
                                acode[runTile[i]] = 1;
                            }
                            break;
                        case 1:
                            if (runTile[i] != roleNo)
                            {
                                scoutPowerCost -= 1;
                                movePowerCost -= 1;
                                occupyPowerCost -= 1;
                                costUpdate();
                                acode[runTile[i]] = 0;
                            }
                            break;
                    }
                    break;
                case 7:     //佔領:扣除2點體力
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    break;
                case 20:
                    break;
            }
        }
    }

    void Grassland()
    {
        if (CanScout(choseTileNo))
        {
            //tileRender[choseTileNo].material.color = openTileColor;
            Debug.Log("hit");
            openTile(choseTileNo);

            ChangeTileStatus(choseTileNo, 1);

            canAction = false;
            FinishedRun();
            CheckMission();
            PushTile();
            StartTile();
            return;
        }
        canAction = true;
        ShowHint("Wrong Target!", false, 0);
    }
}
