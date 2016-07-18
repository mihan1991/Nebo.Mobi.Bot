using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;

namespace Nebo.Mobi.Bot
{
    public class UI
    {
        //контролы на экране
        private TabItem Page;                       //итемка

        private StackPanel spPageHead;                //панель заголовка
        private Image spPageHeadImg;                  //иконка профиля (зависит от уровня
        private TextBlock spPageHeadName;          //имя профиля

        private Grid gPageContent;                   //панель контента

        private Button bClose;
        private Image iStatus;
        private TextBlock lStatus;

        private GroupBox gbUser;
        private Grid gUserData;
        private Label lLogin;
        private TextBox tbLogin;
        private Label lPass;
        private PasswordBox tbPass;
        private CheckBox cbDoNotSaveThePass;
        private Button bStart;
        private Button bStop;

        private GroupBox gbDiapazon;
        private Grid gDiapazonData;
        private Label lMinTime;
        private TextBox tbMinTime;
        private Label lMaxTime;
        private TextBox tbMaxTime;

        private GroupBox gbMethods;
        private Grid gMethodsData;
        private CheckBox cbDoNotPut;
        private CheckBox cbDoNotGetRevard;
        private CheckBox cbFire;
        private TextBox tbFireLess;
        private CheckBox cbFire9;
        private CheckBox cbDoNotShowStatistic;
        private CheckBox cbInvite;
        private GroupBox gbInvite;
        private Grid gInviteData;
        private CheckBox cbInviteFrom;
        private TextBox tbInviteFrom;
        private CheckBox cbInviteTo;
        private TextBox tbInviteTo;

        private Grid gStatLOG;
        private System.Windows.Forms.Integration.WindowsFormsHost wfhIntegr;
        private RichTextBox LOGBox;
        private ImageBrush bgLOG;
        private Image imgLOG;
        private System.Windows.Forms.WebBrowser wbAction;

        private Grid gUserSatusData;



        //тупо для удобства задания высоты TextBlock
        private int tbHeight;

        private bool PageCreated;

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
        private string User_Level;
        private string User_Bucks;
        private string User_Coins;
        private string User_City;
        private string User_Floors;
        private string City_Role;

        //блок общей статистики работы
        private int Lift_Count;
        private int Bucks_Count;
        private int Buy_Count;
        private int Coins_Count;
        private int Merch_Count;
        private int Killed_Count;
        private int New_Worker_Count;
        private int Opened_Floor_Count;
        private int Invited_Count;
        private int Action_Count;
        private string[] Stat;

        private static string SERVER = "https://vnebo.mobi/";     //адрес сервера
        //private static string SERVER = "http://pumpit.mmska.ru/";
        private string CONNECT_STATUS = "";                     //статус соединения
        private string ACTION_STATUS = "";                      //теекущее действие
        private string CURRENT_HTML;

        private Random rnd;
        private DispatcherTimer bot_timer;                      //таймер запуска прохода бота
        private DispatcherTimer ref_timer;                      //таймер обновления страницы
        private string HTML;                                    //html-код текущей страницы
        private string LINK;                                    //переменная для обмена ссылками
        private string HOME_LINK;                               //ссылка на домашнюю страницу
        private string COMMUTATION_STR;                         //строка для логов
        private int timeleft;                                   //секунд до начала нового прохода

        private Thread Bot;                                     //переменная потока бота

        private Config.User user_cfg;                                     //объект класса настроек

        //переменные для фиксации изменений в textbox
        private bool PassChanged;

        private WebClient webClient;


        //конструктор для профиля из конфига
        public UI(Config.User conf)
        {
            //создаем контролы
            CreateControls();

            //получаем настройки и забиваем поля
            user_cfg = conf;
            LoadConfig();
            InitContent();
        }

        //конструктор для нового профиля
        public UI()
        {
            //создаем контролы
            CreateControls();

            //получаем настройки и забиваем поля
            user_cfg = new Config.User();
            user_cfg.InitUserDefault();
            LoadConfig();
            InitContent();             
        }

        private void InitContent()
        {
            PassChanged = false;

            //вывод стартовой страницы
            if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                CreateHTMLPage(Properties.Resources.start);
            else
                CreateHTMLPage(Properties.Resources.start_off);
            wbAction.DocumentText = CURRENT_HTML;

            //отключаем ненужные контролы
            if (!Convert.ToBoolean(user_cfg.Fire))
                tbFireLess.IsEnabled = false;
            else
                tbFireLess.IsEnabled = true;
            if (!Convert.ToBoolean(user_cfg.Invite))
                gbInvite.IsEnabled = false;
            else
                gbInvite.IsEnabled = true;
            if (!Convert.ToBoolean(user_cfg.InviteFrom))
                tbInviteFrom.IsEnabled = false;
            else
                tbInviteFrom.IsEnabled = true;
            if (!Convert.ToBoolean(user_cfg.InviteTo))
                tbInviteTo.IsEnabled = false;
            else
                tbInviteTo.IsEnabled = true;
        }

