using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace Behaviors;


public class ScrollToBottomBehavior
{
    public static readonly BindableProperty NotifierProperty =
        BindableProperty.CreateAttached("Notifier",
                                        typeof(object),
                                        typeof(ScrollToBottomBehavior),
                                        null,
                                        BindingMode.TwoWay,
                                        propertyChanged: OnNotifierChanged);

    public static int GetNotifier(BindableObject view) => (int)view.GetValue(NotifierProperty);

    public static void SetNotifier(BindableObject view, int value) => view.SetValue(NotifierProperty, value);

    private static void OnNotifierChanged(BindableObject sender, object oldValue, object newValue)
    {
        if (sender is not CollectionView view) return;
        if (view.ItemsSource is not IEnumerable<object> collection) return;
        view?.ScrollTo(collection.LastOrDefault(), null, ScrollToPosition.End);
        //view?.ScrollTo(lastScroll, position: ScrollToPosition.End);
        //Debug.WriteLine($"scroll {lastScroll}");
    }
}
