using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Time_Tracking.Logger
{
    public static class EventLogsIdHelper
    {
        // С 1 по 4 события по работе с операциями GRUD таблицы
        public static readonly EventId EventGrudOperation = new EventId(1, "GRUD operation");

        // События ошибок
        public static readonly EventId EventError = new EventId(2, "Exception in application");
    }

    public static class LoggerExtension
    {
        private static readonly Action<ILogger, string, string, string, DateTime, string, int, Exception> Error;

        private static readonly Action<ILogger, DateTime, string, string, string, string, Exception> InfoGRUD;

        static LoggerExtension()
        {
            Error = LoggerMessage.Define<string, string, string, DateTime, string, int>(LogLevel.Information, EventLogsIdHelper.EventError, 
                "Error: {message}, Inner message: {innerMessage}, Stack trace: {stackTrace}, Date: {date}, Class name: {className}, RequestId: {requestId}");
            
            InfoGRUD = LoggerMessage.Define<DateTime, string, string, string, string>(LogLevel.Information, EventLogsIdHelper.EventGrudOperation,
                "{Date}: {Controller}/{Action}, была проведена операция: {GrudOperation} со следующими данными {Message}");
        }

        public static void ErrorMessage(this ILogger logger, string message, string innerMessage, string stackTrace, DateTime date, string className, int requestId)
        {
            Error(logger, message, innerMessage, stackTrace, date, className, requestId, null);
        }

        public static void InfoGrud(this ILogger logger, DateTime date, string controller, string action, string grudOperation, string message)
        {
            InfoGRUD.Invoke(logger, date, controller, action, grudOperation, message, null);
        }

    }
}
