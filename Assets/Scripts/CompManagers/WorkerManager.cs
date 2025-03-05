using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class WorkerManager : MonoBehaviour
{
    [SerializeField] private int _totalWorkers = 1;
    [SerializeField] private int _idleWorkers = 0;
    public int workerPrice = 500;

    [SerializeField] private string saveLocation = "/WorkerState.sav";

    public int IdleWorkers
    {
        get { return _idleWorkers; }
        set { 
            _idleWorkers = value; 
            UIManager.Instance.UpdateWorkerText();
            }
    }

    public int TotalWorkers
    {
        get { return _totalWorkers; }
        set { 
            _totalWorkers = value; 
            UIManager.Instance.UpdateWorkerText();
            }
    }

    public float workingTime = 2;

    public static WorkerManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;

        _idleWorkers = _totalWorkers;
    }

    public void AddWorker(int value)
    {
        if (GameManager.Instance.Money >= workerPrice) {
            TotalWorkers+=value;
            IdleWorkers+=value;
            GameManager.Instance.Money -= workerPrice;
        }
    }

    public void SaveData()
    {
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
    }
}
