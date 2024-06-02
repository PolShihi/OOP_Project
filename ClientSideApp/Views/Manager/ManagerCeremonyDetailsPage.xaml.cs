using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerCeremonyDetailsPage : ContentPage
{
	private readonly ManagerCeremonyDetailsViewModel _viewModel;

	public ManagerCeremonyDetailsPage(ManagerCeremonyDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}