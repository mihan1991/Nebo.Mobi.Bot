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
        private string version = "1.9";                         //версия бота

        //блок разовой статистики
        private int lift_count;                                 //счетчик перевезенных в лифте
        private int buy_count;                                  //счетчик купленных товаров
        private int coins_count;                                //счетчик собранных выручек (точнее этажей)
        private int bucks_count;                                //счетчик полученных баксов (пока только чаевые)
        private int merch_count;                                //счетчик выложенных товаров (точнее этажей)
        private int killed_count;                               //счетчик выселенных 
        private int new_worker_count;                           //счетчик новых нанятых
        
        //блок данных пользователя
        private string User_Level;
        private string User_Bucks;
        private string User_Coins;

        //блок общей статистики работы
        private int Lift_Count;
        private int Bucks_Count;        
        private int Buy_Count;
        private int Coins_Count;
        private int Merch_Count;
        private int Killed_Count;
        private int New_Worker_Count;
        private int Action_Count;

        private static string SERVER = "http://nebo.mobi/";     //адрес сервера
        private static string NAME = "Небоскребы. Бот";         //имя окна
        private string CONNECT_STATUS = "";                     //статус соединения
        private string ACTION_STATUS = "";                      //теекущее действие
        //private WebRequest webReq;
        //private WebResponse webResp;
        //private Stream stream;
        //private StreamReader sr;
        private Random rnd;
        private System.Windows.Forms.Timer bot_timer;           //таймер запуска прохода бота
        private string HTML;                                    //html-код текущей страницы
        private string LINK;                                    //переменная для обмена ссылками
        private string HOME_LINK;                               //ссылка на домашнюю страницу
        private string COMMUTATION_STR;                         //строка для логов
        private int timeleft;                                   //секунд до начала нового прохода

        private Thread Bot;                                     //переменная потока бота

        private Config cfg;                                     //объект класса настроек
        private bool PassChanged;                               //проверка на изменение пароля (нужно для шифрования)
        private WebClient webClient;

        private string Current_Path;                            //строка, содержащая папку с ботом
        private bool AutorunChanged;                            //для фиксации изменения состояния автозагрузки

        public Form1()
        {
            InitializeComponent();

            AutorunChanged = false;

            //получаем директорию файла
            Current_Path = new FileInfo(Application.ExecutablePath).Name;
            int pos = Application.ExecutablePath.IndexOf(Current_Path);
            Current_Path = Application.ExecutablePath.Remove(pos, Current_Path.Length);

            //подгружаем настройки
            cfg = new Config(Current_Path);
            LoadConfig();
            bSave.BackColor = Color.LightGreen;

            //отрисовываем форму
            this.Size = new Size((int)(0.5 * Screen.PrimaryScreen.Bounds.Width), (int)(0.5 * Screen.PrimaryScreen.Bounds.Height));

            lUserInfo.Location = new Point((int)(0.02 * this.Size.Width), (int)(0.02 * this.Size.Height));
                        
            lLogin.Location = new Point(lUserInfo.Location.X, lUserInfo.Location.Y + lUserInfo.Size.Height + (int)(0.01 * this.Size.Height));
            tbLogin.Location = new Point(lLogin.Location.X + lLogin.Size.Width + (int)(0.005 * this.Size.Width), lLogin.Location.Y - 2);
            tbLogin.Size = new Size((int)(0.22 * this.Size.Width), tbLogin.Size.Height);

            lPass.Location = new Point(lLogin.Location.X, lLogin.Location.Y + lLogin.Size.Height + (int)(0.02 * this.Size.Height));
            tbPass.Location = new Point(tbLogin.Location.X, lPass.Location.Y - 2);
            tbPass.Size = new Size((int)(0.22 * this.Size.Width), tbPass.Size.Height);

            lDiapazon.Location = new Point(tbPass.Location.X + tbPass.Size.Width + (int)(0.01 * this.Size.Height), lUserInfo.Location.Y);
            lMinTime.Location = new Point(lDiapazon.Location.X, lLogin.Location.Y);
            tbMinTime.Location = new Point(lMinTime.Location.X + lMinTime.Size.Width + (int)(0.005 * this.Size.Width), lMinTime.Location.Y - 2);
            tbMinTime.Size = new Size((int)(0.05 * this.Size.Width), tbMinTime.Size.Height);

            cbDoNotPut.Location = new Point(lDiapazon.Location.X + lDiapazon.Size.Width + (int)(0.01 * this.Size.Width), tbMinTime.Location.Y);
            cbDoNotGetRevard.Location = new Point(cbDoNotPut.Location.X + cbDoNotPut.Size.Width + (int)(0.01 * this.Size.Width), cbDoNotPut.Location.Y);
            cbFire.Location = new Point(cbDoNotPut.Location.X, cbDoNotPut.Location.Y + cbDoNotPut.Size.Height);
            cbFire9.Location = new Point(cbFire.Location.X, cbFire.Location.Y + cbFire.Size.Height);
            cbAutorun.Location = new Point(cbFire9.Location.X, cbFire9.Location.Y + cbFire9.Size.Height);
            cbHide.Location = new Point(cbDoNotGetRevard.Location.X, cbAutorun.Location.Y);
            tbFireLess.Location = new Point(cbFire.Location.X + cbFire.Size.Width + (int)(0.01 * cbFire.Size.Width), cbFire.Location.Y-2);

            lMaxTime.Location = new Point(lDiapazon.Location.X, lMinTime.Location.Y + lMinTime.Size.Height + (int)(0.02 * this.Size.Height));
            tbMaxTime.Location = new Point(tbMinTime.Location.X, lMaxTime.Location.Y - 2);
            tbMaxTime.Size = new Size((int)(0.05 * this.Size.Width), tbMinTime.Size.Height);

            cbDoNotSaveThePass.Location = new Point(tbPass.Location.X, tbPass.Location.Y + tbPass.Size.Height + (int)(0.01 * this.Size.Height));

            bStart.Location = new Point(lLogin.Location.X, cbDoNotSaveThePass.Location.Y + cbDoNotSaveThePass.Size.Height + (int)(0.02 * this.Size.Height));
            bStop.Location = new Point(tbLogin.Location.X + tbLogin.Size.Width - bStop.Size.Width, bStart.Location.Y);
            bSave.Location = new Point(cbFire9.Location.X, bStart.Location.Y);
            bStop.Enabled = false;

            lLOG.Location = new Point(lLogin.Location.X, bStart.Location.Y + bStart.Size.Height*2);

            lCopyright.Text = "Exclusive by Mr.President  ©  2014 - 2015." + "  ver. " + version;
            TrayIcon.Text = "Nebo.Mobi.Bot ver. " + version;
            lCopyright.Location = new Point(this.Size.Width - (int)(1.1*lCopyright.Size.Width), this.Size.Height - 5 * lCopyright.Size.Height);

            LOGBox.Location = new Point(lLogin.Location.X, lLOG.Location.Y + lLOG.Size.Height + (int)(0.01 * this.Size.Height));
            LOGBox.Size = new Size(this.Size.Width - 3 * LOGBox.Location.X, this.Size.Height - LOGBox.Location.Y - 6 * lCopyright.Size.Height);

            gbStats.Location = new Point(LOGBox.Location.X, lCopyright.Location.Y - 10);
            pbCoin.Size = new Size(16, 16);
            pbCoin.Location = new Point(10, 12);
            lCoin.Location = new Point(pbCoin.Location.X + pbCoin.Size.Width, pbCoin.Location.Y + 2);
            pbGold.Size = new Size(16, 16);
            pbGold.Location = new Point(120, 12);
            lGold.Location = new Point(pbGold.Location.X + pbGold.Size.Width, pbGold.Location.Y + 2);
            pbLevel.Size = new Size(16, 16);
            pbLevel.Location = new Point(180, 12);
            lLevel.Location = new Point(pbLevel.Location.X + pbLevel.Size.Width, pbLevel.Location.Y + 2);
            gbStats.Size = new Size(lLevel.Location.X + 70, 32);
            gbStats.Visible = true;

            this.MinimumSize = new System.Drawing.Size(cbDoNotGetRevard.Location.X + (int)(cbDoNotGetRevard.Size.Width*1.5), (int)(0.5 * Screen.PrimaryScreen.Bounds.Height));
            this.Size = this.MinimumSize;

            webClient = new WebClient();

            tbPass.UseSystemPasswordChar = true;
            rnd = new Random();

            bot_timer = new System.Windows.Forms.Timer();
            bot_timer.Tick += new System.EventHandler(this.bot_timer_Tick);

            ref_timer.Interval = 1000;
            bot_timer.Enabled = false;
            ref_timer.Enabled = false;

            COMMUTATION_STR = "";
            PassChanged = false;

            //обнуление общей статистики
            User_Bucks = User_Coins = User_Level = "";
            Lift_Count = Buy_Count = Coins_Count = Merch_Count = Killed_Count = New_Worker_Count = Action_Count = Bucks_Count = 0;

            //если выбран автостарт и есть логин с паролем - запуск бота
            if (cbAutorun.Checked && tbLogin.Text != "" && tbPass.Text != "")
            {
                bStart.Enabled = false;
                bStop.Enabled = true;
                Bot = new Thread(StartBot);
                if (bot_timer.Enabled) bot_timer.Enabled = false;
                ref_timer.Start();
                Bot.Start();
            }
        }

        //подгружаем настройки
        private void LoadConfig()
        {
            cfg.LoadConfig();
            tbLogin.Text = cfg.Login;
            tbPass.Text = cfg.Pass;
            tbMinTime.Text = cfg.MinTime;
            tbMaxTime.Text = cfg.MaxTime;
            cbDoNotSaveThePass.Checked = Convert.ToBoolean(cfg.DoNotSaveThePass);
            cbDoNotPut.Checked = Convert.ToBoolean(cfg.DoNotPut);            
            cbFire.Checked = Convert.ToBoolean(cfg.Fire);
            tbFireLess.Text = cfg.FireLess;
            cbFire9.Checked = Convert.ToBoolean(cfg.Fire9);
            cbDoNotGetRevard.Checked = Convert.ToBoolean(cfg.DoNotGetRevard);
            cbHide.Checked = Convert.ToBoolean(cfg.Hide);

            //проверка автостарта
            //проверка - было ли ручное отключения из автозагрузки
            Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", false);
            if (myKey.GetValue("Nebo.Mobi.Bot_" + tbLogin.Text, "").ToString() == "")
            {
                //снимаем галку авторана
                cbAutorun.Checked = false;
            }

            //на другой оси сохранили без авторана
            else
                cbAutorun.Checked = true;            

            //активировать возможность включения запуска в скрытом виде только если возможен автостарт
            if (cbAutorun.Checked)
                cbHide.Enabled = true;
            else cbHide.Enabled = false;

            if (cbHide.Checked && cbHide.Enabled && tbLogin.Text != "" && tbPass.Text != "")
                this.WindowState = FormWindowState.Minimized;
        }

        private void SaveConfig()
        {
            cfg.Login = tbLogin.Text;
            if (cbDoNotSaveThePass.Checked)
                cfg.Pass = "";
            else cfg.Pass = tbPass.Text;
            cfg.DoNotSaveThePass = cbDoNotSaveThePass.Checked.ToString().ToLower();
            cfg.MinTime = tbMinTime.Text;
            cfg.MaxTime = tbMaxTime.Text;
            cfg.DoNotPut = cbDoNotPut.Checked.ToString().ToLower();
            cfg.Fire = cbFire.Checked.ToString().ToLower();
            cfg.FireLess = tbFireLess.Text;
            cfg.Fire9 = cbFire9.Checked.ToString().ToLower();
            cfg.DoNotGetRevard = cbDoNotGetRevard.Checked.ToString().ToLower();
            cfg.Hide = cbHide.Checked.ToString().ToLower();
            cfg.WriteConfig();
            bSave.BackColor = Color.LightGreen;

            //модификация автозапуска
            if (AutorunChanged)
            {
                if (cbAutorun.Checked)
                {
                    Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                    if (tbLogin.Text != "")
                        myKey.SetValue("Nebo.Mobi.Bot_" + tbLogin.Text, Application.ExecutablePath);
                    else
                    {
                        MessageBox.Show("Невозможно добавить бота в автозагрузку Windows без указания логина.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                    if (tbLogin.Text != "")
                        myKey.DeleteValue("Nebo.Mobi.Bot_" + tbLogin.Text, false);
                    else
                    {
                        MessageBox.Show("Невозможно удалить бота из автозагрузки Windows без указания логина.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                AutorunChanged = false;
            }
            else if (cbAutorun.Checked && tbLogin.Text == "")
            {
                MessageBox.Show("Невозможно добавить бота в автозагрузку Windows без указания логина.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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

                ClickLink(ab, "");
            }
        }

        //тупо получение главного экрана
        private void GetHomePage()
        {
            ClickLink(HOME_LINK, "");
            
            //получаем ссылку "Показать этажи"
            string ab = Parse(HTML, "Показать этажи");
            if (ab != "")
            {
                ab = ab.Substring(49);
                ab = ab.Remove(ab.IndexOf("\""));

                ClickLink(ab, "");
            }
        }

        //получаем уровень и финансы
        private void GetInfo()
        {
            User_Bucks = User_Coins = User_Level = "";
            GetHomePage();

            string ab = Parse(HTML, "mn_iron.png");
            if (ab != "")
            {
                ab = ab.Substring(122);
                ab = ab.Remove(ab.IndexOf('<'));
                User_Coins = ab.Replace("&#039;", "'");
            }

            ab = Parse(HTML, "mn_gold.png");
            if (ab != "")
            {
                ab = ab.Substring(122);
                ab = ab.Remove(ab.IndexOf('<'));
                User_Bucks = ab.Replace("&#039;", "'");
            }

            ab = Parse(HTML, "уровень");
            if (ab != "")
            {
                ab = ab.Substring(106);
                ab = ab.Remove(ab.IndexOf('<'));
                User_Level = ab + " уровень";
            }            
            TrayIcon.Text = tbLogin.Text + ": " + User_Level + ". Монет: " + User_Coins + " Баксов: " + User_Bucks;
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
            lCoin.Text = User_Coins;
            lGold.Text = User_Bucks;
            lLevel.Text = User_Level;
        }


        //обработчик кнопки Старт
        private void bStart_Click(object sender, EventArgs e)
        {
            bStart.Enabled = false;
            bStop.Enabled = true;
            Bot = new Thread(StartBot);
            if (bot_timer.Enabled) bot_timer.Enabled = false;
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

            if(User_Coins =="") GetInfo(); //получаем инфу до прогона
            //делаем 2 прогона (мб что-то доставят или купят випы)
            for (int i = 0; i < 2; i++)
            {
                if(!cbDoNotGetRevard.Checked) GetReavrd();
                FindWorkers();
                if (!cbDoNotGetRevard.Checked) GetReavrd();
                CollectMoney();
                if (!cbDoNotGetRevard.Checked) GetReavrd();
                if (!cbDoNotPut.Checked)
                {
                    PutMerch();
                    if (!cbDoNotGetRevard.Checked) GetReavrd();
                }
                Buy();
                if (!cbDoNotGetRevard.Checked) GetReavrd();
                GoneLift();


                ///Разработка
                ///
                //InviteToCity();
                ///
                ///
            }
            Action_Count++; //считаем оба прогона за 1
            GetInfo();      //получем инфу после прогона

            RelaxMan();
        }

        private void RelaxMan()
        {
            int min_time = 0, max_time = 0;

            //защита от дурака
            try
            {
                min_time = Convert.ToInt32(tbMinTime.Text);
            }
            catch
            {
                ThreadAbort("ОШИБКА. Поле \"От:\" должно содержать значение в диапазоне от 1 до 200.\n");
            }

            if (min_time < 1 || min_time > 200)
                ThreadAbort("ОШИБКА. Поле \"От:\" должно содержать значение в диапазоне от 1 до 200.\n");

            try
            {
                max_time = Convert.ToInt32(tbMaxTime.Text);
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
            bot_timer.Interval = rnd.Next(min_time * 60000, max_time * 60000);
            bot_timer.Interval = (int)(bot_timer.Interval * 0.001) * 1000;
            CONNECT_STATUS = "   -   Стоп";
            timeleft = (int)(bot_timer.Interval * 0.001);
            COMMUTATION_STR = string.Format("Жду   {0}", string.Format("{0}мин : {1:d2}сек\n\n", (int)(timeleft / 60), timeleft - ((int)(timeleft / 60) * 60)));
        }

        //жмакнуть по ссылке
        private void ClickLink(string link, string param)
        {
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0");
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Encoding = Encoding.UTF8;
            try
            {
                HTML = webClient.UploadString(SERVER + link, param);
            }
            catch (Exception ex)
            {
                ThreadSleep("ОШИБКА. " + ex.Message + '\n');
            }
            if (HTML.Contains("502 Bad Gateway")) GetHomePage();
        }

        //подключение к серверу
        private void Connect()
        {
            Entery();

            CONNECT_STATUS = "  -  Попытка авторизции";
            string param = string.Format("id5_hf_0=&login={0}&password={1}&%3Asubmit=%D0%92%D1%85%D0%BE%D0%B4", tbLogin.Text.Replace(' ', '+'), Crypto.DecryptStr(tbPass.Text));

            ClickLink(LINK, param);
            
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

        //метод торможения потока бота
        private void ThreadSleep(string reason)
        {
            COMMUTATION_STR = reason;
            CONNECT_STATUS = "";
            ACTION_STATUS = "";           
            RelaxMan();
            Thread.CurrentThread.Abort();
            //Bot.Start();
        }
        
        //входим на домашнюю станицу получаем ссылку на форму входа
        private void Entery()
        {
            CONNECT_STATUS = "  -  Подключение к серверу";
            
            ClickLink("login","");


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

        /*
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
        */
                
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
            ClickLink(lnk, "");

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

                    //считаем пассажира
                    lift_count++;
                    Lift_Count++;

                    //если дал бакс - считаем
                    if (HTML.Contains("static.nebo.mobi/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>1</span></b>"))
                    {
                        bucks_count++;
                        Bucks_Count++;
                    }
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

        //основной метод получения награды (баксов)
        private void GetReavrd()
        {
            //получаем ссылку на квесты и кликаем если есть награда
            string ab = TryRevard();
            if (ab != "")
            {
                ACTION_STATUS = "   -   Собираю награду";
                ClickLink(ab, "");
                Thread.Sleep(rnd.Next(300, 500));

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

                        //считаем награду
                        string stat = str[i-3];
                        if(stat.Contains("Бонус X2!"))
                            stat = str[i-6];
                        stat = stat.Substring("static.nebo.mobi/images/icons/mn_gold.png\" width=\"16\" height=\"16\" alt=\"$\"/><span>".Length + "<span><img src=\"http://".Length);
                        stat = stat.Remove(stat.IndexOf('<'));
                        bucks_count += Convert.ToInt32(stat);
                        
                        //получаем ссылку, кликаем и ждем
                        ab = ab.Substring("<div><a class=\"btng btn60\" href=\"".Length);
                        ab = ab.Remove(ab.IndexOf('\"'));
                        ClickLink(ab, "");
                        Thread.Sleep(rnd.Next(500, 800));
                    }
                }                
                Bucks_Count += bucks_count;
                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Собрано наградных баксов: {1}.\n", GetTime(), bucks_count);

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
            //проверяем, надо ли гнать лифт
            string ab = TryLift();

            lift_count = 0;
            bucks_count = 0;
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
                if (bucks_count != 0) COMMUTATION_STR += string.Format("{0}  -  Собрано чаевых баксов: {1}.\n", GetTime(), bucks_count);
            }
            Thread.Sleep(rnd.Next(1001, 1500));
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
            ClickLink(lnk, "");

            string ab = Parse(HTML, "Собрать выручку!");
            if (ab != "")
            {
                ab = ab.Substring(114);
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
                ACTION_STATUS = "   -   Собираю выручку";
                coins_count = 1;
                Coins_Count++;
                while (ab != "")
                {
                    ab = GetMoneyLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.\n", GetTime(), coins_count);
            }
            Thread.Sleep(rnd.Next(1001, 1500));
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
            ClickLink(lnk, "");

            string ab = Parse(HTML, "Выложить товар");
            if (ab != "")
            {
                ab = ab.Substring(117);
                ab = ab.Remove(ab.IndexOf('\"'));
                merch_count++;
                Merch_Count++;
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
                Merch_Count++;
                while (ab != "")
                {
                    ab = GetMerchLink(ab);
                    Thread.Sleep(rnd.Next(100, 300));
                }

                ACTION_STATUS = "";
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых выложен товар: {1}.\n", GetTime(), merch_count);
            }
            Thread.Sleep(rnd.Next(1001, 1500));
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
                    Buy_Count++;
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
            User_Level = User_Coins = User_Bucks = "";
            TrayIcon.Text = "Nebo.Mobi.Bot ver. " + version;
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
            ClickLink(ab, "");
        }

        //увольняем жильцов ниже заданного уровня
        private void FindWorkers()
        {
            int i = 0;
            string ab = "";
            int bak_i=0;

            killed_count = 0;
            new_worker_count = 0;
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
                        
                        bak_i = i + 7; //бекапим строку чтобы снова не начать с чуваком возиться
                        
                        int free=0; //количество свободных мест в гостинице
                        string ss = Parse(HTML, "Свободно: <b><span>");
                        ss = ss.Substring(103);
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

                    //если дармоед - выкинуть
                    else 
                    {
                        int level = 0;  //уровень, жильцов меньше которого выселять

                        //защита от дурака
                        try
                        {
                            level = Convert.ToInt32(tbFireLess.Text);
                        }
                        catch
                        {
                            ThreadAbort("ОШИБКА. Уровень должен быть от 1 до 9\n");
                        }

                        //если там число, но не в диапазоне 1:9
                        if (level <= 1 || level >= 10)
                            ThreadAbort("ОШИБКА. Уровень должен быть от 1 до 9\n");  

                        //получаем уровень и сверяем с заданным
                        string rank = str[i + 2];
                        rank = rank.Substring(17);
                        rank = rank.Remove(1);
                        int l = Convert.ToInt32(rank);

                        //если уровень меньше заданного и стоит галочка выселения, или если он 9 и стоит галочка выселения (-)
                        if (l < level && cbFire.Checked || level == 9 && cbFire9.Checked && str[i + 7].Contains("(-)"))
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
            ACTION_STATUS = "";
            if (killed_count!=0) COMMUTATION_STR = string.Format("{0}  -  Выселено дармоедов: {1}.\n", GetTime(), killed_count);
            if (new_worker_count != 0) COMMUTATION_STR += string.Format("{0}  -  Нанято новых рабочих: {1}.\n", GetTime(), new_worker_count);
            Thread.Sleep(rnd.Next(1000, 1500));
        }


        //метод выселения
        private void Kill(string ab)
        {
            //входим в чувака
            ClickLink(ab, "");

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
                    ClickLink(ab, "");
                    
                    HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    killed_count++;
                    Killed_Count++;
                    
                    Thread.Sleep(rnd.Next(100, 500));
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
                ab = ab.Substring(115);
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
                str = HTML.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("принять на работу"))
                    {
                        ab = str[i];
                        break;
                    }
                }
                ab = ab.Substring(115);
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
            GetHomePage();

            //ссылка в Город
            string ab = "";
            ab = Parse(HTML, "Мой город");
            if (ab != "")
            {
                ab = ab.Substring(45);
                ab = ab.Remove(ab.IndexOf('\"'));
            }

            //идем в Город
            ClickLink(ab, "");

            //ищем ссылку "поиск игроков"
            ab = Parse(HTML, "поиск игроков");
            if (ab != "")
            {
                ab = ab.Substring(44);
                ab = ab.Remove(ab.IndexOf('\"'));
            }

            //идем к бомжам
            ClickLink(ab, "");

            ///
            ///до сюда фурычит
            ///
            COMMUTATION_STR += HTML + '\n';
            Thread.Sleep(1000);
            ThreadAbort("ОТЛАДКА.\n");
        }
        


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Bot.Abort();
            }
            catch { }
        }

        //обработчик собычия изменения чек-бокса увольнения по уровню
        private void cbFire_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFire.Checked) tbFireLess.Enabled = true;
            else tbFireLess.Enabled = false;
        }


        //метод для перерисовки окна при изменении размера
        private void Form1_Resize(object sender, EventArgs e)
        {
            lCopyright.Location = new Point(this.Size.Width - (int)(1.1 * lCopyright.Size.Width), this.Size.Height - 5 * lCopyright.Size.Height);
            LOGBox.Size = new Size(this.Size.Width - 3 * LOGBox.Location.X, this.Size.Height - LOGBox.Location.Y - 6 * lCopyright.Size.Height);
            gbStats.Location = new Point(LOGBox.Location.X, lCopyright.Location.Y - 10);
        }

        //показ статистики работы бота в трее по клику
        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (this.Visible == true)
                        this.Hide();
                    else
                    {                        
                        this.Show();
                        if (this.WindowState == FormWindowState.Minimized)
                            this.WindowState = FormWindowState.Normal;
                    }
                }

            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                TrayIcon.BalloonTipTitle = tbLogin.Text + ": " + User_Level;
                TrayIcon.BalloonTipText = string.Format(@"Всего прогонов: {0}

Доставлено пассажиров: {1}
Собрано баксов: {2}
Этажей, с которых собрана выручка: {3}
Этажей, на которых выложен товар: {4}
Этажей, на которых закуплен товар: {5}
Выселено дармоедов: {6}
Нанято новых рабочих: {7}", Action_Count, Lift_Count, Bucks_Count, Coins_Count, Merch_Count, Buy_Count, Killed_Count, New_Worker_Count);
                TrayIcon.ShowBalloonTip(1000);
            }
        }

        //сохранить настройки
        private void bSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        //событие изменения поля "Пароль"
        private void tbPass_TextChanged(object sender, EventArgs e)
        {
            PassChanged = true;
        }

        //событие выхода курсора из поля "Пароль"
        private void tbPass_Leave(object sender, EventArgs e)
        {
            if (PassChanged)
            {
                tbPass.Text = Crypto.EncryptStr(tbPass.Text);
                PassChanged = false;
                bSave.BackColor = Color.Orange;
            }
        }

        //событие по изменению чек-бокса Авторана
        private void cbAutorun_Click(object sender, EventArgs e)
        {
            AutorunChanged = true;
            //MessageBox.Show("Не забудьте сохранить настройки!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
            bSave.BackColor = Color.Orange;
            if (cbAutorun.Checked)
                cbHide.Enabled = true;
            else cbHide.Enabled = false; 
        }

        //скрываем окно если стоит галочка
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (cbHide.Checked && cbHide.Enabled && tbLogin.Text != "" && tbPass.Text != "")
                this.Hide();
        }

        private void cbDoNotSaveThePass_Click(object sender, EventArgs e)
        {
            bSave.BackColor = Color.Orange;
        }

        private void cbHide_Click(object sender, EventArgs e)
        {
            bSave.BackColor = Color.Orange;
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            bSave.BackColor = Color.Orange;
        }

        private void tbMinTime_TextChanged(object sender, EventArgs e)
        {
            bSave.BackColor = Color.Orange;
        }

        private void tbMaxTime_TextChanged(object sender, EventArgs e)
        {
            bSave.BackColor = Color.Orange;
        }

    }
}
