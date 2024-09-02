namespace JokersAndMarbles;

public class Program {
    public static void Main(string[] args) {
        int players = args.Length > 0 ? int.Parse(args[0]) : 4;
        Random random = args.Length > 1 ? new(int.Parse(args[1])) : new();
        Game game = new(players, random);
        game.Run();
    }
}