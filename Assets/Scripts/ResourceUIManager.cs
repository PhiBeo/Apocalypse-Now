using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUIManager : MonoBehaviour
{
    [SerializeField] private List<ImageHolder> itemImages;

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private Transform parent;

    private List<ItemUI> itemUIs;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = FindAnyObjectByType<ResourceManager>();

        itemUIs = new List<ItemUI>();

        for(int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            GameObject gameObject = Instantiate(itemPrefab, parent);

            ItemType type = (ItemType)i;

            Sprite sprite = itemImages.Find(x => x.itemName == type.ToString()).itemImage;

            int amount = resourceManager.GetResourceAmount(type);

            ItemUI item = gameObject.GetComponent<ItemUI>();
            item.SetItemUI(sprite, amount);

            itemUIs.Add(item);
        }
    }

    public void UpdateResourceUI(int i)
    {
        itemUIs[i].SetQuantity(resourceManager.GetResourceAmount((ItemType)i));
    }
}
