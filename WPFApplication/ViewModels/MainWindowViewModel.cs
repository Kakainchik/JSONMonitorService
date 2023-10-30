using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using FileMonitorService.JsonService;
using WPFApplication.Core;

namespace WPFApplication.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private static int s_checkCount = 0;

        private IJsonMonitor _jsonMonitor;
        private bool _isDataChanged;
        private ObservableCollection<JsonNodeTree> _nodesCollection = new ObservableCollection<JsonNodeTree>();
        private CollectionViewSource _collectionViewSource = new CollectionViewSource();

        public ICollectionView View => _collectionViewSource.View;

        public int CheckCount
        {
            get => s_checkCount;
            set
            {
                s_checkCount = value;
                OnPropertyChanged(nameof(CheckCount));
            }
        }
        public bool IsDataChanged
        {
            get => _isDataChanged;
            set
            {
                _isDataChanged = value;
                OnPropertyChanged(nameof(IsDataChanged));
            }
        }

        public ICommand ForceCheckCommand { get; set; }

        public MainWindowViewModel(IJsonMonitor jsonMonitor)
        {
            _jsonMonitor = jsonMonitor;
            _collectionViewSource.Source = _nodesCollection;

            //Click button 'Force check'
            ForceCheckCommand = new RelayCommand(OnForceCheck);

            _jsonMonitor.DataChecked += JsonMonitor_DataChanged;
        }

        public void UpdateData()
        {
            _nodesCollection.Clear();
            _nodesCollection.Add(_jsonMonitor.Data);
        }

        private void OnForceCheck(object? obj)
        {
            CheckCount++;
            IsDataChanged = _jsonMonitor.ForceCheck();
            if(IsDataChanged)
                UpdateData();
        }

        private void JsonMonitor_DataChanged(object? sender, bool changedArg)
        {
            CheckCount++;
            IsDataChanged = changedArg;
            if(changedArg)
                UpdateData();
        }
    }
}