using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace WebsiteTuyenDung.Models.Services
{
    public static class AuditLogService
    {
        private static readonly object SyncRoot = new object();

        public static void Write(string actorId, string actorName, string action, string target, string detail = null)
        {
            try
            {
                var folder = ResolveLogFolder();
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, "audit-" + DateTime.UtcNow.ToString("yyyyMMdd") + ".log");
                var line = string.Join("\t", new[]
                {
                    DateTime.UtcNow.ToString("o"),
                    Clean(actorId),
                    Clean(actorName),
                    Clean(action),
                    Clean(target),
                    Clean(detail),
                    Clean(GetClientIp())
                });

                lock (SyncRoot)
                {
                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Audit log should never break the user-facing flow.
            }
        }

        private static string ResolveLogFolder()
        {
            var mappedPath = HostingEnvironment.MapPath("~/App_Data/AuditLogs");
            if (!string.IsNullOrWhiteSpace(mappedPath))
            {
                return mappedPath;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "AuditLogs");
        }

        private static string GetClientIp()
        {
            return HttpContext.Current?.Request?.UserHostAddress;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            return value.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();
        }
    }
}
