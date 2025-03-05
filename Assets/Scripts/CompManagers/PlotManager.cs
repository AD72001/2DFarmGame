using System.Linq;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    [SerializeField] private GameObject[] plots;

    public GameObject[] Plots 
    {
        get { 
            if (plots == null)
                plots = GameObject.FindGameObjectsWithTag("Plot").OrderBy(obj => obj.name).ToArray();
            return plots; }
    }
    public static PlotManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        plots = GameObject.FindGameObjectsWithTag("Plot").OrderBy(obj => obj.name).ToArray();
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLoading && WorkerManager.Instance.IdleWorkers > 0)
        {
            ManagePlot();
        }
    }

    void ManagePlot()
    {
        foreach (GameObject obj in plots)
        {
            if (WorkerManager.Instance.IdleWorkers <= 0) break;

            Plot plot = obj.GetComponent<Plot>();

            if (plot.isWorking || plot.Locked) continue;

            if (!plot.hasPlant)
            {
                GameManager.Instance.selectedPlotItem = GameManager.Instance.CheckPlotItemAvailable();
                //Debug.Log($"{obj.name}--{GameManager.Instance.selectedPlotItem}");
                if (GameManager.Instance.selectedPlotItem == PlotItemTypeEnum.None) continue;

                plot.PlotItem = plot.plotItemDictionary[GameManager.Instance.selectedPlotItem].GetComponent<PlotItem>();

                GameManager.Instance.plotItemAvailable[(int)GameManager.Instance.selectedPlotItem]--;

                plot.workingTimer = WorkerManager.Instance.workingTime;
                plot.isPlanting = true;
                plot.isWorking = true;
                WorkerManager.Instance.IdleWorkers--;

                continue;
            }

            if (plot.isHarvestable)
            {
                plot.isHarvesting = true;
                plot.isWorking = true;
                plot.workingTimer = WorkerManager.Instance.workingTime;
                WorkerManager.Instance.IdleWorkers--;
            }
        }
    }

    public void SaveData()
    {
        foreach (var plot in plots)
        {
            plot.GetComponent<Plot>().SaveData();
        }
    }

    public void LoadData()
    {
        foreach (var plot in plots)
        {
            plot.GetComponent<Plot>().LoadData();
        }
    }
}
