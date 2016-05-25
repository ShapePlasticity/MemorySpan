
using System;
using UnityEngine;


public class TimeHandler : MonoBehaviour {

    public bool EndTask = false;
    public bool SwitchBlock = false;
    private int _taskLengthLimit;
    private int _trialBlockLimit = 40000;
    private int _baselineBlockLimit = 10000;
    private int _currentBlockLengthLimit = 0;
    private long _currentBlockLength = 0;
    private long _blockEndTime = 0;
    private long _currentTaskLength = 0;
    private long _taskStartTime = 0;


    private TaskSettings _gameManager;
    private TaskEngine _taskEngine;
    private ScannerHandler _scannerIn;

    void Awake()
    {
        _gameManager = TaskSettingsManager.TaskSettings;
        _taskEngine = FindObjectOfType<TaskEngine>();
        _scannerIn = FindObjectOfType<ScannerHandler>();

        //bring in task length from config..
        int.TryParse(_gameManager.TaskDuration, out _taskLengthLimit);
        //convert to ms...
        _taskLengthLimit = _taskLengthLimit * 1000;

        //take time stamp at task start
        _taskStartTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //TaskTimeManager();
        //BlockTimeManager();
	}

    public void BlockTimeManager()
    {
        SwitchBlock = false;

        if (_taskEngine.CurrentBlockType == BlockType.TrialBlock)
        {
            _currentBlockLengthLimit = _trialBlockLimit;
        }
        if (_taskEngine.CurrentBlockType == BlockType.BaselineBlock)
        {
            _currentBlockLengthLimit = _baselineBlockLimit;
        }


        _blockEndTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        _currentBlockLength = (_blockEndTime - _taskEngine.BlockStartTime);

        if (_currentBlockLength > _currentBlockLengthLimit)
        {
            SwitchBlock = true;
        }
    }

    public void TaskTimeManager()
    {
        long now = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        _currentTaskLength = (now - _taskStartTime);

        if (_currentTaskLength > _taskLengthLimit)
        {
            EndTask = true;
        }
    }
}
