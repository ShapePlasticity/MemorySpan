using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;

public class KeyboardHandler : MonoBehaviour
{

    private TaskEngine _taskEngine;
    private ConfigureViewModel _configView;

    // Use this for initialization
    void Awake()
    {
        _taskEngine = FindObjectOfType<TaskEngine>();
        _configView = FindObjectOfType<ConfigureViewModel>();
    }

    void ProcessKeyboardChoice()  // TODO: Really need to use the input manager to help with this; may consider using rewired instead
    {
        try
        {
            var abort = Input.GetKeyDown(TaskSettingsManager.TaskSettings.AbortKeyVal);
            if (abort)
            {
                _taskEngine.AbortTrial();
                _configView.Abort();
            }
        }

        catch (System.Exception e)
        {
            int i = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessKeyboardChoice();
    }
}
