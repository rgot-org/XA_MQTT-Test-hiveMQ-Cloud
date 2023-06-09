﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using AppMQTT;
//
//    var config = Config.FromJson(jsonString);

namespace AppMQTT
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Config
    {
        public static Config init(string configFileName)
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{configFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                return Config.FromJson(jsonString);
            }
        }
        [JsonProperty("Server")]
        public string Server { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("User")]
        public string User { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("PubTopic")]
        public string PubTopic { get; set; }

        [JsonProperty("SubTopic")]
        public string SubTopic { get; set; }
    }

    public partial class Config
    {
        public static Config FromJson(string json) => JsonConvert.DeserializeObject<Config>(json, AppMQTT.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Config self) => JsonConvert.SerializeObject(self, AppMQTT.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
