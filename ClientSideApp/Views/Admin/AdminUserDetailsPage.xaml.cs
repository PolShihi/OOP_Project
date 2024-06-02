using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Admin;

public partial class AdminUserDetailsPage : ContentPage
{
	private readonly AdminUserDetailsViewModel _viewModel;

	public AdminUserDetailsPage(AdminUserDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}