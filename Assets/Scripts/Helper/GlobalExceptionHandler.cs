using System;
using System.Threading.Tasks;
using UnityEngine;

public class GlobalExceptionHandler : MonoBehaviour
{
    void Awake()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        Application.logMessageReceived += OnLogMessageReceived;
    }

    void OnDestroy()
    {
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ShowErrorDialog("Unhandled Exception: " + e.ExceptionObject.ToString());
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        ShowErrorDialog("Unobserved Task Exception: " + e.Exception.ToString());
        e.SetObserved();
    }

    private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            ShowErrorDialog("Unity Exception: " + condition + "\n" + stackTrace);
        }
    }

    private void ShowErrorDialog(string message)
    {
        // Replace this with your own dialog logic
        Debug.LogError(message);
        // Example: DialogHandler.Instance?.ShowError(message);
    }
}