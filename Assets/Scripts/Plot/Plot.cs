using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public bool hasPlant = false;
    [SerializeField] private bool _locked = false;
    public bool Locked {
        get { return _locked; }
        set { 
            _locked = value; 
            lockObject.SetActive(value);
            }
    }

    // ID
    public int ID {
        get { return int.Parse(gameObject.name.Split('_')[1]); }
    }

    // harvestable status
    public bool isWorking = false;
    public bool isHarvesting = false;
    public bool isPlanting = false;
    public bool isHarvestable = false;
    public float workingTimer = 0;

    // List of plot game objects
    public GameObject blueberryGO;
    public GameObject tomatoGO;
    public GameObject cowGO;
    public GameObject strawberryGO;

    // Locked sprite
    public GameObject lockObject;

    [SerializeField] private PlotItem currentPlotItem = null;

    // save the current plot
    [SerializeField] private string saveLocation;

    public PlotItem PlotItem
    {
        get { return currentPlotItem; }
        set { currentPlotItem = value; }
    }

    public Dictionary<PlotItemTypeEnum, GameObject> plotItemDictionary = new Dictionary<PlotItemTypeEnum, GameObject>();

    void Start()
    {
        SetDict();

        lockObject.SetActive(_locked);

        GetComponent<SpriteRenderer>().sortingOrder = int.Parse(gameObject.name.Split('_')[1]);

        saveLocation = $"/{gameObject.name}.sav";
    }

    void SetDict()
    {
        plotItemDictionary = new Dictionary<PlotItemTypeEnum, GameObject>()     
        {
            {PlotItemTypeEnum.Blueberry, blueberryGO},
            {PlotItemTypeEnum.Tomato, tomatoGO},
            {PlotItemTypeEnum.Cow, cowGO},
            {PlotItemTypeEnum.Strawberry, strawberryGO},
        };
    }

    void Update()
    {
        if (_locked || GameManager.Instance.isLoading)
        {
            return;
        }

        if (isWorking)
        {
            workingTimer-=Time.deltaTime;

            if (workingTimer <= 0)
            {
                if (isHarvesting) HarvestPlant();

                if (isPlanting) Planting();

                WorkerManager.Instance.IdleWorkers++;
            }

            return;
        }
    }

    private void OnMouseDown() 
    {
        if (isWorking || ShopManager.Instance.isShopping) return;

        if (Locked)
        {
            lockObject.GetComponent<Lock>().PurchasePlot();
            return;
        }

        if (hasPlant) 
        {
            HarvestPlant();
        }
        else 
        { 
            PlayerPlanting();
        }
    }

    public void HarvestPlant()
    {
        if (currentPlotItem != null)
        {
            GameManager.Instance.harvestProducts[currentPlotItem.GetComponent<PlotItem>().ID] 
            += currentPlotItem.availableHarvest;

            if (currentPlotItem.availableHarvest <= 0)
                return;
            
            isHarvestable = false;
            isHarvesting = false;

            isWorking = false;

            currentPlotItem.availableHarvest = 0;
            currentPlotItem.currentStage = currentPlotItem.stages.Length-2;
            currentPlotItem.UpdateStage();

            UIManager.Instance.UpdateProductInfo();
        }

    }

    public void Planting()
    {
        if (currentPlotItem != null)
        {
            currentPlotItem.gameObject.SetActive(true);
            hasPlant = true;
            currentPlotItem.Reset();
        }

        isPlanting = false;
        isWorking = false;

        UIManager.Instance.UpdatePlotItemInfo();
    }

    private void PlayerPlanting()
    {
        if (GameManager.Instance.playerSelectedPlotItem != PlotItemTypeEnum.None) 
        {
            currentPlotItem = plotItemDictionary[GameManager.Instance.playerSelectedPlotItem].GetComponent<PlotItem>();

            if (GameManager.Instance.plotItemAvailable[currentPlotItem.ID] > 0)
            {
                plotItemDictionary[GameManager.Instance.playerSelectedPlotItem].SetActive(true);
                GameManager.Instance.plotItemAvailable[currentPlotItem.ID]--;
                hasPlant = true;

                isPlanting = false;
                isWorking = false;

                UIManager.Instance.UpdatePlotItemInfo();
            }
        }
    }

    public void SaveData()
    {
        //Debug.Log(gameObject.name);
        if (currentPlotItem != null) currentPlotItem.SaveData();
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, saveLocation));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void LoadData()
    {
        //Debug.Log($"Loading data: {gameObject.name} {saveLocation}");
        if (File.Exists(string.Concat(Application.persistentDataPath, saveLocation)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, saveLocation), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }

        SetDict();

        if (currentPlotItem != null) 
        {
            currentPlotItem.LoadData();
            currentPlotItem.gameObject.SetActive(true);
        }

        lockObject.SetActive(Locked);
    }

    //
}

