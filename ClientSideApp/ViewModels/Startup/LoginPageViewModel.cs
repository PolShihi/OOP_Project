using ClientSideApp.Models;
using ClientSideApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClientSideApp.ViewModels.Startup
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;


        private readonly IUserService _loginService;
        public LoginPageViewModel(IUserService loginService)
        {
            _loginService = loginService;
        }

        [RelayCommand]
        async Task Login()
        {
            if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                // calling api 
                var response = await _loginService.Authenticate(new LoginRequest
                {
                    Email = Email,
                    Password = Password
                });

                if(response != null)
                {
                    await SecureStorage.SetAsync("Token", response.Token);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(response.Token);
                    var userInfo = new UserInfo
                    {
                        Id = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                        FirstName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                        LastName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname).Value,
                        PhoneNumber = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value,
                        Email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                        Role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value,
                    };

                    string userDetailStr = JsonConvert.SerializeObject(userInfo);
                    Preferences.Set(nameof(App.UserDetails), userDetailStr);
                    App.UserDetails = userInfo;
                    await AppConstant.AddFlyoutMenusDetails();
                 
                }
                else
                {
                    await AppShell.Current.DisplayAlert("Invalid User Name Or Password", "Invalid UserName or Password", "OK");
                }


              
            }


        }    
    }
}
