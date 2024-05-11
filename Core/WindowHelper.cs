#if WINDOWS
using PInvoke;
using static PInvoke.User32;
//using Extensions;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Reflection;
using Windows.System;
using MicrosoftuiWindowing = Microsoft.UI.Windowing;
using MicrosoftuiXaml = Microsoft.UI.Xaml;
using MicrosoftuixamlData = Microsoft.UI.Xaml.Data;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace Helpers;

public static class WindowHelper
{

    // 以前这一部分是放到App.xaml.cs里面，把WindowSet作为Handler使用
    // 现在放在这里，因为是静态类，所以无法使用
    // _IsBinding 这部分正好是不太需要（不会点返回按钮），所以屏蔽掉也没问题
        // MAUI大佬 2023 / 1 / 9 
        // 因为maui不支持无标题，所以他每次刷新都会把标题的高度搞回去
        // 你可以试一下如果用了标题栏的返回按钮那么你点了就会有一个30高度的标题栏
        // 用返回按钮的方法是用异步导航
        // async void Navigate(Type pageType)
        // {
        //    Page page = (Page)Activator.CreateInstance(pageType);
        //    await Navigation.PushAsync(page);
        // }
        //  所以当时用了个这个很二的方法，只要maui一改这个值我就把他改成0

#if false
#if WINDOWS
    private MicrosoftuiXaml.Thickness _NavigationViewContentMargin;
    public MicrosoftuiXaml.Thickness NavigationViewContentMargin
    {
        get => _NavigationViewContentMargin;
        set
        {
            _NavigationViewContentMargin = value;

            var page = Windows.FirstOrDefault()?.Page;
            if (page is null)
                return;
            var mauiContext = page.RequireMauiContext();
            if (mauiContext is null)
                return;

            try
            {
                var windowRootView = mauiContext.GetNavigationRootManager()?.RootView as WindowRootView;
                var navigationView = windowRootView?.NavigationViewControl;
                if (navigationView is null)
                    return;

                var thicknessProperty = typeof(RootNavigationView).GetProperty("NavigationViewContentMargin", BindingFlags.Instance | BindingFlags.NonPublic);
                if (thicknessProperty?.GetValue(navigationView) is MicrosoftuiXaml.Thickness thickness)
                {
                    if (thickness == new MicrosoftuiXaml.Thickness(0))
                        return;

                    thicknessProperty.SetValue(navigationView, new MicrosoftuiXaml.Thickness(0));
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }

    bool _IsBinding = false;

    bool SetBindingConfig()
    {
        var page = Windows.FirstOrDefault()?.Page;
        if (page is null)
            return false;

        var mauiContext = page.RequireMauiContext();
        if (mauiContext is null)
            return false;

        var windowRootView = mauiContext.GetNavigationRootManager()?.RootView as WindowRootView;
        var navigationView = windowRootView?.NavigationViewControl;
        if (navigationView is null)
            return false;

        var contentProperty = typeof(RootNavigationView).GetProperty("ContentGrid", BindingFlags.Instance | BindingFlags.NonPublic);
        if (contentProperty is null)
            return false;

        var contentGrid = contentProperty.GetValue(navigationView);
        if (contentGrid is not MicrosoftuiXaml.FrameworkElement frameworkELement)
            return false;

        MicrosoftuixamlData.Binding marginBinding = new();
        marginBinding.Source = this;
        marginBinding.Path = new MicrosoftuiXaml.PropertyPath("NavigationViewContentMargin");
        marginBinding.Mode = MicrosoftuixamlData.BindingMode.TwoWay;
        marginBinding.UpdateSourceTrigger = MicrosoftuixamlData.UpdateSourceTrigger.PropertyChanged;
        MicrosoftuixamlData.BindingOperations.SetBinding(frameworkELement, MicrosoftuiXaml.FrameworkElement.MarginProperty, marginBinding);

        return true;
    }
#endif
#endif

    public static void WindowSet(Window window, int W, int H, int XPos = 0, int YPos = 0)
    {
#if WINDOWS
        // 去除标题栏按钮等，实现无边框窗口
        var winuiWindow = window.Handler?.PlatformView as MicrosoftuiXaml.Window;
        if (winuiWindow is null)
            return;

        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        //winuiWindow.ExtendsContentIntoTitleBar = false;
        //var customOverlappedPresenter = MicrosoftuiWindowing.OverlappedPresenter.CreateForContextMenu();
        //appWindow.SetPresenter(customOverlappedPresenter);

        //if (!_IsBinding)
        //    SetBindingConfig();

        //NavigationViewContentMargin = new MicrosoftuiXaml.Thickness(0);

        // 最大化
        //User32.PostMessage(winuiWindow.GetWindowHandle(), WindowMessage.WM_SYSCOMMAND, new IntPtr((int)SysCommands.SC_MAXIMIZE), IntPtr.Zero);

        // 自定义大小
        var displyArea = MicrosoftuiWindowing.DisplayArea.Primary;
        appWindow.MoveAndResize(new (XPos, YPos, W, H), displyArea);
#endif
    }

}
