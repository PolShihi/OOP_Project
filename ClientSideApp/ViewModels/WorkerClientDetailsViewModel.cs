using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Validators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Client), nameof(Client))]
    public partial class WorkerClientDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkerClientDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ObservableProperty]
        private Client? _client;

        [ObservableProperty]
        private Order? _order;

        public int? OrderId { get; set; }

        public int? CeremonyId { get; set; }

        [ObservableProperty]
        private decimal _totalPrice;

        [ObservableProperty]
        private DeadPerson? _deadPerson;

        [ObservableProperty]
        private Ceremony? _selectedCeremony;

        public ObservableCollection<ProductOrder> ProductOrdersCompleted { get; } = new();

        public ObservableCollection<ProductOrder> ProductOrdersAdded{ get; } = new();

        public ObservableCollection<Ceremony> Ceremonies { get; } = new();

        public ObservableCollection<Product> Products { get; } = new();


        [ObservableProperty]
        private bool _isProcessed = false;


        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Address is required.")]
        private string _orderAddress = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [FutureDate(ErrorMessage = "Order date must be later than now.")]
        private DateTime _orderDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan _orderTime = TimeSpan.Zero;


        [ObservableProperty]
        private string _orderAddressError = "";

        [ObservableProperty]
        private string _orderDateError = "";


        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Amount is required.")]
        [NonNegativeInteger]
        private string _addedProductOrderAmount = "0";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Comment is required.")]
        private string _addedProductOrderComment = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Product is required.")]
        private Product? _addedProductOrderSelectedProduct;


        [ObservableProperty]
        private string _addedProductOrderAmountError = "";

        [ObservableProperty]
        private string _addedProductOrderCommentError = "";

        [ObservableProperty]
        private string _addedProductOrderSelectedProductError = "";


        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "First name is required.")]
        private string _deadPersonFirstName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Last name is required.")]
        private string _deadPersonLastName = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Details is required.")]
        private string _deadPersonDetails = "";

        [ObservableProperty]
        private DateTime _deadPersonDateOfBirth = DateTime.Today;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [PastDate(ErrorMessage = "Date of death must be earlier than now.")]
        [LaterThan(nameof(DeadPersonDateOfBirth), ErrorMessage = "Date of death must be later than date of birth.")]
        private DateTime _deadPersonDateOfDeath = DateTime.Today;


        [ObservableProperty]
        private string _deadPersonFirstNameError = "";

        [ObservableProperty]
        private string _deadPersonLastNameError = "";

        [ObservableProperty]
        private string _deadPersonDetailsError = "";

        [ObservableProperty]
        private string _deadPersonDateOfDeathError = "";

        void GetTotalPrice()
        {
            decimal totalPrice = 0;

            if (SelectedCeremony is not null)
            {
                totalPrice += SelectedCeremony.Price;
            }

            foreach (var order in ProductOrdersCompleted)
            {
                totalPrice += order.GetTotalPrice();
            }

            TotalPrice = totalPrice;
        }

        partial void OnSelectedCeremonyChanged(Ceremony? value)
        {
            GetTotalPrice();
        }

        async partial void OnClientChanged(Client? value)
        {
            if (value is null) return;

            IsProcessed = value.IsProcessed;

            IsBusy = true;

            var response = await _unitOfWork.OrderRepository.ListAsync(o => o.ClientId == value.Id);

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

            IsBusy = false;

            Order = response.Data.IsNullOrEmpty() ? null : response.Data.First();
            await ChangeOrder(Order);
        }

        async Task ChangeOrder(Order? value)
        {
            if (value is null)
            {
                IsBusy = false;
                await GetProducts();

                IsBusy = false;
                await GetCeremonies();

                IsBusy = false;
                return;
            }

            OrderAddress = value.Address;
            OrderDate = value.Date.Value;
            OrderTime = value.Date.Value.TimeOfDay;
            TotalPrice = value.TotalPrice;
            OrderId = value.Id;
            CeremonyId = value.CeremonyId;

            IsBusy = false;
            await GetProductsAndProductOrders();

            IsBusy = false;
            await GetCeremonies();

            IsBusy = true;

            var response = await _unitOfWork.DeadPersonRepository.ListAsync(d => d.OrderId == value.Id);

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

            IsBusy = false;

            DeadPerson = response.Data.IsNullOrEmpty() ? null : response.Data[0];
            DeadPersonChange(DeadPerson);
        }

        void DeadPersonChange(DeadPerson? value)
        {
            if (value is null) return;

            DeadPersonFirstName = value.FirstName;
            DeadPersonLastName = value.LastName;
            DeadPersonDetails = value.Details;
            DeadPersonDateOfBirth = value.DateOfBirth.Value;
            DeadPersonDateOfDeath = value.DateOfDeath.Value;
        }

        [RelayCommand]
        public async Task GetCeremonies()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.CeremonyRepository.ListAllAsync();

                if (response.Success)
                {
                    Ceremonies.Clear();
                    foreach (var ceremony in response.Data)
                    {
                        Ceremonies.Add(ceremony);
                        if (CeremonyId is not null && CeremonyId == ceremony.Id)
                        {
                            SelectedCeremony = ceremony;
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
        public async Task GetProducts()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ProductRepository.ListAllAsync();

                if (response.Success)
                {
                    Products.Clear();
                    foreach (var product in response.Data)
                    {
                        Products.Add(product);
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
        public async Task GetProductsAndProductOrders()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (OrderId is null)
                {
                    IsBusy = false;
                    return;
                }

                var response = await _unitOfWork.ProductOrderRepository.ListAsync(p => p.OrderId == OrderId);

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

                Products.Clear();
                foreach (var product in responseProducts.Data)
                {
                    Products.Add(product);
                }


                ProductOrdersCompleted.Clear();
                foreach (var productOrder in response.Data)
                {
                    ProductOrdersCompleted.Add(productOrder);
                }
                GetTotalPrice();

                List<ProductOrder> productOrdersRemovable = new();

                foreach (var productOrder in ProductOrdersAdded)
                {
                    if (!Products.Contains(productOrder.Product))
                    {
                        productOrdersRemovable.Add(productOrder);
                    }
                }

                foreach(var productOrder in productOrdersRemovable)
                {
                    ProductOrdersAdded.Remove(productOrder);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddProductOrder()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ValidateAllProperties();

                if (HasErrors)
                {
                    AddedProductOrderAmountError = string.Join(Environment.NewLine, GetErrors(nameof(AddedProductOrderAmount)).Select(e => e.ErrorMessage));
                    AddedProductOrderSelectedProductError = string.Join(Environment.NewLine, GetErrors(nameof(AddedProductOrderSelectedProduct)).Select(e => e.ErrorMessage));
                    AddedProductOrderCommentError = string.Join(Environment.NewLine, GetErrors(nameof(AddedProductOrderComment)).Select(e => e.ErrorMessage));

                    if (!(AddedProductOrderCommentError.IsNullOrEmpty() && AddedProductOrderSelectedProductError.IsNullOrEmpty() && AddedProductOrderCommentError.IsNullOrEmpty()))
                    {
                        await Shell.Current.DisplayAlert("Error", "You entered incorrect data", "Ok");
                        IsBusy = false;
                        return;
                    }
                }

                AddedProductOrderAmountError = "";
                AddedProductOrderSelectedProductError = "";
                AddedProductOrderCommentError = "";

                var amount = int.Parse(AddedProductOrderAmount);

                ProductOrdersAdded.Add(new ProductOrder
                {
                    ProductId = AddedProductOrderSelectedProduct.Id,
                    Amount = amount,
                    Comment = AddedProductOrderComment,
                    OrderId = -1,
                    Product = AddedProductOrderSelectedProduct,
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteProductOrderCompleted(ProductOrder productOrder)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ProductOrderRepository.DeleteAsync(productOrder.Id);

                if (response.Success)
                {
                    ProductOrdersCompleted.Remove(productOrder);

                    IsBusy = false ;
                    return;
                }

                await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                {
                    await AppConstant.LogOut();

                    IsBusy = false;
                    return;
                }

                IsBusy = false;
                await GetProductsAndProductOrders();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        void DeleteProductOrderAdded(ProductOrder productOrder)
        {
            if (IsBusy) return;

            IsBusy = true;

            ProductOrdersAdded.Remove(productOrder);

            IsBusy = false;
        }


        [RelayCommand]
        async Task SaveOrder()
        {
            if(IsBusy) return;

            try
            {
                IsBusy = true;

                OrderDate = new DateTime(OrderDate.Year, OrderDate.Month, OrderDate.Day, OrderTime.Hours, OrderTime.Minutes, OrderTime.Seconds);

                ValidateAllProperties();

                if (HasErrors)
                {
                    OrderAddressError = string.Join(Environment.NewLine, GetErrors(nameof(OrderAddress)).Select(e => e.ErrorMessage));
                    OrderDateError = string.Join(Environment.NewLine, GetErrors(nameof(OrderDate)).Select(e => e.ErrorMessage));
                    DeadPersonDateOfDeathError = string.Join(Environment.NewLine, GetErrors(nameof(DeadPersonDateOfDeath)).Select(e => e.ErrorMessage));
                    DeadPersonDetailsError = string.Join(Environment.NewLine, GetErrors(nameof(DeadPersonDetails)).Select(e => e.ErrorMessage));
                    DeadPersonFirstNameError = string.Join(Environment.NewLine, GetErrors(nameof(DeadPersonFirstName)).Select(e => e.ErrorMessage));
                    DeadPersonLastNameError = string.Join(Environment.NewLine, GetErrors(nameof(DeadPersonLastName)).Select(e => e.ErrorMessage));

                    if (!(OrderAddressError.IsNullOrEmpty() && OrderDateError.IsNullOrEmpty() && DeadPersonDateOfDeathError.IsNullOrEmpty() &&
                        DeadPersonDetailsError.IsNullOrEmpty() && DeadPersonFirstNameError.IsNullOrEmpty() && DeadPersonLastNameError.IsNullOrEmpty()))
                    {
                        await Shell.Current.DisplayAlert("Error", "You entered incorrect data", "Ok");
                        IsBusy = false;
                        return;
                    }
                }

                OrderAddressError = "";
                OrderDateError = "";
                DeadPersonDateOfDeathError = "";
                DeadPersonDetailsError = "";
                DeadPersonFirstNameError = "";
                DeadPersonLastNameError = "";

                ApiResponse<Order?> response;

                if (Order is null)
                {
                    response = await _unitOfWork.OrderRepository.AddAsync(new OrderDTO
                    {
                        ClientId = Client.Id,
                        Date = OrderDate,
                        Adress = OrderAddress,
                        CeremonyId = SelectedCeremony?.Id
                    });
                }
                else
                {
                    response = await _unitOfWork.OrderRepository.UpdateAsync(Order.Id, new OrderDTO
                    {
                        ClientId = Client.Id,
                        Date = OrderDate,
                        Adress = OrderAddress,
                        CeremonyId = SelectedCeremony?.Id
                    });
                }

                if (!response.Success)
                {
                    await Shell.Current.DisplayAlert("Error", response.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, response.Errors), "Ok");

                    if (response.StatusCode == 401 || response.StatusCode == 403 || response.StatusCode == 0)
                    {
                        await AppConstant.LogOut();
                    }

                    if (response.StatusCode == 404 || response.StatusCode == 400)
                    {
                        await Shell.Current.GoToAsync("..");
                    }

                    IsBusy = false;
                    return;
                }

                OrderId = response.Data.Id;

                ApiResponse<Client?> clientResponse;

                clientResponse = await _unitOfWork.ClientRepository.UpdateAsync(Client.Id, new ClientDTO
                {
                    Address = Client.Address,
                    AppUserId = Client.AppUserId,
                    Email = Client.Email,
                    FirstName = Client.FirstName,
                    LastName = Client.LastName,
                    PhoneNumber = Client.PhoneNumber,
                    IsProcessed = IsProcessed,
                });

                if (!clientResponse.Success)
                {
                    await Shell.Current.DisplayAlert("Error", clientResponse.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, clientResponse.Errors), "Ok");

                    if (clientResponse.StatusCode == 401 || clientResponse.StatusCode == 403 || clientResponse.StatusCode == 0)
                    {
                        await AppConstant.LogOut();
                    }

                    if (response.StatusCode == 404 || response.StatusCode == 400)
                    {
                        await Shell.Current.GoToAsync("..");
                    }

                    IsBusy = false;
                    return;
                }

                ApiResponse<DeadPerson?> responseDeadPerson;

                if (DeadPerson is null)
                {
                    responseDeadPerson = await _unitOfWork.DeadPersonRepository.AddAsync(new DeadPersonDTO
                    {
                        Details = DeadPersonDetails,
                        DateOfBirth = DeadPersonDateOfBirth,
                        DateOfDeath = DeadPersonDateOfDeath,
                        FirstName = DeadPersonFirstName,
                        LastName = DeadPersonLastName,
                        OrderId = OrderId.Value,
                    });
                }
                else
                {
                    responseDeadPerson = await _unitOfWork.DeadPersonRepository.UpdateAsync(DeadPerson.Id, new DeadPersonDTO
                    {
                        Details = DeadPersonDetails,
                        DateOfBirth = DeadPersonDateOfBirth,
                        DateOfDeath = DeadPersonDateOfDeath,
                        FirstName = DeadPersonFirstName,
                        LastName = DeadPersonLastName,
                        OrderId = OrderId.Value,
                    });
                }

                if (!responseDeadPerson.Success)
                {
                    await Shell.Current.DisplayAlert("Error", responseDeadPerson.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, responseDeadPerson.Errors), "Ok");

                    if (responseDeadPerson.StatusCode == 401 || responseDeadPerson.StatusCode == 403 || responseDeadPerson.StatusCode == 0)
                    {
                        await AppConstant.LogOut();
                    }

                    if (response.StatusCode == 404 || response.StatusCode == 400)
                    {
                        await Shell.Current.GoToAsync("..");
                    }

                    IsBusy = false;
                    return;
                }

                List<ProductOrder> productOrdersRemovable = new();
                bool IsProductOrders = true;
                foreach (var productOrder in ProductOrdersAdded)
                {
                    ApiResponse<ProductOrder?> responseProductOrder;

                    responseProductOrder = await _unitOfWork.ProductOrderRepository.AddAsync(new ProductOrderDTO
                    {
                        Amount = productOrder.Amount,
                        Comment = productOrder.Comment,
                        OrderId = OrderId.Value,
                        ProductId = productOrder.ProductId,
                    });

                    if (!responseProductOrder.Success)
                    {
                        await Shell.Current.DisplayAlert("Error", responseProductOrder.ErrorMessage + Environment.NewLine + string.Join(Environment.NewLine, responseProductOrder.Errors), "Ok");

                        if (responseProductOrder.StatusCode == 401 || responseProductOrder.StatusCode == 403 || responseProductOrder.StatusCode == 0)
                        {
                            await AppConstant.LogOut();

                            IsBusy = false;
                            return;
                        }

                        if (response.StatusCode == 400 && response.ErrorMessage == "Invalid order id.")
                        {
                            await Shell.Current.GoToAsync("..");

                            IsBusy = false;
                            return;
                        }

                        IsProductOrders = false;
                        break;
                    }

                    productOrdersRemovable.Add(productOrder);
                }

                foreach (var product in productOrdersRemovable)
                {
                    ProductOrdersAdded.Remove(product);
                }

                IsBusy = false;
                await GetProductsAndProductOrders();

                if (!IsProductOrders)
                {
                    IsBusy = false;
                    return;
                }

                await Shell.Current.DisplayAlert("Success", "Order is saved successfully.", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
