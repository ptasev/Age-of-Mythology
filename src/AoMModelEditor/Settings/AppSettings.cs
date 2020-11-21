using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AoMModelEditor.Settings
{
    public class AppSettings
    {
        private static string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\app_settings.xml";

        public string OpenFileDialogFileName { get; private set; }
        public string SaveFileDialogFileName { get; private set; }
        public string MtrlFolderDialogDirectory { get; private set; }
        public string GameDirectory { get; private set; }

        public AppSettings()
        {
            OpenFileDialogFileName = string.Empty;
            SaveFileDialogFileName = string.Empty;
            MtrlFolderDialogDirectory = string.Empty;
            GameDirectory = string.Empty;
        }

        public void Read()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlDocument settings = new XmlDocument();
                    settings.Load(fs);
                    if (settings.DocumentElement is null) return;
                    foreach (XmlElement? elem in settings.DocumentElement)
                    {
                        if (elem is null) continue;

                        if (elem.Name == "defaultOpenDirectory")
                        {
                            OpenFileDialogFileName = elem.InnerText;
                        }
                        else if (elem.Name == "defaultSaveDirectory")
                        {
                            SaveFileDialogFileName = elem.InnerText;
                        }
                        else if (elem.Name == "defaultMtrlSaveDirectory")
                        {
                            MtrlFolderDialogDirectory = elem.InnerText;
                        }
                        else if (elem.Name == "gameDirectory")
                        {
                            GameDirectory = elem.InnerText;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
