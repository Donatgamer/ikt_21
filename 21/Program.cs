using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Blackjack!");
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Play Blackjack");
            Console.WriteLine("2. Quit");

            char choice = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            switch (choice)
            {
                case '1':
                    PlayBlackjack();
                    break;
                case '2':
                    Console.WriteLine("Thanks for playing!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void PlayBlackjack()
    {
        Console.Clear();

        Deck deck = new Deck();
        deck.Shuffle();

        Player player = new Player();
        ComputerPlayer computerPlayer = new ComputerPlayer();

        player.AddCard(deck.DealCard());
        computerPlayer.AddCard(deck.DealCard());
        player.AddCard(deck.DealCard());
        computerPlayer.AddCard(deck.DealCard());

        player.ShowHand();
        computerPlayer.ShowPartialHand(-1);

        bool playerStands = false;
        bool computerStands = false;

        while (!(playerStands && computerStands))
        {
            char choice;
            do
            {
                playerStands = false;
                computerStands = false;
                Console.Clear();
                player.ShowHand();
                computerPlayer.ShowPartialHand(-1);
                Console.WriteLine("Hit or Stand? (h/s)");
                choice = Char.ToLower(Console.ReadKey(true).KeyChar);
            } while (choice != 'h' && choice != 's');
            Console.Clear();
            if (choice == 'h')
            {
                Card newCard = deck.DealCard();
                player.AddCard(newCard);
                player.ShowHand();
            }
            else if (choice == 's')
            {
                playerStands = true;
            }

            if (computerPlayer.ShouldHit(deck))
            {
                Card newCard = deck.DealCard();
                computerPlayer.AddCard(newCard);
            }
            else
            {
                computerStands = true;
            }
        }

        Console.Clear();
        Console.WriteLine("Player's Hand:");
        player.ShowHand();

        Console.WriteLine("Computer's Hand:");
        computerPlayer.ShowHand();

        int playerScore = player.Score();
        int computerScore = computerPlayer.Score();
        // Determine the winner
        if (playerScore == 21 || playerScore > computerScore && computerScore < 21 && playerScore < 21 || playerScore < 21 && computerScore > 21)
        {
            Console.WriteLine("You win!");
        }
        else if (computerScore == 21 || computerScore > playerScore && playerScore < 21 && computerScore < 21 || computerScore < 21 && playerScore > 21)
        {
            Console.WriteLine("Computer wins!");
        }
        else
        {
            Console.WriteLine("Both players lost!");
        }

        Console.WriteLine("Press anything to start a new game or 'q' to return to the main menu.");
        char continueChoice = Console.ReadKey(true).KeyChar;
        if (Char.ToLower(continueChoice) == 'q')
        {
            return;
        }
        else
        {
            PlayBlackjack(); // Start a new game
        }


    }

}

class Card
{
    public string Suit { get; set; }
    public string Rank { get; set; }
    public int Value { get; set; }
}

class Deck
{
    private List<Card> cards;
    private List<Card> dealtCards;

    public IEnumerable<Card> RemainingCards => cards.Except(dealtCards);

    public Deck()
    {
        InitializeDeck();
        dealtCards = new List<Card>();
    }

    private void InitializeDeck()
    {
        cards = new List<Card>();

        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                int value = 0;
                if (rank == "Ace")
                {
                    value = 11;
                }
                else if (rank == "Jack" || rank == "Queen" || rank == "King")
                {
                    value = 10;
                }
                else
                {
                    value = int.Parse(rank);
                }

                cards.Add(new Card { Suit = suit, Rank = rank, Value = value });
            }
        }
    }

    public void Shuffle()
    {
        Random rng = new Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public Card DealCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        dealtCards.Add(card);
        return card;
    }
}

class Player
{
    protected List<Card> hand;

    public Player()
    {
        hand = new List<Card>();
    }

    public void AddCard(Card card)
    {
        hand.Add(card);
    }

    public void ShowHand()
    {
        foreach (var card in hand)
        {
            Console.WriteLine($"{card.Rank} of {card.Suit}");
        }
        Console.WriteLine($"Total: {Score()}");
    }

    public int Score()
    {
        int score = 0;
        int numAces = 0;

        foreach (var card in hand)
        {
            score += card.Value;
            if (card.Rank == "Ace")
            {
                numAces++;
            }
        }

        while (score > 21 && numAces > 0)
        {
            score -= 10;
            numAces--;
        }

        return score;
    }
}

class ComputerPlayer : Player
{
    public void ShowPartialHand(int newCardIndex)
    {
        Console.WriteLine("Computer's Hand:");
        Console.WriteLine("Card 1: Hidden");

        if (newCardIndex >= 0 && newCardIndex < hand.Count)
        {
            for (int i = 1; i < hand.Count; i++)
            {
                Console.WriteLine($"Card {i + 1}: {hand[i].Rank} of {hand[i].Suit}");
            }
            Console.WriteLine($"Card {newCardIndex + 1}: {hand[newCardIndex].Rank} of {hand[newCardIndex].Suit}");
        }
        else
        {
            for (int i = 1; i < hand.Count; i++)
            {
                Console.WriteLine($"Card {i + 1}: {hand[i].Rank} of {hand[i].Suit}");
            }
        }
        int total = 0;
        for (int i = 1; i < hand.Count; i++)
        {
            total += hand[i].Value;
        }
        Console.WriteLine($"Total: {total}");
    }

    public bool ShouldHit(Deck deck)
    {
        int myScore = Score();

        return myScore < 17;
    }
}
