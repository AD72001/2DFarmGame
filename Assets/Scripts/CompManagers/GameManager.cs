using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int _money = 0;
    [SerializeField] private int _level = 1;
    [SerializeField] private string saveLocation = "/GameState.sav";

    public int victoryCondition = 2000000;

    public int Money
    {
        get { return _money; }
        set { 
            _money = value; 
            UIManager.Instance.UpdateMoneyText();

            if (_money >= victoryCondition)
                UIManager.Instance.Victory();
            }
    }

    public int Level
    {
        get { return _level; }
        set { 
            _level = value;
            UIManager.Instance.UpdateLevelText();
            }
    }

    public int moneyLeveling = 500;
    public int plotPrice = 500;

    public List<int> harvestProducts;
    public List<int> plotItemAvailable;
    public List<float> harvestTime;

    // Time
    public string lastPlayTime = "";
    public bool isLoading = false;

    public PlotItemTypeEnum selectedPlotItem; //game
    public PlotItemTypeEnum playerSelectedPlotItem; //player

    void Awake()
    {
        if (Instance == null) Instance = this;

        harvestProducts = new List<int>( new int[Enum.GetNames(typeof(PlotItemTypeEnum)).Length-1] );
        plotItemAvailable = new List<int>( new int[Enum.GetNames(typeof(PlotItemTypeEnum)).Length-1] );
        harvestTime = new List<float>( new float[Enum.GetNames(typeof(PlotItemTypeEnum)).Length-1] );
        selectedPlotItem = PlotItemTypeEnum.None;
    }

    public void SetPlotItem(int ID)
    {
        switch (ID)
        {
            case 0: playerSelectedPlotItem = PlotItemTypeEnum.Blueberry; break;
            case 1: playerSelectedPlotItem = PlotItemTypeEnum.Tomato; break;
            case 2: playerSelectedPlotItem = PlotItemTypeEnum.Cow; break;
            case 3: playerSelectedPlotItem = PlotItemTypeEnum.Strawberry; break;
            default: playerSelectedPlotItem = PlotItemTypeEnum.None; break;
        }
    }

    public PlotItemTypeEnum CheckPlotItemAvailable()
    {
        for (int i = 0; i < plotItemAvailable.Count; i++)
        {
            if (plotItemAvailable[i] > 0)
            {
                return ((PlotItemTypeEnum[])Enum.GetValues(typeof(PlotItemTypeEnum)))[i];
            }
        }

        return PlotItemTypeEnum.None;
    }

    public void LevelUp()
    {
        if (Money >= moneyLeveling)
        {
            Level++;
            Money -= moneyLeveling;
        }
        else Debug.Log("Not enough money");
    }

    // New Game
    public void NewGame()
    {
        isLoading = true;

        string initialFileDir = Application.dataPath + "/InitialSaveFiles" + "/GameState.csv";

        Debug.Log(initialFileDir);

        if (File.Exists(initialFileDir))
        {
            string[] line = File.ReadAllLines(initialFileDir);

            string[] line_1 = line[1].Split(',');
            string[] line_2 = line[2].Split(',');

            victoryCondition = Int32.Parse(line_1[11]);

            Level = Int32.Parse(line_1[0]);
            Money = Int32.Parse(line_1[1]);
            WorkerManager.Instance.TotalWorkers = Int32.Parse(line_1[2]);

            plotItemAvailable[0] = Int32.Parse(line_1[3]);
            plotItemAvailable[1] = Int32.Parse(line_1[4]);
            plotItemAvailable[2] = Int32.Parse(line_1[5]);
            plotItemAvailable[3] = Int32.Parse(line_1[6]);

            harvestProducts[0] = Int32.Parse(line_1[7]);
            harvestProducts[1] = Int32.Parse(line_1[8]);
            harvestProducts[2] = Int32.Parse(line_1[9]);
            harvestProducts[3] = Int32.Parse(line_1[10]);

            moneyLeveling = Int32.Parse(line_2[0]);
            WorkerManager.Instance.workerPrice = Int32.Parse(line_2[2]);

            ShopManager.Instance.PurchasePrices[0] = Int32.Parse(line_2[3].Split('_')[0]);
            ShopManager.Instance.PurchasePrices[1] = Int32.Parse(line_2[4].Split('_')[0]);            
            ShopManager.Instance.PurchasePrices[2] = Int32.Parse(line_2[5].Split('_')[0]);            
            ShopManager.Instance.PurchasePrices[3] = Int32.Parse(line_2[6].Split('_')[0]);            
            
            ShopManager.Instance.SellPrices[0] = Int32.Parse(line_2[7]);   
            ShopManager.Instance.SellPrices[1] = Int32.Parse(line_2[8]);
            ShopManager.Instance.SellPrices[2] = Int32.Parse(line_2[9]);   
            ShopManager.Instance.SellPrices[3] = Int32.Parse(line_2[10]);
     
        }

        initialFileDir = Application.dataPath + "/InitialSaveFiles" + "/Time.csv";

        if (File.Exists(initialFileDir))
        {
            string[] line = File.ReadAllLines(initialFileDir);

            string[] line_1 = line[1].Split(',');

            WorkerManager.Instance.workingTime = float.Parse(line_1[0]);

            harvestTime[0] = float.Parse(line_1[1]);
            harvestTime[1] = float.Parse(line_1[2]);
            harvestTime[2] = float.Parse(line_1[3]);
            harvestTime[3] = float.Parse(line_1[4]);
        }

        UIManager.Instance.UpdateGameInfo();

        isLoading = false;
    }

    public void SaveData()
    {
        lastPlayTime = DateTime.UtcNow.ToString();

        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, saveLocation));
        bf.Serialize(file, saveData);
        file.Close();

        WorkerManager.Instance.SaveData();

        PlotManager.Instance.SaveData();
    }

    public void LoadData()
    {
        isLoading = true;
        if (File.Exists(string.Concat(Application.persistentDataPath, saveLocation)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, saveLocation), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();

            WorkerManager.Instance.LoadData();

            PlotManager.Instance.LoadData();

            CalculateInbetween();
        }

        else {
            NewGame();
        }

        UIManager.Instance.UpdateGameInfo();
        isLoading = false;
    }

    public void CalculateInbetween()
    {
        double timeLapse = (DateTime.UtcNow - DateTime.Parse(lastPlayTime)).TotalSeconds;
        double maxTime = 0d;

        while (timeLapse > 0)
        {
            Debug.Log(timeLapse);
            foreach (var obj in PlotManager.Instance.Plots)
            {
                Plot plot = obj.GetComponent<Plot>();

                if (plot.Locked) continue;

                if (plot.isWorking) // Worker is here
                {
                    if (plot.isHarvesting)
                    {
                        maxTime = Math.Max(plot.PlotItem.harvestTime - plot.PlotItem.harvestTimer, maxTime);

                        if (timeLapse >= (plot.PlotItem.harvestTime - plot.PlotItem.harvestTimer))
                        {
                            plot.HarvestPlant();
                            WorkerManager.Instance.IdleWorkers+=1;
                        }
                        else {
                            plot.PlotItem.harvestTimer+=(float)timeLapse;
                        }
                        continue;
                    }
                    if (plot.isPlanting)
                    {
                        maxTime = Math.Max(maxTime, plot.workingTimer);
                        if (timeLapse >= plot.workingTimer)
                        {
                            plot.Planting();
                            WorkerManager.Instance.IdleWorkers+=1;
                        }
                            
                        else
                            plot.workingTimer -= (float)timeLapse;
                        
                        continue;
                    }
                }
                
                // No workers here
                if (plot.isHarvestable)
                {
                    if (WorkerManager.Instance.IdleWorkers >= 0)
                    {
                        plot.isWorking = true;
                        plot.isHarvesting = true;

                        WorkerManager.Instance.IdleWorkers--;
                    }
                    continue;
                }
                else {
                    selectedPlotItem = CheckPlotItemAvailable();

                    if (selectedPlotItem == PlotItemTypeEnum.None) continue;

                    plot.PlotItem = plot.plotItemDictionary[selectedPlotItem].GetComponent<PlotItem>();

                    if (WorkerManager.Instance.IdleWorkers >= 0)
                    {
                        plotItemAvailable[(int)selectedPlotItem]--;

                        plot.isPlanting = true;
                        plot.isWorking = true;

                        plot.Planting();
                    }
                    continue;
                }
            } // End foreach
            timeLapse -= maxTime;

            if (maxTime == 0) break;

            maxTime = default;
        } // End while

    }

    private void OnApplicationQuit() {
        SaveData();
    }
}
