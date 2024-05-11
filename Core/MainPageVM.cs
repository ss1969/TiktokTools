using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helpers;
using TiktokTools.Config;
using LogHelper;
using System.Diagnostics;
using UserControl;

namespace TiktokTools;

public partial class MainPageVM : ObservableObject
{
    #region Properties
    [ObservableProperty] private int videoWidth = 400;
    [ObservableProperty] private int videoHeight = 300;
    [ObservableProperty] private int videoOriginalWidth;
    partial void OnVideoOriginalWidthChanged(int value)
    {
        Log($"宽度：{value}");
    }
    [ObservableProperty] private int videoOriginalHeight;
    partial void OnVideoOriginalHeightChanged(int value)
    {
        Log($"高度：{value}");
    }
    [ObservableProperty] private bool videoMute = true;
    [ObservableProperty] private bool videoLoop = true;
    [ObservableProperty] private string videoSource;
    partial void OnVideoSourceChanged(string value)
    {
        Log($"新播放文件：{value}");
    }
    [ObservableProperty] private ObservableCollection<LogString> logData;
    [ObservableProperty] private int logDataChange;

    [ObservableProperty] private string barrageServer;
    [ObservableProperty] private bool onlyBarrage = true;
    [ObservableProperty] private bool autoScroll = true;

    [ObservableProperty] private ObservableCollection<DataClass> mediaData;
    [ObservableProperty] private string netAddressEntry = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    #endregion

    #region Variables
    private ConfigManager config;
    #endregion

    #region Commands
    [RelayCommand]
    private void RestoreVideoSize()
    {
        VideoWidth = VideoOriginalWidth;
        VideoHeight = VideoOriginalHeight;
    }

    [RelayCommand]
    private async void OpenLocalFile()
    {
        var fileInfo = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "打开本地视频",
            FileTypes = FilePickerFileType.Videos
        });

        if (fileInfo != null && fileInfo.FullPath != null)
        {
            VideoSource = fileInfo.FullPath;
        }
    }

    [RelayCommand]
    private void OpenNetAddress()
    {
        VideoSource = NetAddressEntry;
    }

    [RelayCommand]
    private void ClearLog()
    {
        LogData.Clear();
    }

    [RelayCommand]
    private void OpenConfigFile()
    {
        try
        {
#if WINDOWS10_0_17763_0_OR_GREATER
            System.Diagnostics.Process.Start("explorer.exe", config.ConfigFilePath);
#endif
        }
        catch (Exception)
        {
        }
    }

    [RelayCommand]
    private void SaveConfigFile()
    {
        config.VideoWidth = VideoWidth;
        config.VideoHeight = VideoHeight;
        config.VideoLoop = VideoLoop;
        config.VideoMute = VideoMute;
        config.VideoFiles = MediaData.Where(item => !string.IsNullOrEmpty(item.Entry1) && !string.IsNullOrEmpty(item.Entry2)).ToList();
        config.SaveSettings();

        foreach(var s in MediaData)
        {
            Debug.WriteLine($"{s.Serial} - {s.Entry1} - {s.Entry2}");
        }
    }

    #endregion

    #region Constructor
    public MainPageVM()
    {
        LogData = new ObservableCollection<LogString>();
        config = new ConfigManager();
        config.LoadSettings<ConfigManager>();
        VideoWidth = config.VideoWidth;
        VideoHeight = config.VideoHeight;
        VideoLoop = config.VideoLoop;
        VideoMute = config.VideoMute;
        MediaData = new ObservableCollection<DataClass>(config.VideoFiles);
        BarrageServer = config.ServerAddress;
        WebSocketClient.Instance.SetSeverAddress(uri: new Uri(config.ServerAddress));
        WebSocketClient.Instance.AddStatusHandler(Dp_StatusReceived);
        WebSocketClient.Instance.AddDataHandler(Dp_DataReceived);
        WebSocketClient.Instance.Start();
    }
    #endregion

    #region Data Received
    private void Log(string message, int level = 0)
    {
        lock (LogData)
        {
            if (AutoScroll) LogDataChange++;
            LogData.Add(new LogString(message, level));
            while (LogData.Count > 100)
                LogData.RemoveAt(0);
        }
    }

    public void Dp_DataReceived(object sender, string e)
    {
        // 把 \" 改成仅一个引号
        e = e.Replace("\\\"", "\"");
        // 把原有带引号的内容做清理
        e = e.Replace("\\\\\"", "\\\"");
        // 移除Data后的引号
        e = e.Replace("\"Data\":\"", "\"Data\":");
        // 移除最后一个}之前的引号
        var stringInfo = new StringInfo(e);
        e = stringInfo.SubstringByTextElements(0, stringInfo.LengthInTextElements - 2);
        e += "}";
        try
        {
            var m = Message.FromJson(e);
            if (OnlyBarrage && m.Type != 1) return;
            switch (m.Type)
            {
                case 1:
                    Log($"[弹幕] {m.Data.User.Nickname} : {m.Data.Content}", m.Type);
                    BarrageToVideo(m.Data.Content);
                    break;
                case 2:
                    Log($"[点赞] {m.Data.Content}", m.Type);
                    break;
                case 3:
                    Log($"[来人] {m.Data.Content}", m.Type);
                    break;
                case 4:
                    Log($"[关注] {m.Data.Content}", m.Type);
                    break;
                case 5:
                    Log($"[送礼] {m.Data.Content}", m.Type);
                    break;
                case 6:
                    Log($"[统计] {m.Data.Content}", m.Type);
                    break;
                case 7:
                    Log($"[粉丝] {m.Data.Content}", m.Type);
                    break;
                default:
                    break;
            }
        }
        catch (Exception)
        {
        }
    }

    public void Dp_StatusReceived(object sender, string e)
    {
        Log($"[系统信息] {e}", 0);
    }
    #endregion

    private void BarrageToVideo(string barrage)
    {
        foreach (DataClass data in MediaData)
        {
            if (barrage.Contains(data.Entry1))
            {
                Log($"[关键字] {data.Entry1} => {data.Entry2}", 9);
                if (!File.Exists(data.Entry2))
                {
                    Log("文件不存在");
                    return;
                }
                VideoSource = data.Entry2;
                return;
            }
        }
    }

}