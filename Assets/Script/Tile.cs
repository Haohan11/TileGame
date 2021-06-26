using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public GameObject manager;
    TileManager tileManager;

    Camera cam;
    GameObject tileCard;

    public GameObject choseFrame;
    Renderer choseFrameRender;

    public Color openFramecolor;
    Color closeFramecolor = new Color(0, 0, 0, 0);

    int tileNo;
    int length;
    int status;
    float timer;
    int timer_d;
    float timeDelta;
    Vector3 size;
    Vector3 enlargeSize = Vector3.zero;

    void Awake()
    {
        timeDelta = Time.deltaTime * 0.3f;
        tileManager = manager.GetComponent<TileManager>();

        tileCard = tileManager.tileCard;

        cam = tileManager.Camera.GetComponent<Camera>();

        length = tileManager.length;
        choseFrameRender = choseFrame.GetComponent<Renderer>();
        size = choseFrame.transform.localScale;
        tileNo = length * (int)transform.localPosition.x + (int)transform.localPosition.y;
    }

    public void OnMouseDown()
    {
        if (CheckUIinfront()) return;

        if (tileManager.canAction)
        {
            tileManager.BeChose(tileNo);
            tileManager.GoAction();
        }
        Debug.Log("TileNo." + tileNo + "(" + (int)this.transform.localPosition.x + "," + (int)this.transform.localPosition.y + ") TileStatus:" + tileManager.tile.status[tileNo] + " TileType:" + tileManager.tile.type[tileNo]);
    }

    private void OnMouseEnter()
    {
        Vector3 posi = tileManager.tileMap[tileNo].transform.localPosition;
        posi.z = 0;
        choseFrame.transform.localPosition = posi;

        choseFrameRender.material.color = openFramecolor;
    }

    private void OnMouseOver()
    {
        if (CheckUIinfront())
        {
            choseFrameRender.material.color = closeFramecolor;
            CloseTileCard();
            return;
        }
        
        ZoomIt();

        if (tileManager.tile.status[tileNo] == 0)
        {
            CloseTileCard();
            return;
        }
        ShowTileCard();
    }

    private void OnMouseExit()
    {
        choseFrameRender.material.color = closeFramecolor;
        CloseTileCard();
    }

    void ZoomIt()
    {
        switch (status)
        {
            case 0:
                size.x += timeDelta;
                size.y += timeDelta;
                if (size.x > 1.1f)
                    status = 1;
                break;
            case 1:
                size.x -= timeDelta;
                size.y -= timeDelta;
                if (size.x < 1.0f)
                    status = 0;
                break;
        }
        choseFrame.transform.localScale = size;
    }

    void ShowTileCard()
    {
        if (timer > 0.175f)
        {
            if (tileCard.gameObject.activeInHierarchy)
            {
                if (enlargeSize.x < 1.0f)
                {
                    enlargeSize.x += 0.25f;
                    enlargeSize.y += 0.25f;
                    enlargeSize.z += 0.25f;
                    tileCard.transform.localScale = enlargeSize;
                }
                return;
            }
            Vector3 newposi = cam.WorldToScreenPoint(gameObject.transform.position);

            newposi.x = 800.0f * newposi.x / Screen.width - 300.0f;
            newposi.y = 450.0f * newposi.y / Screen.height - 200.0f;
            tileCard.transform.localPosition = newposi;
            tileCard.transform.localScale = Vector3.zero;
            enlargeSize = Vector3.zero;

            tileCard.GetComponent<TileCardDisplay>().tileCard = manager.GetComponent<Library>().tileCardLibrary[tileManager.tile.type[tileNo]];
            tileCard.transform.SetSiblingIndex(0);
            tileCard.SetActive(true);
        }
        timer += timeDelta;
    }

    void CloseTileCard()
    {
        timer = 0;
        tileCard.SetActive(false);
    }

    bool CheckUIinfront()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> hitWhat = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, hitWhat);
        return hitWhat.Count > 0;
    }
}
