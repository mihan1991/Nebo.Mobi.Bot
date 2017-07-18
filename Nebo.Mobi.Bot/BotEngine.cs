using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Threading;
using System.Threading;
using System.Web;
using System.Net;
using System.IO;

namespace Nebo.Mobi.Bot
{
    public class BotEngine
    {
        public bool PageCreated;
        public TimeSpan BotTimeSleep;

        /// <summary>
        //блок разовой статистики
        private int lift_count;                                 //счетчик перевезенных в лифте
        private int buy_count;                                  //счетчик купленных товаров
        private int coins_count;                                //счетчик собранных выручек (точнее этажей)
        private int bucks_count;                                //счетчик полученных баксов (пока только чаевые)
        private int merch_count;                                //счетчик выложенных товаров (точнее этажей)
        private int killed_count;                               //счетчик выселенных 
        private int new_worker_count;                           //счетчик новых нанятых
        private int opened_floor_count;                         //счетчик открытых этажей
        private int invited_count;                              //счетчик приглашенных

        //блок данных пользователя
        public string User_Level;
        public string User_Bucks;
        public string User_Coins;
        public string User_City;
        public string User_Floors;
        public string City_Role;

        //блок общей статистики работы
        public int Lift_Count;
        public int Bucks_Count;
        public int Buy_Count;
        public int Coins_Count;
        public int Merch_Count;
        public int Killed_Count;
        public int New_Worker_Count;
        public int Opened_Floor_Count;
        public int Invited_Count;
        public int Action_Count;
        public string[] Stat;

        private static string SERVER = "https://vnebo.mobi/";     //адрес сервера
        //private static string SERVER = "http://pumpit.mmska.ru/";
        public string CONNECT_STATUS = "";                     //статус соединения
        public string ACTION_STATUS = "";                      //теекущее действие
        public string CURRENT_HTML;

        private Random rnd;
        private string HTML;                                    //html-код текущей страницы
        private string LINK;                                    //переменная для обмена ссылками
        private string HOME_LINK;                               //ссылка на домашнюю страницу
        public List<string> COMMUTATION_STR;                   //коллекция строк для логов
        

        private Thread BotThread;                                     //переменная потока бота

        public Config.User user_cfg;                                     //объект класса настроек

        private WebClient webClient;


        public BotEngine(Config.User usr)
        {
            user_cfg = usr;
            Initialize();
        }

        public BotEngine()
        {
            user_cfg = new Config.User();
            user_cfg.InitUserDefault();
            Initialize();
        }

        //вернуть таймер бота
        public Thread GetBotThread()
        {
            return BotThread;
        }

        private void Initialize()
        {
            COMMUTATION_STR = new List<string>();
            webClient = new WebClient();

            rnd = new Random();

            //обнуление общей статистики
            User_Bucks = User_Coins = User_Level = User_City = City_Role = "";
            Lift_Count = Buy_Count = Coins_Count = Merch_Count = Killed_Count = New_Worker_Count = Action_Count = Bucks_Count = Opened_Floor_Count = Invited_Count = 0;

            BotThread = new Thread(StartBot);
        }

        //открытый метод переинициализации потока (из интерфейсной части)
        public void ResetThread()
        {
            BotThread = new Thread(StartBot);
        }


        //создание страницы с отчетом
        public void CreateHTMLPage(string res)
        {
            Stat = res.Split('\n');

            for (int i = 0; i < Stat.Length; i++)
            {
                if (Stat[i].Contains("<div class=\"prg\""))
                {
                    Stat[i] = Parse(HTML, "<div class=\"prg\"");
                }

                if (Stat[i].Contains("/icons/user/"))
                {
                    Stat[i] = "<img src=\"http://static.nebo.mobi/images/icons/user/" + user_cfg.Avatar + "\" width=\"16\" height=\"16\" alt=\"u\"\r";
                }
                if (Stat[i].Contains("UserName"))
                {
                    Stat[i] = "<UserName>" + user_cfg.Login + "</UserName>\r";
                }

                if (Stat[i].Contains("UserCoins"))
                {
                    Stat[i] = "<UserCoins>" + User_Coins + "</UserCoins>\r";
                }

                if (Stat[i].Contains("UserBucks"))
                {
                    Stat[i] = "<UserBucks>" + User_Bucks + "</UserBucks>\r";
                }

                if (User_City != "")
                {
                    if (Stat[i].Contains("UserCity"))
                    {
                        Stat[i] = "<UserCity>" + User_City + "</UserCity>\r";
                    }

                    if (Stat[i].Contains("CityRole"))
                    {
                        Stat[i] = "<CityRole>" + City_Role + "</CityRole>\r";
                    }
                }

                else
                {
                    if (Stat[i].Contains("UserCity"))
                    {
                        Stat[i] = "<UserCity></UserCity>\r";
                    }

                    if (Stat[i].Contains("CityRole"))
                    {
                        Stat[i] = "<CityRole> (без города) </CityRole>\r";
                    }
                }

                if (Stat[i].Contains("UserLevel"))
                {
                    Stat[i] = "<UserLevel>" + User_Level + "</UserLevel>\r";
                }

                if (Stat[i].Contains("UserFloors"))
                {
                    Stat[i] = "<UserFloors>" + User_Floors + "</UserFloors>\r";
                }

            }

            CURRENT_HTML = "";
            foreach (string ss in Stat)
                CURRENT_HTML += ss;

            PageCreated = true;
        }


