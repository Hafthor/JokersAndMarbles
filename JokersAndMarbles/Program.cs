namespace JokersAndMarbles;

public static class Program {
    public static void Main(string[] args) {
        int players = args.Length > 0 ? int.Parse(args[0]) : 4;
        int seed = args.Length > 1 ? int.Parse(args[1]) : new Random().Next();
        Game game = new(players, seed);
        game.Run();
    }
}