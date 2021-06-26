using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public Image artworkImage;
    public Text nameText;
    public Text descriptionText;

    public Text mainCostText;
    public Text secondCostText;

    public Text typeText;

    public GameObject cardTemplate;

    public GameObject manager;
    TileManager tileManager;

    float timeDelta;
    float moveSpeed;
    float timer = 0.2f;

    bool rise;
    bool fall;

    bool updateLock = false;

    Image cardFrame;
    Color cardFrameColor;

    RectTransform rectransform;
    Vector3 posi;

    void Start()
    {
        tileManager = manager.GetComponent<TileManager>();

        timeDelta = 2.0f * Time.deltaTime;
        moveSpeed = 100.0f * timeDelta;

        rectransform = gameObject.transform as RectTransform;
        posi = rectransform.anchoredPosition3D;

        cardFrame = gameObject.transform.GetChild(0).GetComponent<Image>();
        cardFrameColor = cardFrame.color;
        cardFrame.color = Color.clear;

        SetCarddata();

        updateLock = true;
    }

    public void UpdateMe()
    {
        if (!updateLock) return;

        if (rise) Rise();
        if (fall) Fall();
        ShowCardFrame();
    }

    void SetCarddata()
    {
        //cardTemplateImage = cardTemplate.GetComponent<Image>();
        nameText.text = card.name;
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
        typeText.text = card.type.ToString();

        switch (card.type.ToString())
        {
            case "Action":
                mainCostText.text = card.powerCost.ToString();
                secondCostText.text = card.timeCost.ToString();
                break;
            case "Operation":
                mainCostText.text = card.manaCost.ToString();
                secondCostText.text = card.timeCost.ToString();
                break;
            case "Effect":
                mainCostText.text = card.manaCost.ToString();
                secondCostText.text = card.timeCost.ToString();
                break;
            case "Equipment":
                mainCostText.text = card.manaCost.ToString();
                secondCostText.text = "x";
                break;
            case "Godgiven":
                mainCostText.text = "x";
                secondCostText.text = "x";
                break;
        }
    }

    void Rise()
    {
        if (rectransform.anchoredPosition3D.y > 89.5)
        {
            posi.y = 90;
            rectransform.anchoredPosition3D = posi;
            //rectransform.localScale = maxsize;
            rise = false;
            return;
        }
        rectransform.anchoredPosition3D = posi;
        //rectransform.localScale += size;
        moveSpeed -= timeDelta;
        posi.y += moveSpeed;
    }

    void Fall()
    {
        timer += timeDelta;
        if (rectransform.anchoredPosition3D.y < 75.5)
        {
            posi.y = 75;
            rectransform.anchoredPosition3D = posi;
            //rectransform.localScale = minsize;
            fall = false;
            return;
        }
        rectransform.anchoredPosition3D = posi;
        //rectransform.localScale -= size;
        moveSpeed += timeDelta;
        posi.y -= moveSpeed;
    }

    public void Selected()
    {
        tileManager.StartCard(card.number);
    }

    public void GoRise()
    {
        fall = false;
        if(timer > 0.15f)
        {
            timer = 0;
            rise = true;
        }
    }

    public void GoFall()
    {
        rise = false;
        fall = true;
    }

    void ShowCardFrame()
    {
        cardFrame.color = tileManager.activeCard == card.number ? cardFrameColor : Color.clear;
    }
}
