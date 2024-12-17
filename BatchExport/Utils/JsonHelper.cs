﻿using System;
using System.IO;
using System.Windows;
using System.Reflection;
using Newtonsoft.Json;

namespace VLS.BatchExport.Utils
{
    public static class JsonHelper<T>
    {
        public static T DeserializeResource(string path) =>
            JsonConvert.DeserializeObject<T>(
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(path)).ReadToEnd());

        public static T DeserializeConfig(FileStream file) =>
            HandleSerialization(() => JsonConvert.DeserializeObject<T>(new StreamReader(file).ReadToEnd()));

        public static void SerializeConfig(T value, string path) =>
            HandleSerialization(() =>
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(JsonConvert.SerializeObject(value, Formatting.Indented));
                    }
                }
                return default;
            });

        private static T HandleSerialization(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                return default;
            }
        }
    }
}