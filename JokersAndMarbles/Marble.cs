namespace JokersAndMarbles;

public class Marble(char letter, int player) {
    public const int HOME = -5, START = 0, ENTRY = 67, MAX = 72, SIDE = 18, LIMBOS = 4;
    public int Player { get; } = player;
    public int Teammate(int players) => (Player + players / 2) % players;
    public int Position { get; set; } = HOME; // -5=home, -4 to -1=limbo, 0-67=relative board position, 68-72=safe
    public char Letter { get; } = letter;
    public int Offset => Player * SIDE;
    public bool IsHome => Position == HOME;
    public bool IsSafe => Position > ENTRY;
    public bool InLimbo => !IsHome && Position < HOME; // will be 18*players - 4
    public bool OnBoard => !IsHome && !IsSafe && !InLimbo;

    public int AbsPosition =>
        !IsHome && !IsSafe ? (Position + LIMBOS + Offset) % MAX - LIMBOS : Position; // for non-home/safe marbles

    public string Move(int steps, bool hostile, List<Marble> sameColorMarbles) {
        if (hostile) {
            if (IsHome) return "Can't move marble in home";
            if (IsSafe) return "Cannot move safe marble";
        } else if (steps < 0) {
            if (IsHome) return "Can't back up marble in home";
            if (IsSafe) return "Can't back up safe marble";
        } else if (Position + steps > MAX) return "Can't move marble past end of safe zone";

        // teleport to location (Joker)
        if (sameColorMarbles == null) {
            Position = (Position + steps + MAX + LIMBOS) % MAX - LIMBOS;
            return null;
        }

        // go step by step to see if there's a same color marble in the way
        int pos = Position, sign = Math.Sign(steps), abs = Math.Abs(steps);
        for (int i = 0; i < abs; i++) {
            if (hostile || sign == -1) pos = (pos + sign + MAX + LIMBOS) % MAX - LIMBOS;
            else pos++;
            if (sameColorMarbles.Any(m => m != this && m.Position == pos))
                return "Can't hop over same color marble";
        }
        Position = pos;
        return null;
    }

    public int Score() {
        if (InLimbo) return 0; // worst possible score would be 10 across 5 marbles (1=home, 4=limbo)
        if (IsHome) return 10; // start is 50 across 5 marbles
        if (IsSafe) return 110 - (MAX - Position) * 5; // win is 500 across 5 marbles (110+105+100+95+90)
        if (Position is >= 0 and <= 3) return 30 - Position; // can back up spots
        int score = 15 + Position; // 15 to 82 
        if (Position is SIDE or SIDE * 3) score -= 10; // opponent start
        if (Position is >= 10 and <= 13 or >= 10 + SIDE * 2 and <= 13 + SIDE * 2) score -= 5; // entry to opponent safe
        return score;
    }
}