using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Worker;

public partial class WorkerClientDetailsPage : ContentPage
{
	private readonly WorkerClientDetailsViewModel _viewModel;

	public WorkerClientDetailsPage(WorkerClientDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}