using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Worker;

public partial class WorkerOrdersPage : ContentPage
{
	private readonly WorkerOrdersViewModel _viewModel;

	public WorkerOrdersPage(WorkerOrdersViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetClients();
    }
}