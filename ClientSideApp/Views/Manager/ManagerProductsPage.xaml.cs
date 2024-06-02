using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerProductsPage : ContentPage
{
	private readonly ManagerProductsViewModel _viewModel;

	public ManagerProductsPage(ManagerProductsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetProducts();
    }
}