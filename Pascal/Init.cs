using System.Globalization;

namespace Pascal;

public static class Init
{
    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
    {
        if (ex.ExceptionObject is Exception e)
        {
            Console.WriteLine("*** ERROR ***");

            Console.WriteLine(e.Message.ToString());

            Environment.Exit(-1);
        }
    }

    public static void Initialize()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }
}   

