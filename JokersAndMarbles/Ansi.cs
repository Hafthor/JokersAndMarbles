namespace JokersAndMarbles;

public static class Ansi {
    public static readonly string Reset = "\e[0m",
        ClearScreen = "\e[2J\e[0;0H",
        ClearLine = "\e[2K",
        ClearRestOfLine = "\e[K",
        HideCursor = "\e[?25l",
        ShowCursor = "\e[?25h",
        SaveCursor = "\e[s",
        RestoreCursor = "\e[u",
        HomeCursor = "\e[H",
        Bold = "\e[1m",
        Underline = "\e[4m",
        Inverse = "\e[7m";

    public static string MoveCursor(int row, int col) => $"\e[{row};{col}H";

    public static readonly string Black = "\e[30m",
        Red = "\e[31m",
        Green = "\e[32m",
        Brown = "\e[33m",
        Blue = "\e[34m",
        Magenta = "\e[35m",
        Cyan = "\e[36m",
        White = "\e[37m";

    public static readonly string BlackBg = "\e[40m",
        RedBg = "\e[41m",
        GreenBg = "\e[42m",
        BrownBg = "\e[43m",
        BlueBg = "\e[44m",
        MagentaBg = "\e[45m",
        CyanBg = "\e[46m",
        WhiteBg = "\e[47m";

    public static readonly string BBlack = "\e[90m",
        BRed = "\e[91m",
        BGreen = "\e[92m",
        Yellow = "\e[93m",
        BBlue = "\e[94m",
        Pink = "\e[95m",
        BCyan = "\e[96m",
        BWhite = "\e[97m";

    public static readonly string BBlackBg = "\e[100m",
        BRedBg = "\e[101m",
        BGreenBg = "\e[102m",
        YellowBg = "\e[103m",
        BBlueBg = "\e[104m",
        PinkBg = "\e[105m",
        BCyanBg = "\e[106m",
        BWhiteBg = "\e[107m";
}