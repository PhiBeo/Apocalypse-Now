using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstSlider;

    public void UpdateStat(float health, float hunger, float thirst)
    {
        healthSlider.value = health / 100;
        hungerSlider.value = hunger / 100;
        thirstSlider.value = thirst / 100;
    }

    public void OnSelectedCharacter()
    {
        Color color = Color.white;
        color.a = 0.5f;
        characterImage.color = color;
    }

    public void OnDeselectedCharacter()
    {
        Color color = Color.white;
        color.a = 1f;
        characterImage.color = color;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
