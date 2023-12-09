using System.Globalization;

namespace Pascal;

public static class Init
{
    public static bool LogEnabled { get; set; }

    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
    {
        if (ex.ExceptionObject is Exception e)
        {
            Console.WriteLine(e.Message.ToString());

            Environment.Exit(-1);
        }
    }

    public static void Initialize(bool enableLog)
    {
        LogEnabled = enableLog;

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }
}   

