using ClientSideApp.ViewModels;
using ClientSideApp.Views;
using ClientSideApp.Views.Admin;
using ClientSideApp.Views.Manager;
using ClientSideApp.Views.Worker;

namespace ClientSideApp
{
    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;
        public AppShell()
        {
            _viewModel = new AppShellViewModel();
            InitializeComponent();
            BindingContext = _viewModel;
            Routing.RegisterRoute(nameof(AdminUserDetailsPage), typeof(AdminUserDetailsPage));
            Routing.RegisterRoute(nameof(ManagerCeremonyDetailsPage), typeof(ManagerCeremonyDetailsPage));
            Routing.RegisterRoute(nameof(ManagerProductDetailsPage), typeof(ManagerProductDetailsPage));
            Routing.RegisterRoute(nameof(ManagerClientDetailsPage), typeof(ManagerClientDetailsPage));
            Routing.RegisterRoute(nameof(WorkerClientDetailsPage), typeof(WorkerClientDetailsPage));
            Routing.RegisterRoute(nameof(ManagerReportDetailsPage), typeof(ManagerReportDetailsPage));
            Routing.RegisterRoute(nameof(WorkerReportDetailsPage), typeof(WorkerReportDetailsPage));
            Routing.RegisterRoute(nameof(EmailSendPage), typeof(EmailSendPage));
        }
    }
}
