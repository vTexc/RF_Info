using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using SharpDX;
using System.Collections.Generic;
using System.Windows.Forms;

public class Settings: SettingsBase
{
    public Settings()
    {
        Enable = true;
        PosX = new RangeNode<float>(13, 0.0f, 100.0f);
        PosY = new RangeNode<float>(85, 0.0f, 100.0f);
        TextColor = new ColorBGRA(255, 255, 255, 255);
        TextSize = new RangeNode<int>(25, 15, 40);
        BorderColor = new ColorBGRA(255, 255, 255, 255);
        BorderSize = new RangeNode<int>(2, 0, 4);
        BackgroundColor = new ColorBGRA(255, 255, 255, 20);
        Padding = new RangeNode<int>(4, 4, 8);
    }

    [Menu("Pos X")]
    public RangeNode<float> PosX { get; set; }

    [Menu("Pos Y")]
    public RangeNode<float> PosY { get; set; }

    [Menu("Text Color")]
    public ColorNode TextColor { get; set; }

    [Menu("Tex Size")]
    public RangeNode<int> TextSize { get; set; }

    [Menu("Border Color")]
    public ColorNode BorderColor { get; set; }

    [Menu("Border Size")]
    public RangeNode<int> BorderSize { get; set; }

    [Menu("Background Color")]
    public ColorNode BackgroundColor { get; set; }

    [Menu("Padding")]
    public RangeNode<int> Padding { get; set; }
}