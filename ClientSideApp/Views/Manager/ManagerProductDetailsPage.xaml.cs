using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerProductDetailsPage : ContentPage
{
	private readonly ManagerProductDetailsViewModel _viewModel;

	public ManagerProductDetailsPage(ManagerProductDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}