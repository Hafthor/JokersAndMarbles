namespace JokersAndMarbles;

public static class Ansi {
    public static readonly string Reset = "\u001b[0m",
        ClearScreen = "\u001b[2J\u001b[0;0H",
        ClearLine = "\u001b[2K",
        ClearRestOfLine = "\u001b[K",
        HideCursor = "\u001b[?25l",
        ShowCursor = "\u001b[?25h",
        SaveCursor = "\u001b[s",
        RestoreCursor = "\u001b[u",
        HomeCursor = "\u001b[H",
        Bold = "\u001b[1m",
        Underline = "\u001b[4m",
        Inverse = "\u001b[7m";

    public static string MoveCursor(int row, int col) => $"\u001b[{row};{col}H";

    public static readonly string Black = "\u001b[30m",
        Red = "\u001b[31m",
        Green = "\u001b[32m",
        Brown = "\u001b[33m",
        Blue = "\u001b[34m",
        Magenta = "\u001b[35m",
        Cyan = "\u001b[36m",
        White = "\u001b[37m";

    public static readonly string BlackBg = "\u001b[40m",
        RedBg = "\u001b[41m",
        GreenBg = "\u001b[42m",
        BrownBg = "\u001b[43m",
        BlueBg = "\u001b[44m",
        MagentaBg = "\u001b[45m",
        CyanBg = "\u001b[46m",
        WhiteBg = "\u001b[47m";

    public static readonly string BBlack = "\u001b[90m",
        BRed = "\u001b[91m",
        BGreen = "\u001b[92m",
        Yellow = "\u001b[93m",
        BBlue = "\u001b[94m",
        Pink = "\u001b[95m",
        BCyan = "\u001b[96m",
        BWhite = "\u001b[97m";

    public static readonly string BBlackBg = "\u001b[100m",
        BRedBg = "\u001b[101m",
        BGreenBg = "\u001b[102m",
        YellowBg = "\u001b[103m",
        BBlueBg = "\u001b[104m",
        PinkBg = "\u001b[105m",
        BCyanBg = "\u001b[106m",
        BWhiteBg = "\u001b[107m";
}