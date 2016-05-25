using UnityEngine;
using System;
using Assets.Scripts;
using System.Collections;

public class ScannerHandler : MonoBehaviour
{

    private TaskEngine _taskEngine;
    private ConfigureViewModel _configView;

    //do I need a new instance or log to the instance that is created in task engine??
    //private IPerfLog _logger = new FileSystemLogger();

    Sprite[] _waitingSprite;

    private bool _trigger = false;
    private bool _firstTrigger = false;
    private long _scannerDelayStart;
    private long _scannerDelayEnd;
    public long ScannerDelay = 0;

    private int _scanCount;
    private DateTime _scanTime = DateTime.Now;

    // Use this for initialization
    void Awake()
    {
        _taskEngine = FindObjectOfType<TaskEngine>();
        _configView = FindObjectOfType<ConfigureViewModel>();

        _waitingSprite = Resources.LoadAll<Sprite>("Sprites/hourglass");
        _trigger = Input.GetKeyDown(TaskSettingsManager.TaskSettings.TriggerKeyVal);

        _scannerDelayStart = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
    }

    public void WaitForTrigger()
    {
        if (!_trigger && _taskEngine.CurrentTaskState == TaskState.WaitForTrigger)
        {
            _taskEngine.MainGO.GetComponent<SpriteRenderer>().sprite = _waitingSprite[0];
            _taskEngine.MainGO.SetActive(true);
        }

        if (_trigger && _taskEngine.CurrentTaskState == TaskState.WaitForTrigger)
        {
            _scannerDelayEnd = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            ScannerDelay = (_scannerDelayEnd - _scannerDelayStart);

            _taskEngine.MainGO.SetActive(false);
            _taskEngine.CurrentTaskState = TaskState.RunTask;
        }
    }

    // Update is called once per frame 
    void Update()
    {
        //listening for trigger --> give this job to keyboardHandler
        _trigger = Input.GetKeyDown(TaskSettingsManager.TaskSettings.TriggerKeyVal);

        if (_trigger)
        {
            _scanCount++;
            _scanTime = DateTime.Now;

            string _sT = _scanTime.ToString("yyyyMMdd-HHmmss.fff");

            //_taskEngine._logger.LogScan(_scanCount + " , " + _sT);
        }
    }
}
