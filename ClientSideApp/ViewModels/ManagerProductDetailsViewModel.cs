using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Validators;
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
    [QueryProperty(nameof(Product), nameof(Product))]
    public partial class ManagerProductDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private Product? _product;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Name is required.")]
        private string _name = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Description is required.")]
        private string _description = "";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Price is required.")]
        [Decimal]
        private string _price = "0,00";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Amount is required.")]
        [NonNegativeInteger]
        private string _amount = "0";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Reorder level is required.")]
        [NonNegativeInteger]
        private string _reorderLevel = "0";


        [ObservableProperty]
        private string _nameError = "";

        [ObservableProperty]
        private string _descriptionError = "";

        [ObservableProperty]
        private string _priceError = "";

        [ObservableProperty]
        private string _amountError = "";

        [ObservableProperty]
        private string _reorderLevelError = "";

        partial void OnProductChanged(Product? value)
        {
            if (value is null) return;

            Name = value.Name;
            Description = value.Description;
            Price = value.Price.ToString();
            Amount = value.Amount.ToString();
            ReorderLevel = value.ReorderLevel.ToString();
        }

        public ManagerProductDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RelayCommand]
        async Task SaveProduct()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ValidateAllProperties();

                if (HasErrors)
                {
                    NameError = string.Join(Environment.NewLine, GetErrors(nameof(Name)).Select(e => e.ErrorMessage));
                    PriceError = string.Join(Environment.NewLine, GetErrors(nameof(Price)).Select(e => e.ErrorMessage));
                    DescriptionError = string.Join(Environment.NewLine, GetErrors(nameof(Description)).Select(e => e.ErrorMessage));
                    AmountError = string.Join(Environment.NewLine, GetErrors(nameof(Amount)).Select(e => e.ErrorMessage));
                    ReorderLevelError = string.Join(Environment.NewLine, GetErrors(nameof(ReorderLevel)).Select(e => e.ErrorMessage));

                    IsBusy = false;
                    return;
                }

                NameError = "";
                PriceError = "";
                DescriptionError = "";
                AmountError = "";
                ReorderLevelError = "";

                ApiResponse<Product?> response;

                var price = decimal.Parse(Price);
                var amount = int.Parse(Amount);
                var reorderLevel = int.Parse(ReorderLevel);

                if (Product is null)
                {
                    response = await _unitOfWork.ProductRepository.AddAsync(new ProductDTO
                    {
                        Name = Name,
                        Description = Description,
                        Price = price,
                        Amount = amount,
                        ReorderLevel = reorderLevel,
                    });
                }
                else
                {
                    response = await _unitOfWork.ProductRepository.UpdateAsync(Product.Id, new ProductDTO
                    {
                        Name = Name,
                        Description = Description,
                        Price = price,
                        Amount = amount,
                        ReorderLevel = reorderLevel,
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
