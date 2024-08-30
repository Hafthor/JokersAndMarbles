namespace JokersAndMarbles;

public class Program {
    static void Main(string[] args) => new Game(args.Length > 0 ? new Random(int.Parse(args[0])) : new Random()).Run();
}