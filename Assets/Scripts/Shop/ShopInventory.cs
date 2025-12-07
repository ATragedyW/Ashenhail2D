using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewShopInventory", menuName = "Shop/Shop Inventory")]
public class ShopInventory : ScriptableObject
{
    public string shopName = "Shop";
    public Sprite shopkeeperIcon;
    public List<ShopItemData> itemsForSale = new List<ShopItemData>();
    
    public ShopItemData GetItemData(Item item)
    {
        return itemsForSale.Find(data => data.item == item);
    }
}

[System.Serializable]
public class ShopItemData
{
    public Item item;
    public int price = 10;
    public int sellPrice = 5;
    public int stock = -1; // -1 for infinite
    
    public ShopItemData(Item item, int price, int sellPrice = -1, int stock = -1)
    {
        this.item = item;
        this.price = price;
        this.sellPrice = sellPrice > 0 ? sellPrice : Mathf.RoundToInt(price * 0.5f);
        this.stock = stock;
    }
}