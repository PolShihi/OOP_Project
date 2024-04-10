using ClientSideApp.ViewModels;

namespace ClientSideApp.Views;

public partial class AdminUserRegistrationPage : ContentPage
{
	public AdminUserRegistrationPage(AdminUserRegistrationViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}