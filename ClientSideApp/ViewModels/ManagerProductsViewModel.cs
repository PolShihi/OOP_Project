using ClientSideApp.Models;
using ClientSideApp.Services;
using ClientSideApp.Views.Manager;
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
    public partial class ManagerProductsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<Product> Products { get; } = new();

        public ManagerProductsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        async Task AddProduct()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(ManagerProductDetailsPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task EditProduct(Product product)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var queryParametrs = new Dictionary<string, object>()
                {
                    { nameof(Product), product },
                };

                await Shell.Current.GoToAsync($"{nameof(ManagerProductDetailsPage)}", queryParametrs);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteProduct(Product product)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _unitOfWork.ProductRepository.DeleteAsync(product.Id);

                if (!response.Success)
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
                await GetProducts();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
