using System;
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
        private int coins_count;                                //счетчик полученного бабла (чаевые, выручка...)
        //private int bucks_count;                                //счетчик полученных баксов (пока только чаевые)
        private int merch_count;                                //счетчик выложенных товаров (точнее этажей)

        private static string SERVER = "http://nebo.mobi/";     //адрес сервера
        private static string NAME = "Небоскребы. Бот";         //имя окна
        private string CONNECT_STATUS = "";                     //статус соединения
        private string ACTION_STATUS = "";                      //теекущее действие
        private WebRequest webReq;
        private WebResponse webResp;
        private Stream stream;
        private StreamReader sr;
        private Random rnd;
        private System.Windows.Forms.Timer upd_timer;           //таймер запуска прохода бота
        private string HTML;                                    //html-код текущей страницы
        private string LINK;                                    //переменная для обмена ссылками
        private string HOME_LINK;                               //ссылка на домашнюю страницу
        private string COMMUTATION_STR;                         //строка для логов
        private int timeleft;                                   //секунд до начала нового прохода
        //private int upd_synch;                                  //переменная для синхронизации

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

            lMaxTime.Location = new Point(lDiapazon.Location.X, lMinTime.Location.Y + lMinTime.Size.Height + (int)(0.02 * this.Size.Height));
            tbMaxTime.Location = new Point(tbMinTime.Location.X, lMaxTime.Location.Y - 2);
            tbMaxTime.Size = new Size((int)(0.05 * this.Size.Width), tbMinTime.Size.Height);

            bStart.Location = new Point(lLogin.Location.X, (int)(0.15 * this.Size.Height));
            bStop.Location = new Point(tbLogin.Location.X + tbLogin.Size.Width - bStop.Size.Width, bStart.Location.Y);
            bStop.Enabled = false;

            lLOG.Location = new Point(lLogin.Location.X, (int)(0.3 * this.Size.Height));

            LOGBox.Location = new Point(lLogin.Location.X, lLOG.Location.Y + lLOG.Size.Height + (int)(0.01 * this.Size.Height));
            LOGBox.Size = new Size(this.Size.Width - 3 * LOGBox.Location.X, (int)(0.5 * this.Size.Height));

            lCopyright.Text = "Exclusive by Mr.President  ©  2014." + "  ver. 1.2";
            lCopyright.Location = new Point(lLOG.Location.X + LOGBox.Size.Width - lCopyright.Size.Width, (int)(this.Size.Height * 0.88));
            
            tbPass.UseSystemPasswordChar = true;
            rnd = new Random();

            upd_timer = new System.Windows.Forms.Timer();
            upd_timer.Enabled = false;
            upd_timer.Tick += new System.EventHandler(this.upd_timer_Tick);

            ref_timer.Interval = 1000;
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

                webReq = WebRequest.Create(SERVER + ab);
                try
                {
                    webResp = webReq.GetResponse();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                stream = webResp.GetResponseStream();
                sr = new StreamReader(stream, Encoding.UTF8);
                HTML = sr.ReadToEnd();
            }
        }

        //тупо получение главного экрана
        private void GetHomePage()
        {
            webReq = WebRequest.Create(SERVER + HOME_LINK);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            HTML = sr.ReadToEnd();

            //получаем ссылку "Показать этажи"
            string ab = Parse(HTML, "Показать этажи");
            if (ab != "")
            {
                ab = ab.Substring(49);
                ab = ab.Remove(ab.IndexOf("\""));

                webReq = WebRequest.Create(SERVER + ab);
                try
                {
                    webResp = webReq.GetResponse();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                stream = webResp.GetResponseStream();
                sr = new StreamReader(stream, Encoding.UTF8);
                HTML = sr.ReadToEnd();
            }
        }
        
        //обновление содержания формы
        private void UpdForm()
        {
            this.Text = NAME + CONNECT_STATUS + ACTION_STATUS;
            if (COMMUTATION_STR != "")
            {
                LOGBox.Text += COMMUTATION_STR;
                COMMUTATION_STR = "";
            }
            if (this.Text.Contains("Стоп"))
            {
                upd_timer.Start();
                CONNECT_STATUS = "";
            }
            if (timeleft > 0)
            {
                CONNECT_STATUS = string.Format("   Жду   {0}мин : {1:d2}сек\n\n", timeleft / 60, timeleft - ((timeleft / 60) * 60));
                //if(upd_synch % 100 == 0) 
                timeleft--;
            }
            //upd_synch++;
        }


        //обработчик кнопки Старт
        private void bStart_Click(object sender, EventArgs e)
        {
            ref_timer.Enabled = true;
            bStart.Enabled = false;
            bStop.Enabled = true;
            Bot = new Thread(StartBot);
            ref_timer.Start();
            Bot.Start();
        }

        //список дел бота
        private void StartBot()
        {
            timeleft = 0;
            //upd_synch = 0;
            //подключаеся, идем на главную, раскрываем этажи
            GoHome();

            //делаем 2 прогона (после лифта мб что-то доставят или купят випы)
            CollectMoney();
            PutMerch();
            Buy();
            GoneLift();

            CollectMoney();
            PutMerch();
            Buy();
            GoneLift();

            
            //получаем рандомное время ожидания
            upd_timer.Interval = rnd.Next(Convert.ToInt32(tbMinTime.Text)*60000, Convert.ToInt32(tbMaxTime.Text)*60000);
            CONNECT_STATUS = "   -   Стоп";
            timeleft = upd_timer.Interval / 1000;
            COMMUTATION_STR = string.Format("Жду   {0}", string.Format("{0}мин : {1:d2}сек\n\n", timeleft / 60, timeleft - ((timeleft / 60) * 60)) );
        }

        //подключение к серверу
        private void Connect()
        {
            webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            Entery();
            GetLoginLink();

            CONNECT_STATUS = "  -  Попытка авторизции";
            string param = string.Format("id5_hf_0=&login={0}&password={1}&%3Asubmit=%D0%92%D1%85%D0%BE%D0%B4", tbLogin.Text.Replace(' ', '+'), tbPass.Text);

            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            HTML = webClient.UploadString(SERVER + LINK, param);
            CONNECT_STATUS = "  -  Онлайн";

            //фиксируем ссылку на Главную
            HOME_LINK = Parse(HTML, "/home");
            HOME_LINK = HOME_LINK.Remove(0, HOME_LINK.IndexOf('/') + 1);
            HOME_LINK = HOME_LINK.Remove(HOME_LINK.IndexOf('\"') - 1);

            Thread.Sleep(rnd.Next(1001, 2000));
        }
        
        //входим на домашнюю станицу получаем ссылку на форму входа
        private void Entery()
        {
            CONNECT_STATUS = "  -  Подключение к серверу";
            webReq = WebRequest.Create(SERVER);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            HTML = sr.ReadToEnd();

            LINK = Parse(HTML, ">Вход</a>");
            LINK = LINK.Substring(25, 49);
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
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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
            webReq = WebRequest.Create(SERVER + lnk);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            string s = sr.ReadToEnd();
            string ab = Parse(s, "Поднять");
            if (ab != "")
            {
                ab = ab.Substring(111);
                if (ab.IndexOf('\"') != -1) ab = ab.Remove(ab.IndexOf('\"'));
            }
            else
            {
                ab = Parse(s, "Получить");
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
        private void upd_timer_Tick(object sender, EventArgs e)
        {
            upd_timer.Stop();
            Bot = new Thread(StartBot);
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
                lift_count = 1; //один этаж-то уже собран)
                ACTION_STATUS = "   -   Катаю лифт";

                while (ab != "")
                {
                    ab = GetLiftLink(ab);
                    Thread.Sleep(rnd.Next(351, 1500));
                }
                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Доставлено пассажиров: {1}.\n", GetTime(), lift_count);
            }
            //COMMUTATION_STR = string.Format("{0}  -  Доставлено пассажиров: {1}.\n", GetTime(), lift_count);
            Thread.Sleep(rnd.Next(30, 100));
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
            webReq = WebRequest.Create(SERVER + lnk);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            string s = sr.ReadToEnd();
            string ab = Parse(s, "Собрать выручку!");
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
            string ab = TryMoney();
            coins_count = 0;
            if (ab != "")
            {
                ACTION_STATUS = "   -   Собираю выручку";
                coins_count = 1;
                while (ab != "")
                {
                    ab = GetMoneyLink(ab);
                    Thread.Sleep(rnd.Next(351, 1500));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.\n", GetTime(), coins_count);
            }
            //COMMUTATION_STR = string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.\n", GetTime(), coins_count);
            Thread.Sleep(rnd.Next(30, 100));
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
            webReq = WebRequest.Create(SERVER + lnk);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            string s = sr.ReadToEnd();
            string ab = Parse(s, "Выложить товар");
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
                    Thread.Sleep(rnd.Next(351, 1500));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых выложен товар: {1}.\n", GetTime(), merch_count);
            }
            //COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых выложен товар: {1}.\n", GetTime(), merch_count);
            Thread.Sleep(rnd.Next(30, 100));
        }


        //проверяем есть ли чего закупить
        private string TryBuy()
        {
            GetHomePage();
            string ab = Parse(HTML, "Закупить товар");
            if (ab != "")
            {
                ab = ab.Substring(120);
                ab = ab.Remove(ab.IndexOf('\"'));
            }
            return ab;
        }

        //переход поссылке сбора выручки и получения новой ссылки сбора выручки
        private string GetBuyLink(string lnk)
        {
            webReq = WebRequest.Create(SERVER + lnk);
            try
            {
                webResp = webReq.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            string s = sr.ReadToEnd();

            string[] str = s.Split((char)'\n');
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
            webReq = WebRequest.Create(SERVER + ab);
            Thread.Sleep(rnd.Next(300, 1000));
            webResp = webReq.GetResponse();
            stream = webResp.GetResponseStream();
            sr = new StreamReader(stream, Encoding.UTF8);
            s = sr.ReadToEnd();

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
                    Thread.Sleep(rnd.Next(351, 1500));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых закуплен товар: {1}.\n", GetTime(), buy_count);
            }
            //COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых закуплен товар: {1}.\n", GetTime(), buy_count);
            Thread.Sleep(rnd.Next(30, 100));
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
            upd_timer.Stop();
            ref_timer.Stop();
            bStart.Enabled = true;
            bStop.Enabled = false;
            timeleft = 0; 
            UpdForm();
        }

        //для статусов
        private void ref_timer_Tick(object sender, EventArgs e)
        {
            UpdForm();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Bot.Abort();
            }
            catch { }
        }
    }
}
