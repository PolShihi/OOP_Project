using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views;
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
    public partial class ManagerReportsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManagerReportsViewModel(IUnitOfWork unitOfWork)
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

                if (!response.Success)
                {
                    await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                    if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                    {
                        await AppConstant.LogOut();
                    }

                    IsBusy = false;
                    return;
                }

                foreach (var report in response.Data)
                {
                    if (report.AppUserId is not null)
                    {
                        var responseUser = await _unitOfWork.UserRepository.GetByIdAsync(report.AppUserId);

                        if (!responseUser.Success)
                        {
                            await Shell.Current.DisplayAlert("Error", responseUser.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, responseUser.Errors), "Ok");

                            if (responseUser.StatusCode == 401 || responseUser.StatusCode == 403 || responseUser.StatusCode == 0)
                            {
                                await AppConstant.LogOut();
                            }

                            IsBusy = false;
                            return;
                        }

                        report.UserSession = responseUser.Data;
                    }
                }

                Reports.Clear();
                foreach (var report in response.Data)
                {
                    Reports.Add(report);
                }

            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddReport()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(ManagerReportDetailsPage)}");
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

                await Shell.Current.GoToAsync($"{nameof(ManagerReportDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteReport(Report report)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ReportRepository.DeleteAsync(report.Id);

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
                await GetReports();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SendEmail(Report report)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(Report), report },
                };

                await Shell.Current.GoToAsync($"{nameof(EmailSendPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
