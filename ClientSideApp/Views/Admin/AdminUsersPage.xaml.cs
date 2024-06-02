using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Admin;

public partial class AdminUsersPage : ContentPage
{
	private readonly AdminUsersViewModel _viewModel;
	public AdminUsersPage(AdminUsersViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		await _viewModel.GetUsers();
    }
}