        //инициализация контролов
        private void CreateControls()
        {
            Page = new TabItem();
            spPageHead = new StackPanel();
            spPageHeadImg = new Image();
            spPageHeadName = new TextBlock();
            gPageContent = new Grid();
            bClose = new Button();
            iStatus = new Image();
            lStatus = new TextBlock();

            gbUser = new GroupBox();
            gUserData = new Grid();
            lLogin = new Label();
            tbLogin = new TextBox();
            lPass = new Label();
            tbPass = new PasswordBox();
            cbDoNotSaveThePass = new CheckBox();
            bStart = new Button();
            bStop = new Button();

            gbDiapazon = new GroupBox();
            gDiapazonData = new Grid();
            lMinTime = new Label();
            tbMinTime = new TextBox();
            lMaxTime = new Label();
            tbMaxTime = new TextBox();

            gbMethods = new GroupBox();
            gMethodsData = new Grid();
            cbDoNotPut = new CheckBox();
            cbDoNotGetRevard = new CheckBox();
            cbFire = new CheckBox();
            tbFireLess = new TextBox();
            cbFire9 = new CheckBox();
            cbDoNotShowStatistic = new CheckBox();
            cbInvite = new CheckBox();
            gbInvite = new GroupBox();
            gInviteData = new Grid();
            cbInviteFrom = new CheckBox();
            tbInviteFrom = new TextBox();
            cbInviteTo = new CheckBox();
            tbInviteTo = new TextBox();

            gStatLOG = new Grid();
            wfhIntegr = new System.Windows.Forms.Integration.WindowsFormsHost();
            wbAction = new System.Windows.Forms.WebBrowser();
            imgLOG = new Image();
            LOGBox = new RichTextBox();

            gUserSatusData = new Grid();

            webClient = new WebClient();

            rnd = new Random();

            bot_timer = new DispatcherTimer();
            ref_timer = new DispatcherTimer();

            bot_timer.Tick += bot_timer_Tick;
            ref_timer.Tick += ref_timer_Tick;

            ref_timer.Interval = TimeSpan.FromMilliseconds(100);
            bot_timer.IsEnabled = false;
            ref_timer.IsEnabled = false;

            COMMUTATION_STR = "";


            //обнуление общей статистики
            User_Bucks = User_Coins = User_Level = User_City = City_Role = "";
            Lift_Count = Buy_Count = Coins_Count = Merch_Count = Killed_Count = New_Worker_Count = Action_Count = Bucks_Count = Opened_Floor_Count = Invited_Count = 0;

            Bot = new Thread(StartBot);

            tbHeight = 25;

            //заголовок (снизу вверх)
            //иконка
            spPageHeadImg.Width = 16;
            spPageHeadImg.Height = 16;
            spPageHeadImg.Name = "spPageHeadImg";

            //текст заголовка
            spPageHeadName.Text = "(Новый персонаж)";
            spPageHeadName.Margin = new Thickness(3);
            spPageHeadName.Name = "spPageHeadName";

            //формируем заголовок
            spPageHead.Orientation = Orientation.Horizontal;
            spPageHead.Children.Add(spPageHeadImg);
            spPageHead.Children.Add(spPageHeadName);
            spPageHead.Name = "spPageHead";
            spPageHead.MouseEnter += ShowToolTip;


            //формируем контент (снизу вверх)
            lLogin.Content = "Логин:";
            lLogin.Height = 25;
            lLogin.Width = 55;
            lLogin.HorizontalAlignment = HorizontalAlignment.Left;
            lLogin.Margin = new Thickness(0, 5, 0, 0);
            lLogin.VerticalAlignment = VerticalAlignment.Top;
            lLogin.HorizontalContentAlignment = HorizontalAlignment.Right;


            tbLogin.Name = "tbLogin";
            tbLogin.Width = 180;
            tbLogin.Height = tbHeight;
            tbLogin.VerticalContentAlignment = VerticalAlignment.Center;
            tbLogin.Margin = new Thickness(55, 5, 0, 0);
            tbLogin.VerticalAlignment = VerticalAlignment.Top;
            tbLogin.HorizontalAlignment = HorizontalAlignment.Left;
            tbLogin.MouseEnter += ShowToolTip;
            tbLogin.TextChanged += tbTextChanged;


            lPass.Content = "Пароль:";
            lPass.Height = 25;
            lPass.Width = 55;
            lPass.HorizontalAlignment = HorizontalAlignment.Left;
            lPass.Margin = new Thickness(0, 35, 0, 0);
            lPass.VerticalAlignment = VerticalAlignment.Top;
            lPass.HorizontalContentAlignment = HorizontalAlignment.Right;



            tbPass.Name = "tbPass";
            tbPass.Width = 180;
            tbPass.Height = tbHeight;
            tbPass.VerticalContentAlignment = VerticalAlignment.Center;
            tbPass.Margin = new Thickness(55, 35, 0, 0);
            tbPass.VerticalAlignment = VerticalAlignment.Top;
            tbPass.HorizontalAlignment = HorizontalAlignment.Left;
            tbPass.MouseEnter += ShowToolTip;
            tbPass.PasswordChanged += tbTextChanged;
            tbPass.LostFocus += tbLostFocus;


            cbDoNotSaveThePass.Name = "cbDoNotSaveThePass";
            cbDoNotSaveThePass.Content = "НЕ сохранять пароль";
            cbDoNotSaveThePass.Height = 16;
            cbDoNotSaveThePass.HorizontalAlignment = HorizontalAlignment.Left;
            cbDoNotSaveThePass.Margin = new Thickness(55, 65, 0, 0);
            cbDoNotSaveThePass.VerticalAlignment = VerticalAlignment.Top;
            cbDoNotSaveThePass.MouseEnter += ShowToolTip;
            cbDoNotSaveThePass.Click += cbClick;


            bStart.Name = "bStart";
            bStart.Content = "Старт";
            bStart.Height = 23;
            bStart.HorizontalAlignment = HorizontalAlignment.Left;
            bStart.Margin = new Thickness(5, 98, 0, 0);
            bStart.VerticalAlignment = VerticalAlignment.Top;
            bStart.Width = 75;
            bStart.Click += button_Click;
            bStart.MouseEnter += ShowToolTip;


            bStop.Name = "bStop";
            bStop.Content = "Стоп";
            bStop.Height = 23;
            bStop.HorizontalAlignment = HorizontalAlignment.Left;
            bStop.Margin = new Thickness(157, 98, 0, 0);
            bStop.VerticalAlignment = VerticalAlignment.Top;
            bStop.Width = 75;
            bStop.Click += button_Click;
            bStop.MouseEnter += ShowToolTip;
            bStop.IsEnabled = false;

            gUserData.Children.Add(lLogin);
            gUserData.Children.Add(tbLogin);
            gUserData.Children.Add(lPass);
            gUserData.Children.Add(tbPass);
            gUserData.Children.Add(cbDoNotSaveThePass);
            gUserData.Children.Add(bStart);
            gUserData.Children.Add(bStop);

            gbUser.Height = 150;
            gbUser.VerticalAlignment = VerticalAlignment.Top;
            gbUser.Header = "Настройки пользователя";
            gbUser.Content = gUserData;
            gbUser.Width = 250;
            gbUser.HorizontalAlignment = HorizontalAlignment.Left;
            gbUser.Margin = new Thickness(1, 25, 0, 0);




            //groupbox диапазона

            lMinTime.Name = "lMinTime";
            lMinTime.Content = "От:";
            lMinTime.Height = 25;
            lMinTime.Width = 30;
            lMinTime.HorizontalContentAlignment = HorizontalAlignment.Right;
            lMinTime.HorizontalAlignment = HorizontalAlignment.Left;
            lMinTime.Margin = new Thickness(0, 5, 0, 0);
            lMinTime.VerticalAlignment = VerticalAlignment.Top;


            tbMinTime.Name = "tbMinTime";
            tbMinTime.Height = tbHeight;
            tbMinTime.Width = 70;
            tbMinTime.HorizontalAlignment = HorizontalAlignment.Left;
            tbMinTime.VerticalAlignment = VerticalAlignment.Top;
            tbMinTime.VerticalContentAlignment = VerticalAlignment.Center;
            tbMinTime.Margin = new Thickness(30, 5, 0, 0);
            tbMinTime.MouseEnter += ShowToolTip;
            tbMinTime.TextChanged += tbTextChanged;


            lMaxTime.Name = "lMaxTime";
            lMaxTime.Content = "До:";
            lMaxTime.Height = tbHeight;
            lMaxTime.Width = 30;
            lMaxTime.HorizontalContentAlignment = HorizontalAlignment.Right;
            lMaxTime.HorizontalAlignment = HorizontalAlignment.Left;
            lMaxTime.Margin = new Thickness(0, 35, 0, 0);
            lMaxTime.VerticalAlignment = VerticalAlignment.Top;


            tbMaxTime.Name = "tbMaxTime";
            tbMaxTime.Height = tbHeight;
            tbMaxTime.Width = 70;
            tbMaxTime.HorizontalAlignment = HorizontalAlignment.Left;
            tbMaxTime.VerticalAlignment = VerticalAlignment.Top;
            tbMaxTime.VerticalContentAlignment = VerticalAlignment.Center;
            tbMaxTime.Margin = new Thickness(30, 35, 0, 0);
            tbMaxTime.MouseEnter += ShowToolTip;
            tbMaxTime.TextChanged += tbTextChanged;


            gDiapazonData.Children.Add(lMinTime);
            gDiapazonData.Children.Add(tbMinTime);
            gDiapazonData.Children.Add(lMaxTime);
            gDiapazonData.Children.Add(tbMaxTime);


            gbDiapazon.Header = "Диапазон повторов (мин)";
            gbDiapazon.Height = 150;
            gbDiapazon.Name = "gbDiapazon";
            gbDiapazon.VerticalAlignment = VerticalAlignment.Top;
            gbDiapazon.HorizontalAlignment = HorizontalAlignment.Left;
            gbDiapazon.Width = 165;
            gbDiapazon.Content = gDiapazonData;
            gbDiapazon.Margin = new Thickness(255, 25, 0, 0);



            //groupbox режимов
            cbDoNotPut.Name = "cbDoNotPut";
            cbDoNotPut.Content = "НЕ выкладывать товар";
            cbDoNotPut.Height = 20;
            cbDoNotPut.HorizontalAlignment = HorizontalAlignment.Left;
            cbDoNotPut.VerticalAlignment = VerticalAlignment.Top;
            cbDoNotPut.Margin = new Thickness(5, 5, 0, 0);
            cbDoNotPut.Width = 145;
            cbDoNotPut.MouseEnter += ShowToolTip;
            cbDoNotPut.Click += cbClick;


            cbDoNotGetRevard.Name = "cbDoNotGetRevard";
            cbDoNotGetRevard.Content = "НЕ собирать награды";
            cbDoNotGetRevard.Height = 20;
            cbDoNotGetRevard.HorizontalAlignment = HorizontalAlignment.Left;
            cbDoNotGetRevard.VerticalAlignment = VerticalAlignment.Top;
            cbDoNotGetRevard.Margin = new Thickness(5, 25, 0, 0);
            cbDoNotGetRevard.Width = 145;
            cbDoNotGetRevard.MouseEnter += ShowToolTip;
            cbDoNotGetRevard.Click += cbClick;


            cbFire.Name = "cbFire";
            cbFire.Content = "Выселять жильцов с уровнем ниже:";
            cbFire.Height = 20;
            cbFire.HorizontalAlignment = HorizontalAlignment.Left;
            cbFire.VerticalAlignment = VerticalAlignment.Top;
            cbFire.Margin = new Thickness(5, 45, 0, 0);
            cbFire.Width = 220;
            cbFire.MouseEnter += ShowToolTip;
            cbFire.Click += cbClick;


            tbFireLess.Name = "tbFireLess";
            tbFireLess.Height = 25;
            tbFireLess.Width = tbHeight;
            tbFireLess.HorizontalAlignment = HorizontalAlignment.Left;
            tbFireLess.VerticalAlignment = VerticalAlignment.Top;
            tbFireLess.Margin = new Thickness(225, 40, 0, 0);
            tbFireLess.VerticalContentAlignment = VerticalAlignment.Center;
            tbFireLess.MouseEnter += ShowToolTip;
            tbFireLess.TextChanged += tbTextChanged;


            cbFire9.Name = "cbFire9";
            cbFire9.Content = "Выселять жильцов 9 уровня со знаком (-)";
            cbFire9.Height = 20;
            cbFire9.HorizontalAlignment = HorizontalAlignment.Left;
            cbFire9.VerticalAlignment = VerticalAlignment.Top;
            cbFire9.Margin = new Thickness(5, 65, 0, 0);
            cbFire9.Width = 250;
            cbFire9.MouseEnter += ShowToolTip;
            cbFire9.Click += cbClick;

            cbDoNotShowStatistic.Name = "cbDoNotShowStatistic";
            cbDoNotShowStatistic.Content = "НЕ отображать статистику";
            cbDoNotShowStatistic.Height = 20;
            cbDoNotShowStatistic.HorizontalAlignment = HorizontalAlignment.Left;
            cbDoNotShowStatistic.VerticalAlignment = VerticalAlignment.Top;
            cbDoNotShowStatistic.Margin = new Thickness(5, 85, 0, 0);
            cbDoNotShowStatistic.MouseEnter += ShowToolTip;
            cbDoNotShowStatistic.Click += cbClick;

            cbInvite.Name = "cbInvite";
            cbInvite.Content = "Приглашать в город";
            cbInvite.Height = 20;
            cbInvite.HorizontalAlignment = HorizontalAlignment.Left;
            cbInvite.VerticalAlignment = VerticalAlignment.Top;
            cbInvite.Margin = new Thickness(280, 5, 0, 0);
            cbInvite.MouseEnter += ShowToolTip;
            cbInvite.Click += cbClick;

            gbInvite.Name = "gbInvite";
            gbInvite.HorizontalAlignment = HorizontalAlignment.Left;
            gbInvite.Margin = new Thickness(280, 22, 0, 0);
            gbInvite.Width = 154;
            gbInvite.Height = 75;
            gbInvite.VerticalAlignment = VerticalAlignment.Top;

            cbInviteFrom.Name = "cbInviteFrom";
            cbInviteFrom.Content = "С уровнем от:";
            cbInviteFrom.Height = 20;
            cbInviteFrom.Margin = new Thickness(5, 10, 0, 0);
            cbInviteFrom.HorizontalAlignment = HorizontalAlignment.Left;
            cbInviteFrom.VerticalAlignment = VerticalAlignment.Top;
            cbInviteFrom.MouseEnter += ShowToolTip;
            cbInviteFrom.Click += cbClick;

            tbInviteFrom.Name = "tbInviteFrom";
            tbInviteFrom.Height=25;
            tbInviteFrom.HorizontalAlignment = HorizontalAlignment.Left;
            tbInviteFrom.Margin = new Thickness(110,5,0,0);
            tbInviteFrom.VerticalAlignment = VerticalAlignment.Top;
            tbInviteFrom.VerticalContentAlignment = VerticalAlignment.Center;
            tbInviteFrom.Width = 25;
            tbInviteFrom.MouseEnter += ShowToolTip;
            tbInviteFrom.TextChanged += tbTextChanged;

            cbInviteTo.Name = "cbInviteTo";
            cbInviteTo.Content = "С уровнем до:";
            cbInviteTo.Height = 20;
            cbInviteTo.Margin = new Thickness(5, 40, 0, 0);
            cbInviteTo.HorizontalAlignment = HorizontalAlignment.Left;
            cbInviteTo.VerticalAlignment = VerticalAlignment.Top;
            cbInviteTo.MouseEnter += ShowToolTip;
            cbInviteTo.Click += cbClick;

            tbInviteTo.Name = "tbInviteTo";
            tbInviteTo.Height = 25;
            tbInviteTo.HorizontalAlignment = HorizontalAlignment.Left;
            tbInviteTo.Margin = new Thickness(110, 35, 0, 0);
            tbInviteTo.VerticalAlignment = VerticalAlignment.Top;
            tbInviteTo.VerticalContentAlignment = VerticalAlignment.Center;
            tbInviteTo.Width = 25;
            tbInviteTo.MouseEnter += ShowToolTip;
            tbInviteTo.TextChanged += tbTextChanged;

            gInviteData.Name = "gInviteData";
            gInviteData.Children.Add(cbInviteFrom);
            gInviteData.Children.Add(tbInviteFrom);
            gInviteData.Children.Add(cbInviteTo);
            gInviteData.Children.Add(tbInviteTo);

            gbInvite.Content = gInviteData;


            gMethodsData.Children.Add(cbDoNotPut);
            gMethodsData.Children.Add(cbDoNotGetRevard);
            gMethodsData.Children.Add(cbFire);
            gMethodsData.Children.Add(tbFireLess);
            gMethodsData.Children.Add(cbFire9);
            gMethodsData.Children.Add(cbDoNotShowStatistic);
            gMethodsData.Children.Add(cbInvite);
            gMethodsData.Children.Add(gbInvite);

            gbMethods.Header = "Режимы работы";
            gbMethods.Height = 150;
            gbMethods.Name = "gbMethods";
            gbMethods.VerticalAlignment = VerticalAlignment.Top;
            gbMethods.Margin = new Thickness(425, 25, 0, 0);
            gbMethods.Content = gMethodsData;



            bClose.Content = "X";
            bClose.VerticalContentAlignment = VerticalAlignment.Center;
            bClose.Background = Brushes.Red;
            bClose.BorderBrush = Brushes.Red;
            bClose.Height = 25;
            bClose.HorizontalAlignment = HorizontalAlignment.Right;
            bClose.Margin = new Thickness(0, 0, 0, 0);
            bClose.Name = "bRemoveUser";
            bClose.VerticalAlignment = VerticalAlignment.Top;
            bClose.Width = 35;
            bClose.Foreground = Brushes.White;
            bClose.FontSize = 16;
            bClose.FontFamily = new FontFamily("Calibri");
            bClose.FontWeight = FontWeights.Bold;
            //событие клика ко этой кнопке привязываем непосредственно в форме

            iStatus.Name = "iStatus";
            iStatus.Height = 16;
            iStatus.Width = 16;
            iStatus.Margin = new Thickness(10, 0, 0, 0);
            iStatus.HorizontalAlignment = HorizontalAlignment.Left;
            iStatus.Source = new BitmapImage(new Uri("/Resources/waiting.png", UriKind.Relative));


            lStatus.Name = "lStatus";
            lStatus.Margin = new Thickness(36, 5, 0, 0);
            lStatus.HorizontalAlignment = HorizontalAlignment.Left;
            lStatus.Text = "Остановлен";


            gUserSatusData.Height = 25;
            gUserSatusData.Name = "gUserSatusData";
            gUserSatusData.VerticalAlignment = VerticalAlignment.Top;
            gUserSatusData.Background = Brushes.White;
            gUserSatusData.Margin = new Thickness(0, -2, -2, 0);
            gUserSatusData.Children.Add(bClose);
            gUserSatusData.Children.Add(iStatus);
            gUserSatusData.Children.Add(lStatus);


            wbAction.Name = "wbAction";
            //wbAction.Margin = new System.Windows.Forms.Padding(2);
            //wbAction.Width = 350;
            wbAction.Dock = System.Windows.Forms.DockStyle.Fill;
            wbAction.IsWebBrowserContextMenuEnabled = false;

            wfhIntegr.Name = "wfhIntegr";
            wfhIntegr.Margin = new Thickness(0, 0, 0, 0);
            wfhIntegr.Width = 350;
            wfhIntegr.HorizontalAlignment = HorizontalAlignment.Left;
            wfhIntegr.Child = wbAction;

            LOGBox.HorizontalAlignment = HorizontalAlignment.Left;
            LOGBox.Margin = new Thickness(355, 2, 2, 2);
            LOGBox.Name = "LOGBox";
            LOGBox.Document.LineHeight = 1;
            LOGBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            LOGBox.FontWeight = FontWeights.SemiBold;
            imgLOG = new Image();
            //Properties.Resources.fon;
            imgLOG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/fon.png", UriKind.Absolute));
            bgLOG = new ImageBrush(imgLOG.Source);
            bgLOG.Opacity = 0.1;
            bgLOG.Stretch = Stretch.Uniform;
            LOGBox.Background = bgLOG;
            

            gStatLOG.Margin = new Thickness(-1, 180, -1, 0);
            gStatLOG.HorizontalAlignment = HorizontalAlignment.Left;
            gStatLOG.Background = Brushes.WhiteSmoke;
            gStatLOG.Children.Add(wfhIntegr);
            gStatLOG.Children.Add(LOGBox);


            gPageContent.Children.Add(gbUser);
            gPageContent.Children.Add(gbDiapazon);
            gPageContent.Children.Add(gbMethods);
            gPageContent.Children.Add(gUserSatusData);
            gPageContent.Children.Add(gStatLOG);

            Page.Header = spPageHead;
            Page.Content = gPageContent;
        }

