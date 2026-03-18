using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HwidSpoofer
{
    public class MainForm : Form
    {
        private Label hwidLabel = null!;
        private Label launcherLabel = null!;
        private Button spoofButton = null!;
        private RadioButton hwidOnlyRadio = null!;
        private RadioButton launcherOnlyRadio = null!;
        private RadioButton allRadio = null!;
        private System.Windows.Forms.Timer checkTimer = null!;
        private bool isProcessing = false;

        private const string RegistryPath = @"Software\Space Wizards\Robust";
        private string LauncherPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Space Station 14\launcher"
        );

        public MainForm()
        {
            InitializeComponent();
            SetupTimer();
            CheckStatus();
        }

        private void InitializeComponent()
        {
            this.Text = "t.me/RobusterHome";
            this.Size = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            
            try
            {
                this.Icon = new Icon("ico.ico");
            }
            catch
            {
            }

            hwidLabel = new Label
            {
                AutoSize = false,
                Size = new Size(350, 30),
                Location = new Point(25, 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };

            launcherLabel = new Label
            {
                AutoSize = false,
                Size = new Size(350, 30),
                Location = new Point(25, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };

            hwidOnlyRadio = new RadioButton
            {
                Text = "Только HWID",
                Location = new Point(50, 100),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Checked = true
            };

            launcherOnlyRadio = new RadioButton
            {
                Text = "Только данные лаунчера",
                Location = new Point(50, 130),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };

            var launcherWarning = new Label
            {
                Text = "(слетят аккаунты!)",
                Location = new Point(235, 130),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleLeft
            };

            allRadio = new RadioButton
            {
                Text = "Все вместе",
                Location = new Point(50, 160),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };

            var allWarning = new Label
            {
                Text = "(слетят аккаунты!)",
                Location = new Point(155, 160),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleLeft
            };

            spoofButton = new Button
            {
                Text = "Spoof!",
                Size = new Size(200, 50),
                Location = new Point(100, 200),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            spoofButton.FlatAppearance.BorderSize = 0;
            spoofButton.Click += SpoofButton_Click;

            this.Controls.Add(hwidLabel);
            this.Controls.Add(launcherLabel);
            this.Controls.Add(hwidOnlyRadio);
            this.Controls.Add(launcherOnlyRadio);
            this.Controls.Add(launcherWarning);
            this.Controls.Add(allRadio);
            this.Controls.Add(allWarning);
            this.Controls.Add(spoofButton);
        }

        private void SetupTimer()
        {
            checkTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            checkTimer.Tick += (s, e) => CheckStatus();
            checkTimer.Start();
        }

        private void CheckStatus()
        {
            bool hwidFound = CheckHwidInRegistry();
            bool launcherFound = Directory.Exists(LauncherPath);

            hwidLabel.Text = hwidFound ? "HWID: Найден" : "HWID: Не найден";
            UpdateLabelColor(hwidLabel, "HWID: ", hwidFound);

            launcherLabel.Text = launcherFound ? "Дата лаунчера: Найдена" : "Дата лаунчера: Не найдена";
            UpdateLabelColor(launcherLabel, "Дата лаунчера: ", launcherFound);
        }

        private void UpdateLabelColor(Label label, string prefix, bool isFound)
        {
            label.ForeColor = Color.White;
            
            label.Paint += (s, e) =>
            {
                e.Graphics.Clear(this.BackColor);
                
                var prefixSize = e.Graphics.MeasureString(prefix, label.Font);
                e.Graphics.DrawString(prefix, label.Font, Brushes.White, 
                    (label.Width - e.Graphics.MeasureString(label.Text, label.Font).Width) / 2, 
                    (label.Height - prefixSize.Height) / 2);
                
                var valueText = label.Text.Substring(prefix.Length);
                var valueBrush = isFound ? Brushes.Red : Brushes.LimeGreen;
                e.Graphics.DrawString(valueText, label.Font, valueBrush,
                    (label.Width - e.Graphics.MeasureString(label.Text, label.Font).Width) / 2 + prefixSize.Width,
                    (label.Height - prefixSize.Height) / 2);
            };
            
            label.Invalidate();
        }

        private bool CheckHwidInRegistry()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key == null) return false;
                    return key.GetValue("hwid1") != null || key.GetValue("hwid2") != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private async void SpoofButton_Click(object? sender, EventArgs e)
        {
            if (isProcessing) return;

            if (launcherOnlyRadio.Checked)
            {
                var result = MessageBox.Show(
                    "ВНИМАНИЕ! Будут удалены ВСЕ аккаунты и настройки лаунчера!\nПродолжить?",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result != DialogResult.Yes) return;
            }
            else if (allRadio.Checked)
            {
                var result = MessageBox.Show(
                    "ВНИМАНИЕ! Будут удалены HWID И ВСЕ аккаунты и настройки лаунчера!\nПродолжить?",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result != DialogResult.Yes) return;
            }

            isProcessing = true;
            spoofButton.Enabled = false;

            await Task.Run(() =>
            {
                if (hwidOnlyRadio.Checked || allRadio.Checked)
                {
                    DeleteHwidFromRegistry();
                }
                
                if (launcherOnlyRadio.Checked || allRadio.Checked)
                {
                    DeleteLauncherFolder();
                }
            });

            MessageBox.Show(
                "Успешно! Не забудь поменять айпи адрес и входи в новый аккаунт!",
                "Успешно",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            spoofButton.Enabled = true;
            isProcessing = false;
            CheckStatus();
        }

        private void DeleteHwidFromRegistry()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
                {
                    if (key != null)
                    {
                        try { key.DeleteValue("hwid1", false); } catch { }
                        try { key.DeleteValue("hwid2", false); } catch { }
                    }
                }
            }
            catch { }
        }

        private void DeleteLauncherFolder()
        {
            try
            {
                if (Directory.Exists(LauncherPath))
                {
                    Directory.Delete(LauncherPath, true);
                }
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                checkTimer?.Stop();
                checkTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
