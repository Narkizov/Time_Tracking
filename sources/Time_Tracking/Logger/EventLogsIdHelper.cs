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
        private static readonly Action<ILogger, string, Exception> Error;
        // date, controller, action, EventName, message
        private static readonly Action<ILogger, DateTime, string, string, string, string, Exception> InfoGRUD;

        static LoggerExtension()
        {
            Error = LoggerMessage.Define<string>(LogLevel.Information, EventLogsIdHelper.EventError, "Error: {message}");
            InfoGRUD = LoggerMessage.Define<DateTime, string, string, string, string>(LogLevel.Information, EventLogsIdHelper.EventGrudOperation,
                "{Date}: {Controller}/{Action}, была проведена операция: {GrudOperation} со следующими данными {Message}");
        }

        public static void ErrorMessage(this ILogger logger, string message)
        {
            Error(logger, message, null);
        }

        public static void InfoGrud(this ILogger logger, DateTime date, string controller, string action, string grudOperation, string message)
        {
            InfoGRUD.Invoke(logger, date, controller, action, grudOperation, message, null);
        }

    }
}
