using Serilog;
using Serilog.Core;

namespace VoxMod.Main;

public static class VoxLogger
{
    private static Logger _logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt").CreateLogger();

    public static void LogInfo(string msg, params object[] contexts) => _logger.Information(msg, contexts);
    public static void LogError(string msg, params object[] contexts) => _logger.Error(msg, contexts);
}