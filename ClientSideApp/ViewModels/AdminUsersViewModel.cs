using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public ObservableCollection<UserInfo> Users { get; } = new();

        IUserService _userService;

        public AdminUsersViewModel(IUserService userService)
        {
            _userService = userService;
        }

        [RelayCommand]
        public async Task GetUsers()
        {
            if (IsBusy) 
                return;

            try
            {
                IsBusy = true;
                var users = await _userService.GetUsers();
                Users.Clear();
                foreach (var user in users) { Users.Add(user); }
            }
            catch (Exception ex) 
            { 

            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToRegister()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(AdminUserRegistrationPage)}");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DeleteUser(UserInfo user)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await _userService.DeleteUser(user.Id);

                IsBusy = false;

                await GetUsers();
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
