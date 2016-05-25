
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskSettingsManager : MonoBehaviour
{

    public static TaskSettings TaskSettings = new TaskSettings();

    void Awake()
    {
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
    }
}

public class TaskSettings
{
    public string SpriteFileName;
    public int SpriteSheetNumber;
    public string StartingSpan;//the initial span length (how many numbers will be displayed for the first iteration)
    public string SubjectName;
    public string SubjectId;
    public string EventId;
    public string TaskDuration;
    public string MaxSpan;
    public string ResponseTimeLimit;
    public string Item01KeyVal;
    public string Item02KeyVal;
    public string Item03KeyVal;
    public string Item04KeyVal;
    public string TriggerKeyVal = "5";
    public bool EnableTriggerOnScanner;
    public string AbortKeyVal = "x";

}