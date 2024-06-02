using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using MyModel.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientSideApp.Models;
using MyModel.Models.Entitties;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Report), nameof(Report))]
    [QueryProperty(nameof(Client), nameof(Client))]
    [QueryProperty(nameof(UserSession), nameof(UserSession))]
    public partial class EmailSendViewModel : BaseViewModel
    {
        private IUnitOfWork _unitOfWork;
        private ISettingsService _settingsService;

        public EmailSendViewModel(IUnitOfWork unitOfWork, ISettingsService settingsService)
        {
            _unitOfWork = unitOfWork;
            _settingsService = settingsService;
        }

        [ObservableProperty]
        private Client? _client;

        [ObservableProperty]
        private UserSession? _userSession;

        [ObservableProperty]
        private Report? _report;


        partial void OnUserSessionChanged(UserSession? value)
        {
            if (value is null) return;

            Body = $@"Dear {value.FullName},

I'm pleased to provide you with the details for your account and access to the application.

Your account email (username): {value.Email}
Your Role: {value.Role}
Application Host: {_settingsService.Host}
Application Port: {_settingsService.Port}

Please use these credentials to log in to the application. 

If you have any issues accessing the application or need further assistance, please don't hesitate to reach out to the IT support team.

Best regards,
{App.UserSession.FullName} ({App.UserSession.Email})";

            Subject = "Your Account Details";
            ReceiverEmail = value.Email;
        }

        async partial void OnReportChanged(Report? value)
        {
            if (value is null) return;

            Report = value;

            IsBusy = false;
            await GetUser();

            var userFullNameEmail = Report.UserSession is null ? string.Empty : $"{Report.UserSession.FullName} ({Report.UserSession.Email})";

            Body = $@"Dear {Report.FullName},

Thank you for bringing your complaint to our attention. We have carefully reviewed the details you provided:

Your Complaint:
{Report.Text}

Response:
{Report.Answer}

This issue has been resolved by {userFullNameEmail}, one of our employee. I want to assure you that we take all customer feedback seriously and have addressed your concerns.

Please let me know if you have any other questions or require further assistance.

Best regards,
{App.UserSession.FullName} ({App.UserSession.Email})";

            Subject = "Response to Your Complaint";
            ReceiverEmail = Report.Email;
        }

        async Task GetUser()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.UserRepository.ListAsync(u => u.Id == Report.AppUserId);

                if (response.Success)
                {
                    Report.UserSession = response.Data.Count == 0 ? null : response.Data.First();

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

        async partial void OnClientChanged(Client? value)
        {
            if (value is null) return;

            Client = value;

            IsBusy = false;
            await GetOrder();

            if (Client.Order is null)
            {
                await Shell.Current.DisplayAlert("Error", "There is no order", "Ok");
                await Shell.Current.GoToAsync("..");
                return;
            }

            await GetCeremony();
            await GetProductsAndProductOrders();
            await GetDeadPerson();

            string productOrders = "";
            foreach (var productOrder in Client.Order.ProductOrders)
            {
                productOrders += $"- {productOrder.Product.Name}, amount - {productOrder.Amount} units, total cost - {productOrder.GetTotalPrice()}, comment - {productOrder.Comment}\n";
            }

            Body = $@"Dear {Client.FullName},

Thank you for entrusting us with the funeral arrangements for your loved one. We have carefully noted the details you provided and are pleased to confirm the following:

Deceased Person Information:

Full Name: {Client.Order.DeadPerson.FirstName} {Client.Order.DeadPerson.LastName}
Date of Birth: {Client.Order.DeadPerson.DateOfBirth}
Date of Passing: {Client.Order.DeadPerson.DateOfDeath}
Additional Details: {Client.Order.DeadPerson.Details}

Funeral Ceremony Details:

Type: {Client.Order.Ceremony.Name}
Description: {Client.Order.Ceremony.Description}
Cost: {Client.Order.Ceremony.Price}
Date and time: {Client.Order.Date.Value}
Address: {Client.Order.Address}

Ordered Products:
{productOrders}

Total Cost: {Client.Order.GetTotalPrice()}

We appreciate your trust in our services and are committed to honoring the memory of your loved one. Please let us know if you have any other questions or require further assistance.

Best regards,
{App.UserSession.FullName} ({App.UserSession.Email})";

            Subject = $"Confirmation of Funeral Arrangements for {Client.Order.DeadPerson.FirstName} {Client.Order.DeadPerson.LastName}";
            ReceiverEmail = Client.Email;

        }

        async Task GetOrder()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.OrderRepository.ListAsync(o => o.ClientId == Client.Id);

                if (response.Success)
                {
                    Client.Order = response.Data.Count == 0 ? null : response.Data.First();

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

        async Task GetProductsAndProductOrders()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ProductOrderRepository.ListAsync(p => p.OrderId == Client.Order.Id);

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


                var responseProducts = await _unitOfWork.ProductRepository.ListAllAsync();

                if (!responseProducts.Success)
                {
                    await Shell.Current.DisplayAlert("Error", responseProducts.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, responseProducts.Errors), "Ok");

                    if (responseProducts.StatusCode == 401 || responseProducts.StatusCode == 403 || responseProducts.StatusCode == 0)
                    {
                        await AppConstant.LogOut();
                    }

                    IsBusy = false;
                    return;
                }


                foreach (var productOrder in response.Data)
                {
                    var product = responseProducts.Data.FirstOrDefault(p => p.Id == productOrder.ProductId);
                    productOrder.Product = product;
                }

                Client.Order.ProductOrders = response.Data;
            }
            finally
            {
                IsBusy = false;
            }
        }


        async Task GetCeremony()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.CeremonyRepository.ListAsync(c => c.Id == Client.Order.CeremonyId);

                if (response.Success)
                {
                    Client.Order.Ceremony = response.Data.Count == 0 ? null : response.Data.First();

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

        async Task GetDeadPerson()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.DeadPersonRepository.ListAsync(d => d.OrderId == Client.Order.Id);

                if (response.Success)
                {
                    Client.Order.DeadPerson = response.Data.Count == 0 ? null : response.Data.First();

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

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Receiver email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        private string _receiverEmail = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Subject is required.")]
        private string _subject = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Body is required.")]
        private string _body = "";

        [ObservableProperty]
        private string _receiverEmailError = "";

        [ObservableProperty]
        private string _subjectError = "";

        [ObservableProperty]
        private string _bodyError = "";

        [RelayCommand]
        async Task SendEmail()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ValidateAllProperties();

                if (HasErrors)
                {
                    ReceiverEmailError = string.Join(Environment.NewLine, GetErrors(nameof(ReceiverEmail)).Select(e => e.ErrorMessage));
                    SubjectError = string.Join(Environment.NewLine, GetErrors(nameof(Subject)).Select(e => e.ErrorMessage));
                    BodyError = string.Join(Environment.NewLine, GetErrors(nameof(Body)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                ReceiverEmailError = "";
                SubjectError = "";
                BodyError = "";

                var response = await _unitOfWork.EmailService.SendEmailAsync(new EmailSendRequestDTO
                {
                    ToEmail = ReceiverEmail,
                    Body = Body,
                    Subject = Subject,
                });

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert("Success", "Email has been sended.", "Ok");
                    await Shell.Current.GoToAsync("..");

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
    }
}
