using ClientSideApp.Models;
using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(UserSession), nameof(UserSession))]
    public partial class AdminUserDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private UserSession? _userSession;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "First name is required.")]
        private string _firstName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Last name is required.")]
        private string _lastName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        private string _email= "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Phone number is required.")]
        private string _phoneNumber = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Role is required.")]
        private string _role = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _passwordMention = "";


        [ObservableProperty]
        private string _firstNameError = "";

        [ObservableProperty]
        private string _lastNameError = "";

        [ObservableProperty]
        private string _emailError = "";

        [ObservableProperty]
        private string _phoneNumberError = "";

        [ObservableProperty]
        private string _roleError = "";

        [ObservableProperty]
        private string _passwordError = "";


        partial void OnUserSessionChanged(UserSession? value)
        {
            if (value is null) return;

            FirstName = value.FirstName;
            LastName = value.LastName;
            Email = value.Email;
            PhoneNumber = value.PhoneNumber;
            Role = value.Role;
            PasswordMention = "If you dont want to change password, leave this field empty";
        }

        public AdminUserDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RelayCommand]
        async Task SaveUser()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ValidateAllProperties();

                var passwordValid = true;
                if (UserSession is null && Password.IsNullOrEmpty())
                {
                    passwordValid = false;
                }

                if (HasErrors || !passwordValid)
                {
                    PasswordError = passwordValid ? "" : "Password is required.";
                    FirstNameError = string.Join(Environment.NewLine, GetErrors(nameof(FirstName)).Select(e => e.ErrorMessage));
                    LastNameError = string.Join(Environment.NewLine, GetErrors(nameof(LastName)).Select(e => e.ErrorMessage));
                    EmailError = string.Join(Environment.NewLine, GetErrors(nameof(Email)).Select(e => e.ErrorMessage));
                    PhoneNumberError = string.Join(Environment.NewLine, GetErrors(nameof(PhoneNumber)).Select(e => e.ErrorMessage));
                    RoleError = string.Join(Environment.NewLine, GetErrors(nameof(Role)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                PasswordError = "";
                FirstNameError = "";
                LastNameError = "";
                EmailError = "";
                PhoneNumberError = "";
                RoleError = "";

                ApiResponse<string> response; 

                if (UserSession is null)
                {
                    response = await _unitOfWork.UserRepository.CreateAccount(new RegisterDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Role = Role,
                        Password = Password,
                    });
                }
                else
                {
                    response = await _unitOfWork.UserRepository.UpdateAsync(UserSession.Id, new RegisterDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Role = Role,
                        Password = Password,
                    });
                }

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
