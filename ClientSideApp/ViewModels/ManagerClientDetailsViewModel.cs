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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Client), nameof(Client))]
    public partial class ManagerClientDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<UserSession?> Users { get; } = new();

        [ObservableProperty]
        private Client? _client;

        [ObservableProperty]
        private UserSession? _selectedUserSession;

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
        [Required(ErrorMessage = "Email name is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        private string _email = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Phone number is required.")]
        private string _phoneNumber = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Adress is required.")]
        private string _address = "";

        [ObservableProperty]
        private bool _isProcessed = false;


        [ObservableProperty]
        private string _firstNameError = "";

        [ObservableProperty]
        private string _lastNameError = "";

        [ObservableProperty]
        private string _emailError = "";

        [ObservableProperty]
        private string _phoneNumberError = "";

        [ObservableProperty]
        private string _addressError = "";


        async partial void OnClientChanged(Client? value)
        {
            if (value is null) return;

            FirstName = value.FirstName;
            LastName = value.LastName;
            Email = value.Email;
            PhoneNumber = value.PhoneNumber;
            Address = value.Address;
            IsProcessed = value.IsProcessed;

            IsBusy = false;
            await GetUsers();
        }

        public ManagerClientDetailsViewModel(IUnitOfWork unitOfWork)
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
                    Users.Add(null);
                    foreach (var user in response.Data)
                    {
                        Users.Add(user);
                        if (Client is not null && Client.AppUserId == user.Id)
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
        async Task SaveClient()
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
                    AddressError = string.Join(Environment.NewLine, GetErrors(nameof(Address)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                FirstNameError = "";
                LastNameError = "";
                EmailError = "";
                PhoneNumberError = "";
                AddressError = "";

                ApiResponse<Client?> response;

                if (Client is null)
                {
                    response = await _unitOfWork.ClientRepository.AddAsync(new ClientDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Address = Address,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        IsProcessed = IsProcessed,
                        AppUserId = SelectedUserSession?.Id, 
                    });
                }
                else
                {
                    response = await _unitOfWork.ClientRepository.UpdateAsync(Client.Id, new ClientDTO
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Address = Address,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        IsProcessed = IsProcessed,
                        AppUserId = SelectedUserSession?.Id,
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
                    IsBusy = false ;
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
