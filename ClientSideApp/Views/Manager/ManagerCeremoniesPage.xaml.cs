using ClientSideApp.ViewModels;

namespace ClientSideApp.Views.Manager;

public partial class ManagerCeremoniesPage : ContentPage
{
	private readonly ManagerCeremoniesViewModel _viewModel;

	public ManagerCeremoniesPage(ManagerCeremoniesViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetCeremonies();
    }
}