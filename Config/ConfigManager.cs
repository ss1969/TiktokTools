using System.Reflection;
using System.Text.Json;
using UserControl;

namespace TiktokTools.Config;

public class ConfigManager
{
    #region 配置项目
    //public string ConfigFilePath { get; set; } = Path.Combine(FileSystem.Current.AppDataDirectory, "appsettings.json");
    public string ConfigFilePath { get; set; } = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    public string ServerAddress { get; set; } = "ws://127.0.0.1:8888";
    public List<string> BarrageFilter { get; set; } = new() { "直播间统计", "进直播间", "orange" };
    public int VideoWidth { get; set; } = 400;
    public int VideoHeight { get; set; } = 300;
    public bool VideoMute { get; set; } = false;
    public bool VideoLoop { get; set; } = true;
    public List<DataClass> VideoFiles{get; set; } = new();
    #endregion

    public ConfigManager()
    {
    }

    public void LoadSettings<T>(string filePath = null) where T : class
    {
        // 无文件就创建默认
        filePath ??= ConfigFilePath;
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"配置文件 '{filePath}' 不存在。");
            this.SaveSettings(this.ConfigFilePath);
        }

        // 读取，解析
        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        try
        {
            var result = JsonSerializer.Deserialize<T>(json, options);
            Console.WriteLine($"成功从文件 '{filePath}' 中读取设置。");
            // 通过反射将配置文件的结果赋值给属性
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                object propValue = prop.GetValue(result);
                prop.SetValue(this, propValue);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"无法从文件 '{filePath}' 中读取设置：{ex.Message}");
        }
    }

    public void SaveSettings(string filePath = null)
    {
        filePath ??= ConfigFilePath;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(filePath, json);
        Console.WriteLine($"设置已保存到文件: {filePath}");
    }

}
