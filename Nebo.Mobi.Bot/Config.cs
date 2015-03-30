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
        public string Login;
        public string Pass;
        public string MinTime;
        public string MaxTime;
        public string DoNotPut;
        public string Fire;
        public string FireLess;
        public string Fire9;
        public string DoNotSaveThePass;

        //конструктор
        public Config()
        {
        }
        
        //метод подгрузки настроек
        public void LoadConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");

            XmlNode node;
            node = doc.SelectSingleNode("config/Login");
            Login = node.InnerText;

            node = doc.SelectSingleNode("config/Pass");
            Pass = node.InnerText;

            node = doc.SelectSingleNode("config/MinTime");
            MinTime = node.InnerText;

            node = doc.SelectSingleNode("config/MaxTime");
            MaxTime = node.InnerText;

            node = doc.SelectSingleNode("config/DoNotPut");
            DoNotPut = node.InnerText;

            node = doc.SelectSingleNode("config/Fire");
            Fire = node.InnerText;

            node = doc.SelectSingleNode("config/FireLess");
            FireLess = node.InnerText;

            node = doc.SelectSingleNode("config/Fire9");
            Fire9 = node.InnerText;

            node = doc.SelectSingleNode("config/DoNotSaveThePass");
            DoNotSaveThePass = node.InnerText;
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

            doc = XmlWriter.Create("config.xml", settings);
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
            doc.WriteEndElement();
            doc.Flush();
            doc.Close();
        }
    }
}
