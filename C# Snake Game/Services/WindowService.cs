using System.Drawing;
using System.Windows.Forms;

namespace CleanSnakeGame.Services
{
    public class WindowService
    {
        private readonly Form form;

        public WindowService(Form targetForm)
        {
            form = targetForm;
        }

        public void ApplyFullscreenSetting()
        {
            if (SettingsManager.Settings.Fullscreen)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.TopMost = true;
            }
            else
            {
                form.TopMost = false;
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                form.WindowState = FormWindowState.Normal;
                form.Size = new Size(1024, 768);
                //form.CenterToScreen();
            }

            form.Refresh();
        }

        public void ToggleFullscreen()
        {
            SettingsManager.Settings.Fullscreen = !SettingsManager.Settings.Fullscreen;

            SettingsManager.SaveSettings();

            ApplyFullscreenSetting();
        }
    }
}