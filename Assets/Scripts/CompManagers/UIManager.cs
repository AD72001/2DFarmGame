using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text levelText;
    public TMP_Text workerText;
    public TMP_Text plotItemText;
    public TMP_Text plotText;
    public TMP_Text productText;

    public GameObject victoryScreen;

    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateGameInfo();
    }

    void Update()
    {
        UpdatePlotInfo();
        ShopManager.Instance.SetShoppingStatus();
    }

    public void Victory()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void UpdateGameInfo()
    {
        UpdateMoneyText();
        UpdateLevelText();
        UpdateWorkerText();
        UpdatePlotItemInfo();
        UpdatePlotInfo();
        UpdateProductInfo();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = GameManager.Instance.Money.ToString();
    }

    public void UpdateLevelText()
    {
        levelText.text = GameManager.Instance.Level.ToString();
    }

    public void UpdateWorkerText()
    {
        workerText.text = $"{WorkerManager.Instance.IdleWorkers} / {WorkerManager.Instance.TotalWorkers}";
    }

    public void UpdatePlotInfo()
    {
        plotText.text = default;

        for (int i = 0; i < PlotManager.Instance.Plots.Length; i++)
        {
            Plot currPlot = PlotManager.Instance.Plots[i].GetComponent<Plot>();

            string plotName = currPlot.PlotItem == null ? "Empty" : currPlot.PlotItem.itemName;
            string plotTimer = "0";

            if (currPlot.PlotItem != null)
            {
                plotTimer = (currPlot.PlotItem.harvestTime - currPlot.PlotItem.harvestTimer).ToString("F2");
            }

            plotText.text += $"{currPlot.ID}: {plotName} -- {plotTimer}" + "\n";
        }
    }

    public void UpdatePlotItemInfo()
    {
        var items = (PlotItemTypeEnum[])Enum.GetValues(typeof(PlotItemTypeEnum));

        plotItemText.text = default;

        foreach (var item in items)
        {
            if (item == PlotItemTypeEnum.None) return;
            plotItemText.text += $"{item}: {GameManager.Instance.plotItemAvailable[(int)item]}" + "\n";
        }
    }

    public void UpdateProductInfo()
    {
        var items = (PlotItemTypeEnum[])Enum.GetValues(typeof(PlotItemTypeEnum));

        productText.text = default;

        foreach (var item in items)
        {
            if (item == PlotItemTypeEnum.None) return;
            productText.text += $"{item}: {GameManager.Instance.harvestProducts[(int)item]}" + "\n";
        }
    }
}
