

using ClientSideApp.Controls;
using ClientSideApp.Views;

namespace ClientSideApp.Models
{
    internal class AppConstant
    {
        public async static Task AddFlyoutMenusDetails()
        {
            AppShell.Current.FlyoutHeader = new FlyoutHeaderControl();

            var adminInfo = AppShell.Current.Items.Where(f => f.Route == nameof(AdminUsersPage)).FirstOrDefault();
            if (adminInfo != null) AppShell.Current.Items.Remove(adminInfo);

            var managerInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ManagerClientsPage)).FirstOrDefault();
            if (managerInfo != null) AppShell.Current.Items.Remove(managerInfo);

            var workerInfo = AppShell.Current.Items.Where(f => f.Route == nameof(WorkerClientsPage)).FirstOrDefault();
            if (workerInfo != null) AppShell.Current.Items.Remove(workerInfo);


            if (App.UserDetails.Role == "Admin")
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Admin Page",
                    Route = nameof(AdminUsersPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                    Items ={
                                new ShellContent
                                {
                                    Title = "Admin Users Page",
                                    ContentTemplate = new DataTemplate(typeof(AdminUsersPage)),
                                },
                            }
                };
                if (!AppShell.Current.Items.Contains(flyoutItem))
                {
                    AppShell.Current.Items.Add(flyoutItem);
                    await Shell.Current.GoToAsync($"//{nameof(AdminUsersPage)}");
                }

            }

            if (App.UserDetails.Role == "Manager")
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Manager Page",
                    Route = nameof(ManagerClientsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                    Items ={
                                new ShellContent
                                {
                                    Title = "Manager Clients",
                                    ContentTemplate = new DataTemplate(typeof(ManagerClientsPage)),
                                },
                            }
                };
                if (!AppShell.Current.Items.Contains(flyoutItem))
                {
                    AppShell.Current.Items.Add(flyoutItem);
                    await Shell.Current.GoToAsync($"//{nameof(ManagerClientsPage)}");
                }

            }

            if (App.UserDetails.Role == "Worker")
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Worker Page",
                    Route = nameof(WorkerClientsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                    Items = {
                                new ShellContent
                                {
                                    Title = "Worker Clients",
                                    ContentTemplate = new DataTemplate(typeof(WorkerClientsPage)),
                                },
                            }
                };
                if (!AppShell.Current.Items.Contains(flyoutItem))
                {
                    AppShell.Current.Items.Add(flyoutItem);
                    await Shell.Current.GoToAsync($"//{nameof(WorkerClientsPage)}");
                }

            }
        }
    }
}
