using ClientSideApp.Models;
using ClientSideApp.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {

        [RelayCommand]
        async Task LogOut()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await AppConstant.LogOut();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
