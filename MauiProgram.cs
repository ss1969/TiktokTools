using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace TiktokTools;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement()
            .UseMauiCommunityToolkit()
            //.ConfigureFonts(fonts =>
            //{
            //    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            //    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            //})
            .Services.ConfigureServices();
#if WINDOWS
        Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping("FontFamily", (handler, element) =>
        {
            if (element.Font.Family == "Open Sans")
            {
                const string FontAwesomeFamily = "ms-appx:///OpenSans-Regular.ttf";
                handler.PlatformView.FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(FontAwesomeFamily);
            }
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<MainPage, MainPageVM>();
    }
}
