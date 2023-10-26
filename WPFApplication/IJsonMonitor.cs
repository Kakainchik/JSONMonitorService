using System;
using WPFApplication.Models;

namespace WPFApplication
{
    public interface IJsonMonitor : IDisposable
    {
        string FilePath { get; }
        JsonNodeTree Data { get; }

        /// <summary>
        /// Start monitoring for any changes in the file.
        /// </summary>
        void StartMonitoring();
        void StopMonitoring();
        bool ForceCheck();
    }
}