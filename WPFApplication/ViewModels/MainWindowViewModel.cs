using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using WPFApplication.Core;
using WPFApplication.Models;

namespace WPFApplication.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private IJsonMonitor _jsonMonitor;
        private ObservableCollection<JsonNodeTree> _nodesCollection = new ObservableCollection<JsonNodeTree>();
        private CollectionViewSource _collectionViewSource = new CollectionViewSource();

        public ICollectionView View => _collectionViewSource.View;

        public ICommand ForceCheckCommand { get; set; }

        public MainWindowViewModel(IJsonMonitor jsonMonitor)
        {
            _jsonMonitor = jsonMonitor;
            _collectionViewSource.Source = _nodesCollection;

            ForceCheckCommand = new RelayCommand(OnForceCheck);
        }

        public void UpdateData()
        {
            _nodesCollection.Add(_jsonMonitor.Data);
        }

        private void OnForceCheck(object? obj)
        {
            _jsonMonitor.ForceCheck();
            UpdateData();
        }

        private void PopulateData()
        {
            
        }
    }
}