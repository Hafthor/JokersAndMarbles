namespace JokersAndMarbles;

public class Game {
    private Board board;
    private Deck deck;

    public Game(Random rnd) {
        deck = new Deck();
        deck.Shuffle(rnd);
        board = new Board(deck);
    }

    public void Run() {
        for (;;) {
            Console.Clear();
            board.Paint();
            Console.Write("Player " + board.Turn + "[" + board.Score() + "] (" + board.Hand() + "):");
            string sCmd = Console.ReadLine() ?? "";
            if (sCmd == "exit") {
                break;
            } else if (sCmd == "") {
                sCmd = board.AutoPlay();
                board.Print("Auto" + board.Turn + ": " + sCmd);
            }
            string s = board.Play(sCmd);
            if (s != null) {
                board.Print(s);
            } else {
                if (board.Win) {
                    board.Paint();
                    Console.WriteLine("Players " + board.Turn + " and " + board.Teammate + " win!");
                    break;
                }
                board.NextTurn();
            }
        }
    }
}