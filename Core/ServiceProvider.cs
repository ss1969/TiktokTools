using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

public static class ServiceProvider
{
    public static IServiceProvider Current =>
#if WINDOWS10_0_17763_0_OR_GREATER
            MauiWinUIApplication.Current.Services;
#elif ANDROID
            MauiApplication.Current.Services;
#elif IOS || MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#else
            null;
#endif
    public static T GetService<T>() => Current.GetService<T>();
    public static IEnumerable<T> GetServices<T>() => Current.GetServices<T>();
}