        //получение полного списка этажей
        private void ConnectAndGoHome()
        {
            //подключаемся и переходим на Главную и раскрываем этажи
            Connect();
            GetHomePage();
        }

        //тупо получение главного экрана и проверка на открытие этажей
        private void GetHomePage()
        {
            string ab;
            ClickLink(HOME_LINK, "");

            //получаем ссылку "Показать этажи"
            ab = Parse(HTML, "Показать этажи");
            if (ab != "")
            {
                ab = ab.Substring(49);
                ab = ab.Remove(ab.IndexOf("\""));

                ClickLink(ab, "");
                Thread.Sleep(rnd.Next(100, 300));
            }
        }

        //получаем уровень и финансы
        private void GetInfo()
        {
            User_Bucks = User_Coins = User_Level = User_City = User_Floors = City_Role = "";
            GetHomePage();

            string ab = Parse(HTML, "mn_iron.png");
            if (ab != "")
            {
                //ab = ab.Substring(122);
                ab = ab.Substring(99);
                ab = ab.Remove(ab.IndexOf('<'));
                User_Coins = ab.Replace("&#039;", "'");
            }

            ab = Parse(HTML, "mn_gold.png");
            if (ab != "")
            {
                //ab = ab.Substring(122);
                ab = ab.Substring(99);
                ab = ab.Remove(ab.IndexOf('<'));
                User_Bucks = ab.Replace("&#039;", "'");
            }

            //получаем уровень
            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int i;
            for (i = 0; i < str.Length; i++)
            {
                if (str[i].Contains("уровень"))
                {
                    //фиксируем строчку с кубком, днюхой или еще каким дерьмом
                    if (!str[i].Contains("star.png"))
                        ab = str[i - 1];
                    //ииначе - как обычный игрок в обычный день
                    else
                        ab = str[i];

                    //ab = ab.Substring(106);
                    ab = ab.Substring(83);
                    ab = ab.Remove(ab.IndexOf('<'));
                    break;
                }
            }
            User_Level = ab;// + " уровень";
            ab = "";

            //получаем аватар
            for (i = 0; i < str.Length; i++)
            {
                if (str[i].Contains(user_cfg.Login))
                {
                    ab = str[i - 1];
                    //ab = ab.Substring(58);
                    ab = ab.Substring(35);
                    ab = ab.Remove(ab.IndexOf('\"'));
                    break;
                }
            }

            user_cfg.Avatar = ab;
            ab = "";

            //получаем количество этажей
            for (i = 0; i < str.Length; i++)
            {
                if (str[i].Contains("class=\"flhdr\""))
                {
                    ab = str[i + 1];
                    ab = ab.Substring(15);
                    ab = ab.Remove(ab.IndexOf('.'));
                    break;
                }
            }
            User_Floors = ab;
            ab = "";

            //Thread.Sleep(rnd.Next(500, 1000));

            //лезем в Мой город 
            for (i = 0; i < str.Length; i++)
            {
                if (str[i].Contains("Мой город"))
                {
                    ab = str[i];
                    ab = ab.Substring(45);
                    ab = ab.Remove(ab.IndexOf('\"'));
                    ClickLink(ab, "");

                    str = HTML.Split('\n');

                    //ищем строку с городом
                    for (int j = 0; j < str.Length; j++)
                    {
                        if (str[j].Contains("&laquo;"))
                        {
                            ab = str[j];
                            ab = ab.Substring(42);
                            ab = ab.Remove(ab.IndexOf('<'));

                            //метим название города
                            User_City = ab;
                            ab = "";
                            break;
                        }
                    }

                    //ищем строку с должностью
                    for (int j = 0; j < str.Length; j++)
                    {
                        if (str[j].Contains(user_cfg.Login) && !str[j + 1].Contains("уровень"))
                        {
                            //пропускаем строку с шарами (днюха)
                            if (str[j + 1].Contains("st_builded.png")) j++;
                            ab = str[j + 1];
                            ab = ab.Substring(26);
                            ab = ab.Remove(ab.IndexOf('<'));

                            //метим название города
                            City_Role = "- " + ab;
                            ab = "";
                            break;
                        }
                    }
                    break;
                }
            }

            CreateHTMLPage(Properties.Resources.stat);
        }

        //список дел бота
        public void StartBot()
        {
            //подключаеся, идем на главную, раскрываем этажи
            ConnectAndGoHome();
            Thread.Sleep(rnd.Next(100, 300));

            if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                GetInfo(); //получаем инфу до прогона

            //открываем этажи
            TryToOpenFloor();

            //делаем 2 прогона (мб что-то доставят или купят випы)
            for (int i = 0; i < 2; i++)
            {
                //собираем награды
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetRevard();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }

                //шмонаем гостиницу
                FindWorkers();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetRevard();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }

