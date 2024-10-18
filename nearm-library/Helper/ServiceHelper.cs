using System;
using System.ServiceProcess;

namespace nearm_library.Helper;

internal static class ServiceHelper
{
    public static bool IsServiceRunning(string serviceName)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            return sc.Status == ServiceControllerStatus.Running;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return false;
        }
    }
}
