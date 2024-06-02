using ClientSideApp.ViewModels;

namespace ClientSideApp.Views;

public partial class LoginPage : ContentPage
{
	private readonly LoginViewModel _viewModel;
	public LoginPage(LoginViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = viewModel;
	}
}