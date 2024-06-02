using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerClientDetailsPage : ContentPage
{
	private readonly ManagerClientDetailsViewModel _viewModel;

	public ManagerClientDetailsPage(ManagerClientDetailsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetUsers();
    }
}