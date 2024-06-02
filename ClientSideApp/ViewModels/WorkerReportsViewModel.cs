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
    public partial class WorkerReportsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkerReportsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ObservableCollection<Report> Reports { get; } = new();

        [RelayCommand]
        public async Task GetReports()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ReportRepository.ListAllAsync();

                if (response.Success)
                {
                    Reports.Clear();
                    foreach (var report in response.Data)
                    {
                        Reports.Add(report);
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
        async Task EditReport(Report report)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(Report), report },
                };

                await Shell.Current.GoToAsync($"{nameof(WorkerReportDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
