using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ConfigureViewModel : MonoBehaviour
{
    public GameObject ConfigForm;
    public GameObject MainGO;

    public InputField SubjectName;
    public InputField SubjectId;
    public InputField EventId;
    public InputField StartingSpan;
    public InputField TriggerVal;
    public InputField TaskDuration;
    public InputField MaxSpan;
    public InputField ResponseTimeLimit;
    public Toggle TriggerOnScanner;
    public Button OpenExplorer;

    [SerializeField] public Dropdown SpriteFile;

    public void Awake() //method to preserve user input into the configure screen
    {
        var subName = PlayerPrefs.GetString("SubjectName");
        var subId = PlayerPrefs.GetString("SubjectId");
        var eventId = PlayerPrefs.GetString("EventId");
        var startingSpan = PlayerPrefs.GetString("StartingSpan");
        var trigger = PlayerPrefs.GetString("TriggerVal");
        var taskDuration = PlayerPrefs.GetString("TaskDuration");
        var maxSpan = PlayerPrefs.GetString("MaxSpan");
        var responseTimeLimit = PlayerPrefs.GetString("ResponseTimeLimit");
        
                
        if (!string.IsNullOrEmpty(subName))
            SubjectName.text = subName;
        else
            SubjectName.text = "N/A";

        if (!string.IsNullOrEmpty(subId))
            SubjectId.text = subId;
        else
            SubjectId.text = "N/A";

        if (!string.IsNullOrEmpty(eventId))
            EventId.text = eventId;
        else
            EventId.text = "N/A";

        if (!string.IsNullOrEmpty(startingSpan))
            StartingSpan.text = startingSpan;
        else
            StartingSpan.text = "2";

        if (!string.IsNullOrEmpty(taskDuration))
            TaskDuration.text = taskDuration;
        else
            TaskDuration.text = "N/A";

        if (!string.IsNullOrEmpty(maxSpan))
            MaxSpan.text = maxSpan;
        else
            MaxSpan.text = "N/A";

        if (!string.IsNullOrEmpty(responseTimeLimit))
            ResponseTimeLimit.text = responseTimeLimit;
        else
            ResponseTimeLimit.text = "N/A";

        if (!string.IsNullOrEmpty(trigger))
            TriggerVal.text = trigger;
        else
            TriggerVal.text = "5";
        

#if UNITY_WSA
        var itm = UnityEngine.WSA.Application.arguments;

        Debug.Log("Command Arguments: " + itm + " Length: " + itm.Length);

        if(itm.Length > 0)
            Application.LoadLevel("TaskMain");
#endif

    }

    public void Run()
    {

        Debug.Log("Run Called");
        Application.LoadLevel("TaskMain");

        Destroy(ConfigForm);
    }

    public void SetSubjectName(string subjectName)
    {
        TaskSettingsManager.TaskSettings.SubjectName = subjectName;
        PlayerPrefs.SetString("SubjectName", subjectName);
    }

    public void SetSubjectId(string subjectId)
    {
        TaskSettingsManager.TaskSettings.SubjectId = subjectId;
        PlayerPrefs.SetString("SubjectId", subjectId);
    }

    public void SetEventId(string eventId)
    {
        TaskSettingsManager.TaskSettings.EventId = eventId;
        PlayerPrefs.SetString("EventId", eventId);
    }

    public void EnableTriggerOnScanner(bool val)
    {
        TaskSettingsManager.TaskSettings.EnableTriggerOnScanner = val;
    }

    public void SetStartingSpan(string startingSpan)
    {
        TaskSettingsManager.TaskSettings.StartingSpan = startingSpan;
        PlayerPrefs.SetString("StartingSpan", startingSpan);
    }

    public void SetTaskDuration(string taskDuration) 
    {
        TaskSettingsManager.TaskSettings.TaskDuration = taskDuration;
        PlayerPrefs.SetString("TaskDuration", taskDuration);
    }

    public void SetMaxSpan(string maxSpan) 
    {
        TaskSettingsManager.TaskSettings.MaxSpan = maxSpan;
        PlayerPrefs.SetString("MaxSpan", maxSpan);
    }

    public void SetResponseTimeLimit(string responseTimeLimit)
    {
        TaskSettingsManager.TaskSettings.ResponseTimeLimit = responseTimeLimit;
        PlayerPrefs.SetString("ResponseTimeLimit", responseTimeLimit);
    }

    public void SetItem01KeyVal(string item01KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item01KeyVal = item01KeyVal;
        PlayerPrefs.SetString("Button1", item01KeyVal);
    }

    public void SetItem02KeyVal(string item02KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item02KeyVal = item02KeyVal;
        PlayerPrefs.SetString("Button2", item02KeyVal);
    }

    public void SetItem03KeyVal(string item03KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item03KeyVal = item03KeyVal;
        PlayerPrefs.SetString("Button3", item03KeyVal);
    }

    public void SetItem04KeyVal(string item04KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item04KeyVal = item04KeyVal;
        PlayerPrefs.SetString("Button4", item04KeyVal);
    }

    public void SetTriggerKeyVal(string triggerKeyVal) //scanner trigger value
    {
        TaskSettingsManager.TaskSettings.TriggerKeyVal = triggerKeyVal;
        PlayerPrefs.SetString("TriggerVal", triggerKeyVal);
    }

    public void Quit()
    {
        Debug.Log("Quit Called");
        Application.Quit();
    }

    public void SetupDisplay()
    {
        Debug.Log("Setup Screen Called");
        SceneManager.LoadScene("SetupDisplayScreen");
    }

    public void Return()
    {
        Debug.Log("Return to Config Screen");
        SceneManager.LoadScene("ConfigureScreen");
    }

    public void SetSprites (int selc)
    {
        Debug.Log(selc);
        TaskSettingsManager.TaskSettings.SpriteSheetNumber = selc;

        switch (selc)
        {
            case 0:
                TaskSettingsManager.TaskSettings.SpriteFileName = "Sprites/Numbers";
                break;

            case 1:
                TaskSettingsManager.TaskSettings.SpriteFileName = "Sprites/Symbols";
                break;

            case 2:
                TaskSettingsManager.TaskSettings.SpriteFileName = "Sprites/Foods";
                break;
        }
    }

    public void OpenFileExplorer ()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();

        p.StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe");
        p.Start();
    }

    public void Abort()
    {
        Debug.Log("Aborting Trial...Return to Config");
        SceneManager.LoadScene("ConfigureScreen");
    }

}
