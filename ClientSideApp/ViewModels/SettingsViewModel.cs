using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Host = _settingsService.Host;
            Port = _settingsService.Port;
        }

        [ObservableProperty]
        private string _host;

        [ObservableProperty]
        private int _port;

        [RelayCommand]
        private void SaveSettings()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _settingsService.Host = Host;
                _settingsService.Port = Port;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
