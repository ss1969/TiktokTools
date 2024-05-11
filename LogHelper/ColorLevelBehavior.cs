namespace LogHelper;

#if true
public class ColorLevelBehavior
{
    public static readonly BindableProperty LevelProperty =
        BindableProperty.CreateAttached("Level", typeof(int), typeof(ColorLevelBehavior), 0, propertyChanged: OnLevelChanged);

    public static int GetLevel(BindableObject view) => (int)view.GetValue(LevelProperty);

    public static void SetLevel(BindableObject view, int value) => view.SetValue(LevelProperty, value);

    private static void OnLevelChanged(BindableObject sender, object oldValue, object newValue)
    {
        if (sender is not Label label) return;
        SetLabelColor(label, (int)newValue);
    }

    /*
     Assert 0
     Debug 1
     Error 2
     Info 3
     Verbose 4
     Warning 5
     */
    private static void SetLabelColor(Label label, int level)
    {
        label.TextColor = level switch
        {
            0 => Colors.White,
            1 => Colors.LightGreen,
            2 => Colors.Yellow,
            3 => Colors.Gold,
            4 => Colors.Orange,
            5 => Colors.LightSalmon,
            6 => Colors.Coral,
            7 => Colors.Tomato,
            8 => Colors.Red,
            9 => Colors.DarkRed,
            _ => Colors.Grey,
        };
    }
}
#endif

#if false
public class ColorLevelBehavior : Behavior<Label>
{
    public static readonly BindableProperty LevelProperty =
            BindableProperty.Create(nameof(Level), typeof(int), typeof(ColorLevelBehavior), null, propertyChanged: OnLevelChanged);
    public int Level
    {
        get => (int)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }
    private static void OnLevelChanged(BindableObject sender, object oldValue, object newValue)
    {
        if (sender is not Label label) return;
        SetLabelColor(label, (int)newValue);
    }

    /*
     Assert 0
     Debug 1
     Error 2
     Info 3
     Verbose 4
     Warning 5
     */
    private static void SetLabelColor(Label label, int level)
    {
        label.TextColor = level switch
        {
            0 => Colors.LightGreen,
            1 => Colors.DarkOliveGreen,
            2 => Colors.Red,
            3 => Colors.White,
            4 => Colors.LightBlue,
            5 => Colors.Yellow,
            _ => Colors.Grey,
        };
    }
}
#endif