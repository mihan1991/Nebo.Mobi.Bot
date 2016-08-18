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
        private CheckBox cbAppoint;
        private ComboBox cboxAppointTo;

        private Grid gStatLOG;
        private System.Windows.Forms.Integration.WindowsFormsHost wfhIntegr;
        private RichTextBox LOGBox;
        private ImageBrush bgLOG;
        private Image imgLOG;
        private System.Windows.Forms.WebBrowser wbAction;

        private Grid gUserSatusData;

        //тупо для удобства задания высоты TextBlock
        private int tbHeight;

        //переменные для фиксации изменений в textbox
        public bool tbLoginChanged;
        public bool tbPassChanged;
        public bool tbMinTimeChanged;
        public bool tbMaxTimeChanged;
        public bool tbFireLessChanged;
        public bool tbInviteFromChanged;
        public bool tbInviteToChanged;


        private double timeleft;                                   //секунд до начала нового прохода

        private BotEngine BOT;                                  //движуха бота
        private Config.User user_cfg;                           //для удобства здесь будем хранить конфиги

        private DispatcherTimer bot_timer;                      //таймер запуска прохода бота
        private DispatcherTimer ref_timer;                      //таймер обновления страницы


        //конструктор для профиля из конфига
        public UI(Config.User cfg)
        {
            //создаем контролы
            CreateControls();

            //инициализируем конфиги и бота
            user_cfg = cfg;
            BOT = new BotEngine(cfg);

            //получаем настройки и забиваем поля
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
            BOT = new BotEngine();

            LoadConfig();
            InitContent();             
        }

        private void InitContent()
        {
            //вывод стартовой страницы
            if (!Convert.ToBoolean(user_cfg.DoNotShowStatistic))
                BOT.CreateHTMLPage(Properties.Resources.start);
            else
                BOT.CreateHTMLPage(Properties.Resources.start_off);
            wbAction.DocumentText = BOT.CURRENT_HTML;

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
            if (!Convert.ToBoolean(user_cfg.Appoint))
                cboxAppointTo.IsEnabled = false;
            else
                cboxAppointTo.IsEnabled = true;
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
            cbAppoint = new CheckBox();
            cboxAppointTo = new ComboBox();

            gStatLOG = new Grid();
            wfhIntegr = new System.Windows.Forms.Integration.WindowsFormsHost();
            wbAction = new System.Windows.Forms.WebBrowser();
            imgLOG = new Image();
            LOGBox = new RichTextBox();

            gUserSatusData = new Grid();

            bot_timer = new DispatcherTimer();
            ref_timer = new DispatcherTimer();

            //события на тик таймеров
            bot_timer.Tick += bot_timer_Tick;
            ref_timer.Tick += ref_timer_Tick;

            ref_timer.Interval = TimeSpan.FromMilliseconds(100);
            bot_timer.IsEnabled = false;
            ref_timer.IsEnabled = false;

            //сброс флагов изменения текстбоксов
            tbLoginChanged = tbPassChanged = tbMinTimeChanged = tbMaxTimeChanged = tbFireLessChanged = tbInviteFromChanged = tbInviteToChanged = false;


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
            tbLogin.LostFocus += tbLostFocus;

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
            tbMinTime.LostFocus += tbLostFocus;
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
            tbMaxTime.LostFocus += tbLostFocus;
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
            tbFireLess.LostFocus += tbLostFocus;
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
            gbInvite.Width = 225;
            gbInvite.Height = 100;
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
            tbInviteFrom.LostFocus += tbLostFocus;
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
            tbInviteTo.LostFocus += tbLostFocus;
            tbInviteTo.TextChanged += tbTextChanged;

            cbAppoint.Name = "cbAppoint";
            cbAppoint.Content = "На должность:";
            cbAppoint.Height = 20;
            cbAppoint.Margin = new Thickness(5, 70, 0, 0);
            cbAppoint.HorizontalAlignment = HorizontalAlignment.Left;
            cbAppoint.VerticalAlignment = VerticalAlignment.Top;
            cbAppoint.MouseEnter += ShowToolTip;
            cbAppoint.Click += cbClick;

            cboxAppointTo.Name = "cboxAppointTo";
            cboxAppointTo.Items.Add("вице-мэр");
            cboxAppointTo.Items.Add("советник");
            cboxAppointTo.Items.Add("бизнесмен");
            cboxAppointTo.Items.Add("горожанин");
            cboxAppointTo.Height = 23;
            cboxAppointTo.Margin = new Thickness(110, 65, 0, 0);
            cboxAppointTo.HorizontalAlignment = HorizontalAlignment.Left;
            cboxAppointTo.VerticalAlignment = VerticalAlignment.Top;
            cboxAppointTo.Width = 100;
            cboxAppointTo.IsEditable = false;
            cboxAppointTo.MouseEnter += ShowToolTip;
            cboxAppointTo.SelectionChanged += cboxSelectedChanged;

            gInviteData.Name = "gInviteData";
            gInviteData.Children.Add(cbInviteFrom);
            gInviteData.Children.Add(tbInviteFrom);
            gInviteData.Children.Add(cbInviteTo);
            gInviteData.Children.Add(tbInviteTo);
            gInviteData.Children.Add(cbAppoint);
            gInviteData.Children.Add(cboxAppointTo);

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
            user_cfg = BOT.user_cfg;

            tbLogin.Text = user_cfg.Login;
            //если логин не пустой, то в заголовок пишем его
            if (BOT.user_cfg.Login != "")
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
            cbAppoint.IsChecked = Convert.ToBoolean(user_cfg.Appoint);
            cboxAppointTo.SelectedItem = user_cfg.AppointTo;
            spPageHeadImg.Source = new BitmapImage(new Uri("/Resources/" + user_cfg.Avatar, UriKind.Relative));
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
            if (BOT.GetBotThread() != null && BOT.GetBotThread().IsAlive) StopBotThread();

            //переинициалицазия потока бота (иначе считается "живым")
            BOT.ResetThread();
            if (bot_timer.IsEnabled) bot_timer.IsEnabled = false;
            ref_timer.Start();

            timeleft = 0;
            //spPageHeadImg.Source = new BitmapImage(new Uri("/Resources/player-m.png", UriKind.Relative));
            BOT.GetBotThread().Start();
        }

        //если клик по чек-боксу - сохраняем конфиг (в памяти)
        private void cbClick(object sender, EventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                //выбор отображения статистики
                case "cbDoNotShowStatistic":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        BOT.CreateHTMLPage(Properties.Resources.start_off);
                    else
                        BOT.CreateHTMLPage(Properties.Resources.start);

                    wbAction.DocumentText = BOT.CURRENT_HTML;
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

                //лочим комбобокс должностей
                case "cbAppoint":
                    if (((CheckBox)sender).IsChecked.Value == true)
                        cboxAppointTo.IsEnabled = true;
                    else
                        cboxAppointTo.IsEnabled = false;
                    break;
            }            
            SaveUserConfig();
        }

        //событие по смене выбранной позиции в комбобоксе
        private void cboxSelectedChanged(object sender, EventArgs e)
        {
            SaveUserConfig();
        }

        //сохранить настройки
        public void SaveUserConfig()
        {
            if ((Convert.ToBoolean(user_cfg.DoNotSaveThePass)))
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
            user_cfg.Appoint = cbAppoint.IsChecked.Value.ToString().ToLower();
            user_cfg.AppointTo = cboxAppointTo.SelectedItem.ToString();

            //сброс флагов изменения
            tbLoginChanged = tbPassChanged = tbMinTimeChanged = tbMaxTimeChanged = tbFireLessChanged = tbInviteFromChanged = tbInviteToChanged = false;
            //и копируем настройки в бота
            BOT.user_cfg = user_cfg;
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
                    tbLoginChanged = true;
                    break;
                case "tbPass":
                    tbPassChanged = true;
                    break;
                case "tbMinTime":
                    tbMinTimeChanged = true;
                    break;
                case "tbMaxTime":
                    tbMaxTimeChanged = true;
                    break;
                case "tbFireLess":
                    tbFireLessChanged = true;
                    break;
                case "tbInviteFrom":
                    tbInviteFromChanged = true;
                    break;
                case "tbInviteTo":
                    tbInviteToChanged = true;
                    break;
            }
        }

        //событие по выходу их textbox
        private void tbLostFocus(object sender, EventArgs e)
        {
            switch ((sender as Control).Name)
            {
                case "tbLogin":
                    if(tbLoginChanged)
                        user_cfg.Login = tbLogin.Text;
                    break;
                case "tbPass":
                    if (tbPassChanged)
                    {
                        tbPass.Password = Crypto.EncryptStr(tbPass.Password);
                        user_cfg.Pass = tbPass.Password;
                    }
                    break;
                case "tbMinTime":
                    if (tbMinTimeChanged)
                        user_cfg.MinTime = tbMinTime.Text;                                       
                    break;
                case "tbMaxTime":
                    if(tbMaxTimeChanged)
                        user_cfg.MaxTime = tbMaxTime.Text;
                    break;
                case "tbFireLess":
                    if(tbFireLessChanged)
                        user_cfg.FireLess = tbFireLess.Text;
                    break;
                case "tbInviteFrom":
                    if(tbInviteFromChanged)
                        user_cfg.InviteFromMeaning = tbInviteFrom.Text;
                    break;
                case "tbInviteTo":
                    if(tbInviteToChanged)
                        user_cfg.InviteToMeaning = tbInviteTo.Text;
                    break;
            }

            //если хоть что-то поменялось - обновляем конфиг
            if (tbLoginChanged || tbPassChanged || tbMinTimeChanged || tbMaxTimeChanged || tbFireLessChanged || tbInviteFromChanged || tbInviteToChanged)
                SaveUserConfig();
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

            tbAction.Text = string.Format("Всего прогонов: {0}\n", BOT.Action_Count);
            gROOT.Children.Add(tbAction);

            iLift.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/lift.png", UriKind.Absolute));
            iLift.Width = 16;
            iLift.Height = 16;
            tbLift.Text = string.Format("Доставлено пассажиров: {0}", BOT.Lift_Count);
            tbLift.Margin = new Thickness(10, 0, 0, 0);
            gLift.Children.Add(iLift);
            gLift.Children.Add(tbLift);
            gROOT.Children.Add(gLift);

            iBucks.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mn_gold.png", UriKind.Absolute));
            iBucks.Width = 16;
            iBucks.Height = 16;
            tbBucks.Text = string.Format("Собрано баксов: {0}", BOT.Bucks_Count);
            tbBucks.Margin = new Thickness(10, 0, 0, 0);
            gBucks.Children.Add(iBucks);
            gBucks.Children.Add(tbBucks);
            gROOT.Children.Add(gBucks);

            iCoins.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mn_iron.png", UriKind.Absolute));
            iCoins.Width = 16;
            iCoins.Height = 16;
            tbCoins.Text = string.Format("Этажей, с которых собрана выручка: {0}", BOT.Coins_Count);
            tbCoins.Margin = new Thickness(10, 0, 0, 0);
            gCoins.Children.Add(iCoins);
            gCoins.Children.Add(tbCoins);
            gROOT.Children.Add(gCoins);

            iMerch.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/st_stocked.png", UriKind.Absolute));
            iMerch.Width = 16;
            iMerch.Height = 16;
            tbMerch.Text = string.Format("Этажей, на которых выложен товар: {0}", BOT.Merch_Count);
            tbMerch.Margin = new Thickness(10, 0, 0, 0);
            gMerch.Children.Add(iMerch);
            gMerch.Children.Add(tbMerch);
            gROOT.Children.Add(gMerch);

            iBuy.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/tb_empty.png", UriKind.Absolute));
            iBuy.Width = 16;
            iBuy.Height = 16;
            tbBuy.Text = string.Format("Этажей, на которых закуплен товар: {0}", BOT.Buy_Count);
            tbBuy.Margin = new Thickness(10, 0, 0, 0);
            gBuy.Children.Add(iBuy);
            gBuy.Children.Add(tbBuy);
            gROOT.Children.Add(gBuy);

            iKilled.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/users_minus.png", UriKind.Absolute));
            iKilled.Width = 16;
            iKilled.Height = 16;
            tbKilled.Text = string.Format("Выселено дармоедов: {0}", BOT.Killed_Count);
            tbKilled.Margin = new Thickness(10, 0, 0, 0);
            gKilled.Children.Add(iKilled);
            gKilled.Children.Add(tbKilled);
            gROOT.Children.Add(gKilled);

            iNewWorker.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/users_plus.png", UriKind.Absolute));
            iNewWorker.Width = 16;
            iNewWorker.Height = 16;
            tbNewWorker.Text = string.Format("Нанято новых рабочих: {0}", BOT.New_Worker_Count);
            tbNewWorker.Margin = new Thickness(10, 0, 0, 0);
            gNewWorker.Children.Add(iNewWorker);
            gNewWorker.Children.Add(tbNewWorker);
            gROOT.Children.Add(gNewWorker);

            iOpenedFloor.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/hd_nebo.png", UriKind.Absolute));
            iOpenedFloor.Width = 16;
            iOpenedFloor.Height = 16;
            tbOpenedFloor.Text = string.Format("Открыто этажей: {0}", BOT.Opened_Floor_Count);
            tbOpenedFloor.Margin = new Thickness(10, 0, 0, 0);
            gOpenedFloor.Children.Add(iOpenedFloor);
            gOpenedFloor.Children.Add(tbOpenedFloor);
            gROOT.Children.Add(gOpenedFloor);

            iInvited.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/invite.png", UriKind.Absolute));
            iInvited.Width = 16;
            iInvited.Height = 16;
            tbInvited.Text = string.Format("Приглашено игроков: {0}", BOT.Invited_Count);
            tbInvited.Margin = new Thickness(10, 0, 0, 0);
            gInvited.Children.Add(iInvited);
            gInvited.Children.Add(tbInvited);
            gROOT.Children.Add(gInvited);
            return gROOT;
        }
        

        //обновление содержания формы
        private void UpdForm()
        {
            DateTime first = DateTime.Now;
            if (BOT.GetBotThread().IsAlive)
            {
                switch (BOT.CONNECT_STATUS)
                {
                    case " Подключение к серверу ":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connect.png", UriKind.Absolute));
                        break;
                    case " Попытка авторизации ":
                        iStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connect.png", UriKind.Absolute));
                        break;
                }
                switch (BOT.ACTION_STATUS)
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

            lStatus.Text = BOT.CONNECT_STATUS + BOT.ACTION_STATUS;

            //отображаем страницу в раузере            
            if (BOT.PageCreated)
            {
                wbAction.DocumentText = BOT.CURRENT_HTML;
                BOT.PageCreated = false;
            }


            if (user_cfg.Avatar != "")
                spPageHeadImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/" + user_cfg.Avatar, UriKind.Absolute));

            if (!BOT.GetBotThread().IsAlive)
            {
                bStart.IsEnabled = true;
                //wbAction.NavigateToString(CURRENT_HTML);
                //bStop.IsEnabled = false;
            }
            //this.Text = NAME + CONNECT_STATUS + ACTION_STATUS;

            while (BOT.COMMUTATION_STR.Count != 0)
            {
                LOGBox.AppendText(BOT.COMMUTATION_STR[0] + Environment.NewLine);
                LOGBox.ScrollToEnd();
                BOT.COMMUTATION_STR.RemoveAt(0);
            }
            if (BOT.CONNECT_STATUS.Contains("Стоп"))
            {
                bot_timer.Interval = BOT.BotTimeSleep;
                timeleft = (int)(bot_timer.Interval.TotalMilliseconds);
                bot_timer.Start();
                BOT.CONNECT_STATUS = "";
                BOT.ACTION_STATUS = "";
                //wbAction.NavigateToString(CURRENT_HTML);
            }

            if (timeleft > 0)
            {
                timeleft -= 100 + (DateTime.Now - first).Milliseconds;
                int minutes = (int)TimeSpan.FromMilliseconds(timeleft).TotalMinutes;
                int seconds = (int)TimeSpan.FromMilliseconds(timeleft).TotalSeconds - minutes * 60;
                BOT.CONNECT_STATUS = string.Format("Жду   {0}мин : {1:d2}сек\n\n", minutes, seconds);
            }
        }        


        //событие таймера
        private void bot_timer_Tick(object sender, EventArgs e)
        {
            //сбрасываем таймер обратного отсчета
            timeleft = 0;

            bot_timer.Stop();
            bStart.IsEnabled = false;
            bStop.IsEnabled = true;
            ref_timer.Start();

            BOT.ResetThread();
            BOT.GetBotThread().Start();
        }

        //остановка бота и очистка синхрострок
        public void StopBotThread()
        {
            BOT.GetBotThread().Abort();
            BOT.CONNECT_STATUS = "";
            BOT.ACTION_STATUS = "";
            bot_timer.Stop();
            ref_timer.Stop();
            bStart.IsEnabled = true;
            bStop.IsEnabled = false;
            timeleft = 0;
            BOT.User_Level = BOT.User_Coins = BOT.User_Bucks = "";
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

        //вернуть поток бота (для главного окна)
        public Thread GetBotThread()
        {
            return BOT.GetBotThread();
        }

        //вернуть конфиги юзера
        public Config.User GetUserCfg()
        {
            return user_cfg;
        }
    }
}

