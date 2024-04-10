using ClientSideApp.Models;
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
    public partial class AdminUserRegistrationViewModel : BaseViewModel
    {
        IUserService _userService;

        [ObservableProperty]
        private UserInfo _user = new UserInfo();

        [ObservableProperty]
        private string? _password;

        public AdminUserRegistrationViewModel(IUserService service)
        {
            _userService = service;
        }

        [RelayCommand]
        async Task AddUser()
        {
            if (IsBusy) 
                return;

            try
            {
                IsBusy = true;

                var registrationRequest = new RegistrationRequest
                {
                    Id = User.Id,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Email = User.Email,
                    PhoneNumber = User.PhoneNumber,
                    Role = User.Role,
                    Password = Password
                };

                await _userService.Register(registrationRequest);

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
