using ClientSideApp.Models;
using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        private string _email = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Password is required.")]
        private string _password = "";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private string _passwordError = "";

        [ObservableProperty]
        private string _emailError = "";


        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ValidateAllProperties();

                if (HasErrors)
                {
                    ErrorMessage = "Please correct the validation errors.";
                    PasswordError = string.Join(Environment.NewLine, GetErrors(nameof(Password)).Select(e => e.ErrorMessage));
                    EmailError = string.Join(Environment.NewLine, GetErrors(nameof(Email)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                ErrorMessage = "";
                PasswordError = "";
                EmailError = "";

                var loginDTO = new LoginDTO
                {
                    Email = Email,
                    Password = Password,
                };

                var response = await _unitOfWork.UserRepository.LoginAccount(loginDTO);

                if (response.Success)
                {
                    await SecureStorage.SetAsync("Token", response.Data.Token);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(response.Data.Token);
                    var userSession = new UserSession
                    {
                        Id = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                        FirstName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                        LastName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname).Value,
                        PhoneNumber = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value,
                        Email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                        Role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value,
                    };

                    App.UserSession = userSession;

                    await AppConstant.AddFlyoutMenusDetails();

                    IsBusy = false;
                    return;
                }

                ErrorMessage = response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
