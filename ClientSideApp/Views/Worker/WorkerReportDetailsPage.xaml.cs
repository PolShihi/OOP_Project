using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Worker;

public partial class WorkerReportDetailsPage : ContentPage
{
	private readonly WorkerReportDetailsViewModel _viewModel;

	public WorkerReportDetailsPage(WorkerReportDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}