        //подгружаем настройки
        private void LoadConfig()
        {
            tbLogin.Text = user_cfg.Login;
            //если логин не пустой, то в заголовок пишем его
            if (user_cfg.Login != "")
                spPageHeadName.Text = user_cfg.Login;
            tbPass.Password = user_cfg.Pass;
            tbMinTime.Text = user_cfg.MinTime;
            tbMaxTime.Text = user_cfg.MaxTime;
            cbDoNotSaveThePass.IsChecked = Convert.ToBoolean(user_cfg.DoNotSaveThePass);
            cbDoNotPut.IsChecked = Convert.ToBoolean(user_cfg.DoNotPut);
            cbFire.IsChecked = Convert.ToBoolean(user_cfg.Fire);
            tbFireLess.Text = user_cfg.FireLess;
            cbFire9.IsChecked = Convert.ToBoolean(user_cfg.Fire9);
            cbDoNotShowStatistic.IsChecked = Convert.ToBoolean(user_cfg.DoNotShowStatistic);
            cbDoNotGetRevard.IsChecked = Convert.ToBoolean(user_cfg.DoNotGetRevard);
            cbInvite.IsChecked = Convert.ToBoolean(user_cfg.Invite);
            cbInviteFrom.IsChecked = Convert.ToBoolean(user_cfg.InviteFrom);
            tbInviteFrom.Text = user_cfg.InviteFromMeaning;
            cbInviteTo.IsChecked = Convert.ToBoolean(user_cfg.InviteTo);
            tbInviteTo.Text = user_cfg.InviteToMeaning;
            spPageHeadImg.Source = new BitmapImage(new Uri("/Resources/" + user_cfg.Avatar, UriKind.Relative));
        }

