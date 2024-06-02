using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views.Manager;
using CommunityToolkit.Mvvm.Input;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    public partial class ManagerCeremoniesViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<Ceremony> Ceremonies { get; } = new();

        public ManagerCeremoniesViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RelayCommand]
        public async Task GetCeremonies()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.CeremonyRepository.ListAllAsync();

                if (response.Success)
                {
                    Ceremonies.Clear();
                    foreach (var ceremony in response.Data)
                    {
                        Ceremonies.Add(ceremony);
                    }

                    IsBusy = false;
                    return;
                }

                await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                {
                    await AppConstant.LogOut();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddCeremony()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(ManagerCeremonyDetailsPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task EditCeremony(Ceremony ceremony)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(Ceremony), ceremony },
                };

                await Shell.Current.GoToAsync($"{nameof(ManagerCeremonyDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteCeremony(Ceremony ceremony)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.CeremonyRepository.DeleteAsync(ceremony.Id);

                if (!response.Success)
                {
                    await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                    if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                    {
                        await AppConstant.LogOut();

                        IsBusy = false;
                        return;
                    }
                }

                IsBusy = false;
                await GetCeremonies();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
