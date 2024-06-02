using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerClientsPage : ContentPage
{
	private readonly ManagerClientsViewModel _viewModel;

	public ManagerClientsPage(ManagerClientsViewModel viewModel)
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