                //собираем выручку
                CollectMoney();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetRevard();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }

                if (!Convert.ToBoolean(user_cfg.DoNotPut))
                {
                    //выкладываем товары
                    PutMerch();
                    if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                    {
                        GetRevard();
                        if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                            GetInfo();
                    }
                }

                //закупаем товары
                Buy();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetRevard();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }

                //катаем лифт (если не запрещено)
                if (!Convert.ToBoolean(user_cfg.DoNotLift))
                {
                    GoneLift();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }

                //зовем народ
                if (Convert.ToBoolean(user_cfg.Invite))
                    InviteToCity();
            }
            Action_Count++; //считаем оба прогона за 1

            if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                GetInfo();      //получем инфу после прогона

            RelaxMan();
        }

        private void RelaxMan()
        {
            int min_time = 0, max_time = 0;

            //защита от дурака
            try
            {
                min_time = Convert.ToInt32(user_cfg.MinTime);
            }
            catch
            {
                ThreadAbort("ОШИБКА. Поле \"От:\" должно содержать значение в диапазоне от 1 до 200.\n");
            }

            if (min_time < 1 || min_time > 200)
                ThreadAbort("ОШИБКА. Поле \"От:\" должно содержать значение в диапазоне от 1 до 200.\n");

            try
            {
                max_time = Convert.ToInt32(user_cfg.MaxTime);
            }
            catch
            {
                ThreadAbort("ОШИБКА. Поле \"До:\" должно содержать значение в диапазоне от 1 до 200.\n");
            }

            if (max_time < 1 || max_time > 200)
                ThreadAbort("ОШИБКА. Поле \"До:\" должно содержать значение в диапазоне от 1 до 200.\n");

            if (max_time < min_time)
                ThreadAbort("ОШИБКА. Значение поля \"От:\" не может быть меньше значения поля \"До:\".\n");

            //получаем рандомное время ожидания
            BotTimeSleep = TimeSpan.FromMilliseconds(rnd.Next(min_time * 60, max_time * 60) * 1000);
            //bot_timer.Interval = TimeSpan.FromMilliseconds((bot_timer.Interval.TotalMilliseconds * 0.001) * 1000);
            CONNECT_STATUS = " Стоп ";
            int timeleft = (int)(BotTimeSleep.TotalMilliseconds);
            int minutes = (int)TimeSpan.FromMilliseconds(timeleft).TotalMinutes;
            int seconds = (int)TimeSpan.FromMilliseconds(timeleft).TotalSeconds - minutes * 60;
            COMMUTATION_STR.Add("Жду  " + string.Format("{0}мин : {1:d2}сек\n", minutes, seconds));
        }

        //жмакнуть по ссылке
        private void ClickLink(string link, string param)
        {
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:54.0) Gecko/20100101 Firefox/54.0");
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //webClient.Headers[HttpRequestHeader.Host] = SERVER;
            webClient.Encoding = Encoding.UTF8;

            //пробуем кликнуть на ссылку
            try
            {
                HTML = webClient.UploadString(SERVER + link, param);
            }
            catch (Exception ex1)
            {
                //если ошибка сервера - пробуем подождать и снова кликнуть
                string s = ex1.Message;

                //если разорвано соединение - реконнект
                if (s.Contains("Базовое соединение закрыто"))
                {
                    try
                    {
                        Thread.Sleep(rnd.Next(500, 800));
                        ConnectAndGoHome();
                    }
                    catch (Exception ex2)
                    {
                        ThreadSleep("ОШИБКА. " + ex2.Message + "\n");
                    }
                }

                //иначе пробуем подождать и снова кликнуть
                else
                {
                    try
                    {
                        Thread.Sleep(rnd.Next(500, 800));
                        HTML = webClient.UploadString(SERVER + link, param);
                    }
                    catch (Exception ex2)
                    {
                        ThreadSleep("ОШИБКА. " + ex2.Message + "\n");
                    }
                }
            }


            //для отмороженных акков - пока не работает
            if (HTML.Contains("pumpit") || HTML.Contains("одноклассники"))
            {
                ThreadAbort("ОШИБКА. Аккаунты данного типа не поддерживаются.\n");
            }

            //если слишком быстро
            if (HTML.Contains("Вы попытались загрузить"))
            {
                Thread.Sleep(rnd.Next(100, 300));
                try
                {
                    Thread.Sleep(rnd.Next(500, 800));
                    HTML = webClient.UploadString(SERVER + link, param);
                }
                catch (Exception ex)
                {
                    ThreadSleep("ОШИБКА. " + ex.Message + "\n");
                }
            }
            else if (HTML.Contains("502 Bad Gateway"))
            {
                Thread.Sleep(rnd.Next(100, 300));
                GetHomePage();
            }

            Thread.Sleep(rnd.Next(100, 300));
        }

        //подключение к серверу
        private void Connect()
        {
            Entery();

            CONNECT_STATUS = " Попытка авторизции ";
            string param = string.Format("id5_hf_0=&login={0}&password={1}&%3Asubmit=%D0%92%D1%85%D0%BE%D0%B4", user_cfg.Login.Replace(' ', '+'), Crypto.DecryptStr(user_cfg.Pass));

            ClickLink(LINK, param);

            if (HTML.Contains("Поле 'Имя в игре' обязательно для ввода.") || HTML.Contains("Неверное имя или пароль") || !HTML.Contains(user_cfg.Login))
            {
                ThreadAbort("ОШИБКА. Неверный логин или пароль.\n");
            }
            CONNECT_STATUS = " Онлайн ";

            //фиксируем ссылку на Главную
            HOME_LINK = Parse(HTML, "/home");
            HOME_LINK = HOME_LINK.Remove(0, HOME_LINK.IndexOf('/') + 1);
            HOME_LINK = HOME_LINK.Remove(HOME_LINK.IndexOf('\"') - 1);

            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        //метод сброса потока бота
        private void ThreadAbort(string reason)
        {
            COMMUTATION_STR.Add(reason);
            CONNECT_STATUS = "";
            ACTION_STATUS = "";
            Thread.CurrentThread.Abort();
        }

        //метод торможения потока бота
        private void ThreadSleep(string reason)
        {
            COMMUTATION_STR.Add(reason);
            CONNECT_STATUS = "";
            ACTION_STATUS = "";
            RelaxMan();
            Thread.CurrentThread.Abort();
            //Bot.Start();
        }

        //входим на домашнюю станицу получаем ссылку на форму входа
        private void Entery()
        {
            CONNECT_STATUS = " Подключение к серверу ";

            ClickLink("login", "");
            Thread.Sleep(rnd.Next(100, 300));


            LINK = Parse(HTML, "<form action=");
            try
            {
                LINK = LINK.Substring(14, 107);
            }
            catch (Exception ex)
            {
                ThreadSleep("ОШИБКА. " + ex.Message + '\n');
            }
        }

        //парсит страницу, возвращает строку с ссылкой по маске
        private string Parse(string page, string mask)
        {
            string[] str = page.Split((char)'\n');
            string ab = "";
            foreach (string a in str)
            {
                if (a.Contains(mask))
                {
                    ab = a;
                    break;
                }
            }
            return ab;
        }

        //проверяем, надо ли гонять лифт. возвращает ссылку на лифт если надо или пустоту если нет
        private string TryLift()
        {
            string[] str = HTML.Split((char)'\n');
            string ab = "";
            foreach (string a in str)
            {
                if (a.Contains("/images/icons/tb_lift2.png")) //если пусто
                    break;
                else if (a.Contains("/images/icons/tb_lift.png") || a.Contains("/images/icons/tb_lift_vip.png") || a.Contains("/images/icons/tb_lift_ny.png") || a.Contains("/images/icons/tb_lift_vip_ny.png")) //если есть народ или ВИПы
                {
                    ab = a.Substring(21, 48);
                    break;
                }
            }
            return ab;
        }

        //катаем лифт и получаем очередную ссылку
        private string GetLiftLink(string lnk)
        {
            ClickLink(lnk, "");

            string ab = Parse(HTML, "Поднять");
            if (ab != "")
            {
                //ab = ab.Substring(111);
                ab = ab.Substring(88);
                if (ab.IndexOf('\"') != -1) ab = ab.Remove(ab.IndexOf('\"'));
            }
            else
            {
                ab = Parse(HTML, "Получить");
                if (ab != "")
                {
                    ab = ab.Substring(27);
                    if (ab.IndexOf('\"') != -1) ab = ab.Remove(ab.IndexOf('\"'));

                    //считаем пассажира
                    lift_count++;
                    Lift_Count++;

                    //если дал бакс - считаем
                    if (HTML.Contains("/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>1</span></b>"))
                    {
                        bucks_count++;
                        Bucks_Count++;
                    }
                }
            }
            return ab;
        }

        //основной метод получения награды (баксов)
        private void GetRevard()
        {
            GetHomePage();
            //получаем ссылку на квесты и кликаем если есть награда
            string ab = TryRevard();
            if (ab != "")
            {
                ACTION_STATUS = "Собираю награду";
                ClickLink(ab, "");
                Thread.Sleep(rnd.Next(100, 300));

                bucks_count = 0;

                //пока есть кнопки "Получить награду!" - жмакаем
                string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int i;
                for (i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("Получить награду!"))
                    {
                        //фиксируем строчку со ссылкой на получение
                        ab = str[i];

                        //если это не Коллекции
                        if (!HTML.Contains("Коллекции"))
                        {
                            //считаем награду
                            string stat = str[i - 3];
                            if (stat.Contains("Бонус X2!"))
                                stat = str[i - 6];
                            //stat = stat.Substring("<span><img src=\"http://static.nebo.mobi/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>".Length);
                            stat = stat.Substring("<span><img src=\"/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>".Length);
                            stat = stat.Remove(stat.IndexOf('<'));
                            bucks_count += Convert.ToInt32(stat);

                            //получаем ссылку, кликаем и ждем
                            ab = ab.Substring("<div><a class=\"btng btn60\" href=\"".Length);
                            ab = ab.Remove(ab.IndexOf('\"'));
                            ClickLink(ab, "");
                            Thread.Sleep(rnd.Next(100, 300));

                            Bucks_Count += bucks_count;
                            ACTION_STATUS = "Анализ ситуации";
                            COMMUTATION_STR.Add(string.Format("{0}  -  Собрано наградных баксов: {1}.", GetTime(), bucks_count));
                        }

                        //если это награда за коллекцию
                        else
                        {
                            //получаем ссылку, кликаем и ждем
                            ab = ab.Substring("<div><a class=\"btng btn60\" href=\"".Length);
                            ab = ab.Remove(ab.IndexOf('\"'));
                            ClickLink(ab, "");
                            Thread.Sleep(rnd.Next(100, 300));

                            //считаем награду
                            str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            string stat = "";
                            for(int j=0; j<str.Length; j++)
                                if(str[j].Contains("Награда"))
                                    stat = str[j+3];
                            //stat = stat.Substring("<span><img src=\"http://static.nebo.mobi/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>".Length);
                            stat = stat.Substring(81);
                            stat = stat.Remove(stat.IndexOf('<'));
                            bucks_count += Convert.ToInt32(stat);

                            Bucks_Count += bucks_count;
                            ACTION_STATUS = "Анализ ситуации";
                            COMMUTATION_STR.Add(string.Format("{0}  -  Получено баксов за коллекцию: {1}.", GetTime(), bucks_count));
                        }
                    }
                }                

                GetHomePage();
            }
        }

        //проверка- есть ли награда
        private string TryRevard()
        {
            string ab = Parse(HTML, "tb_quests.png");
            if (ab != "")
            {
                ab = ab.Substring(27);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //основной метод отправки лифта
        private void GoneLift()
        {
            GetHomePage();

            //проверяем, надо ли гнать лифт
            string ab = TryLift();

            lift_count = 0;
            bucks_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "Катаю лифт";

                while (ab != "")
                {
                    ab = GetLiftLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }
                //ACTION_STATUS = "";
                COMMUTATION_STR.Add(string.Format("{0}  -  Доставлено пассажиров: {1}.", GetTime(), lift_count));
                if (bucks_count != 0) COMMUTATION_STR.Add(string.Format("{0}  -  Собрано чаевых баксов: {1}.", GetTime(), bucks_count));
            }
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        //проверка есть ли выручка
        private string TryMoney()
        {
            string ab = Parse(HTML, "Собрать выручку!");
            if (ab != "")
            {
                //ab = ab.Substring(114);
                ab = ab.Substring(91);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetMoneyLink(string lnk)
        {
            ClickLink(lnk, "");

            string ab = Parse(HTML, "Собрать выручку!");
            if (ab != "")
            {
                //ab = ab.Substring(114);
                ab = ab.Substring(91);
                ab = ab.Remove(ab.IndexOf('\"'));
                coins_count++;
                Coins_Count++;
            }
            return ab;
        }

        //базовый меод сбора выручки
        private void CollectMoney()
        {
            //ищем ссылку сбора выручки
            GetHomePage();

            string ab = TryMoney();
            coins_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "Собираю выручку";
                coins_count = 1;
                Coins_Count++;
                while (ab != "")
                {
                    ab = GetMoneyLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                //ACTION_STATUS = "";
                COMMUTATION_STR.Add(string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.", GetTime(), coins_count));
            }
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        //проверяем есть ли чего выложить
        private string TryPutMerch()
        {
            string ab = Parse(HTML, "Выложить товар");
            if (ab != "")
            {
                //ab = ab.Substring(117);
                ab = ab.Substring(94);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetMerchLink(string lnk)
        {
            ClickLink(lnk, "");

            string ab = Parse(HTML, "Выложить товар");
            if (ab != "")
            {
                //ab = ab.Substring(117);
                ab = ab.Substring(94);
                ab = ab.Remove(ab.IndexOf('\"'));
                merch_count++;
                Merch_Count++;
            }
            return ab;
        }

        //базовый меод сбора выручки
        private void PutMerch()
        {
            GetHomePage();

            //ищем ссылку сбора выручки
            string ab = TryPutMerch();
            merch_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "Выкладываю товар";
                merch_count = 1;
                Merch_Count++;
                while (ab != "")
                {
                    ab = GetMerchLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                //ACTION_STATUS = "";
                COMMUTATION_STR.Add(string.Format("{0}  -  Этажей, на которых выложен товар: {1}.", GetTime(), merch_count));
            }
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }


        //проверяем есть ли чего закупить
        private string TryBuy()
        {
            string ab = "";
            GetHomePage();

            //простейший случай - постоянный товарооборот
            if (!Convert.ToBoolean(user_cfg.DoNotPut))
            {
                ab = Parse(HTML, "Закупить товар");
                if (ab != "")
                {
                    //ab = ab.Substring(120);
                    ab = ab.Substring(97);
                    ab = ab.Remove(ab.IndexOf('\"'));
                }
            }

            //а теперь если ждем инвесторов и ничего не выкладываем
            else
            {
                string[] str = HTML.Split((char)'\n');
                int i;
                for (i = 0; i < str.Length; i++)
                {
                    //если есть чего закупать и ничего не доставляется
                    if (str[i].Contains("st_empty.png") && !(str[i].Contains("st_stocking.png")))
                        break;
                }

                if (i != str.Length) //типа если нашлось
                {
                    //переходим к строке с ссылкой на этаж
                    i += 4;

                    //получаем строку с сылкой на этаж
                    ab = str[i];
                    ab = ab.Substring(9);
                    ab = ab.Remove(ab.IndexOf('\"'));
                }
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetBuyLink(string lnk)
        {
            ClickLink(lnk, "");

            string[] str = HTML.Split((char)'\n');
            string ab = "";
            foreach (string ss in str)
            {
                //вычленяем ссылку на самый дорогой
                if (ss.Contains("Закупить за"))
                {
                    ab = ss.Substring(21);
                    ab = ab.Remove(ab.IndexOf('\"'));
                }
            }

            //сама закупка
            Thread.Sleep(rnd.Next(100, 300));

            ClickLink(ab, "");

            ab = TryBuy();
            return ab;
        }

        //базовый меод закупки товара
        private void Buy()
        {
            GetHomePage();

            //ищем ссылку сбора выручки
            string ab = TryBuy();
            buy_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "Закупаю товар";
                while (ab != "")
                {
                    ab = GetBuyLink(ab);
                    buy_count++;
                    Buy_Count++;
                    Thread.Sleep(rnd.Next(100, 300));
                }

                //ACTION_STATUS = "";
                COMMUTATION_STR.Add(string.Format("{0}  -  Этажей, на которых закуплен товар: {1}.", GetTime(), buy_count));
            }
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        private string GetTime()
        {
            return string.Format(@"{0:d2}.{1:d2}.{2:d4}  [{3:d2}:{4:d2}:{5:d2}]",
                                 DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }


        //тупо идем на главную, а затем в Гостиницу
        private void GoHotel()
        {
            GetHomePage();

            //идем в Гостиницу
            string ab = "";
            string[] str = HTML.Split((char)'\n');
            for (int i = 0; i < str.Length; i++)
            {
                //получаем ссылку на Гостиницу
                if (str[i].Contains("Гостиница"))
                {
                    ab = str[i - 2];
                    ab = ab.Substring(23);
                    ab = ab.Remove(ab.IndexOf('\"'));
                    break;
                }
            }

            //идем в Гостиницу
            ClickLink(ab, "");
        }

        //увольняем жильцов ниже заданного уровня
        private void FindWorkers()
        {
            int i = 0;
            string ab = "";
            int bak_i = 0;

            killed_count = 0;
            new_worker_count = 0;
            ACTION_STATUS = "Шмонаю гостиницу";

            //идем в Гостиницу
            GoHotel();

            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //начинаем зачистку

            for (i = 0; i < str.Length; i++)
            {

                //анализ жильца
                if (str[i].Contains("\" class=\"white\""))
                {
                    //получаем ссылку на чувака
                    ab = str[i].Substring(17);
                    ab = ab.Remove(ab.IndexOf('\"'));


                    //если дармоед - выкинуть
                    if (!str[i + 7].Contains("(+)"))
                    {
                        int level_to_kill = 0;  //уровень, жильцов меньше которого выселять

                        //защита от дурака
                        try
                        {
                            level_to_kill = Convert.ToInt32(user_cfg.FireLess);
                        }
                        catch
                        {
                            ThreadAbort("ОШИБКА. Уровень должен быть от 1 до 9\n");
                        }

                        //если там число, но не в диапазоне 1:9
                        if (level_to_kill <= 1 || level_to_kill >= 10)
                            ThreadAbort("ОШИБКА. Уровень должен быть от 1 до 9\n");

                        //получаем уровень и сверяем с заданным
                        string rank = str[i + 2];
                        rank = rank.Substring(17);
                        rank = rank.Remove(1);
                        int human_level = Convert.ToInt32(rank);

                        //если уровень меньше заданного и стоит галочка выселения, или если он 9 и стоит галочка выселения, и есть (-)
                        if (human_level < level_to_kill && Convert.ToBoolean(user_cfg.Fire) || human_level == 9 && Convert.ToBoolean(user_cfg.Fire9) && str[i + 7].Contains("(-)"))
                        {
                            Kill(ab);
                            //а теперь результаты надо сбросить
                            str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            i = bak_i; //если остались (+)
                            Thread.Sleep(rnd.Next(100, 300));
                        }
                    }
                }
            }

            //иначе проверяем, не (+) ли он
            for (i = 0; i < str.Length; i++)
            {
                //анализ жильца
                if (str[i].Contains("\" class=\"white\""))
                {
                    //получаем ссылку на чувака
                    ab = str[i].Substring(17);
                    ab = ab.Remove(ab.IndexOf('\"'));

                    if (str[i + 7].Contains("(+)"))
                    {
                        bak_i = i + 7; //бекапим строку чтобы снова не начать с чуваком возиться

                        int free = 0; //количество свободных мест в гостинице
                        string ss = Parse(HTML, "Свободно: <b><span>");
                        //ss = ss.Substring(103);
                        ss = ss.Substring(80);
                        ss = ss.Remove(ss.IndexOf('<'));

                        try
                        {
                            free = Convert.ToInt32(ss);
                        }
                        catch (Exception ex)
                        {
                            ThreadSleep("ОШИБКА. " + ex.Message + '\n');
                        }

                        //пытаемся назначить на работу    
                        //если назначен, то парсим страницу сначала
                        if (GoToWork(ab, free))
                        {
                            i = 0;
                            bak_i = 0;
                            new_worker_count++;
                            New_Worker_Count++;
                        }
                        //иначе - со строки (+) - т.е. со следующего парня
                        else i = bak_i;

                        //а теперь разбиваем новую страницу на строки
                        str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        Thread.Sleep(rnd.Next(100, 300));
                    }
                }
            }
            if (killed_count != 0) COMMUTATION_STR.Add(string.Format("{0}  -  Выселено дармоедов: {1}.", GetTime(), killed_count));
            if (new_worker_count != 0) COMMUTATION_STR.Add(string.Format("{0}  -  Нанято новых рабочих: {1}.", GetTime(), new_worker_count));
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }


        //метод выселения
        private void Kill(string ab)
        {
            //входим в чувака
            ClickLink(ab, "");

            Thread.Sleep(rnd.Next(100, 300));

            //ищем кнопку "Выселить"
            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str.Length; i++)
            {
                //получаем ссылку "Выселить"
                if (str[i].Contains("Выселить"))
                {
                    ab = str[i];
                    ab = ab.Substring(22);
                    ab = ab.Remove(ab.IndexOf('\"'));

                    //пытаемся выкинуть нах
                    ClickLink(ab, "");

                    //HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    killed_count++;
                    Killed_Count++;

                    Thread.Sleep(rnd.Next(100, 300));
                    break;
                }
            }
        }

        //попытка устроить на работу
        //на входе ссылка и количество свободных мест 
        private bool GoToWork(string ab, int free)
        {
            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //входим в чувака
            ClickLink(ab, "");

            Thread.Sleep(rnd.Next(100, 300));

            ab = Parse(HTML, "Найти работу");
            ab = ab.Substring(22);
            ab = ab.Remove(ab.IndexOf('\"'));


            //жмакаем на "Найти работу"
            ClickLink(ab, "");

            Thread.Sleep(rnd.Next(100, 300));

            //а вдруг есть пустота
            if ((ab = Parse(HTML, "устроить на работу")) != "")
            {
                //ab = ab.Substring(115);
                ab = ab.Substring(92);
                ab = ab.Remove(ab.IndexOf('\"'));

                //жмакаем на "устроить на работу"
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));

                return true;
            }

            //иначе идем дальше только если ничего не доставляется
            else if ((HTML.Contains("/icons/st_sell.png") || HTML.Contains("/icons/st_sold.png") || HTML.Contains("/icons/st_stocked.png") || HTML.Contains("/icons/st_empty.png")) && !HTML.Contains("/icons/st_stocking.png") && free > 0)
            {
                //разбиваем страницу на строки
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                //получаем ссылку на этаж
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("/icons/sml_happy.png"))
                    {
                        ab = str[i - 3].Substring(9);
                        ab = ab.Remove(ab.IndexOf('\"'));

                        break;
                    }
                }

                //жмакаем на этаж
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));

                //разбиваем страницу на строки
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


                //получаем худшего работника
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("class=\"w3\""))
                    {
                        ab = str[i + 1].Substring(21);
                        ab = ab.Remove(ab.IndexOf('\"'));

                        break;
                    }
                }

                //жмакаем на худшего работника
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));


                //получаем ссылку "Уволить"
                ab = Parse(HTML, "Уволить");
                ab = ab.Substring(22);
                ab = ab.Remove(ab.IndexOf('\"'));

                //жмакаем на "Уволить"
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));


                //получаем ссылку на возвращение на этаж
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("Уволен из") || str[i].Contains("Уволена из"))
                    {
                        ab = str[i + 1];
                        break;
                    }
                }

                ab = ab.Substring(20);
                ab = ab.Remove(ab.IndexOf('\"'));


                //возвращаемся на этаж
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));


                //получаем ссылку "найти"
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("найти"))
                    {
                        ab = str[i - 1];
                        break;
                    }
                }
                ab = ab.Substring(21);
                ab = ab.Remove(ab.IndexOf('\"'));


                //нажимием на "найти"
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));


                //получаем ссылку "принять на работу"
                ab = Parse(HTML, "принять на работу");

                //ab = ab.Substring(115);
                ab = ab.Substring(92);
                ab = ab.Remove(ab.IndexOf('\"'));

                //и, наконец, нажимием на "принять на работу"
                ClickLink(ab, "");

                Thread.Sleep(rnd.Next(100, 300));

                //после всех манипуляций надо вернуться в Гостиницу
                GoHotel();

                return true;
            }

            //ну а если доставляется - надо идти по остальным
            return false;
        }


        //метод приглашения в город
        private void InviteToCity()
        {
            string[] str;
            int InvMinLvl = -1, InvMaxLvl = -1;

            if (Convert.ToBoolean(user_cfg.InviteFrom))
            {
                try
                {
                    InvMinLvl = Convert.ToInt32(user_cfg.InviteFromMeaning);
                }
                catch
                {
                    ThreadAbort("ОШИБКА. Минимальный уровень игрока должен быть от 10 до 75\n");
                }
            }
            else InvMinLvl = 10;

            if (Convert.ToBoolean(user_cfg.InviteTo))
            {
                try
                {
                    InvMaxLvl = Convert.ToInt32(user_cfg.InviteToMeaning);
                }
                catch
                {
                    ThreadAbort("ОШИБКА. Максимальный уровень игрока должен быть от 10 до 75\n");
                }
            }
            else InvMaxLvl = 75;

            invited_count = 0;

            //пока не будет приглашен хотя бы 1
            while (invited_count < 1)
            {
                GetHomePage();

                //ссылка в Город
                string ab = "";
                ab = Parse(HTML, "Мой город");
                if (ab != "")
                {
                    ab = ab.Substring(45);
                    ab = ab.Remove(ab.IndexOf('\"'));

                    //идем в Город
                    ClickLink(ab, "");

                    //ищем ссылку "поиск игроков"
                    ab = Parse(HTML, "поиск игроков");
                    if (ab != "")
                    {
                        ab = ab.Substring(44);
                        ab = ab.Remove(ab.IndexOf('\"'));

                        //идем к бомжам
                        ClickLink(ab, "");

                        ACTION_STATUS = "Зову народ";

                        str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < str.Length; i++)
                        {
                            if (str[i].Contains("(+)") && !str[i - 1].Contains("Мой город"))
                            {
                                //получаем уровень игрока
                                int Level;

                                if (str[i].Contains("star.png"))
                                    ab = str[i].Substring(83);
                                else ab = str[i - 1].Substring(83);
                                ab = ab.Remove(ab.IndexOf('<'));

                                Level = Convert.ToInt32(ab);

                                //если уровень входит в диапазон - пробуем войти
                                if (Level >= InvMinLvl && Level <= InvMaxLvl)
                                {
                                    if (str[i].Contains("star.png"))
                                        ab = str[i - 1].Substring(28);
                                    else ab = str[i - 2].Substring(28);

                                    ab = ab.Remove(ab.IndexOf('\"'));

                                    ClickLink(ab, "");

                                    //ищем строку  Пригласить в город (+)
                                    str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int j = 0; j < str.Length; j++)
                                    {
                                        if (str[j].Contains("Пригласить в город"))
                                        {
                                            ab = str[j].Substring(29);
                                            ab = ab.Remove(ab.IndexOf('\"'));

                                            ClickLink(ab, "");

                                            if (HTML.Contains("Приглашение отправлено!"))
                                            {
                                                invited_count++;
                                                Invited_Count++;
                                                break;
                                            }

                                            else
                                                break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }

                    //иначе выходим, чтобы бесконечно не зациклиться
                    else
                        return;
                }
            }
            if (invited_count != 0) COMMUTATION_STR.Add(string.Format("{0}  -  Отправлено приглашений: {1}.", GetTime(), invited_count));
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        //пробуем открыть этаж
        private void TryToOpenFloor()
        {
            GetHomePage();

            string ab = "";
            //проверяем - не надо ли открыть этаж
            opened_floor_count = 0;
            while ((ab = Parse(HTML, "Открыть этаж!")) != "")
            {
                ACTION_STATUS = "Открываю этажи";
                //ab = ab.Substring(117);
                ab = ab.Substring(94);
                ab = ab.Remove(ab.IndexOf("\""));

                ClickLink(ab, "");
                opened_floor_count++;
                Opened_Floor_Count++;
                Thread.Sleep(rnd.Next(100, 300));
            }
            if (opened_floor_count != 0) COMMUTATION_STR.Add(string.Format("{0}  -  Открыто этажей: {1}.", GetTime(), opened_floor_count));
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        //пробуем назначить на должность
        private void TryToAppoint()
        {
            string[] str;

            GetHomePage();

            //ссылка в Город
            string ab = "";
            ab = Parse(HTML, "Мой город");
            if (ab != "")
            {
                ab = ab.Substring(45);
                ab = ab.Remove(ab.IndexOf('\"'));

                //идем в Город
                ClickLink(ab, "");
            }
        }


    }
}