        //вернуть таймер бота
        public Thread GetBotThread()
        {
            return Bot;
        }

        //вернуть конфиги юзера
        public Config.User GetUserCfg()
        {
            return user_cfg;
        }

        //сохранить настройки
        public void SaveUserConfig()
        {
            user_cfg.Login = tbLogin.Text;
            if (cbDoNotSaveThePass.IsChecked.Value)
                user_cfg.Pass = "";
            else user_cfg.Pass = tbPass.Password;
            user_cfg.DoNotSaveThePass = cbDoNotSaveThePass.IsChecked.Value.ToString().ToLower();
            user_cfg.MinTime = tbMinTime.Text;
            user_cfg.MaxTime = tbMaxTime.Text;
            user_cfg.DoNotPut = cbDoNotPut.IsChecked.Value.ToString().ToLower();
            user_cfg.Fire = cbFire.IsChecked.Value.ToString().ToLower();
            user_cfg.FireLess = tbFireLess.Text;
            user_cfg.Fire9 = cbFire9.IsChecked.Value.ToString().ToLower();
            user_cfg.DoNotGetRevard = cbDoNotGetRevard.IsChecked.Value.ToString().ToLower();
            user_cfg.DoNotShowStatistic = cbDoNotShowStatistic.IsChecked.Value.ToString().ToLower();
            user_cfg.Invite = cbInvite.IsChecked.Value.ToString().ToLower();
            user_cfg.InviteFrom = cbInviteFrom.IsChecked.Value.ToString().ToLower();
            user_cfg.InviteFromMeaning = tbInviteFrom.Text;
            user_cfg.InviteTo = cbInviteTo.IsChecked.Value.ToString().ToLower();
            user_cfg.InviteToMeaning = tbInviteTo.Text;
        }

