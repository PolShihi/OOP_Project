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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    [QueryProperty(nameof(Ceremony), nameof(Ceremony))]
    public partial class ManagerCeremonyDetailsViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private Ceremony? _ceremony;

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
        private string _nameError = "";

        [ObservableProperty]
        private string _descriptionError = "";

        [ObservableProperty]
        private string _priceError = "";

        partial void OnCeremonyChanged(Ceremony? value)
        {
            if (value is null) return;

            Name = value.Name;
            Description = value.Description;
            Price = value.Price.ToString();
        }

        public ManagerCeremonyDetailsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RelayCommand]
        async Task SaveCeremony()
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

                    IsBusy = false;
                    return;
                }

                NameError = "";
                PriceError = "";
                DescriptionError = "";

                ApiResponse<Ceremony?> response;

                var price = decimal.Parse(Price);

                if (Ceremony is null)
                {
                    response = await _unitOfWork.CeremonyRepository.AddAsync(new CeremonyDTO
                    {
                        Name = Name,
                        Description = Description,
                        Price = price,
                    });
                }
                else
                {
                    response = await _unitOfWork.CeremonyRepository.UpdateAsync(Ceremony.Id, new CeremonyDTO
                    {
                        Name = Name,
                        Description = Description,
                        Price = price,
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
