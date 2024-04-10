using ClientSideApp.ViewModels;

namespace ClientSideApp.Views;


public partial class AdminUsersPage : ContentPage
{
	public AdminUsersPage(AdminUsersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		var viewModel = BindingContext as AdminUsersViewModel;
		viewModel.GetUsers();
    }
}