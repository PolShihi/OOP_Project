

using ClientSideApp.Controls;
using ClientSideApp.Views;
using ClientSideApp.Views.Admin;
using ClientSideApp.Views.Manager;
using ClientSideApp.Views.Worker;

namespace ClientSideApp.Models
{
    public class AppConstant
    {
        public async static Task LogOut()
        {
            App.UserSession = null;

            await Shell.Current.GoToAsync("//StartUp/LoginPage");

            var flyoutItems = Shell.Current.Items.OfType<FlyoutItem>().ToList();

            foreach (var item in flyoutItems)
            {
                if (item.Route != "StartUp")
                {
                    Shell.Current.Items.Remove(item);
                }
            }
        }

        public async static Task AddFlyoutMenusDetails()
        {
            Shell.Current.FlyoutHeader = new FlyoutHeaderControl();

            if (App.UserSession.Role == "Admin")
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Users",
                    Route = nameof(AdminUsersPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(AdminUsersPage)),
                                },
                            }
                };

                Shell.Current.Items.Add(flyoutItem);

                await Shell.Current.GoToAsync($"//{nameof(AdminUsersPage)}");

            }

            if (App.UserSession.Role == "Manager")
            {
                var flyoutItemCeremonies = new FlyoutItem()
                {
                    Title = "Ceremonies",
                    Route = nameof(ManagerCeremoniesPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(ManagerCeremoniesPage)),
                                },
                            }
                };

                var flyoutItemProducts = new FlyoutItem()
                {
                    Title = "Products",
                    Route = nameof(ManagerProductsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(ManagerProductsPage)),
                                },
                            }
                };

                var flyoutItemClients = new FlyoutItem()
                {
                    Title = "Clients",
                    Route = nameof(ManagerClientsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(ManagerClientsPage)),
                                },
                            }
                };

                var flyoutItemReports = new FlyoutItem()
                {
                    Title = "Reports",
                    Route = nameof(ManagerReportsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(ManagerReportsPage)),
                                },
                            }
                };

                Shell.Current.Items.Add(flyoutItemCeremonies);
                Shell.Current.Items.Add(flyoutItemProducts);
                Shell.Current.Items.Add(flyoutItemClients);
                Shell.Current.Items.Add(flyoutItemReports);

                await Shell.Current.GoToAsync($"//{nameof(ManagerCeremoniesPage)}");
            }

            if (App.UserSession.Role == "Worker")
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Clients",
                    Route = nameof(WorkerOrdersPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(WorkerOrdersPage)),
                                },
                            }
                };

                var flyoutItemReports = new FlyoutItem()
                {
                    Title = "Reports",
                    Route = nameof(WorkerReportsPage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsSingleItem,
                    Items ={
                                new ShellContent
                                {
                                    ContentTemplate = new DataTemplate(typeof(WorkerReportsPage)),
                                },
                            }
                };

                Shell.Current.Items.Add(flyoutItem);
                Shell.Current.Items.Add(flyoutItemReports);

                await Shell.Current.GoToAsync($"//{nameof(WorkerOrdersPage)}");
            }
        }
    }
}
