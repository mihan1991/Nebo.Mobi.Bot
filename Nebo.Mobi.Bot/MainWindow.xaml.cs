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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Nebo.Mobi.Bot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    [Serializable]
    public partial class MainWindow : Window
    {
        private string version = "2.2b";                          //версия бота
        private Config cfg;                                      //хранилище настроек
        private List<UI> ui;                                     //коллекция объектов страниц профиля и работы боты

        private string Current_Path;                             //строка, содержащая папку с ботом
        private bool AutorunChanged;                             //для фиксации изменения состояния автозагрузки
        private System.Windows.Forms.NotifyIcon tray_icon;       //иконка в трее
        private uint _savedVolume;                               //глушим звук

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);


        public MainWindow()
        {
            InitializeComponent();

            //пока так с tray_icon
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/bot.ico", UriKind.Absolute));
            tray_icon = new System.Windows.Forms.NotifyIcon();// System.ComponentModel.IContainer
            tray_icon.Icon = Properties.Resources.bot;
            tray_icon.Visible = true;
            tray_icon.Click += new EventHandler(TrayIcon_Click);
        }

        //удаление персонажа
        private void RemoveUser()
        {
            MessageBoxResult mbr = MessageBox.Show("Вы действительно хотите закрыть персонажа?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                int index = tcUsers.SelectedIndex;                
                cfg.users.RemoveAt(index);
                cfg.UsersCount = cfg.users.Count;
                tcUsers.Items.RemoveAt(index);
                ui.RemoveAt(index);
            }
        }

        //обработчик глобальных чекбоксов
        private void cb_Click(object sender, EventArgs e)
        {
            switch ((sender as CheckBox).Name)
            {
                case "cbHide":
                    cfg.Hide = cbHide.IsChecked.Value.ToString().ToLower();
                    break;
                case "cbAutorun":
                    if (cbAutorun.IsChecked.Value)
                        cbHide.IsEnabled = true;
                    else cbHide.IsEnabled = false;
                    AutorunChanged = true;
                    cfg.Autorun = cbAutorun.IsChecked.Value.ToString().ToLower();
                    break;
            }
        }

        //обработчик нажатия кнопок
        private void button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "bAddUser":
                    AddUser();
                    break;
                case "bSave":
                    SaveConfig();
                    break;
                case "bRemoveUser":
                    RemoveUser();
                    break;
                case "bStartAll":
                    for (int i = 0; i < ui.Count; i++)
                    {
                        if (!ui[i].GetBotThread().IsAlive)
                            ui[i].StartBotThread();
                    }
                    break;
                case "bStopAll":
                    for (int i = 0; i < ui.Count; i++)
                    {
                        ui[i].StopBotThread();
                    }
                    break;
            }
        }

        //обработчик клика на TrayIcon
        private void TrayIcon_Click(object sender, EventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
                this.Hide();
            else
            {
                this.Show();
                //для того, чтобы окно развернулось, но перестало быть топмост
                this.Topmost = true;
                this.Topmost = false;

                if (this.WindowState == System.Windows.WindowState.Minimized)
                    this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        
        //добавляем пустого пользователя
        public void AddUser()
        {
            UI tib = new UI();                                  //создаем новую страницу
            ui.Add(tib);                                        //дабы не просрать
            tcUsers.Items.Add(tib.GetPage());                   //добавляем в контейнер
            tcUsers.SelectedIndex = tcUsers.Items.Count - 1;    //делаем активной
            tib.Get_bClose().Click += button_Click;             //вешаем событие на нажатие кнопки "Закрыть"
            tib.Get_bClose().MouseEnter += ToolTip_MouseEnter;  //вешаем событие на наведение курсора
            cfg.AddUser(tib.GetUserCfg());                      //добавляем юзер конфиг
        }

        //добавляем сохраненного пользователя
        public void AddUser(Config.User u)
        {
            UI tib = new UI(u);                                 //создаем новую страницу
            ui.Add(tib);                                        //дабы не просрать
            tcUsers.Items.Add(tib.GetPage());                   //добавляем в контейнер
            tcUsers.SelectedIndex = tcUsers.Items.Count - 1;    //делаем активной
            tib.Get_bClose().Click += button_Click;             //вешаем событие на нажатие кнопки "Закрыть"
            tib.Get_bClose().MouseEnter += ToolTip_MouseEnter;  //вешаем событие на наведение курсора
        }

        private void ToolTip_MouseEnter(object sender, MouseEventArgs e)
        {
            ToolTip tt = new System.Windows.Controls.ToolTip();

            //TextBlock tbHeader = new TextBlock();
            //tbHeader.FontWeight = FontWeights.Bold;
            //tbHeader.Text = "Глобальное действие";
            TextBlock tbText = new TextBlock();
            //tbText.FontWeight = FontWeights.Bold;
            try
            {
                tbText.Text = Properties.Resources.ResourceManager.GetString((sender as Control).Name);
            }
            catch
            {
                tbText.Text = "???";
            }
            StackPanel sp = new StackPanel();
            //sp.Children.Add(tbHeader);
            sp.Children.Add(tbText);
            tt.Content = sp;
            (sender as Control).ToolTip = tt;
        }

        //сохранение всех настроек
        private void SaveConfig()
        {
            for (int i = 0; i < cfg.UsersCount; i++)
            {
                ui[i].SaveUserConfig();
            }

            //сохраняем расмер окна или фиксируем maximized
            if (!(this.WindowState == System.Windows.WindowState.Maximized))
            {
                cfg.SizeX = this.Width.ToString();
                cfg.SizeY = this.Height.ToString();
            }
            else
            {
                cfg.SizeX = "MAX";
                cfg.SizeY = "MAX";
            }

            //модификация автозапуска
            string FullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            if (AutorunChanged)
            {
                if (cbAutorun.IsChecked.Value)
                {
                    Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                    myKey.SetValue("Nebo.Mobi.Bot", FullPath);
                }

                else
                {
                    Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                    myKey.DeleteValue("Nebo.Mobi.Bot", false);
                }
                AutorunChanged = false;
            }

            List<Config.User> usr = new List<Config.User>();
            for (int i = 0; i < ui.Count; i++)
                usr.Add(ui[i].GetUserCfg());
            cfg.WriteConfig(usr);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            //проверка автостарта
            //проверка - было ли ручное отключения из автозагрузки
            Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", false);
            if (myKey.GetValue("Nebo.Mobi.Bot", "").ToString() == "")
            {
                //снимаем галку авторана
                cbAutorun.IsChecked = false;
            }

            //на другой оси сохранили без авторана
            else
                cbAutorun.IsChecked = true;

            //активировать возможность включения запуска в скрытом виде только если возможен автостарт
            if (cbAutorun.IsChecked.Value)
                cbHide.IsEnabled = true;
            else cbHide.IsEnabled = false;

            //получаем директорию с настройками. если нет - создаем
            Current_Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Nebo.Mobi.Bot/";
            if(!(new DirectoryInfo(Current_Path).Exists))
            {
                Directory.CreateDirectory(Current_Path);
            }
            //Current_Path = new FileInfo(AppDomain.CurrentDomain.BaseDirectory).ToString();

            //подгружаем настройки
            cfg = new Config(Current_Path);

            //пробуем задать размер окна из настроек
            try
            {
                this.Width = Convert.ToInt32(cfg.SizeX);
                this.Height = Convert.ToInt32(cfg.SizeY);
            }

            catch
            {
                //если не получается проверяем - не maximized ли
                if (cfg.SizeX == "MAX" && cfg.SizeY == "MAX")
                    this.WindowState = System.Windows.WindowState.Maximized;
                else
                {
                    this.Width = this.MinWidth;
                    this.Height = this.MinHeight;
                }
            }

            //глушим все звуки
            waveOutGetVolume(IntPtr.Zero, out _savedVolume);
            waveOutSetVolume(IntPtr.Zero, 0);

            //инициализируем коллекцию
            ui = new List<UI>();

            //удаляем шаблонный (костыль)
            tcUsers.Items.RemoveAt(0);

            cboxAppointTo.Items.Add("вице-мэр");
            cboxAppointTo.Items.Add("советник");
            cboxAppointTo.Items.Add("бизнесмен");
            cboxAppointTo.Items.Add("горожанин");
            cboxAppointTo.SelectedItem = "горожанин";

            //добавляем страницы пользователей
            for (int i = 0; i < cfg.UsersCount; i++)
            {
                AddUser(cfg.users[i]);
            }
            lCopyright.Content = "Exclusive by Mr.President  ©  2014 - 2016." + "  ver. " + version;

            if (cbAutorun.IsChecked.Value)
                for (int i = 0; i < ui.Count; i++)
                {
                    try
                    {
                        ui[i].StartBotThread();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            cbHide.IsChecked = Convert.ToBoolean(cfg.Hide);

            if (cbHide.IsChecked.Value && cbHide.IsEnabled)
            {
                //this.WindowState = System.Windows.WindowState.Minimized;
                this.Hide();
            }
        }

        //при закрытии окна отключить все потоки
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < ui.Count; i++)
                try
                {
                    ui[i].GetBotThread().Abort();
                }
                catch { }
            waveOutSetVolume(IntPtr.Zero, _savedVolume);
            tray_icon.Visible = false;
        }

        //метод клика левой кнопкой по верхней полосе (двигать окно)
        private void MovieWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
            
        
    }
}
