﻿namespace Nebo.Mobi.Bot
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.LOGBox = new System.Windows.Forms.RichTextBox();
            this.lLogin = new System.Windows.Forms.Label();
            this.lPass = new System.Windows.Forms.Label();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.bStart = new System.Windows.Forms.Button();
            this.bStop = new System.Windows.Forms.Button();
            this.lLOG = new System.Windows.Forms.Label();
            this.lCopyright = new System.Windows.Forms.Label();
            this.ref_timer = new System.Windows.Forms.Timer(this.components);
            this.lMinTime = new System.Windows.Forms.Label();
            this.lMaxTime = new System.Windows.Forms.Label();
            this.tbMinTime = new System.Windows.Forms.TextBox();
            this.tbMaxTime = new System.Windows.Forms.TextBox();
            this.lDiapazon = new System.Windows.Forms.Label();
            this.lUserInfo = new System.Windows.Forms.Label();
            this.cbDoNotPut = new System.Windows.Forms.CheckBox();
            this.tbFireLess = new System.Windows.Forms.TextBox();
            this.cbFire = new System.Windows.Forms.CheckBox();
            this.cbFire9 = new System.Windows.Forms.CheckBox();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cbDoNotSaveThePass = new System.Windows.Forms.CheckBox();
            this.bSave = new System.Windows.Forms.Button();
            this.gbStats = new System.Windows.Forms.GroupBox();
            this.lLevel = new System.Windows.Forms.Label();
            this.lGold = new System.Windows.Forms.Label();
            this.lCoin = new System.Windows.Forms.Label();
            this.pbLevel = new System.Windows.Forms.PictureBox();
            this.pbGold = new System.Windows.Forms.PictureBox();
            this.pbCoin = new System.Windows.Forms.PictureBox();
            this.cbAutorun = new System.Windows.Forms.CheckBox();
            this.cbDoNotGetRevard = new System.Windows.Forms.CheckBox();
            this.cbHide = new System.Windows.Forms.CheckBox();
            this.gbStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoin)).BeginInit();
            this.SuspendLayout();
            // 
            // LOGBox
            // 
            this.LOGBox.Location = new System.Drawing.Point(13, 189);
            this.LOGBox.Name = "LOGBox";
            this.LOGBox.Size = new System.Drawing.Size(1219, 326);
            this.LOGBox.TabIndex = 9;
            this.LOGBox.Text = "";
            // 
            // lLogin
            // 
            this.lLogin.AutoSize = true;
            this.lLogin.Location = new System.Drawing.Point(13, 24);
            this.lLogin.Name = "lLogin";
            this.lLogin.Size = new System.Drawing.Size(41, 13);
            this.lLogin.TabIndex = 1;
            this.lLogin.Text = "Логин:";
            // 
            // lPass
            // 
            this.lPass.AutoSize = true;
            this.lPass.Location = new System.Drawing.Point(12, 49);
            this.lPass.Name = "lPass";
            this.lPass.Size = new System.Drawing.Size(48, 13);
            this.lPass.TabIndex = 1;
            this.lPass.Text = "Пароль:";
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(60, 21);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(145, 20);
            this.tbLogin.TabIndex = 1;
            this.tbLogin.TextChanged += new System.EventHandler(this.tbLogin_TextChanged);
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(59, 46);
            this.tbPass.Name = "tbPass";
            this.tbPass.Size = new System.Drawing.Size(145, 20);
            this.tbPass.TabIndex = 2;
            this.tbPass.TextChanged += new System.EventHandler(this.tbPass_TextChanged);
            this.tbPass.Leave += new System.EventHandler(this.tbPass_Leave);
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(13, 103);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(75, 23);
            this.bStart.TabIndex = 3;
            this.bStart.Text = "Старт";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // bStop
            // 
            this.bStop.Location = new System.Drawing.Point(157, 101);
            this.bStop.Name = "bStop";
            this.bStop.Size = new System.Drawing.Size(75, 23);
            this.bStop.TabIndex = 4;
            this.bStop.Text = "Стоп";
            this.bStop.UseVisualStyleBackColor = true;
            this.bStop.Click += new System.EventHandler(this.bStop_Click);
            // 
            // lLOG
            // 
            this.lLOG.AutoSize = true;
            this.lLOG.Location = new System.Drawing.Point(13, 173);
            this.lLOG.Name = "lLOG";
            this.lLOG.Size = new System.Drawing.Size(60, 13);
            this.lLOG.TabIndex = 4;
            this.lLOG.Text = "Действия:";
            // 
            // lCopyright
            // 
            this.lCopyright.AutoSize = true;
            this.lCopyright.Location = new System.Drawing.Point(513, 522);
            this.lCopyright.Name = "lCopyright";
            this.lCopyright.Size = new System.Drawing.Size(176, 13);
            this.lCopyright.TabIndex = 5;
            this.lCopyright.Text = "Exclusive by Mr.President  ©  2014.";
            // 
            // ref_timer
            // 
            this.ref_timer.Interval = 20;
            this.ref_timer.Tick += new System.EventHandler(this.ref_timer_Tick);
            // 
            // lMinTime
            // 
            this.lMinTime.AutoSize = true;
            this.lMinTime.Location = new System.Drawing.Point(267, 28);
            this.lMinTime.Name = "lMinTime";
            this.lMinTime.Size = new System.Drawing.Size(23, 13);
            this.lMinTime.TabIndex = 1;
            this.lMinTime.Text = "От:";
            // 
            // lMaxTime
            // 
            this.lMaxTime.AutoSize = true;
            this.lMaxTime.Location = new System.Drawing.Point(267, 53);
            this.lMaxTime.Name = "lMaxTime";
            this.lMaxTime.Size = new System.Drawing.Size(25, 13);
            this.lMaxTime.TabIndex = 1;
            this.lMaxTime.Text = "До:";
            // 
            // tbMinTime
            // 
            this.tbMinTime.Location = new System.Drawing.Point(298, 25);
            this.tbMinTime.Name = "tbMinTime";
            this.tbMinTime.Size = new System.Drawing.Size(145, 20);
            this.tbMinTime.TabIndex = 5;
            this.tbMinTime.Text = "2";
            this.tbMinTime.TextChanged += new System.EventHandler(this.tbMinTime_TextChanged);
            // 
            // tbMaxTime
            // 
            this.tbMaxTime.Location = new System.Drawing.Point(298, 50);
            this.tbMaxTime.Name = "tbMaxTime";
            this.tbMaxTime.Size = new System.Drawing.Size(145, 20);
            this.tbMaxTime.TabIndex = 6;
            this.tbMaxTime.Text = "25";
            this.tbMaxTime.TextChanged += new System.EventHandler(this.tbMaxTime_TextChanged);
            // 
            // lDiapazon
            // 
            this.lDiapazon.AutoSize = true;
            this.lDiapazon.Location = new System.Drawing.Point(267, 5);
            this.lDiapazon.Name = "lDiapazon";
            this.lDiapazon.Size = new System.Drawing.Size(131, 13);
            this.lDiapazon.TabIndex = 4;
            this.lDiapazon.Text = "Диапазон повтора (мин)";
            // 
            // lUserInfo
            // 
            this.lUserInfo.AutoSize = true;
            this.lUserInfo.Location = new System.Drawing.Point(13, 5);
            this.lUserInfo.Name = "lUserInfo";
            this.lUserInfo.Size = new System.Drawing.Size(88, 13);
            this.lUserInfo.TabIndex = 4;
            this.lUserInfo.Text = "Учетная запись";
            // 
            // cbDoNotPut
            // 
            this.cbDoNotPut.AutoSize = true;
            this.cbDoNotPut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDoNotPut.Location = new System.Drawing.Point(477, 27);
            this.cbDoNotPut.Name = "cbDoNotPut";
            this.cbDoNotPut.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbDoNotPut.Size = new System.Drawing.Size(145, 17);
            this.cbDoNotPut.TabIndex = 10;
            this.cbDoNotPut.Text = "НЕ выкладывать товар";
            this.cbDoNotPut.UseVisualStyleBackColor = true;
            // 
            // tbFireLess
            // 
            this.tbFireLess.Location = new System.Drawing.Point(693, 42);
            this.tbFireLess.Name = "tbFireLess";
            this.tbFireLess.Size = new System.Drawing.Size(25, 20);
            this.tbFireLess.TabIndex = 6;
            this.tbFireLess.Text = "9";
            // 
            // cbFire
            // 
            this.cbFire.AutoSize = true;
            this.cbFire.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFire.Checked = true;
            this.cbFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFire.Location = new System.Drawing.Point(477, 45);
            this.cbFire.Name = "cbFire";
            this.cbFire.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbFire.Size = new System.Drawing.Size(210, 17);
            this.cbFire.TabIndex = 10;
            this.cbFire.Text = "Выселять жильцов с уровнем ниже ";
            this.cbFire.UseVisualStyleBackColor = true;
            this.cbFire.CheckedChanged += new System.EventHandler(this.cbFire_CheckedChanged);
            // 
            // cbFire9
            // 
            this.cbFire9.AutoSize = true;
            this.cbFire9.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFire9.Checked = true;
            this.cbFire9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFire9.Location = new System.Drawing.Point(477, 68);
            this.cbFire9.Name = "cbFire9";
            this.cbFire9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbFire9.Size = new System.Drawing.Size(241, 17);
            this.cbFire9.TabIndex = 10;
            this.cbFire9.Text = "(-) Выселять жильцов 9 уровня со знаком ";
            this.cbFire9.UseVisualStyleBackColor = true;
            this.cbFire9.CheckedChanged += new System.EventHandler(this.cbFire_CheckedChanged);
            // 
            // TrayIcon
            // 
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "Nebo.Mobi.Bot";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // cbDoNotSaveThePass
            // 
            this.cbDoNotSaveThePass.AutoSize = true;
            this.cbDoNotSaveThePass.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDoNotSaveThePass.Location = new System.Drawing.Point(16, 68);
            this.cbDoNotSaveThePass.Name = "cbDoNotSaveThePass";
            this.cbDoNotSaveThePass.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbDoNotSaveThePass.Size = new System.Drawing.Size(135, 17);
            this.cbDoNotSaveThePass.TabIndex = 11;
            this.cbDoNotSaveThePass.Text = "НЕ сохранять пароль";
            this.cbDoNotSaveThePass.UseVisualStyleBackColor = true;
            this.cbDoNotSaveThePass.Click += new System.EventHandler(this.cbDoNotSaveThePass_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(157, 72);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 4;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // gbStats
            // 
            this.gbStats.Controls.Add(this.lLevel);
            this.gbStats.Controls.Add(this.lGold);
            this.gbStats.Controls.Add(this.lCoin);
            this.gbStats.Controls.Add(this.pbLevel);
            this.gbStats.Controls.Add(this.pbGold);
            this.gbStats.Controls.Add(this.pbCoin);
            this.gbStats.Location = new System.Drawing.Point(19, 521);
            this.gbStats.Name = "gbStats";
            this.gbStats.Size = new System.Drawing.Size(271, 82);
            this.gbStats.TabIndex = 12;
            this.gbStats.TabStop = false;
            this.gbStats.Text = "О персонаже";
            this.gbStats.Visible = false;
            // 
            // lLevel
            // 
            this.lLevel.AutoSize = true;
            this.lLevel.Location = new System.Drawing.Point(135, 49);
            this.lLevel.Name = "lLevel";
            this.lLevel.Size = new System.Drawing.Size(0, 13);
            this.lLevel.TabIndex = 13;
            // 
            // lGold
            // 
            this.lGold.AutoSize = true;
            this.lGold.Location = new System.Drawing.Point(69, 49);
            this.lGold.Name = "lGold";
            this.lGold.Size = new System.Drawing.Size(0, 13);
            this.lGold.TabIndex = 13;
            // 
            // lCoin
            // 
            this.lCoin.AutoSize = true;
            this.lCoin.Location = new System.Drawing.Point(6, 49);
            this.lCoin.Name = "lCoin";
            this.lCoin.Size = new System.Drawing.Size(0, 13);
            this.lCoin.TabIndex = 13;
            // 
            // pbLevel
            // 
            this.pbLevel.Image = ((System.Drawing.Image)(resources.GetObject("pbLevel.Image")));
            this.pbLevel.Location = new System.Drawing.Point(138, 20);
            this.pbLevel.Name = "pbLevel";
            this.pbLevel.Size = new System.Drawing.Size(34, 26);
            this.pbLevel.TabIndex = 0;
            this.pbLevel.TabStop = false;
            // 
            // pbGold
            // 
            this.pbGold.Image = ((System.Drawing.Image)(resources.GetObject("pbGold.Image")));
            this.pbGold.Location = new System.Drawing.Point(70, 20);
            this.pbGold.Name = "pbGold";
            this.pbGold.Size = new System.Drawing.Size(34, 26);
            this.pbGold.TabIndex = 0;
            this.pbGold.TabStop = false;
            // 
            // pbCoin
            // 
            this.pbCoin.Image = ((System.Drawing.Image)(resources.GetObject("pbCoin.Image")));
            this.pbCoin.Location = new System.Drawing.Point(7, 20);
            this.pbCoin.Name = "pbCoin";
            this.pbCoin.Size = new System.Drawing.Size(34, 26);
            this.pbCoin.TabIndex = 0;
            this.pbCoin.TabStop = false;
            // 
            // cbAutorun
            // 
            this.cbAutorun.AutoSize = true;
            this.cbAutorun.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAutorun.Checked = true;
            this.cbAutorun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutorun.Location = new System.Drawing.Point(477, 91);
            this.cbAutorun.Name = "cbAutorun";
            this.cbAutorun.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbAutorun.Size = new System.Drawing.Size(148, 17);
            this.cbAutorun.TabIndex = 10;
            this.cbAutorun.Text = "Автозапуск и автостарт";
            this.cbAutorun.UseVisualStyleBackColor = true;
            this.cbAutorun.Click += new System.EventHandler(this.cbAutorun_Click);
            // 
            // cbDoNotGetRevard
            // 
            this.cbDoNotGetRevard.AutoSize = true;
            this.cbDoNotGetRevard.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDoNotGetRevard.Location = new System.Drawing.Point(628, 27);
            this.cbDoNotGetRevard.Name = "cbDoNotGetRevard";
            this.cbDoNotGetRevard.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbDoNotGetRevard.Size = new System.Drawing.Size(137, 17);
            this.cbDoNotGetRevard.TabIndex = 10;
            this.cbDoNotGetRevard.Text = "НЕ собирать награды";
            this.cbDoNotGetRevard.UseVisualStyleBackColor = true;
            // 
            // cbHide
            // 
            this.cbHide.AutoSize = true;
            this.cbHide.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbHide.Location = new System.Drawing.Point(631, 91);
            this.cbHide.Name = "cbHide";
            this.cbHide.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbHide.Size = new System.Drawing.Size(138, 17);
            this.cbHide.TabIndex = 10;
            this.cbHide.Text = "Запускать свёрнутым";
            this.cbHide.UseVisualStyleBackColor = true;
            this.cbHide.Click += new System.EventHandler(this.cbHide_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(1244, 615);
            this.Controls.Add(this.gbStats);
            this.Controls.Add(this.cbDoNotSaveThePass);
            this.Controls.Add(this.cbAutorun);
            this.Controls.Add(this.cbFire9);
            this.Controls.Add(this.cbFire);
            this.Controls.Add(this.cbHide);
            this.Controls.Add(this.cbDoNotGetRevard);
            this.Controls.Add(this.cbDoNotPut);
            this.Controls.Add(this.lCopyright);
            this.Controls.Add(this.lUserInfo);
            this.Controls.Add(this.lDiapazon);
            this.Controls.Add(this.lLOG);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.tbPass);
            this.Controls.Add(this.tbFireLess);
            this.Controls.Add(this.tbMaxTime);
            this.Controls.Add(this.tbMinTime);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.lMaxTime);
            this.Controls.Add(this.lPass);
            this.Controls.Add(this.lMinTime);
            this.Controls.Add(this.lLogin);
            this.Controls.Add(this.LOGBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Небоскребы. Бот";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.gbStats.ResumeLayout(false);
            this.gbStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LOGBox;
        private System.Windows.Forms.Label lLogin;
        private System.Windows.Forms.Label lPass;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button bStop;
        private System.Windows.Forms.Label lLOG;
        private System.Windows.Forms.Label lCopyright;
        private System.Windows.Forms.Timer ref_timer;
        private System.Windows.Forms.Label lMinTime;
        private System.Windows.Forms.Label lMaxTime;
        private System.Windows.Forms.TextBox tbMinTime;
        private System.Windows.Forms.TextBox tbMaxTime;
        private System.Windows.Forms.Label lDiapazon;
        private System.Windows.Forms.Label lUserInfo;
        private System.Windows.Forms.CheckBox cbDoNotPut;
        private System.Windows.Forms.TextBox tbFireLess;
        private System.Windows.Forms.CheckBox cbFire;
        private System.Windows.Forms.CheckBox cbFire9;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.CheckBox cbDoNotSaveThePass;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.GroupBox gbStats;
        private System.Windows.Forms.Label lLevel;
        private System.Windows.Forms.Label lGold;
        private System.Windows.Forms.Label lCoin;
        private System.Windows.Forms.PictureBox pbLevel;
        private System.Windows.Forms.PictureBox pbGold;
        private System.Windows.Forms.PictureBox pbCoin;
        private System.Windows.Forms.CheckBox cbAutorun;
        private System.Windows.Forms.CheckBox cbDoNotGetRevard;
        private System.Windows.Forms.CheckBox cbHide;
    }
}

