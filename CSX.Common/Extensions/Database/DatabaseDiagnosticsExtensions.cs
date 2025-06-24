using System;

namespace CSX.Common.Extensions.Database;

public static class DatabaseDiagnosticsExtensions
{
    public static bool IsUniqueConstraintViolation(this Exception? ex)
    {
        while (ex != null)
        {
            var msg = ex.Message.ToLowerInvariant();

            if (msg.Contains("unique constraint") ||
                msg.Contains("duplicate key") ||
                msg.Contains("violates unique constraint") ||
                msg.Contains("unique index") ||
                msg.Contains("cannot insert duplicate") ||
                msg.Contains("duplicate entry"))
            {
                return true;
            }

            ex = ex.InnerException;
        }
        return false;
    }
}