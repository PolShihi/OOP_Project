using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Worker;

public partial class WorkerReportsPage : ContentPage
{
	private readonly WorkerReportsViewModel _viewModel;

	public WorkerReportsPage(WorkerReportsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetReports();
    }
}