using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public GameObject shopWindows;

    public List<int> SellPrices;
    public List<int> PurchasePrices;

    public bool isShopping = false;

    void Awake()
    {
        if (Instance == null) Instance = this;

        SellPrices = new List<int> (new int[Enum.GetNames(typeof(PlotItemTypeEnum)).Length-1]);
        PurchasePrices = new List<int> (new int[Enum.GetNames(typeof(PlotItemTypeEnum)).Length-1]);
    }

    void Start()
    {

    }

    public void SoldItem(PlotItem soldItem)
    {
        int numberOfProducts = GameManager.Instance.harvestProducts[soldItem.ID];

        GameManager.Instance.harvestProducts[soldItem.ID] = 0;
        GameManager.Instance.Money += numberOfProducts * SellPrices[soldItem.ID];
    }

    public void PurchaseItem(PlotItem purchaseItem, int quantity)
    {
        int total = PurchasePrices[purchaseItem.ID] * quantity;

        if (GameManager.Instance.Money >= total)
        {
            GameManager.Instance.Money -= total;

            GameManager.Instance.plotItemAvailable[purchaseItem.ID] += quantity;
        }
    }

    public void SetShoppingStatus()
    {
        isShopping = shopWindows.activeSelf;
    }
}
