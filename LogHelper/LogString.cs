namespace LogHelper;

public class LogString
{
    public string Message { get; set; }
    public int Level { get; set; }

    public LogString(string message, int level = 0)
    {
        this.Message = message;
        this.Level = level;
    }
}
