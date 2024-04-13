class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Üdvözöljük a blackjackben!");
            Console.WriteLine("1. Játék indítása");
            Console.WriteLine("2. Kilépés");

            char choice = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            switch (choice)
            {
                case '1':
                    PlayBlackjack();
                    break;
                case '2':
                    Console.WriteLine("Köszönjük a játékot!");
                    return;
                default:
                    break;
            }
        }
    }

    static void PlayBlackjack()
    {
        Console.Clear();

        Pakli pakli = new Pakli();
        pakli.Kever();

        Player player = new Player();
        AI ai = new AI();

        player.KartyaAdd(pakli.Osztas());
        player.PowerUpAdd(pakli.PowerUpOsztas());
        ai.KartyaAdd(pakli.Osztas());
        ai.PowerUpAdd(pakli.PowerUpOsztas());
        player.KartyaAdd(pakli.Osztas());
        player.PowerUpAdd(pakli.PowerUpOsztas());
        ai.KartyaAdd(pakli.Osztas());
        ai.PowerUpAdd(pakli.PowerUpOsztas());

        bool playerStands = false;
        bool aiStands = false;

        while (!(playerStands && aiStands))
        {
            char choice;
            do
            {
                playerStands = false;
                aiStands = false;
                Console.Clear();
                player.HandPrint();
                ai.AIHandPrint();
                Console.WriteLine("Kérsz lapot vagy használsz egy powerupot? (i/n/t)");
                choice = Char.ToLower(Console.ReadKey(true).KeyChar);
                if(choice == 't' && player.MegszerzettPowerUps.Count > 0)
                {
                    Console.WriteLine("Használható powerupok:");
                    for (int i = 0; i < player.MegszerzettPowerUps.Count; i++)
                    {
                        Console.WriteLine($"{i+1} {player.MegszerzettPowerUps[i].nev}");
                        
                    }
                    choice = Console.ReadKey(true).KeyChar;
                    int j = choice - 48;
                    if(j <= player.MegszerzettPowerUps.Count)
                    {
                        if (player.MegszerzettPowerUps[j - 1] != null)
                        {
                            player.PowerUpHasznal(player.MegszerzettPowerUps[j - 1]);
                            player.MegszerzettPowerUps.Remove(player.MegszerzettPowerUps[j - 1]);
                        }
                    }
                }
            } while (choice != 'i' && choice != 'n');
            Console.Clear();
            if (choice == 'i')
            {
                Kartya ujKartya = pakli.Osztas();
                player.KartyaAdd(ujKartya);
                player.HandPrint();
            }
            else if (choice == 'n')
            {
                playerStands = true;
            }

            if (ai.LapKer(ai, pakli))
            {
                Kartya ujKartya = pakli.Osztas();
                ai.KartyaAdd(ujKartya);
            }
            else
            {
                aiStands = true;
            }
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Játékos lapjai:");
            player.HandPrint();

            Console.WriteLine("Ai lapjai:");
            ai.HandPrint();

            int playerScore = player.Score();
            int aiScore = ai.Score();
            if (playerScore == 21 || playerScore > aiScore && aiScore < 21 && playerScore < 21 || playerScore < 21 && aiScore > 21)
            {
                Console.WriteLine("Nyertél!");
            }
            else if (aiScore == 21 || aiScore > playerScore && playerScore < 21 && aiScore < 21 || aiScore < 21 && playerScore > 21)
            {
                Console.WriteLine("Az ai nyert!");
            }
            else if(aiScore == playerScore)
            {
                Console.WriteLine("Döntetlen!");
            }
            else
            {
                Console.WriteLine("Mindenki vesztett!");
            }
            Console.WriteLine("Nyomd meg az ENTER-t új kör kezdéséhez vagy az ESC-t a kilépéshez!");
            ConsoleKeyInfo choice = Console.ReadKey(true);
            Console.WriteLine();

            if (choice.Key == ConsoleKey.Enter)
            {
                PlayBlackjack();
                break;
            }
            else if(choice.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}

class Kartya
{
    public string Szin { get; set; }
    public string Magassag { get; set; }
    public int Ertek { get; set; }
}

class PowerUp
{
    public string nev { get; set; }
}

class Pakli
{
    public List<Kartya> kartyak;
    public List<Kartya> kiosztottKartyak;
    public List<PowerUp> powerupok;

    public IEnumerable<Kartya> MaradekKartyak => kartyak.Except(kiosztottKartyak);

    public Pakli()
    {
        PakliKeszit();
        kiosztottKartyak = new List<Kartya>();
    }

    public void PakliKeszit()
    {
        kartyak = new List<Kartya>();

        string[] szinek = { "Kör", "Káró", "Pikk", "Treff" };
        string[] magassagok = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Bubi", "Királynő", "Király", "Ász" };

        foreach (string szin in szinek)
        {
            foreach (string magassag in magassagok)
            {
                int ertek = 0;
                if (magassag == "Ász")
                {
                    ertek = 11;
                }
                else if (magassag == "Bubi" || magassag == "Királynő" || magassag == "Király")
                {
                    ertek = 10;
                }
                else
                {
                    ertek = int.Parse(magassag);
                }

                kartyak.Add(new Kartya { Szin = szin, Magassag = magassag, Ertek = ertek });
            }
        }

        //itt kell letrehozni a powerupokat
        powerupok = new List<PowerUp>();
        powerupok.Add(new PowerUp { nev = "Húzz fel egy 6-t"});
    }

    public void Kever()
    {
        Random r = new Random();
        int db = kartyak.Count;
        while (db > 1)
        {
            db--;
            int i = r.Next(db + 1);
            Kartya value = kartyak[i];
            kartyak[i] = kartyak[db];
            kartyak[db] = value;
        }
    }

    public Kartya Osztas()
    {
        Kartya kartya = kartyak[0];
        kartyak.RemoveAt(0);
        kiosztottKartyak.Add(kartya);
        return kartya;
    }

    public PowerUp PowerUpOsztas()
    {
        Random r = new Random();
        PowerUp powerup = powerupok[r.Next(powerupok.Count)];
        return powerup;
    }
}

class Player
{
    public List<Kartya> hand;
    public List<PowerUp> MegszerzettPowerUps;

    public Player()
    {
        hand = new List<Kartya>();
        MegszerzettPowerUps = new List<PowerUp>();
    }

    public void KartyaAdd(Kartya kartya)
    {
        hand.Add(kartya);
    }

    public void PowerUpAdd(PowerUp powerup)
    {
        MegszerzettPowerUps.Add(powerup);
    }

    public void PowerUpHasznal(PowerUp powerUp)
    {
        //itt mondhatod meg hogy mit csináljon egy powerup
        if (powerUp.nev == "Húzz fel egy 6-t")
            hand.Add(new Kartya { Szin = "powerup", Magassag = "6", Ertek = 6 });
    }

    public void HandPrint()
    {
        int i = 0;
        foreach (var kartya in hand)
        {
            i++;
            Console.WriteLine($"{i} lap: {kartya.Szin} {kartya.Magassag}");
        }
        Console.WriteLine($"pontok: {Score()}");
    }

    public int Score()
    {
        int score = 0;
        int AszDb = 0;

        foreach (var kartya in hand)
        {
            score += kartya.Ertek;
            if (kartya.Magassag == "Ász")
            {
                AszDb++;
            }
        }

        while (score > 21 && AszDb > 0)
        {
            score -= 10;
            AszDb--;
        }

        return score;
    }
}

class AI : Player
{
    public void AIHandPrint()
    {
        Console.WriteLine("ai lapjai:");
        Console.WriteLine("1 lap: Rejtett");
        for (int i = 1; i < hand.Count; i++)
        {
            Console.WriteLine($"{i + 1} lap: {hand[i].Szin} {hand[i].Magassag} ");
        }
        int ossz = 0;
        for (int i = 1; i < hand.Count; i++)
        {
            ossz += hand[i].Ertek;
        }
        Console.WriteLine($"pontok: {ossz}");
    }

    public bool LapKer(Player player, Pakli pakli)
    {
        int aiScore = Score();
        int playerScore = player.Score();
        int joKartyak = 0;
        int ossz = pakli.MaradekKartyak.Count();

        foreach (var kartya in pakli.MaradekKartyak)
        {
            int lehetsegesScore = aiScore + kartya.Ertek;
            if (lehetsegesScore <= 21)
            {
                joKartyak++;
            }
        }

        double valoszinuseg = (double)joKartyak / ossz;

        if (playerScore > aiScore && playerScore < 21)
        {
            if (playerScore - aiScore > 3 || valoszinuseg > 0.3)
                return true;
            return aiScore < 17;
        }
        return aiScore < 17;
    }
}
