using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Nebo.Mobi.Bot
{
    public class Config
    {
        private string path;
        public string Login;
        public string Pass;
        public string MinTime;
        public string MaxTime;
        public string DoNotPut;
        public string Fire;
        public string FireLess;
        public string Fire9;
        public string DoNotSaveThePass;
        public string DoNotGetRevard;
        public string Hide;

        //конструктор с дефолтными настройками
        public Config(string p)
        {
            path = p;
            Login = "";
            Pass = "";
            MinTime = "2";
            MaxTime = "25";
            DoNotPut = "false";
            Fire = "true";
            FireLess = "9";
            Fire9 = "true";
            DoNotSaveThePass = "false";
            DoNotGetRevard = "false";
            Hide = "false";
        }
        
        //метод подгрузки настроек
        public void LoadConfig()
        {
            bool EverythingAlright = true;          //флаг правильности файла настроек
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(path + "config.xml");
            }
            catch
            {
                EverythingAlright = false;
            }

            XmlNode node;
            try
            {
                node = doc.SelectSingleNode("config/Login");
                Login = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/Pass");
                Pass = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/MinTime");
                MinTime = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/MaxTime");
                MaxTime = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/DoNotPut");
                DoNotPut = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/Fire");
                Fire = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/FireLess");
                FireLess = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/Fire9");
                Fire9 = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/DoNotSaveThePass");
                DoNotSaveThePass = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/DoNotGetRevard");
                DoNotGetRevard = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            try
            {
                node = doc.SelectSingleNode("config/Hide");
                Hide = node.InnerText;
            }
            catch
            {
                EverythingAlright = false;
            }

            if (!EverythingAlright)
            {
                WriteConfig();
            }
        }

        //метод записи настроек
        public void WriteConfig()
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

            doc = XmlWriter.Create(path + "config.xml", settings);
            // Создаем элемент <dname>sanchos</dname>
            doc.WriteStartElement("config");
            doc.WriteElementString("Login", Login);
            doc.WriteElementString("Pass", Pass);
            doc.WriteElementString("MinTime", MinTime);
            doc.WriteElementString("MaxTime", MaxTime);
            doc.WriteElementString("DoNotPut", DoNotPut);
            doc.WriteElementString("Fire", Fire);
            doc.WriteElementString("FireLess", FireLess);
            doc.WriteElementString("Fire9", Fire9);
            doc.WriteElementString("DoNotSaveThePass", DoNotSaveThePass);
            doc.WriteElementString("DoNotGetRevard", DoNotGetRevard);
            doc.WriteElementString("Hide", Hide);
            doc.WriteEndElement();
            doc.Flush();
            doc.Close();
        }
    }
}
