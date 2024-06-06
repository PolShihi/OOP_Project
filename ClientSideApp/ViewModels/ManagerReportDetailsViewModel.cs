using ClientSideApp.Models;
using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Report), nameof(Report))]
    public partial class ManagerReportDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManagerReportDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ObservableCollection<UserSession?> Users { get; } = new();

        [ObservableProperty]
        private Report? _report;

        [ObservableProperty]
        private UserSession? _selectedUserSession;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "First name is required")]
        private string _firstName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Last name is required")]
        private string _lastName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        private string _email = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Phone number is required")]
        private string _phoneNumber = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Text is required")]
        private string _text = "";

        [ObservableProperty]
        private bool _isAnswered = false;

        [ObservableProperty]
        private string _answer = "";


        [ObservableProperty]
        private string _firstNameError = "";

        [ObservableProperty]
        private string _lastNameError = "";

        [ObservableProperty]
        private string _emailError = "";

        [ObservableProperty]
        private string _phoneNumberError = "";

        [ObservableProperty]
        private string _textError = "";

        async partial void OnReportChanged(Report? value)
        {
            if (value is null)
            {
                await GetUsers();
                return;
            }

            FirstName = value.FirstName;
            LastName = value.LastName;
            Email = value.Email;
            PhoneNumber = value.PhoneNumber;
            Text = value.Text;  
            IsAnswered = value.IsAnswered;
            Answer = value.Answer;

            IsBusy = false;
            await GetUsers();
        }

        [RelayCommand]
        public async Task GetUsers()
        {

            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.UserRepository.ListAllAsync();

                if (response.Success)
                {
                    Users.Clear();
                    Users.Add(null);
                    foreach (var user in response.Data)
                    {
                        Users.Add(user);
                        if (Report is not null && Report.AppUserId == user.Id)
                        {
                            SelectedUserSession = user;
                        }
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
        async Task SaveReport()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ValidateAllProperties();

                if (HasErrors)
                {
                    FirstNameError = string.Join(Environment.NewLine, GetErrors(nameof(FirstName)).Select(e => e.ErrorMessage));
                    LastNameError = string.Join(Environment.NewLine, GetErrors(nameof(LastName)).Select(e => e.ErrorMessage));
                    EmailError = string.Join(Environment.NewLine, GetErrors(nameof(Email)).Select(e => e.ErrorMessage));
                    PhoneNumberError = string.Join(Environment.NewLine, GetErrors(nameof(PhoneNumber)).Select(e => e.ErrorMessage));
                    TextError = string.Join(Environment.NewLine, GetErrors(nameof(Text)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                FirstNameError = "";
                LastNameError = "";
                EmailError = "";
                PhoneNumberError = "";
                TextError = "";

                ApiResponse<Report?> response;

                if (Report is null)
                {
                    response = await _unitOfWork.ReportRepository.AddAsync(new ReportDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Text = Text,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        IsAnswered = IsAnswered,
                        AppUserId = SelectedUserSession?.Id,
                        Answer = Answer,
                    });
                }
                else
                {
                    response = await _unitOfWork.ReportRepository.UpdateAsync(Report.Id, new ReportDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Text = Text,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        IsAnswered = IsAnswered,
                        AppUserId = SelectedUserSession?.Id,
                        Answer = Answer,
                    });
                }

                if (response.Success)
                {
                    await Shell.Current.GoToAsync("..");

                    IsBusy = false;
                    return;
                }

                await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                if (response.StatusCode == 400)
                {
                    IsBusy = false;
                    await GetUsers();
                    IsBusy = true;
                }

                if (response.StatusCode == 404)
                {
                    await Shell.Current.GoToAsync("..");
                }

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

    }
}
