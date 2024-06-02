namespace ClientSideApp.Controls;

public partial class FlyoutHeaderControl : StackLayout
{
	public FlyoutHeaderControl()
	{
		InitializeComponent();

        if (App.UserSession != null)
        {
			lblUserName.Text = App.UserSession.FullName;
            lblUserEmail.Text = App.UserSession.Email;
            lblUserRole.Text = App.UserSession.Role;
        }
    }
}