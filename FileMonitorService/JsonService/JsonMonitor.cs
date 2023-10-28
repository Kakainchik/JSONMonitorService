﻿using System.Text.Json;

namespace FileMonitorService.JsonService
{
    public class JsonMonitor : IJsonMonitor
    {
        private const int DEFAULT_MILLISECONDS = 2000;

        private PeriodicTimer _timer;
        private FileSystemWatcher _watcher;
        private bool _disposedValue;
        private bool _fileChanged = false;

        public string FilePath { get; private set; }
        public JsonNodeTree Data { get; private set; }

        public event EventHandler<bool>? DataChecked;

        public JsonMonitor(string path)
        {
            FilePath = path;
            Data = new JsonNodeTree();
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(DEFAULT_MILLISECONDS));

            _watcher = new FileSystemWatcher();
            _watcher.Path = Path.GetDirectoryName(path)!;
            _watcher.Filter = Path.GetFileName(path);
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;
        }

        public void StartMonitoring()
        {
            Data = RetrieveData();

            _watcher.Changed += OnFileChanged;

            RepeatForEver();
        }

        public void StopMonitoring()
        {
            _watcher.Changed -= OnFileChanged;
        }

        public bool ForceCheck()
        {
            _fileChanged = false;

            JsonNodeTree newData = RetrieveData();

            //Check for changes if any
            bool changed = !Data.Equals(newData);
            if (changed)
                Data = newData;

            return changed;
        }

        public JsonNodeTree RetrieveData()
        {
            JsonNodeTree mainNode = new JsonNodeTree()
            {
                Root = null,
                IsArray = false
            };
            ReadOnlySpan<byte> jsonSpan = File.ReadAllBytes(FilePath);
            Utf8JsonReader rd = new Utf8JsonReader(jsonSpan);

            void DisassembleBlock(JsonNodeTree root, ref Utf8JsonReader reader)
            {
                int index = 0;
                while (reader.Read())
                {
                    JsonTokenType tokenType = reader.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.StartObject:
                            {
                                if (root.IsArray)
                                {
                                    JsonNodeTree arrayNode = new JsonNodeTree()
                                    {
                                        Root = root,
                                        ArrayIndex = index++
                                    };
                                    root.Branches.AddLast(arrayNode);
                                    DisassembleBlock(arrayNode, ref reader);
                                    break;
                                }

                                JsonNodeTree? subNode = root.Branches.Last?.Value;
                                if (subNode == null)
                                    continue;

                                subNode.IsComplex = true;
                                DisassembleBlock(subNode, ref reader);
                                break;
                            }
                        case JsonTokenType.StartArray:
                            {
                                JsonNodeTree subNode = root.Branches.Last!.Value;
                                subNode.IsArray = true;

                                DisassembleBlock(subNode, ref reader);
                                break;
                            }
                        case JsonTokenType.EndObject:
                        case JsonTokenType.EndArray:
                            {
                                return;
                            }
                        case JsonTokenType.PropertyName:
                            {
                                JsonNodeTree node = new JsonNodeTree()
                                {
                                    PropertyName = reader.GetString(),
                                    Root = root
                                };
                                root.Branches.AddLast(node);
                                break;
                            }
                        case JsonTokenType.Null:
                            {
                                root.Branches.Last!.Value.Value = null;
                                break;
                            }
                        case JsonTokenType.String:
                            {
                                root.Branches.Last!.Value.Value = reader.GetString();
                                break;
                            }
                        case JsonTokenType.Number:
                            {
                                if (reader.TryGetInt64(out long lValue))
                                    root.Branches.Last!.Value.Value = lValue;
                                else if (reader.TryGetDouble(out double dValue))
                                    root.Branches.Last!.Value.Value = dValue;
                                break;
                            }
                        case JsonTokenType.False:
                        case JsonTokenType.True:
                            {
                                root.Branches.Last!.Value.Value = reader.GetBoolean();
                                break;
                            }
                    }
                }
            }

            try
            {
                DisassembleBlock(mainNode, ref rd);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("The JSON is corrupted to be parsed.", ex);
            }

            return mainNode;
        }

        private async void RepeatForEver()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                if (!_fileChanged)
                {
                    DataChecked?.Invoke(this, false);
                    continue;
                }

                bool isChecked = ForceCheck();
                DataChecked?.Invoke(this, isChecked);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
                _fileChanged = true;
        }

        #region IDisposable implementation

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Ddispose managed state (managed objects)
                    _timer.Dispose();
                    _watcher.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                Data = null;
                FilePath = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        #endregion
    }
}