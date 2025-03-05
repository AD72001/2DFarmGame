using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlotItem : MonoBehaviour
{
    public string itemName;
    public int ID;
    public PlotItemTypeEnum itemType;

    // Sprites
    public Sprite[] stages;
    public float stageTransitionTime = 2.0f;
    public int currentStage = 0;
    public float stageTimer = 0.0f;

    // Harvest cycles
    public float harvestTime = 2.0f;
    public float harvestTimer = 0.0f;

    public int cycles = 10; // number of cycles an item lasts
    public int maxHarvest = 10; // maximum number of products
    public int availableHarvest = 0;
    // avail = extra + base: base = maxHarvest/cycle (1); extra = maxHarvest%cycle

    public int baseHarvest = 1;
    public int extraHarvest = 0;
    public int currentHarvest = 0;

    public float decayTime = 5;
    public float decayTimer = 0;

    // Save Location
    public string saveLocation = "";

    void Awake()
    {
        saveLocation = $"{transform.parent.name}_{name}";
    }

    void Start()
    {
        Reset();
    }

    void OnEnable()
    {

    }

    public void Reset()
    {
        harvestTime = GameManager.Instance.harvestTime[ID];
        currentStage = 0;

        stageTimer = 0;
        harvestTimer = 0;

        availableHarvest = 0;
        currentHarvest = 0;

        maxHarvest = cycles + cycles*GameManager.Instance.Level/10;
        extraHarvest = maxHarvest%cycles;
        baseHarvest = maxHarvest/cycles;
        // avail = extra + base: base = maxHarvest/cycle (1) extra = maxHarvest%cycle

        decayTimer = 0;

        GetComponent<SpriteRenderer>().sortingOrder = int.Parse( 
            transform.parent.name.Split('_')[1]
        );

        UpdateStage();
    }

    void Update()
    {
        stageTimer += Time.deltaTime;

        if (stageTimer >= stageTransitionTime)
        {
            stageTimer = 0;
            currentStage = Mathf.Clamp(currentStage+1, 0, stages.Length - 1);
            UpdateStage();
        }

        if (currentHarvest >= maxHarvest)
        {
            decayTimer += Time.deltaTime;

            if (decayTimer >= decayTime || availableHarvest == 0)
            {
                GetComponentInParent<Plot>().hasPlant = false;
                gameObject.SetActive(false);
            }

            return;
        }
        
        harvestTimer += Time.deltaTime;

        if (harvestTimer >= harvestTime)
        {
            harvestTimer = 0;
            availableHarvest = extraHarvest + baseHarvest;

            currentHarvest+=extraHarvest + baseHarvest;

            extraHarvest = Mathf.Clamp(extraHarvest-1, 0, extraHarvest);

            GetComponentInParent<Plot>().isHarvestable = true;
        }
    }

    public void UpdateStage()
    {
        GetComponent<SpriteRenderer>().sprite = stages[currentStage];
    }

    public void SaveData()
    {
        Debug.Log(gameObject.name);
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, saveLocation));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void LoadData()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, saveLocation)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, saveLocation), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }

        UpdateStage();
    }
}
