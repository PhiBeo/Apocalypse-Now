using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItemUI(Sprite sprite, int quantity)
    {
        image.sprite = sprite;

        string quantityString = $"x{quantity}";

        quantityText.text = quantityString;
    }

    public void SetQuantity(int quantity)
    {
        string quantityString = $"x{quantity}";

        quantityText.text = quantityString;
    }
}
