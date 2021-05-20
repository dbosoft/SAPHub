﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Dbosoft.YaNco.ILogger;

namespace SAPHub.ConnectorModule
{
    /// <summary>
    /// Adapter for YaNco Logging to Microsoft.Extensions.Logging
    /// </summary>
    public class RfcLoggingAdapter : ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        // ReSharper disable once SuggestBaseTypeForParameter
        public RfcLoggingAdapter(ILogger<RfcLoggingAdapter> logger)
        {
            _logger = logger;
        }

        public void LogException(Exception exception, string message)
        {
            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.LogError(exception, message);

        }

        public void LogException(Exception exception)
        {
            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.LogError(exception, "");

        }

        public void LogTrace(string message, object data)
        {
            if (!_logger.IsEnabled(LogLevel.Trace))
                return;
            _logger.LogTrace(JoinMessageAndData(message, data));

        }


        public void LogError(string message, object data)
        {
            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.LogError(JoinMessageAndData(message, data));

        }

        public void LogDebug(string message, object data)
        {
            if (!_logger.IsEnabled(LogLevel.Debug))
                return;
            _logger.LogDebug(JoinMessageAndData(message, data));

        }

        public void LogTrace(string message)
        {
            LogTrace(message, null);
        }

        public void LogDebug(string message)
        {
            LogDebug(message, null);
        }

        public void LogError(string message)
        {
            LogError(message, null);
        }

        private static string JoinMessageAndData(string message, object data)
        {
            if (data == null)
                return message;

            var resultString = new StringBuilder();

            resultString.Append(message);
            resultString.Append(", Data: ");

            resultString.Append(SerializeData(data));

            return resultString.ToString();
        }


        private static string SerializeData(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>(new[] { new HandleToStringJsonConverter() })
            });
        }


        private class HandleToStringJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;

            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var typeName = value.GetType().Name;
                writer.WriteValue($"{typeName}<{value}>");
            }

            public override bool CanRead => false;

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

    }
}