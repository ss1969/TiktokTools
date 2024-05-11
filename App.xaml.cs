namespace TiktokTools;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.HandlerChanged += Window_HandlerChanged;
        return window;
    }

    private void Window_HandlerChanged(object sender, EventArgs e)
    {
        Helpers.WindowHelper.WindowSet(sender as Window, 1150, 1000, 0);
    }

}
