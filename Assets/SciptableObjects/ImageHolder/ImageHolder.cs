using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ImageHolder : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
}
