using ClientSideApp.Services;
using MyModel.Models.DTOs;

namespace ClientSideApp
{
    public partial class App : Application
    {
        public static UserSession? UserSession { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
}
}
