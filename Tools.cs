using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ToolsLib
{
    public static class Tools
    {
        /// <summary>
        /// Serializing (T) Object to selected path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Object for serialization</param>
        /// <param name="path">Path of file for serialization</param>
        /// <returns></returns>
        public static bool Serialize<T>(T data, string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    var f = File.Create(path);
                    f.Close();
                }
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                TextWriter textWriter = new StreamWriter(path);
                xmlSerializer.Serialize(textWriter, data);
                textWriter.Close();
            }
            catch (Exception exc)
            {
                ExceptionLogAndShow(exc, "Tools");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns deserialized file to (T) object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string path)
        {
            T data = default(T);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StreamReader reader = new StreamReader(path);
                data = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception exc)
            {
                ExceptionLogAndShow(exc, "Tools");
                return default(T);
            }
            return data;
        }        
        /// <summary>
        /// Saving exception message and stackTrace to log file in directory indicated in the app settings.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="sender"></param>
        public static void ExceptionLogAndShow(Exception exception, string sender = "")
        {
            try
            {
                DateTime date = DateTime.Now;
                string path = Path.Combine(ReadAppSetting("errorLogPath") , date.ToString("yyyyMMdd") + ".log");
                string content = date.ToLongTimeString().Replace("/", "_").Replace("\\", "") + ": ";
                if (sender != "") content += $"In window '{sender}' ";
                content += exception.ToString() + System.Environment.NewLine;
                File.AppendAllText(path, content);
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                //File.AppendAllText("unhandedErrors.log", exception.ToString());
                //MessageBox.Show(exception.ToString() + Environment.NewLine + exc.ToString(), "CRITICAL ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }        
        /// <summary>
        /// Returns string with value of given key from App.config settings.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string key)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            return appSettings[key] ?? throw new Exception($"Setting key='{key}' was not found in configuration.");
        }
        /// <summary>
        /// Returns string with value of given key from App.config settings.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string key,string defaultValue)
        {            
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            if (appSettings[key] == null)
            {
                WriteAppSetting(key, defaultValue);
                return defaultValue;
            }
            else
                return appSettings[key];
            
        }
        /// <summary>
        /// Returns string value of given key from App.config setting Combined with Current application directory
        /// Dedicated for relative paths
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppSettingPath(string key)
        {
            try
            {
                var combinedpath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), ReadAppSetting(key));
                return System.IO.Path.GetFullPath(combinedpath);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Returns valid value of key, or set path as value to key if this was not set, or not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSetCreateSettingPath(string key,string path)
        {
            try
            {
                string tmp = ReadAppSettingPath(key);
                if (string.IsNullOrWhiteSpace(tmp))
                {                      
                    WriteAppSetting(key, path);
                    tmp = Path.Combine(Directory.GetCurrentDirectory(), path);
                }
                else
                {
                    tmp = Path.Combine(Directory.GetCurrentDirectory(), path);
                }
                if (!Directory.Exists(tmp))
                {
                    try
                    {
                        Directory.CreateDirectory(tmp);
                    }
                    catch
                    {
                        tmp = Path.Combine(Directory.GetCurrentDirectory(), path);
                        WriteAppSetting(key, path);
                    }
                }
                return tmp;
            }
            catch(Exception exc)
            {
                ExceptionLogAndShow(exc, "GetSetCreateSettingPath");
                return null;
            }
        }
        public static string GetOrSetDefaultDataDirectory(string path = "Data\\")
        {
            return GetSetCreateSettingPath("defaultDataDirectory", path);
        }
        public static string GetOrSetErrorLogPath(string path = "Data\\ErrorLogs\\")
        {
            return GetSetCreateSettingPath("errorLogPath", path);
        }
        /// <summary>
        /// Set value to key in appSettings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteAppSetting(string key,string value)
        {
            try
            {
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove(key);
                config.AppSettings.Settings.Add(key, value);
                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                if (System.Configuration.ConfigurationManager.AppSettings.Get(key) == value)
                    return true;
                else
                    return false;
            }
            catch(Exception exc)
            {
                ExceptionLogAndShow(exc, "Tools.WriteAppSetting()");
                return false;
            }
        }
        /// <summary>
        /// Randomly reorder items in list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        
       
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

}

namespace ToolsLib.Wpf
{
    public static class WpfTools
    {
        /// <summary>
        /// Sets Content of selected Label and clearing it async after 2.5 seconds.
        /// </summary>
        /// <param name="l">Handle for label</param>
        /// <param name="text">Text to set on label</param>
        /// <param name="timeSpan">Allows to set custom clear time.</param>
        public static void SetWaitClearLabel(Label l, string text, TimeSpan timeSpan = new TimeSpan())
        {
            if (timeSpan == new TimeSpan())
            {
                timeSpan = TimeSpan.FromSeconds(2.5);
            }
            if (text != "")
                l.Content = text;
            l.Dispatcher.Invoke(new Action(async () => { await Task.Delay(timeSpan); l.Content = ""; }));
        }
    }
    public class AboutWindow : Window
    {
        public AboutWindow(string title, string buildinfo, string content, string created = "Dewrodyn Rafał Kurc", string link = "http://rafalkurc.pl")
        {
            this.Width = 300;
            this.Height = 300;
            this.Title = title;
            Grid grid = new Grid();
            TextBlock textBlock = new TextBlock() { TextWrapping = TextWrapping.WrapWithOverflow, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
            grid.Children.Add(textBlock);
            this.AddChild(grid);
            if (!string.IsNullOrWhiteSpace(title))
            {
                textBlock.Inlines.Add(new Bold(new Run(title)) { FontSize = 18, });
                textBlock.Inlines.Add(new LineBreak());
            }
            if (!string.IsNullOrWhiteSpace(buildinfo))
            {
                textBlock.Inlines.Add(new Run("Build " + buildinfo));
                textBlock.Inlines.Add(new LineBreak());
            }
            if (!string.IsNullOrWhiteSpace(content))
            {
                textBlock.Inlines.Add(new Run(content));
                textBlock.Inlines.Add(new LineBreak());
                textBlock.Inlines.Add(new LineBreak());
            }
            if (!string.IsNullOrWhiteSpace(created))
            {
                textBlock.Inlines.Add(new Italic(new Run("Created by: " + created)));
                textBlock.Inlines.Add(new LineBreak());
            }
            if (!string.IsNullOrWhiteSpace(link))
            {
                textBlock.Inlines.Add(new Hyperlink(new Run(link)));
                textBlock.Inlines.Add(new LineBreak());
            }
            this.Show();
        }
    }
}
