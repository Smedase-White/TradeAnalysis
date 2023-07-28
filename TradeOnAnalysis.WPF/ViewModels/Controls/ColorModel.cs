using System;
using System.Windows.Media;

namespace TradeOnAnalysis.WPF.ViewModels;

public class ColorModel : ViewModelBase
{
    private Color _color = Color.FromRgb(200, 70, 200);
    private string _hex = "#C846C8";

    public ColorModel()
    {

    }

    public byte Red
    {
        get => _color.R;
        set
        {
            _color.R = value;
            _hex = RgbToHex(Red, Green, Blue);
            OnChanged();
            OnChanged(nameof(Hex));
        }
    }

    public byte Green
    {
        get => _color.G;
        set
        {
            _color.G = value;
            _hex = RgbToHex(Red, Green, Blue);
            OnChanged();
            OnChanged(nameof(Hex));
        }
    }

    public byte Blue
    {
        get => _color.B;
        set
        {
            _color.B = value;
            _hex = RgbToHex(Red, Green, Blue);
            OnChanged();
            OnChanged(nameof(Hex));
        }
    }

    public string Hex
    {
        get => _hex;
        set
        { 
            ChangeProperty(ref _hex, value);
            (Red, Green, Blue) = HexToRgb(value);
        }
    }


    public static string RgbToHex(byte red, byte green, byte blue)
    {
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    public static (byte, byte, byte) HexToRgb(string hex)
    {
        try
        {
            int color = Convert.ToInt32(hex.TrimStart('#'), 16);
            int r = (color & 0xff0000) >> 16;
            int g = (color & 0xff00) >> 8;
            int b = (color & 0xff);
            return ((byte)r, (byte)g, (byte)b);
        }
        catch
        {
            return (200, 70, 200);
        }
    }
}
