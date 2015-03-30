using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace Nebo.Mobi.Bot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //если файла конфигурации нет - генерим дефолт
            if (!File.Exists("config.xml"))
            {
                XmlWriter doc;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    "; // задаем отступ, здесь у меня 4 пробела
                settings.NewLineOnAttributes = false;
                // задаем переход на новую строку
                settings.NewLineChars = "\n";
                // Нужно ли опустить строку декларации формата XML документа 
                // речь идет о строке вида "<?xml version="1.0" encoding="utf-8"?>"
                settings.OmitXmlDeclaration = false;

                doc = XmlWriter.Create("config.xml", settings);
                // Создаем элемент <dname>sanchos</dname>
                doc.WriteStartElement("config");
                doc.WriteElementString("Login", "");
                doc.WriteElementString("Pass", "");
                doc.WriteElementString("MinTime", "2");
                doc.WriteElementString("MaxTime", "25");
                doc.WriteElementString("DoNotPut", "false");
                doc.WriteElementString("Fire", "true");
                doc.WriteElementString("FireLess", "9");
                doc.WriteElementString("Fire9", "true");
                doc.WriteElementString("DoNotSaveThePass", "false");
                doc.WriteEndElement();
                doc.Flush();
                doc.Close();
            }
            Application.Run(new Form1());
        }
    }
}