        //возвращает страницу
        public Object GetPage()
        {
            return (Object)Page;
        }

        //вернуть кнопку закрытия
        public Button Get_bClose()
        {
            return bClose;
        }

        //обработчик нажатия кнопок
        private void button_Click(Object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "bStart":
                    StartBotThread();
                    break;
                case "bStop":
                    StopBotThread();
                    break;
            }
        }

        //для запуска бота (в т.ч. и снаружи)
        public void StartBotThread()
        {
            bStart.IsEnabled = false;
            bStop.IsEnabled = true;
            if (Bot != null && Bot.IsAlive) StopBotThread();
            Bot = new Thread(StartBot);
            if (bot_timer.IsEnabled) bot_timer.IsEnabled = false;
            ref_timer.Start();
            //spPageHeadImg.Source = new BitmapImage(new Uri("/Resources/player-m.png", UriKind.Relative));
            Bot.Start();
        }

        //если клик по чек-боксу - сохраняем конфиг (в памяти)
        private void cbClick(object sender, EventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                //выбор отображения статистики
                case "cbDoNotShowStatistic":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        CreateHTMLPage(Properties.Resources.start_off);
                    else
                        CreateHTMLPage(Properties.Resources.start);

                    wbAction.DocumentText = CURRENT_HTML;
                    break;

                //лочим текстбокс с уровнем жильцов на выселение
                case "cbFire":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        tbFireLess.IsEnabled = true;
                    else
                        tbFireLess.IsEnabled = false;
                    break;

                //лочим групбокс инвайтов
                case "cbInvite":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        gbInvite.IsEnabled = true;
                    else
                        gbInvite.IsEnabled = false;
                    break;

                //лочим текстбокс ОТ
                case "cbInviteFrom":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        tbInviteFrom.IsEnabled = true;
                    else
                        tbInviteFrom.IsEnabled = false;
                    break;

                //лочим текстбокс ДО
                case "cbInviteTo":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        tbInviteTo.IsEnabled = true;
                    else
                        tbInviteTo.IsEnabled = false;
                    break;
            }            
            SaveUserConfig();
        }

        //событие по изменению текста в textbox
        private void tbTextChanged(object sender, EventArgs e)
        {
            switch ((sender as Control).Name)
            {
                case "tbLogin":
                    if (tbLogin.Text != "")
                        spPageHeadName.Text = tbLogin.Text;
                    else spPageHeadName.Text = "(Новый персонаж)";
                    user_cfg.Login = tbLogin.Text;
                    break;
                case "tbPass":
                    PassChanged = true;
                    break;
                case "tbMinTime":
                    user_cfg.MinTime = tbMinTime.Text;
                    break;
                case "tbMaxTime":
                    user_cfg.MaxTime = tbMaxTime.Text;
                    break;
                case "tbFireLess":
                    user_cfg.FireLess = tbFireLess.Text;
                    break;
                case "tbInviteFrom":
                    user_cfg.InviteFromMeaning = tbInviteFrom.Text;
                    break;
                case "tbInviteTo":
                    user_cfg.InviteToMeaning = tbInviteTo.Text;
                    break;
            }
        }

        //событие по выходу их textbox
        private void tbLostFocus(object sender, EventArgs e)
        {
            switch ((sender as Control).Name)
            {
                case "tbPass":
                    if (PassChanged)
                    {
                        tbPass.Password = Crypto.EncryptStr(tbPass.Password);
                        user_cfg.Pass = tbPass.Password;
                        PassChanged = false;
                    }
                    break;
            }
        }

        //вывод информации о персонаже при наведении
        private void ShowToolTip(object sender, MouseEventArgs e)
        {
            ToolTip tt = new System.Windows.Controls.ToolTip();

            FrameworkElement cnt = sender as FrameworkElement;

            TextBlock tbText = new TextBlock();
            StackPanel sp = new StackPanel();

            try
            {
                tbText.Text = Properties.Resources.ResourceManager.GetString((sender as Control).Name);
                sp.Children.Add(tbText);
            }
            catch
            {
                //если курсор над заголовком вкладки
                if (cnt.Name == "spPageHead")
                {
                    StackPanel gROOT = CreateStatToolTip();
                    sp.Children.Add(gROOT);
                }

                else
                {
                    tbText.Text = "???";
                    sp.Children.Add(tbText);
                }
            }

            tt.Content = sp;
            (sender as FrameworkElement).ToolTip = tt;
        }

        //создаем панель со статистикой
        private StackPanel CreateStatToolTip()
        {
            TextBlock tbAction = new TextBlock();
            TextBlock tbLift = new TextBlock();
            Image iLift = new Image();
            TextBlock tbBucks = new TextBlock();
            Image iBucks = new Image();
            TextBlock tbCoins = new TextBlock();
            Image iCoins = new Image();
            TextBlock tbMerch = new TextBlock();
            Image iMerch = new Image();
            TextBlock tbBuy = new TextBlock();
            Image iBuy = new Image();
            TextBlock tbKilled = new TextBlock();
            Image iKilled = new Image();
            TextBlock tbNewWorker = new TextBlock();
            Image iNewWorker = new Image();
            TextBlock tbOpenedFloor = new TextBlock();
            Image iOpenedFloor = new Image();
            TextBlock tbInvited = new TextBlock();
            Image iInvited = new Image();
            StackPanel gROOT = new StackPanel();
            StackPanel gLift = new StackPanel();
            StackPanel gBucks = new StackPanel();
            StackPanel gCoins = new StackPanel();
            StackPanel gMerch = new StackPanel();
            StackPanel gBuy = new StackPanel();
            StackPanel gKilled = new StackPanel();
            StackPanel gNewWorker = new StackPanel();
            StackPanel gOpenedFloor = new StackPanel();
            StackPanel gInvited = new StackPanel();

            gROOT.Orientation = Orientation.Vertical;
            gLift.Orientation = Orientation.Horizontal;
            gBucks.Orientation = Orientation.Horizontal;
            gCoins.Orientation = Orientation.Horizontal;
            gMerch.Orientation = Orientation.Horizontal;
            gBuy.Orientation = Orientation.Horizontal;
            gKilled.Orientation = Orientation.Horizontal;
            gNewWorker.Orientation = Orientation.Horizontal;
            gOpenedFloor.Orientation = Orientation.Horizontal;
            gInvited.Orientation = Orientation.Horizontal;

            tbAction.Text = string.Format("Всего прогонов: {0}\n", Action_Count);
            gROOT.Children.Add(tbAction);

            iLift.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/lift.png", UriKind.Absolute));
            iLift.Width = 16;
            iLift.Height = 16;
            tbLift.Text = string.Format("Доставлено пассажиров: {0}", Lift_Count);
            tbLift.Margin = new Thickness(10, 0, 0, 0);
            gLift.Children.Add(iLift);
            gLift.Children.Add(tbLift);
            gROOT.Children.Add(gLift);

            iBucks.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mn_gold.png", UriKind.Absolute));
            iBucks.Width = 16;
            iBucks.Height = 16;
            tbBucks.Text = string.Format("Собрано баксов: {0}", Bucks_Count);
            tbBucks.Margin = new Thickness(10, 0, 0, 0);
            gBucks.Children.Add(iBucks);
            gBucks.Children.Add(tbBucks);
            gROOT.Children.Add(gBucks);

            iCoins.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mn_iron.png", UriKind.Absolute));
            iCoins.Width = 16;
            iCoins.Height = 16;
            tbCoins.Text = string.Format("Этажей, с которых собрана выручка: {0}", Coins_Count);
            tbCoins.Margin = new Thickness(10, 0, 0, 0);
            gCoins.Children.Add(iCoins);
            gCoins.Children.Add(tbCoins);
            gROOT.Children.Add(gCoins);

            iMerch.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/st_stocked.png", UriKind.Absolute));
            iMerch.Width = 16;
            iMerch.Height = 16;
            tbMerch.Text = string.Format("Этажей, на которых выложен товар: {0}", Merch_Count);
            tbMerch.Margin = new Thickness(10, 0, 0, 0);
            gMerch.Children.Add(iMerch);
            gMerch.Children.Add(tbMerch);
            gROOT.Children.Add(gMerch);

            iBuy.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/tb_empty.png", UriKind.Absolute));
            iBuy.Width = 16;
            iBuy.Height = 16;
            tbBuy.Text = string.Format("Этажей, на которых закуплен товар: {0}", Buy_Count);
            tbBuy.Margin = new Thickness(10, 0, 0, 0);
            gBuy.Children.Add(iBuy);
            gBuy.Children.Add(tbBuy);
            gROOT.Children.Add(gBuy);

            iKilled.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/users_minus.png", UriKind.Absolute));
            iKilled.Width = 16;
            iKilled.Height = 16;
            tbKilled.Text = string.Format("Выселено дармоедов: {0}", Killed_Count);
            tbKilled.Margin = new Thickness(10, 0, 0, 0);
            gKilled.Children.Add(iKilled);
            gKilled.Children.Add(tbKilled);
            gROOT.Children.Add(gKilled);

            iNewWorker.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/users_plus.png", UriKind.Absolute));
            iNewWorker.Width = 16;
            iNewWorker.Height = 16;
            tbNewWorker.Text = string.Format("Нанято новых рабочих: {0}", New_Worker_Count);
            tbNewWorker.Margin = new Thickness(10, 0, 0, 0);
            gNewWorker.Children.Add(iNewWorker);
            gNewWorker.Children.Add(tbNewWorker);
            gROOT.Children.Add(gNewWorker);

            iOpenedFloor.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/hd_nebo.png", UriKind.Absolute));
            iOpenedFloor.Width = 16;
            iOpenedFloor.Height = 16;
            tbOpenedFloor.Text = string.Format("Открыто этажей: {0}", Opened_Floor_Count);
            tbOpenedFloor.Margin = new Thickness(10, 0, 0, 0);
            gOpenedFloor.Children.Add(iOpenedFloor);
            gOpenedFloor.Children.Add(tbOpenedFloor);
            gROOT.Children.Add(gOpenedFloor);

            iInvited.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/invite.png", UriKind.Absolute));
            iInvited.Width = 16;
            iInvited.Height = 16;
            tbInvited.Text = string.Format("Приглашено игроков: {0}", Invited_Count);
            tbInvited.Margin = new Thickness(10, 0, 0, 0);
            gInvited.Children.Add(iInvited);
            gInvited.Children.Add(tbInvited);
            gROOT.Children.Add(gInvited);
            return gROOT;
        }

        //создание страницы с отчетом
        private void CreateHTMLPage(string res)
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
                    //фиксируем строчку с кубком или с учетом днюхой
                    if (str[i].Contains("award-g.png") || str[i].Contains("st_builded.png"))
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
                if (str[i].Contains("class=\"flhdr\" href=\"floor"))
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

        //обновление содержания формы
        private void UpdForm()
        {
            DateTime first = DateTime.Now;
            if (Bot.IsAlive)
            {
                switch (CONNECT_STATUS)
                {
                    case " Подключение к серверу ":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connect.png", UriKind.Absolute));
                        break;
                    case " Попытка авторизации ":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connect.png", UriKind.Absolute));
                        break;
                }
                switch (ACTION_STATUS)
                {
                    case "Открываю этажи":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/st_builded.png", UriKind.Absolute));
                        break;
                    case "Шмонаю гостиницу":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/man_minus.png", UriKind.Absolute));
                        break;
                    case "Собираю награду":
                        break;
                    case "Собираю выручку":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/tb_sold.png", UriKind.Absolute));
                        break;
                    case "Выкладываю товар":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/tb_stocked.png", UriKind.Absolute));
                        break;
                    case "Закупаю товар":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/tb_empty.png", UriKind.Absolute));
                        break;
                    case "Катаю лифт":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/lift.png", UriKind.Absolute));
                        break;
                    case "Зову народ":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/invite.png", UriKind.Absolute));
                        break;
                    case "Анализ ситуации":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/loupe.png", UriKind.Absolute));
                        break;
                }
            }
            else
            {
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/waiting.png", UriKind.Absolute));
            }

            lStatus.Text = CONNECT_STATUS + ACTION_STATUS;

            //отображаем страницу в раузере            
            if (PageCreated)
            {
                wbAction.DocumentText = CURRENT_HTML;
                PageCreated = false;
            }


            if (user_cfg.Avatar != "")
                spPageHeadImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/" + user_cfg.Avatar, UriKind.Absolute));

            if (!Bot.IsAlive)
            {
                bStart.IsEnabled = true;
                //wbAction.NavigateToString(CURRENT_HTML);
                //bStop.IsEnabled = false;
            }
            //this.Text = NAME + CONNECT_STATUS + ACTION_STATUS;

            if (COMMUTATION_STR != "")
            {
                LOGBox.AppendText(COMMUTATION_STR + Environment.NewLine);
                LOGBox.ScrollToEnd();
                COMMUTATION_STR = "";
            }
            if (CONNECT_STATUS.Contains("Стоп"))
            {
                bot_timer.Start();
                CONNECT_STATUS = "";
                ACTION_STATUS = "";
                //wbAction.NavigateToString(CURRENT_HTML);
            }

            if (timeleft > 0)
            {
                timeleft -= (100 + (DateTime.Now - first).Milliseconds);
                int minutes = (int)TimeSpan.FromMilliseconds(timeleft).TotalMinutes;
                int seconds = (int)TimeSpan.FromMilliseconds(timeleft).TotalSeconds - minutes * 60;
                CONNECT_STATUS = string.Format("Жду   {0}мин : {1:d2}сек\n\n", minutes, seconds);
            }
        }


        //список дел бота
        private void StartBot()
        {
            //сбрасываем таймер обратного отсчета
            timeleft = 0;

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
                    GetReavrd();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }
                //шмонаем гостиницу
                FindWorkers();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetReavrd();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }
                //собираем выручку
                CollectMoney();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetReavrd();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }
                if (!Convert.ToBoolean(user_cfg.DoNotPut))
                {
                    //выкладываем товары
                    PutMerch();
                    if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                    {
                        GetReavrd();
                        if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                            GetInfo();
                    }
                }
                //закупаем товары
                Buy();
                if (!Convert.ToBoolean(user_cfg.DoNotGetRevard))
                {
                    GetReavrd();
                    if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                        GetInfo();
                }
                //катаем лифт
                GoneLift();

                //зовем народ
                if(Convert.ToBoolean(user_cfg.Invite))
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
            bot_timer.Interval = TimeSpan.FromMilliseconds(rnd.Next(min_time * 60, max_time * 60) * 1000);
            //bot_timer.Interval = TimeSpan.FromMilliseconds((bot_timer.Interval.TotalMilliseconds * 0.001) * 1000);
            CONNECT_STATUS = " Стоп ";
            timeleft = (int)(bot_timer.Interval.TotalMilliseconds);
            int minutes = (int)TimeSpan.FromMilliseconds(timeleft).TotalMinutes;
            int seconds = (int)TimeSpan.FromMilliseconds(timeleft).TotalSeconds - minutes * 60;
            COMMUTATION_STR += string.Format("Жду   {0}", string.Format("{0}мин : {1:d2}сек\n\n", minutes, seconds));
        }

        //жмакнуть по ссылке
        private void ClickLink(string link, string param)
        {
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/39.0");
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //webClient.Headers[HttpRequestHeader.Host] = SERVER;
            webClient.Encoding = Encoding.UTF8;

            //пробуем кликнуть на ссылку
            try
            {
                HTML = webClient.UploadString(SERVER + link, param);
            }
            catch(Exception ex1)
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
                HTML = webClient.UploadString(SERVER + link, param);
            }
            else if(HTML.Contains("502 Bad Gateway"))
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
                else if (a.Contains("/images/icons/tb_lift.png") || a.Contains("/images/icons/tb_lift_vip.png")) //если есть народ или ВИПы
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


        //событие таймера
        private void bot_timer_Tick(object sender, EventArgs e)
        {
            bot_timer.Stop();
            bStart.IsEnabled = false;
            bStop.IsEnabled = true;
            Bot = new Thread(StartBot);
            ref_timer.Start();
            Bot.Start();
        }

        //основной метод получения награды (баксов)
        private void GetReavrd()
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
                    }
                }
                Bucks_Count += bucks_count;
                ACTION_STATUS = "Анализ ситуации";
                COMMUTATION_STR = string.Format("{0}  -  Собрано наградных баксов: {1}.", GetTime(), bucks_count);

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
                COMMUTATION_STR = string.Format("{0}  -  Доставлено пассажиров: {1}.", GetTime(), lift_count);
                if (bucks_count != 0) COMMUTATION_STR += string.Format("\n{0}  -  Собрано чаевых баксов: {1}.", GetTime(), bucks_count);
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
                COMMUTATION_STR = string.Format("{0}  -  Этажей, с которых собрана выручка: {1}.", GetTime(), coins_count);
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
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых выложен товар: {1}.", GetTime(), merch_count);
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
                COMMUTATION_STR = string.Format("{0}  -  Этажей, на которых закуплен товар: {1}.", GetTime(), buy_count);
            }
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }

        private string GetTime()
        {
            return string.Format(@"{0:d2}.{1:d2}.{2:d4}  [{3:d2}:{4:d2}:{5:d2}]",
                                 DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }


        //остановка бота и очистка синхрострок
        public void StopBotThread()
        {
            Bot.Abort();
            CONNECT_STATUS = "";
            ACTION_STATUS = "";
            bot_timer.Stop();
            ref_timer.Stop();
            bStart.IsEnabled = true;
            bStop.IsEnabled = false;
            timeleft = 0;
            User_Level = User_Coins = User_Bucks = "";
            //TrayIcon.Text = "Nebo.Mobi.Bot ver. " + version;
            UpdForm();
            lStatus.Text = "Остановлен";
            iStatus.Source = new BitmapImage(new Uri("/Resources/waiting.png", UriKind.Relative));
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

                    //иначе проверяем, не лучше ли этот житель уже работающих
                    else
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
            //крайние меры - обновление происходит быстрее формирования статуса
            if (killed_count != 0 && new_worker_count != 0) COMMUTATION_STR = string.Format("{0}  -  Выселено дармоедов: {1}.\n{2}  -  Нанято новых рабочих: {3}.", GetTime(), killed_count, GetTime(), new_worker_count);
            else if (killed_count != 0) COMMUTATION_STR = string.Format("{0}  -  Выселено дармоедов: {1}.", GetTime(), killed_count);
            else if (new_worker_count != 0) COMMUTATION_STR = string.Format("{0}  -  Нанято новых рабочих: {1}.", GetTime(), new_worker_count);
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

            try
            {
                InvMinLvl = Convert.ToInt32(user_cfg.InviteFromMeaning);
            }
            catch
            {
                ThreadAbort("ОШИБКА. Минимальный уровень игрока должен быть от 10 до 75\n");
            }

            try
            {
                InvMaxLvl = Convert.ToInt32(user_cfg.InviteToMeaning);
            }
            catch
            {
                ThreadAbort("ОШИБКА. Максимальный уровень игрока должен быть от 10 до 75\n");
            }

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
                            if (str[i].Contains("(+)") && !str[i-1].Contains("Мой город"))
                            {
                                //получаем уровень игрока
                                int Level;
                                
                                ab = str[i].Substring(83);
                                ab = ab.Remove(ab.IndexOf('<'));

                                Level = Convert.ToInt32(ab);

                                //если уровень входит в диапазон - пробуем войти
                                if (Level >= InvMinLvl && Level <= InvMaxLvl)
                                {
                                    ab = str[i-1].Substring(28);
                                    ab = ab.Remove(ab.IndexOf('\"'));

                                    ClickLink(ab,"");

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
            if (invited_count != 0) COMMUTATION_STR = string.Format("{0}  -  Отправлено приглашений: {1}.", GetTime(), invited_count);
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }



        //обработчик собычия изменения чек-бокса увольнения по уровню
        private void cbFire_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFire.IsChecked.Value) tbFireLess.IsEnabled = true;
            else tbFireLess.IsEnabled = false;
        }


        //событие изменения поля "Пароль"
        private void tbPass_TextChanged(object sender, EventArgs e)
        {
            PassChanged = true;
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
            if (opened_floor_count != 0) COMMUTATION_STR = string.Format("{0}  -  Открыто этажей: {1}.", GetTime(), opened_floor_count);
            ACTION_STATUS = "Анализ ситуации";
            Thread.Sleep(rnd.Next(100, 300));
        }
    }
}

