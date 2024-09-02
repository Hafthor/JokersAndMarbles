namespace JokersAndMarbles;

public class Program {
    public static void Main(string[] args) {
        int players = args.Length > 0 ? int.Parse(args[0]) : 8;
        Random random = args.Length > 1 ? new(int.Parse(args[1])) : new(0);
        Game game = new(players, random);
        game.Run();
    }
}