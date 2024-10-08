namespace JokersAndMarbles;

public class Marble(char letter, int player, int playerCount) {
    public static readonly string[] ColorsByTeams = [
            Ansi.BlueBg + Ansi.BWhite,
            Ansi.BBlueBg + Ansi.Black,
            Ansi.GreenBg + Ansi.BWhite,
            Ansi.BGreenBg + Ansi.Black,
            Ansi.RedBg + Ansi.BWhite,
            Ansi.BRedBg + Ansi.Black,
            Ansi.BBlackBg + Ansi.BWhite,
            Ansi.WhiteBg + Ansi.Black
        ],
        MarbleLettersByTeams = ["ABCDE", "FGHIJ", "abcde", "fghij", "KLMNO", "PQRST", "klmno", "pqrst"];

    public const int Home = -5, Start = 0, Side = 18, Limbos = 4;
    public readonly int Max = playerCount * Side, Entry = playerCount * Side - 5;
    public readonly int Player = player;
    public int Teammate => (Player + playerCount / 2) % playerCount;
    public int Position { get; set; } = Home; // -5=home, -4 to -1=limbo, 0-67=relative board position, 68-72=safe
    public char Letter { get; } = letter;
    public bool IsHome => Position == Home;
    public bool IsSafe => Position > Entry;
    public bool InLimbo => !IsHome && Position < Start; // will be 18*players - 4
    public int AbsPosition => !IsHome && !IsSafe ? PlayPosition : Position; // for non-home/safe marbles
    private int PlayPosition => (Position + Limbos + Player * Side) % Max - Limbos;

    public string Move(int steps, bool hostile, Marble[] sameColorMarbles) {
        if (hostile) {
            if (IsHome) return "Can't move marble in home";
            if (IsSafe) return "Cannot move safe marble";
        } else if (steps < 0) {
            if (IsHome) return "Can't back up marble in home";
            if (IsSafe) return "Can't back up safe marble";
        } else if (Position + steps > Max) return "Can't move marble past end of safe zone";

        // teleport to location (Joker)
        if (sameColorMarbles == null) {
            Position = (Position + steps + Max + Limbos) % Max - Limbos;
            return null;
        }

        // go step by step to see if there's a same color marble in the way
        int pos = Position, sign = Math.Sign(steps), abs = Math.Abs(steps);
        for (int i = 0; i < abs; i++) {
            if (hostile || sign == -1) pos = (pos + sign + Max + Limbos) % Max - Limbos;
            else pos++;
            if (sameColorMarbles.Any(m => m != this && m.Position == pos))
                return "Can't hop over same color marble";
        }
        Position = pos;
        return null;
    }

    public int Score() {
        if (InLimbo) return 0; // worst possible score would be 10 across 5 marbles (1=home, 4=limbo)
        if (IsHome) return 10; // initially, score is 50 across 5 marbles
        if (IsSafe)
            return playerCount * 25 + 10 - (Max - Position) * 5; // win is 500 across 5 marbles (110+105+100+95+90)
        if (Position is >= 0 and <= 3) return 30 - Position; // can back up spots
        int score = 15 + Position;
        int side = ((Position + 8) % Max) / Side;
        int posOnSide = (Position + 8) % Side - 8;
        int teammateSide = playerCount / 2;
        if (side == teammateSide && posOnSide is 0)
            score += 10; // teammate start
        if (side != teammateSide && side != 0)
            if (posOnSide is 0)
                score -= 10; // opponent start
            else if (posOnSide is >= -8 and <= -5)
                score -= 5; // entry to opponent safe
        return score;
    }

    public static string ColorForLetter(char letter) {
        for (int i = 0; i < MarbleLettersByTeams.Length; i++)
            if (MarbleLettersByTeams[i].Contains(letter))
                return ColorsByTeams[i];
        return null;
    }

    public static string ColorForPlayer(int player, int playerCount) =>
        ColorsByTeams[player >= playerCount / 2 ? (player - playerCount / 2) * 2 + 1 : player * 2];
}