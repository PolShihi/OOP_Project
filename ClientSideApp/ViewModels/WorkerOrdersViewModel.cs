using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views.Worker;
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
    public partial class WorkerOrdersViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<Client> Clients { get; } = new();

        public WorkerOrdersViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RelayCommand]
        public async Task GetClients()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ClientRepository.ListAllAsync();

                if (response.Success)
                {
                    Clients.Clear();
                    foreach (var client in response.Data)
                    {
                        Clients.Add(client);
                    }

                    IsBusy = false;
                    return;
                }

                await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                {
                    await AppConstant.LogOut();
                }

                IsBusy = false;
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task EditClient(Client client)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(Client), client },
                };

                await Shell.Current.GoToAsync($"{nameof(WorkerClientDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
