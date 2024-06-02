using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views;
using ClientSideApp.Views.Admin;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    public partial class AdminUsersViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<UserSession> Users { get; } = new();

        public AdminUsersViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    foreach (var user in response.Data) 
                    { 
                        Users.Add(user); 
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
        async Task AddUser()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(AdminUserDetailsPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task EditUser(UserSession user)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(UserSession), user },
                };

                await Shell.Current.GoToAsync($"{nameof(AdminUserDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false ;
            }
        }

        [RelayCommand]
        async Task DeleteUser(UserSession user)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.UserRepository.DeleteAsync(user.Id);

                if(!response.Success)
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
                await GetUsers();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SendEmail(UserSession userSession)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(UserSession), userSession },
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
