using Serilog;
using Serilog.Core;

namespace VoxMod.Main;

public static class VoxLogger
{
    private static readonly Logger Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt").CreateLogger();

    public static void LogInfo(string msg, params object[] contexts) => Logger.Information(msg, contexts);
    public static void LogError(string msg, params object[] contexts) => Logger.Error(msg, contexts);
}