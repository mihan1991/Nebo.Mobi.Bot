﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;

namespace Nebo.Mobi.Bot
{
    public partial class Form1 : Form
    {
        private int lift_count;                                 //счетчик перевезенных в лифте
        private int buy_count;                                  //счетчик купленных товаров
        private int coins_count;                                //счетчик собранных выручек (точнее этажей)
        //private int bucks_count;                                //счетчик полученных баксов (пока только чаевые)
        private int merch_count;                                //счетчик выложенных товаров (точнее этажей)
        private int killed_count;                               //счетчик выселенных 

        private static string SERVER = "http://nebo.mobi/";     //адрес сервера
        private static string NAME = "Небоскребы. Бот";         //имя окна
        private string CONNECT_STATUS = "";                     //статус соединения
        private string ACTION_STATUS = "";                      //теекущее действие
        private WebRequest webReq;
        private WebResponse webResp;
        private Stream stream;
        private StreamReader sr;
        private Random rnd;
        private System.Windows.Forms.Timer bot_timer;           //таймер запуска прохода бота
        private string HTML;                                    //html-код текущей страницы
        private string LINK;                                    //переменная для обмена ссылками
        private string HOME_LINK;                               //ссылка на домашнюю страницу
        private string COMMUTATION_STR;                         //строка для логов
        private int timeleft;                                   //секунд до начала нового прохода

        private Thread Bot;                                     //переменная потока бота

        private WebClient webClient;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size((int)(0.5 * Screen.PrimaryScreen.Bounds.Width), (int)(0.5 * Screen.PrimaryScreen.Bounds.Height));

            lUserInfo.Location = new Point((int)(0.02 * this.Size.Width), (int)(0.02 * this.Size.Height));
            lDiapazon.Location = new Point(lUserInfo.Location.X + lUserInfo.Size.Width + (int)(0.3 * this.Size.Height), lUserInfo.Location.Y);
            
            lLogin.Location = new Point(lUserInfo.Location.X, lUserInfo.Location.Y + lUserInfo.Size.Height + (int)(0.01 * this.Size.Height));
            tbLogin.Location = new Point(lLogin.Location.X + lLogin.Size.Width + (int)(0.005 * this.Size.Width), lLogin.Location.Y - 2);
            tbLogin.Size = new Size((int)(0.2 * this.Size.Width), tbLogin.Size.Height);

            lPass.Location = new Point(lLogin.Location.X, lLogin.Location.Y + lLogin.Size.Height + (int)(0.02 * this.Size.Height));
            tbPass.Location = new Point(tbLogin.Location.X, lPass.Location.Y - 2);
            tbPass.Size = new Size((int)(0.2 * this.Size.Width), tbPass.Size.Height);

            lMinTime.Location = new Point(lDiapazon.Location.X, lLogin.Location.Y);
            tbMinTime.Location = new Point(lMinTime.Location.X + lMinTime.Size.Width + (int)(0.005 * this.Size.Width), lMinTime.Location.Y - 2);
            tbMinTime.Size = new Size((int)(0.05 * this.Size.Width), tbMinTime.Size.Height);

            cbDoNotPut.Location = new Point(lDiapazon.Location.X + lDiapazon.Size.Width + (int)(0.01 * this.Size.Width), tbMinTime.Location.Y);
            cbFire.Location = new Point(cbDoNotPut.Location.X, cbDoNotPut.Location.Y + cbDoNotPut.Size.Height);
            tbRank.Location = new Point(cbFire.Location.X + cbFire.Size.Width + (int)(0.01 * cbFire.Size.Width), cbFire.Location.Y-2);

            lMaxTime.Location = new Point(lDiapazon.Location.X, lMinTime.Location.Y + lMinTime.Size.Height + (int)(0.02 * this.Size.Height));
            tbMaxTime.Location = new Point(tbMinTime.Location.X, lMaxTime.Location.Y - 2);
            tbMaxTime.Size = new Size((int)(0.05 * this.Size.Width), tbMinTime.Size.Height);

            bStart.Location = new Point(lLogin.Location.X, (int)(0.15 * this.Size.Height));
            bStop.Location = new Point(tbLogin.Location.X + tbLogin.Size.Width - bStop.Size.Width, bStart.Location.Y);
            bStop.Enabled = false;

            lLOG.Location = new Point(lLogin.Location.X, (int)(0.3 * this.Size.Height));

