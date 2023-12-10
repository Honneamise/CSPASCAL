using System.Globalization;

namespace Pascal;

public static class Init
{
    public static bool LogScope { get; set; }
    public static bool LogStack { get; set; }

    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
    {
        if (ex.ExceptionObject is Exception e)
        {
            Console.WriteLine(e.Message.ToString());

            Environment.Exit(-1);
        }
    }

    public static void Initialize(bool logScope, bool logStack)
    {
        LogScope = logScope;
        LogStack = logStack;    

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }
}   

