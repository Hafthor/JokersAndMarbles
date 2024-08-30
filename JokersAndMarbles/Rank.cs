namespace JokersAndMarbles;

public enum Rank {
    Joker = 0, // always Suit of None
    Ace, // home->start or advance 1 step
    Two, // advance 2 steps
    Three, // advance 3 steps
    Four, // advance 4 steps
    Five, // advance 5 steps
    Six, // advance 6 steps
    Seven, // advance 7 steps or split 7 steps between two marbles
    Eight, // go back 8 steps
    Nine, // advance 9 steps or split 9 steps between two marbles, one forward, one backward
    Ten, // split 10 steps between ANY two marbles of different colors in either direction
    Jack, // home->start or advance 11 steps
    Queen, // home->start or advance 12 steps
    King, // home->start or advance 13 steps
}