            LOGBox.Location = new Point(lLogin.Location.X, lLOG.Location.Y + lLOG.Size.Height + (int)(0.01 * this.Size.Height));
            LOGBox.Size = new Size(this.Size.Width - 3 * LOGBox.Location.X, (int)(0.5 * this.Size.Height));

            lCopyright.Text = "Exclusive by Mr.President  ©  2014." + "  ver. 1.45";
            lCopyright.Location = new Point(lLOG.Location.X + LOGBox.Size.Width - lCopyright.Size.Width, (int)(this.Size.Height * 0.88));

            webClient = new WebClient();

            tbPass.UseSystemPasswordChar = true;
            rnd = new Random();

            bot_timer = new System.Windows.Forms.Timer();
            bot_timer.Tick += new System.EventHandler(this.bot_timer_Tick);

            ref_timer.Interval = 1000;
            bot_timer.Enabled = false;
            ref_timer.Enabled = false;

            COMMUTATION_STR = "";
        }

        //получение полного списка этажей
        private void GoHome()
        {
            //подключаемся и переходим на Главную
            Connect();

            //получаем ссылку "Показать этажи"
            string ab = Parse(HTML, "Показать этажи");
            if (ab != "")
            {
                ab = ab.Substring(49);
                ab = ab.Remove(ab.IndexOf("\""));

                try
                {
                    ClickLink(ab, "");
                }
                catch (Exception ex)
                {
                    ThreadAbort("ОШИБКА. " + ex.Message + '\n');
                }
            }
        }

