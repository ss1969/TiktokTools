using System.Diagnostics;

namespace TiktokTools;

public partial class MainPage : ContentPage
{

    public MainPage(MainPageVM vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }

    void ContentPage_Unloaded(object sender, EventArgs e)
    {
        // Stop and cleanup MediaElement when we navigate away
        mediaElement.Handler?.DisconnectHandler();
    }

    private void mediaElement_MediaOpened(object sender, EventArgs e)
    {
        Debug.WriteLine($"{e}");
    }

    private void mediaElement_MediaFailed(object sender, CommunityToolkit.Maui.Core.Primitives.MediaFailedEventArgs e)
    {
        Debug.WriteLine($"{e}");

    }

    private void mediaElement_MediaEnded(object sender, EventArgs e)
    {
        Debug.WriteLine($"{e}");

    }
}

