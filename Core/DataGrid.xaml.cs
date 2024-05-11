using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UserControl;

public partial class DataClass : ObservableObject
{
    [ObservableProperty] private int serial;    // 需要在删除的时候重新排序，所以必须有通知功能
    public string Entry1 { get; set; }          // 只有新建时候读取，不需要后期在外部更改
    public string Entry2 { get; set; }          // 只有新建时候读取，不需要后期在外部更改
}

public partial class DataGrid : ContentView
{
    public static readonly BindableProperty DataSourceProperty =
        BindableProperty.Create(nameof(DataSource),
                                typeof(ObservableCollection<DataClass>),
                                typeof(DataGrid),
                                null,
                                BindingMode.TwoWay, 
                                propertyChanging: OnDataSourceChanging);
    public ObservableCollection<DataClass> DataSource
    {
        get => (ObservableCollection<DataClass>)GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }


    [RelayCommand]
    private void Add()
    {
        DataSource.Add(new DataClass { Serial = DataSource.Count + 1 });
    }

    [RelayCommand]
    private void Del(DataClass item)
    {
        DataSource.Remove(item);
        for (int i = 0; i < DataSource.Count; i++)
        {
            DataSource[i].Serial = i + 1;
        }
    }

    private static void OnDataSourceChanging(BindableObject bindable, object oldValue, object newValue)
    {
        var items = newValue as ObservableCollection<DataClass>;
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Serial = i + 1;
        }
    }

    public DataGrid()
    {
        InitializeComponent();
    }

    private async void labelAdd_Tapped(object sender, TappedEventArgs e)
    {
        await labelAdd.ScaleTo(0.9, 100);
        await labelAdd.ScaleTo(1, 100);
    }
}