        //тупо получение главного экрана
        private void GetHomePage()
        {
            try
            {
                ClickLink(HOME_LINK, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            //получаем ссылку "Показать этажи"
            string ab = Parse(HTML, "Показать этажи");
            if (ab != "")
            {
                ab = ab.Substring(49);
                ab = ab.Remove(ab.IndexOf("\""));

                try
                {
                    ClickLink(ab, "");
                }
                catch (Exception ex)
                {
                    ThreadAbort("ОШИБКА. " + ex.Message + '\n');
                }
            }
        }

        //обновление содержания формы
        private void UpdForm()
        {
            if (!Bot.IsAlive)
            {
                bStart.Enabled = true;
                bStop.Enabled = false;
            }
            this.Text = NAME + CONNECT_STATUS + ACTION_STATUS;
            if (COMMUTATION_STR != "")
            {
                LOGBox.Text += COMMUTATION_STR;
                LOGBox.SelectionStart = LOGBox.TextLength;
                LOGBox.ScrollToCaret();
                COMMUTATION_STR = "";
            }
            if (this.Text.Contains("Стоп"))
            {
                bot_timer.Start();
                CONNECT_STATUS = "";
            }
            
            if (timeleft > 0)
            {
                CONNECT_STATUS = string.Format("   Жду   {0}мин : {1:d2}сек\n\n", (int)(timeleft / 60), timeleft - ((int)(timeleft / 60) * 60));
                timeleft--;
            }
             
        }


        //обработчик кнопки Старт
        private void bStart_Click(object sender, EventArgs e)
        {
            bStart.Enabled = false;
            bStop.Enabled = true;
            Bot = new Thread(StartBot);
            ref_timer.Start();
            Bot.Start();
        }

        //список дел бота
        private void StartBot()
        {
            //сбрасываем таймер обратного отсчета
            timeleft = 0;
            //подключаеся, идем на главную, раскрываем этажи
            GoHome();

            //делаем 2 прогона (мб что-то доставят или купят випы)
            for (int i = 0; i < 2; i++)
            {
                if(cbFire.Checked) Fire();
                CollectMoney();
                if(!cbDoNotPut.Checked) PutMerch();
                Buy();
                GoneLift();
            }
            
            //получаем рандомное время ожидания
            bot_timer.Interval = rnd.Next(Convert.ToInt32(tbMinTime.Text)*60000, Convert.ToInt32(tbMaxTime.Text)*60000);
            bot_timer.Interval = (int)(bot_timer.Interval * 0.001) * 1000;
            CONNECT_STATUS = "   -   Стоп";
            timeleft = (int)(bot_timer.Interval * 0.001);
            COMMUTATION_STR = string.Format("Жду   {0}", string.Format("{0}мин : {1:d2}сек\n\n", (int)(timeleft / 60), timeleft - ((int)(timeleft / 60) * 60)) );
        }

        //жмакнуть по ссылке
        private void ClickLink(string link, string param)
        {
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0");
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Encoding = Encoding.UTF8;
            HTML = webClient.UploadString(SERVER + link, param);
        }

        //подключение к серверу
        private void Connect()
        {
            Entery();

            CONNECT_STATUS = "  -  Попытка авторизции";
            string param = string.Format("id5_hf_0=&login={0}&password={1}&%3Asubmit=%D0%92%D1%85%D0%BE%D0%B4", tbLogin.Text.Replace(' ', '+'), tbPass.Text);

            try
            {
                ClickLink(LINK, param);
            }
            catch(Exception ex)
            {
                ThreadAbort("ОШИБКА" + ex.Message + '\n');
            }

            if (HTML.Contains("Поле 'Имя в игре' обязательно для ввода.") || HTML.Contains("Неверное имя или пароль"))
            {
                ThreadAbort("ОШИБКА. Неверный логин или пароль.\n");
            }
            CONNECT_STATUS = "  -  Онлайн";

            //фиксируем ссылку на Главную
            HOME_LINK = Parse(HTML, "/home");
            HOME_LINK = HOME_LINK.Remove(0, HOME_LINK.IndexOf('/') + 1);
            HOME_LINK = HOME_LINK.Remove(HOME_LINK.IndexOf('\"') - 1);

            Thread.Sleep(rnd.Next(1001, 2000));
        }

        //метод сброса потока бота
        private void ThreadAbort(string reason)
        {
            COMMUTATION_STR = reason;
            CONNECT_STATUS = "";
            ACTION_STATUS = "";
            Thread.CurrentThread.Abort();
        }
        
        //входим на домашнюю станицу получаем ссылку на форму входа
        private void Entery()
        {
            CONNECT_STATUS = "  -  Подключение к серверу";
            try
            {
                ClickLink("login","");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. "+ ex.Message +'\n');
            }

            LINK = Parse(HTML, "<form action=");
            try
            {
                LINK = LINK.Substring(14, 107);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void GetLoginLink()
        {
            webReq = WebRequest.Create(SERVER + LINK);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            string HTML = sr.ReadToEnd();
            LINK = Parse(HTML, "<form action=");
            try
            {
                LINK = LINK.Substring(14, 107);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //получили ссылку "Вход"            
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
                if (a.Contains("http://static.nebo.mobi/images/icons/tb_lift2.png")) //если пусто
                    break;
                else if (a.Contains("http://static.nebo.mobi/images/icons/tb_lift.png") || a.Contains("http://static.nebo.mobi/images/icons/tb_lift_vip.png")) //если есть народ или ВИПы
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
            try
            {
                ClickLink(lnk, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            string ab = Parse(HTML, "Поднять");
            if (ab != "")
            {
                ab = ab.Substring(111);
                if (ab.IndexOf('\"') != -1) ab = ab.Remove(ab.IndexOf('\"'));
            }
            else
            {
                ab = Parse(HTML, "Получить");
                if (ab != "")
                {
                    ab = ab.Substring(27);
                    if(ab.IndexOf('\"') != -1) ab = ab.Remove(ab.IndexOf('\"'));
                    lift_count++;
                }
            }
            return ab;
        }


        //событие таймера
        private void bot_timer_Tick(object sender, EventArgs e)
        {
            bot_timer.Stop();
            bStart.Enabled = false;
            bStop.Enabled = true;
            Bot = new Thread(StartBot);
            ref_timer.Start();
            Bot.Start();
        }


        //основной метод отправки лифта
        private void GoneLift()
        {
            //проверяем, надо ли гнать лифт
            string ab = TryLift();

            lift_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "   -   Катаю лифт";

                while (ab != "")
                {
                    ab = GetLiftLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }
                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Доставлено пассажиров: {1}.\n", GetTime(), lift_count);
            }
            Thread.Sleep(rnd.Next(1000, 1500));
        }

        //проверка есть ли выручка
        private string TryMoney()
        {
            string ab = Parse(HTML, "Собрать выручку!");
            if (ab != "")
            {
                ab = ab.Substring(114);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetMoneyLink(string lnk)
        {
            try
            {
                ClickLink(lnk, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            string ab = Parse(HTML, "Собрать выручку!");
            if (ab != "")
            {
                ab = ab.Substring(114);
                ab = ab.Remove(ab.IndexOf('\"'));
                coins_count++;
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
                ACTION_STATUS = "   -   Собираю выручку";
                coins_count = 1;
                while (ab != "")
                {
                    ab = GetMoneyLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.\n", GetTime(), coins_count);
            }
            Thread.Sleep(rnd.Next(1000, 1500));
        }

        //проверяем есть ли чего выложить
        private string TryPutMerch()
        {
            string ab = Parse(HTML, "Выложить товар");
            if (ab != "")
            {
                ab = ab.Substring(117);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetMerchLink(string lnk)
        {
            try
            {
                ClickLink(lnk, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            string ab = Parse(HTML, "Выложить товар");
            if (ab != "")
            {
                ab = ab.Substring(117);
                ab = ab.Remove(ab.IndexOf('\"'));
                merch_count++;
            }
            return ab;
        }

        //базовый меод сбора выручки
        private void PutMerch()
        {
            //ищем ссылку сбора выручки
            string ab = TryPutMerch();
            merch_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "   -   Выкладываю товар";
                merch_count = 1;
                while (ab != "")
                {
                    ab = GetMerchLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых выложен товар: {1}.\n", GetTime(), merch_count);
            }
            Thread.Sleep(rnd.Next(1000, 1500));
        }


        //проверяем есть ли чего закупить
        private string TryBuy()
        {
            string ab="";
            GetHomePage();

            //простейший случай - постоянный товарооборот
            if (!cbDoNotPut.Checked)
            {
                ab = Parse(HTML, "Закупить товар");
                if (ab != "")
                {
                    ab = ab.Substring(120);
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
            try
            {
                ClickLink(lnk, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

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
            try
            {
                ClickLink(ab, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            ab = TryBuy(); 
            return ab;
        }

        //базовый меод закупки товара
        private void Buy()
        {
            //ищем ссылку сбора выручки
            string ab = TryBuy();
            buy_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "   -   Закупаю товар";
                while (ab != "")
                {
                    ab = GetBuyLink(ab);
                    buy_count++;
                    Thread.Sleep(rnd.Next(100, 300));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых закуплен товар: {1}.\n", GetTime(), buy_count);
            }
            Thread.Sleep(rnd.Next(1000, 1500));
        }

        private string GetTime()
        {
            return string.Format(@"{0:d2}.{1:d2}.{2:d4}  [{3:d2}:{4:d2}:{5:d2}]", 
                                 DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        //остановка бота и очистка синхрострок
        private void bStop_Click(object sender, EventArgs e)
        {
            Bot.Abort();
            CONNECT_STATUS = "";
            ACTION_STATUS = "";
            bot_timer.Stop();
            ref_timer.Stop();
            bStart.Enabled = true;
            bStop.Enabled = false;
            timeleft = 0; 
            UpdForm();
        }

        //обработчик таймера обновления формы и статусов
        private void ref_timer_Tick(object sender, EventArgs e)
        {
            UpdForm();
        }

        //тупо идем на главную, а затем в Гостиницу
        private void GoHotel()
        {
            GetHomePage();

            //идем в Гостиницу
            string ab="";
            string[] str = HTML.Split((char)'\n');
            for (int i = 0; i < str.Length; i++)
            {
                //получаем ссылку на Гостиницу
                if (str[i].Contains("Гостиница"))
                {
                    ab = str[i - 2];
                    ab = ab.Substring(23);
                    ab = ab.Remove(ab.IndexOf('\"'));
                }
            }

            //идем в Гостиницу
            try
            {
                ClickLink(ab, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }
        }

        //увольняем жильцов ниже заданного уровня
        private void Fire()
        {
            int level = Convert.ToInt32(tbRank.Text); //уровень, жильцов меньше которого выселять
            int i = 0;
            string ab = "";
            int bak_i=0;

            killed_count = 0;
            ACTION_STATUS = "   -   Шмонаю гостиницу";

            //идем в Гостиницу
            GoHotel();
                     

            /*
            foreach (string s in str) COMMUTATION_STR += s+'\n';
            Thread.Sleep(1000);
            ThreadAbort("ОТЛАДКА.\n");
            */

            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            //начинаем зачистку

            for(i=0; i<str.Length; i++)
            {

                //анализ жильца
                if (str[i].Contains("\" class=\"white\""))
                {
                    //получаем ссылку на чувака
                    ab = str[i].Substring(17);
                    ab = ab.Remove(ab.IndexOf('\"'));

                    //проверяем, не лучше ли этот житель уже работающих
                    if (str[i + 7].Contains("(+)"))
                    {
                        /*
                        bak_i = i + 7; //бекапим строку чтобы снова не начать с чуваом возиться

                        //пытаемся назначить на работу    
                        //если назначен, то парсим страницу сначала
                        if (GoToWork(ab)) i = 0;
                        //иначе - со строки (+) - т.е. со следующего парня
                        else i = bak_i;

                        //а теперь результаты надо сбросить
                        str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        Thread.Sleep(rnd.Next(100, 300));
                        */

                        //заглушка
                        bak_i = i + 7;
                        i += 7;
                        str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        Thread.Sleep(rnd.Next(100, 300));
                    }

                    //если сраный дармоед ниже заданного уровня - выкинуть
                    else
                    {
                        //получаем уровень и сверяем с заданным
                        string rank = str[i + 2];
                        rank = rank.Substring(17);
                        rank = rank.Remove(1);
                        int l = Convert.ToInt32(rank);

                        //если уровень меньше заданного
                        if (l < level)
                        {
                            Kill(ab);
                            //а теперь результаты надо сбросить
                            str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            i = bak_i;
                            Thread.Sleep(rnd.Next(100, 300));
                        }
                    }
                }                
            }
            ACTION_STATUS = "";
            if(killed_count!=0) COMMUTATION_STR = string.Format("{0}  -  Выселено дармоедов: {1}.\n", GetTime(), killed_count);
            Thread.Sleep(rnd.Next(1000, 1500));
        }

        private void Kill(string ab)
        {
            //входим в чувака
            try
            {
                ClickLink(ab, "");
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }
            Thread.Sleep(rnd.Next(100, 500));

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
                    try
                    {
                        ClickLink(ab, "");
                        HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        killed_count++;
                    }
                    catch (Exception ex)
                    {
                        ThreadAbort("ОШИБКА. " + ex.Message + '\n');
                    }
                    Thread.Sleep(rnd.Next(100, 500));
                    break;
                }
            }
        }

        //попытка устроить на работу
        private bool GoToWork(string ab)
        {
            string[] str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //входим в чувака
            try
            {
                ClickLink(ab, "");
                
            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            ab = Parse(HTML, "Найти работу");
            ab = ab.Substring(22);
            ab = ab.Remove(ab.IndexOf('\"'));
            Thread.Sleep(rnd.Next(100, 300));

            //жмакаем на "Найти работу"
            try
            {
                ClickLink(ab, "");

            }
            catch (Exception ex)
            {
                ThreadAbort("ОШИБКА. " + ex.Message + '\n');
            }

            
            //идем дальше только если ничего не доставляется
            if (HTML.Contains("/icons/st_sell.png") || HTML.Contains("/icons/st_sold.png") || HTML.Contains("/icons/st_stocked.png") || HTML.Contains("/icons/st_empty.png") && !HTML.Contains("/icons/st_stocking.png"))
            {
                //разбиваем страницу на строки
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                //получаем ссылку на этаж
                for (int i = 0; i < str.Length; i++)
                {

                    ab = str[i - 3].Substring(23);
                    ab = ab.Remove(ab.IndexOf('\"'));
                    /*COMMUTATION_STR = ab + '\n';
                    Thread.Sleep(1000);
                    ThreadAbort("ОТЛАДКА.\n");*/

                    //жмакаем на этаж
                    try
                    {
                        ClickLink(ab, "");

                    }
                    catch (Exception ex)
                    {
                        ThreadAbort("ОШИБКА. " + ex.Message + '\n');
                    }

                    str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in str) COMMUTATION_STR += s + '\n';
                    Thread.Sleep(1000);
                    ThreadAbort("ОТЛАДКА.\n");
                    return true;
                }
            }

            //ну а если доставляется - надо идти по остальным
            foreach (string s in str) COMMUTATION_STR += s +'\n';
            Thread.Sleep(1000);
            ThreadAbort("ОТЛАДКА.\n");
            return false;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Bot.Abort();
            }
            catch { }
        }

        private void cbFire_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFire.Checked) tbRank.Enabled = true;
            else tbRank.Enabled = false;
        }
    }
}
