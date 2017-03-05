using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DnnProjectSystem.Logging
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _packageOrComponentName;

        public ActivityLogger(IServiceProvider serviceProvider, string packageOrComponentName)
        {
            _serviceProvider = serviceProvider;
            _packageOrComponentName = packageOrComponentName;
        }

        public async System.Threading.Tasks.Task LogInfo(string message, params object[] arguments)
        {
            await Log(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, message, arguments);
        }

        public async System.Threading.Tasks.Task LogWarning(string message, params object[] arguments)
        {
            await Log(__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING, message, arguments);
        }

        public async System.Threading.Tasks.Task LogError(string message, params object[] arguments)
        {
            await Log(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, message, arguments);
        }

        private async System.Threading.Tasks.Task Log(__ACTIVITYLOG_ENTRYTYPE type, string message, params object[] arguments)
        {

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var log = _serviceProvider.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            if (log == null)
                return;

            log.LogEntry((UInt32)type, _packageOrComponentName, string.Format(CultureInfo.CurrentCulture, message, arguments));
        }
    }
}