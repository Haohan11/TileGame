using UnityEngine;
using UnityEngine.UI;

public class TileCardDisplay : MonoBehaviour
{
    public TileCard tileCard;

    public Image artworkImage;
    public Text nameText;
    public Text descriptionText;

    public Text typeText;

    public GameObject cardTemplate;
    Image cardTemplateImage;

    private void OnEnable()
    {
        SetCarddata();
    }

    void SetCarddata()
    {
        cardTemplateImage = cardTemplate.GetComponent<Image>();

        nameText.text = tileCard.name;
        descriptionText.text = tileCard.description;
        
        artworkImage.sprite = tileCard.artwork;
        typeText.text = tileCard.type.ToString();
    }
}
