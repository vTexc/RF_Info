using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeHUD.Plugins;
using PoeHUD.Hud;
using PoeHUD.Poe.Components;
using PoeHUD.Controllers;
using SharpDX;
using System.Windows.Forms;

namespace RF_Info
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        public Core()
        {
            PluginName = "RF Info";
        }
        
        private RFSkill skill = new RFSkill();
        private RectangleF textBox;
        private string[] textInfo =
            {
                "Raw Regen",
                "Raw Degen",
                "Dmg Taken",
                "Total %",
                "Total"
            };

        private Vector2 lastPos = new Vector2();

        public override void Initialise()
        {
            textBox = new RectangleF();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnPluginDestroyForHotReload()
        {
            base.OnPluginDestroyForHotReload();
        }
        
        public override void Render()
        {
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            int borderOffScreen = Settings.BorderSize / 2 + 1;
            lastPos.X = (BasePlugin.API.GameController.Window.GetWindowRectangle().Width - textBox.Width - borderOffScreen) * (Settings.PosX / 100) + borderOffScreen;
            lastPos.Y = (BasePlugin.API.GameController.Window.GetWindowRectangle().Height - textBox.Height - borderOffScreen) * (Settings.PosY / 100) + borderOffScreen;

            UpdateBox();
            UpdateText();
        }

        private void UpdateText()
        {
            skill.UpdateInfo();

            float fakeTextBoxHeight = textBox.Height - Settings.Padding * 2;
            float fakeTextPosY = (textBox.Height - fakeTextBoxHeight)/2;

            lastPos.X += Settings.Padding;
            lastPos.Y += fakeTextPosY;

            foreach (string text in textInfo)
            {
                Graphics.DrawText(text, Settings.TextSize, lastPos, Settings.TextColor, SharpDX.Direct3D9.FontDrawFlags.Left);

                lastPos.X += textBox.Width - Settings.Padding * 2;

                Graphics.DrawText(skill.GetValue(text), Settings.TextSize, lastPos, Settings.TextColor, SharpDX.Direct3D9.FontDrawFlags.Right);

                lastPos.X -= textBox.Width - Settings.Padding * 2;
                lastPos.Y += Settings.TextSize;
            }
            
            // Draw the Dot DPS
            lastPos.Y += Settings.TextSize;
            Graphics.DrawText("DoT DPS", Settings.TextSize, lastPos, Settings.TextColor, SharpDX.Direct3D9.FontDrawFlags.Left);
            lastPos.X += textBox.Width - Settings.Padding * 2;
            Graphics.DrawText(skill.GetValue("DoT DPS"), Settings.TextSize, lastPos, Settings.TextColor, SharpDX.Direct3D9.FontDrawFlags.Right);
        }

        // Draw the box and its border
        private void UpdateBox()
        {
            textBox.X = lastPos.X;
            textBox.Y = lastPos.Y;
            textBox.Width = 9 * (Settings.TextSize) + Settings.Padding * 2;
            textBox.Height = (textInfo.Count() + 2) * (Settings.TextSize) + Settings.Padding * 2;

            Graphics.DrawBox(textBox, Settings.BackgroundColor);
            Graphics.DrawFrame(textBox, Settings.BorderSize, Settings.BorderColor);
        }
    }
}
