namespace DaemonMC.Utils;


public static class TextFormat {
    
    private const string Section = "§";
    
    public static string Black => Section + "0";
    public static string DarkBlue => Section + "1";
    public static string DarkGreen => Section + "2";
    public static string DarkAqua => Section + "3";
    public static string DarkRed => Section + "4";
    public static string DarkPurple => Section + "5";
    public static string Gold => Section + "6";
    public static string Gray => Section + "7";
    public static string DarkGray => Section + "8";
    public static string Blue => Section + "9";
    public static string Green => Section + "a";
    public static string Aqua => Section + "b";
    public static string Red => Section + "c";
    public static string LightPurple => Section + "d";
    public static string Yellow => Section + "e";
    public static string White => Section + "f";
    
    public static string MinecoinGold => Section + "g";
    
    public static string Obfuscated => Section + "k";
    public static string Bold => Section + "l";
    public static string Italic => Section + "o";
    public static string Reset => Section + "r";
}