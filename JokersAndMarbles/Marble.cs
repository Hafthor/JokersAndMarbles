namespace JokersAndMarbles;

public class Marble(char letter, int player) {
    public char Letter { get; } = letter;
    public int Player { get; } = player;
    public int Position { get; set; } // 0=home, 1-72=relative board position, -1 to -5=safe

    public int Teammate => (Player + 2) % 4;
    public int Offset => Player * 18;
    public int AbsPosition => Position > 0 ? (Position + Offset - 1) % 72 + 1 : Position; // for non-home/safe marbles
    public bool IsHome => Position == 0;
    public bool IsSafe => Position < 0;
    public bool InLimbo => Position > 68; // will be 18*players - 4

    public string Move(int steps, bool hostile, List<Marble> sameColorMarbles) {
        if (hostile) {
            if (IsHome) {
                return "Can't move marble in home";
            } else if (IsSafe) {
                return "Cannot move safe marble";
            }
        } else if (steps < 0) {
            if (IsHome) {
                return "Can't back up marble in home";
            } else if (IsSafe) {
                return "Can't back up safe marble";
            }
        } else if (Position + steps > 73) {
            return "Can't move marble past home";
        }

        // teleport to location (Joker)
        if (sameColorMarbles == null) {
            Position = (Position + steps + 71) % 72 + 1;
            if (Position > 68) {
                Position -= 74;
            }
            return null;
        }

        // go step by step to see if there's a same color marble in the way
        int pos = Position, sign = Math.Sign(steps), abs = Math.Abs(steps);
        for (int i = 0; i < abs; i++) {
            if (sign > 0 && pos < 0) {
                pos += sign;
            } else {
                pos = (pos + 71 + sign) % 72 + 1;
            }
            if (sign > 0 && pos > 68) {
                pos -= 74;
            }
            if (sameColorMarbles.Any(m => m != this && m.Position == pos)) {
                return "Can't hop over same color marble";
            }
        }
        Position = pos;
        return null;
    }

    public int Score() {
        if (InLimbo) return 0; // worst possible score would be 10 across 5 marbles (1=home, 4=limbo)
        if (IsHome) return 10; // start is 50 across 5 marbles
        if (IsSafe) return 115 + Position * 5; // win is 500 across 5 marbles
        if (Position is >= 1 and <= 4) return 30 - Position;
        int score = 83 - (73 - Position);
        if (Position is 1 + 18 or 1 + 54) score -= 10; // opponent start
        if (Position is >= 11 and <= 14 or >= 11 + 36 and <= 14 + 36) score -= 5; // entry to safe
        return score;
    }
}