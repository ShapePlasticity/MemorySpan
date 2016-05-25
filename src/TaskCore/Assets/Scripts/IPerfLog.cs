using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    interface IPerfLog
    {
        void Init();
        void LogSummary(string SummeryMetrics);
        void LogRaw(string rawLine);
        void Term();
    }

    public class DebugLogger : IPerfLog
    {
        public void LogSummary(string SummeryMetrics)
        {
            Debug.Log(SummeryMetrics);
        }

        public void LogRaw(string rawLine)
        {

        }

        public void Init()
        {

        }

        public void Term()
        {

        }
    }

    public class GridLogger : IPerfLog
    {
        public void LogSummary(string SummeryMetrics)
        {

        }

        public void LogRaw(string rawLine)
        {

        }

        public void Init()
        {

        }

        public void Term()
        {

        }
    }

    public class FileSystemLogger : IPerfLog
    {
        private StringBuilder _sbRaw;
        private string _filePath;
        private string _folderPath;

        public FileSystemLogger() //constructor
        {
            _sbRaw = new StringBuilder();
        }

        public void LogSummary(string SummaryMetrics)
        {
            _filePath = Application.persistentDataPath;

            string _subId = TaskSettingsManager.TaskSettings.SubjectId;

            DateTime d = DateTime.Now;
            string _dateTime = d.ToString("yyyyMMdd_HHmmss");

            string _fileName = _filePath + "_" + "SummeryLog" + "_" + _subId + "_" + _dateTime + ".txt";
            //File.WriteAllText(_fileName, SummaryMetrics);
        }

        public void LogRaw(string rawLine)
        {
            _sbRaw.AppendLine(rawLine);
        }

        public void Init()
        {
            _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\MemorySpan_Data\";

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        public void Term()
        {

            string _subId = TaskSettingsManager.TaskSettings.SubjectId;
            string _eventId = TaskSettingsManager.TaskSettings.EventId;

            DateTime d = DateTime.Now;
            string dateTime = d.ToString("yyyyMMdd_HHmmss");

            string fileName = Path.Combine(_folderPath, dateTime + "_" + _subId + "_" + _eventId + "_" + "RawLog" + ".txt");
            File.WriteAllText(fileName, "iterationCount, currentSpanLength, ResponseEval, MaxSpan" + Environment.NewLine + _sbRaw.ToString() );
        }

    }
}





