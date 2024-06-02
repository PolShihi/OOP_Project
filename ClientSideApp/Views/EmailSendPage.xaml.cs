using ClientSideApp.ViewModels;

namespace ClientSideApp.Views;

public partial class EmailSendPage : ContentPage
{
    private readonly EmailSendViewModel _viewModel;

    public EmailSendPage(EmailSendViewModel viewModel)
	{
        _viewModel = viewModel;
		InitializeComponent();
        BindingContext = _viewModel;
	}
}