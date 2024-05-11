using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControl;

internal class CollectionViewExtended : CollectionView
{
    public static readonly BindableProperty ScrollToButtomProperty =
        BindableProperty.Create(nameof(ScrollToButtom), typeof(bool), typeof(CollectionViewExtended), false);

    public bool ScrollToButtom
    {
        get => (bool)GetValue(ScrollToButtomProperty);
        set => SetValue(ScrollToButtomProperty, value);
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);
        if (ScrollToButtom)
        {
            ScrollTo(child, position: ScrollToPosition.End, animate: true);
        }
    }

}
