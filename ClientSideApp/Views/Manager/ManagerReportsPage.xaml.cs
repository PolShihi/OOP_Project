using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerReportsPage : ContentPage
{
	private readonly ManagerReportsViewModel _viewModel;

	public ManagerReportsPage(ManagerReportsViewModel viewModel)
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