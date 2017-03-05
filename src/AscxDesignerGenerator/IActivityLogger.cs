namespace DnnProjectSystem.Logging
{
    public interface IActivityLogger
    {
        System.Threading.Tasks.Task LogInfo(string message, params object[] arguments);
        System.Threading.Tasks.Task LogWarning(string message, params object[] arguments);
        System.Threading.Tasks.Task LogError(string message, params object[] arguments);
    }
}