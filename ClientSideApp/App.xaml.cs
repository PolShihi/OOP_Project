using ClientSideApp.Models;

namespace ClientSideApp
{
    public partial class App : Application
    {
        public static UserInfo UserDetails = new();
        public static string Host = "localhost";
        public static string Port = "7200";
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
}
}
