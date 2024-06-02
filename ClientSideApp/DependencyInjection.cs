using ClientSideApp.Services;
using ClientSideApp.ViewModels;
using ClientSideApp.Views;
using ClientSideApp.Views.Admin;
using ClientSideApp.Views.Manager;
using ClientSideApp.Views.Worker;
using CommunityToolkit.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterPages(this IServiceCollection services)
        {
            services.AddTransient<SettingsPage>()
                .AddTransient<LoginPage>()
                .AddTransient<AdminUsersPage>()
                .AddTransient<AdminUserDetailsPage>()
                .AddTransient<ManagerCeremoniesPage>()
                .AddTransient<ManagerCeremonyDetailsPage>()
                .AddTransient<ManagerProductsPage>()
                .AddTransient<ManagerProductDetailsPage>()
                .AddTransient<ManagerClientsPage>()
                .AddTransient<ManagerClientDetailsPage>()
                .AddTransient<WorkerOrdersPage>()
                .AddTransient<WorkerClientDetailsPage>()
                .AddTransient<ManagerReportsPage>()
                .AddTransient<ManagerReportDetailsPage>()
                .AddTransient<WorkerReportsPage>()
                .AddTransient<WorkerReportDetailsPage>()
                .AddTransient<EmailSendPage>();

            return services;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddTransient<SettingsViewModel>()
                .AddTransient<LoginViewModel>()
                .AddTransient<AdminUsersViewModel>()
                .AddTransient<AdminUserDetailsViewModel>()
                .AddTransient<ManagerCeremoniesViewModel>()
                .AddTransient<ManagerCeremonyDetailsViewModel>()
                .AddTransient<ManagerProductsViewModel>()
                .AddTransient<ManagerProductDetailsViewModel>()
                .AddTransient<ManagerClientsViewModel>()
                .AddTransient<ManagerClientDetailsViewModel>()
                .AddTransient<WorkerOrdersViewModel>()
                .AddTransient<WorkerClientDetailsViewModel>()
                .AddTransient<ManagerReportsViewModel>()
                .AddTransient<ManagerReportDetailsViewModel>()
                .AddTransient<WorkerReportsViewModel>()
                .AddTransient<WorkerReportDetailsViewModel>()
                .AddTransient<EmailSendViewModel>();

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
