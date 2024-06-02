using ClientSideApp.ViewModels;

namespace ClientSideApp.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;
	public SettingsPage(SettingsViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}
}