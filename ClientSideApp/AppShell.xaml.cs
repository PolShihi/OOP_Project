using ClientSideApp.ViewModels;
using ClientSideApp.Views;

namespace ClientSideApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = new AppShellViewModel();
            Routing.RegisterRoute(nameof(AdminUserRegistrationPage), typeof(AdminUserRegistrationPage));
        }
    }
}
