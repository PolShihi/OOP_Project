using ClientSideApp.Models;
using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Report), nameof(Report))]
    public partial class WorkerReportDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkerReportDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ObservableProperty]
        private Report? _report;

        [ObservableProperty]
        private string _firstName = "";

        [ObservableProperty]
        private string _lastName = "";

        [ObservableProperty]
        private string _email = "";

        [ObservableProperty]
        private string _phoneNumber = "";

        [ObservableProperty]
        private string _text = "";

        [ObservableProperty]
        private bool _isAnswered = false;

        [ObservableProperty]
        private string _answer = "";

        partial void OnReportChanged(Report? value)
        {
            if (value is null) return;

            FirstName = value.FirstName;
            LastName = value.LastName;
            Email = value.Email;
            PhoneNumber = value.PhoneNumber;
            Text = value.Text;
            IsAnswered = value.IsAnswered;
            Answer = value.Answer;
        }

        [RelayCommand]
        async Task SaveReport()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ApiResponse<Report?> response;

                response = await _unitOfWork.ReportRepository.UpdateAsync(Report.Id, new ReportDTO
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Text = Text,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    IsAnswered = IsAnswered,
                    AppUserId = Report.AppUserId,
                    Answer = Answer,
                });

                if (response.Success)
                {
                    await Shell.Current.GoToAsync("..");

                    IsBusy = false;
                    return;
                }

                await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                if (response.StatusCode == 404)
                {
                    await Shell.Current.GoToAsync("..");
                }

                if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0 || response.StatusCode == 400)
                {
                    await AppConstant.LogOut();
                }

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
