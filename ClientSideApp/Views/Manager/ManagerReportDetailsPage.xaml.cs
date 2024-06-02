using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerReportDetailsPage : ContentPage
{
	private readonly ManagerReportDetailsViewModel _viewModel;

	public ManagerReportDetailsPage(ManagerReportDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}