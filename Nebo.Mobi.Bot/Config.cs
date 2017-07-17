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
        public int UsersCount;
        public string Autorun;
        public string Hide;
        public string SizeX;
        public string SizeY;

        public struct User
        {
            public string Login;
            public string Pass;
            public string MinTime;
            public string MaxTime;
            public string DoNotPut;
            public string Fire;
            public string FireLess;
            public string Fire9;
            public string DoNotShowStatistic;
            public string DoNotSaveThePass;
            public string DoNotGetRevard;
            public string DoNotLift;
            public string Invite;
            public string InviteFrom;
            public string InviteFromMeaning;
            public string InviteTo;
            public string InviteToMeaning;
            public string Appoint;
            public string AppointTo;
            public string Avatar;

            //инициализируем дефолтные настройки для конкретного пользователя
            public void InitUserDefault()
            {
                Login = "";
                Pass = "";
                MinTime = "2";
                MaxTime = "25";
                DoNotPut = "false";
                Fire = "true";
                FireLess = "9";
                Fire9 = "true";
                DoNotShowStatistic = "false";
                DoNotLift = "false";
                DoNotSaveThePass = "false";
                DoNotGetRevard = "false";
                Invite = "true";
                InviteFrom = "false";
                InviteFromMeaning = "10";
                InviteTo = "false";
                InviteToMeaning = "75";
                Appoint = "true";
                AppointTo = "горожанин";
                Avatar = "unknown.png";
            }            
        }        

        //коллекция настроек пользователей
        public List<User> users;

        //конструктор с дефолтными настройками
        public Config(string p)
        {
            path = p;
            UsersCount = 1;             //1 т.к. создастся 1 пустой персонаж
            Autorun = "false";
            Hide = "false";
            SizeX = "985";
            SizeY = "700";
            users = new List<User>();
            LoadConfig();
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
                node = doc.SelectSingleNode("config/UsersCount");
                UsersCount = Convert.ToInt32(node.InnerText);
            }
            catch
            {
                EverythingAlright = false;
            }

            if (EverythingAlright)
            {
                for (int i = 0; i < UsersCount; i++)
                {
                    User u = new User();
                    u.InitUserDefault();

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Login", i));
                        u.Login = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Pass", i));
                        u.Pass = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/MinTime", i));
                        u.MinTime = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/MaxTime", i));
                        u.MaxTime = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/DoNotPut", i));
                        u.DoNotPut = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Fire", i));
                        u.Fire = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/FireLess", i));
                        u.FireLess = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Fire9", i));
                        u.Fire9 = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/DoNotSaveThePass", i));
                        u.DoNotSaveThePass = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/DoNotShowStatistic", i));
                        u.DoNotShowStatistic = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/DoNotGetRevard", i));
                        u.DoNotGetRevard = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/DoNotLift", i));
                        u.DoNotLift = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Invite", i));
                        u.Invite = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/InviteFrom", i));
                        u.InviteFrom = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/InviteTo", i));
                        u.InviteTo = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/InviteFromMeaning", i));
                        u.InviteFromMeaning = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/InviteToMeaning", i));
                        u.InviteToMeaning = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Appoint", i));
                        u.Appoint = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/AppointTo", i));
                        u.AppointTo = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    try
                    {
                        node = doc.SelectSingleNode(string.Format("config/u{0}/Avatar", i));
                        u.Avatar = node.InnerText;
                    }
                    catch
                    {
                        EverythingAlright = false;
                    }

                    users.Add(u);
                }

                try
                {
                    node = doc.SelectSingleNode("config/Autorun");
                    Autorun = node.InnerText;
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

                try
                {
                    node = doc.SelectSingleNode("config/SizeX");
                    SizeX = node.InnerText;
                }
                catch
                {
                    EverythingAlright = false;
                }

                try
                {
                    node = doc.SelectSingleNode("config/SizeY");
                    SizeY = node.InnerText;
                }
                catch
                {
                    EverythingAlright = false;
                }
            }

            //пишем дефолтные конфиги если ошибка не внутри пользователя
            else
            {
                WriteDefaultConfig();
                EverythingAlright = true;
            }

            if(!EverythingAlright)
            {
                WriteDefaultConfig();
                EverythingAlright = true;
            }
        }

        //метод записи настроек
        public void WriteConfig(List<User> us)
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
            doc.WriteElementString("UsersCount", UsersCount.ToString());
            //на случай генерации настроек
            if (users.Count == 0)
            {
                User u = new User();
                u.InitUserDefault();
                users.Add(u);
            }
            for (int i = 0; i < UsersCount; i++)
            {
                users[i] = us[i];
                doc.WriteStartElement(string.Format("u{0}", i));
                doc.WriteElementString("Login", users[i].Login);
                doc.WriteElementString("Pass", users[i].Pass);
                doc.WriteElementString("MinTime", users[i].MinTime);
                doc.WriteElementString("MaxTime", users[i].MaxTime);
                doc.WriteElementString("DoNotPut", users[i].DoNotPut);
                doc.WriteElementString("Fire", users[i].Fire);
                doc.WriteElementString("FireLess", users[i].FireLess);
                doc.WriteElementString("Fire9", users[i].Fire9);
                doc.WriteElementString("DoNotSaveThePass", users[i].DoNotSaveThePass);
                doc.WriteElementString("DoNotShowStatistic", users[i].DoNotShowStatistic);
                doc.WriteElementString("DoNotGetRevard", users[i].DoNotGetRevard);
                doc.WriteElementString("DoNotLift", users[i].DoNotLift);
                doc.WriteElementString("Invite", users[i].Invite);
                doc.WriteElementString("InviteFrom", users[i].InviteFrom);
                doc.WriteElementString("InviteFromMeaning", users[i].InviteFromMeaning);
                doc.WriteElementString("InviteTo", users[i].InviteTo);
                doc.WriteElementString("InviteToMeaning", users[i].InviteToMeaning);
                doc.WriteElementString("Appoint", users[i].Appoint);
                doc.WriteElementString("AppointTo", users[i].AppointTo);
                doc.WriteElementString("Avatar", users[i].Avatar);
                doc.WriteEndElement(); //u[i]
            }
            doc.WriteElementString("Autorun", Autorun);
            doc.WriteElementString("Hide", Hide);
            doc.WriteElementString("SizeX", SizeX);
            doc.WriteElementString("SizeY", SizeY);
            doc.WriteEndElement(); //config
            doc.Flush();
            doc.Close();
        }

        //метод записи "пустых" настроек - костыль
        public void WriteDefaultConfig()
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

            if (UsersCount > 0)
            doc.WriteElementString("UsersCount", UsersCount.ToString());

            //на случай генерации настроек
            if (users.Count == 0)
            {
                User u = new User();
                u.InitUserDefault();
                users.Add(u);
            }
            for (int i = 0; i < UsersCount; i++)
            {
                doc.WriteStartElement(string.Format("u{0}", i));
                doc.WriteElementString("Login", users[i].Login);
                doc.WriteElementString("Pass", users[i].Pass);
                doc.WriteElementString("MinTime", users[i].MinTime);
                doc.WriteElementString("MaxTime", users[i].MaxTime);
                doc.WriteElementString("DoNotPut", users[i].DoNotPut);
                doc.WriteElementString("Fire", users[i].Fire);
                doc.WriteElementString("FireLess", users[i].FireLess);
                doc.WriteElementString("Fire9", users[i].Fire9);
                doc.WriteElementString("DoNotSaveThePass", users[i].DoNotSaveThePass);
                doc.WriteElementString("DoNotShowStatistic", users[i].DoNotShowStatistic);
                doc.WriteElementString("DoNotGetRevard", users[i].DoNotGetRevard);
                doc.WriteElementString("DoNotLift", users[i].DoNotLift);
                doc.WriteElementString("Invite", users[i].Invite);
                doc.WriteElementString("InviteFrom", users[i].InviteFrom);
                doc.WriteElementString("InviteFromMeaning", users[i].InviteFromMeaning);
                doc.WriteElementString("InviteTo", users[i].InviteTo);
                doc.WriteElementString("InviteToMeaning", users[i].InviteToMeaning);
                doc.WriteElementString("Appoint", users[i].Appoint);
                doc.WriteElementString("AppointTo", users[i].AppointTo);
                doc.WriteElementString("Avatar", users[i].Avatar);
                doc.WriteEndElement(); //u[i]
            }
            doc.WriteElementString("Autorun", Autorun);
            doc.WriteElementString("Hide", Hide);
            doc.WriteElementString("SizeX", SizeX);
            doc.WriteElementString("SizeY", SizeY);
            doc.WriteEndElement(); //config
            doc.Flush();
            doc.Close();
        }

        //добавить в коллекцию конфиг пользователя
        public void AddUser(User u)
        {
            users.Add(u);
            UsersCount = users.Count;
        }
    }
}
