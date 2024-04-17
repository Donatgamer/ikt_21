using System.Formats.Asn1;
using System.Numerics;

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
                    Console.ForegroundColor = ConsoleColor.Black;
                    return;
                default:
                    break;
            }
        }
    }

    static void PlayBlackjack()
    {

        Pakli pakli = new Pakli();
        pakli.Kever();

        Player player = new Player();
        AI ai = new AI();

        for (int i = 0; i < 2; i++)
        {
            player.KartyaAdd(pakli.Osztas());
            player.PowerUpAdd(pakli.PowerUpOsztas());
            ai.KartyaAdd(pakli.Osztas());
            ai.PowerUpAdd(pakli.PowerUpOsztas());
        }

        bool playerStands = false;
        bool aiStands = false;
        int? targetScore = 21;
        Random r = new Random();
        while (!(playerStands && aiStands))
        {
            char choice;
            do
            {

                playerStands = false;
                aiStands = false;
                Console.Clear();
                Console.WriteLine(targetScore);
                player.HandPrint(targetScore);
                ai.AIHandPrint();
                Console.WriteLine("Kérsz lapot vagy használsz egy powerupot? (i/n/t)");
                choice = Char.ToLower(Console.ReadKey(true).KeyChar);
                if (choice == 't' && player.MegszerzettPowerUps.Count > 0)
                {
                    Console.WriteLine("Használható powerupok:");
                    for (int i = 0; i < player.MegszerzettPowerUps.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} {player.MegszerzettPowerUps[i].nev}");

                    }
                    choice = Console.ReadKey(true).KeyChar;
                    int j = choice - 48;
                    if (j >= 0 && j <= 9 && j <= player.MegszerzettPowerUps.Count && player.MegszerzettPowerUps[j - 1] != null)
                    {
                        int? elozo = targetScore;
                        targetScore = player.PowerUpHasznal(player.MegszerzettPowerUps[j - 1].id, ai, targetScore);
                        if (targetScore == null)
                            targetScore = elozo;
                        player.MegszerzettPowerUps.Remove(player.MegszerzettPowerUps[j - 1]);
                    }
                }
                if (r.Next(1) == 1)
                    player.PowerUpAdd(pakli.PowerUpOsztas());
                if (r.Next(1) == 1)
                    ai.PowerUpAdd(pakli.PowerUpOsztas());
            } while (choice != 'i' && choice != 'n');
            Console.Clear();
            if (choice == 'i')
            {
                Kartya ujKartya = pakli.Osztas();
                player.KartyaAdd(ujKartya);
                player.HandPrint(targetScore);
            }
            else if (choice == 'n')
            {
                playerStands = true;
            }

            if (ai.LapKer(pakli, targetScore, player))
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
            player.HandPrint(targetScore);

            Console.WriteLine("Ai lapjai:");
            ai.HandPrint(targetScore);

            int playerScore = player.Score(targetScore);
            int aiScore = ai.Score(targetScore);
            if (playerScore == targetScore && playerScore != aiScore || playerScore > aiScore && aiScore < targetScore && playerScore < targetScore || playerScore < targetScore && aiScore > targetScore)
            {
                Console.WriteLine("Nyertél!");
            }
            else if (aiScore == targetScore && playerScore != aiScore || aiScore > playerScore && playerScore < targetScore && aiScore < targetScore || aiScore < targetScore && playerScore > targetScore)
            {
                Console.WriteLine("Az ai nyert!");
            }
            else if (aiScore == playerScore)
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
            else if (choice.Key == ConsoleKey.Escape)
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
    public int id { get; set; }
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
        powerupok = new List<PowerUp>
        {
            new PowerUp { nev = "Húzz fel egy 2-t", id = 0 },
            new PowerUp { nev = "Húzz fel egy 3-t", id = 1 },
            new PowerUp { nev = "Húzz fel egy 4-t", id = 2 },
            new PowerUp { nev = "Húzz fel egy 5-t", id = 3 },
            new PowerUp { nev = "Húzz fel egy 6-t", id = 4 },
            new PowerUp { nev = "Húzz fel egy 7-t", id = 5 },
            new PowerUp { nev = "17 a cél", id = 6 },
            new PowerUp { nev = "24 a cél", id = 7 },
            new PowerUp { nev = "27 a cél", id = 8 },
            new PowerUp { nev = "Töröld a legutóbbi kártyádat", id = 9 },
            new PowerUp { nev = "Töröld az ai legutóbbi kártyáját", id = 10 },
            new PowerUp { nev = "Húzd fel a lehető legjobb kártyát", id = 11 },
        };
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

    public int? PowerUpHasznal(int id, Player player, int? targetScore)
    {
        //itt mondhatod meg hogy mit csináljon egy powerup
        switch (id)
        {
            case 0:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "2", Ertek = 2 });
                return null;
            case 1:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "3", Ertek = 3 });
                return null;
            case 2:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "4", Ertek = 4 });
                return null; ;
            case 3:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "5", Ertek = 5 });
                return null;
            case 4:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "6", Ertek = 6 });
                return null;
            case 5:
                hand.Add(new Kartya { Szin = "powerup", Magassag = "7", Ertek = 7 });
                return null;
            case 6:
                return 17;
            case 7:
                return 24;
            case 8:
                return 27;
            case 9:
                hand.RemoveAt(hand.Count - 1);
                return null;
            case 10:
                player.hand.RemoveAt(player.hand.Count - 1);
                return null;
            case 11:
                if (targetScore - Score(targetScore) >= 11)
                    hand.Add(new Kartya { Szin = "powerup", Magassag = "Ász", Ertek = 11 });
                else
                    hand.Add(new Kartya { Szin = "powerup", Magassag = $"{targetScore - Score(targetScore)}", Ertek = targetScore.GetValueOrDefault() - Score(targetScore) });
                return null;
            default:
                return null;
        }
    }

    public void HandPrint(int? targetScore)
    {
        int i = 0;
        foreach (var kartya in hand)
        {
            i++;
            Console.WriteLine($"{i} lap: {kartya.Szin} {kartya.Magassag}");
        }
        Console.WriteLine($"pontok: {Score(targetScore)}");
    }

    public int Score(int? targetScore)
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

        while (score > targetScore && AszDb > 0)
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

    public bool LapKer(Pakli pakli, int? targetScore, Player player)
    {
        AiPowerUpHazsnal(targetScore, player);

        int aiScore = Score(targetScore);
        int playerScore = player.Score(targetScore);
        int joKartyak = 0;
        int ossz = pakli.MaradekKartyak.Count();

        foreach (var kartya in pakli.MaradekKartyak)
        {
            int lehetsegesScore = aiScore + kartya.Ertek;
            if (lehetsegesScore <= targetScore)
            {
                joKartyak++;
            }
        }

        double valoszinuseg = (double)joKartyak / ossz;

        if (playerScore > aiScore && playerScore < targetScore)
        {
            if (playerScore - aiScore > 3 || valoszinuseg > 0.3)
                return true;
            return aiScore < targetScore - 4;
        }
        return aiScore < targetScore - 4;
    }

    public int? AiPowerUpHazsnal(int? targetScore, Player player)
    {
        int aiPowerUpId = AiPowerUpDecide(targetScore, player);

        int? elozo = targetScore;
        targetScore = PowerUpHasznal(aiPowerUpId, player, targetScore);
        if (aiPowerUpId != -1)
            for (int i = 0; i < MegszerzettPowerUps.Count; i++)
            {
                if (MegszerzettPowerUps[i].id == aiPowerUpId)
                    MegszerzettPowerUps.RemoveAt(i);
            }
        if (targetScore == null)
            return elozo;
        return targetScore;
    }
    public int AiPowerUpDecide(int? targetScore, Player player)
    {
        for (int i = 0; i < MegszerzettPowerUps.Count; i++)
        {
            if (MegszerzettPowerUps[i].id == 0 && Score(targetScore) <= targetScore - 2)
                return 0;
            else if (MegszerzettPowerUps[i].id == 1 && Score(targetScore) <= targetScore - 3)
                return 1;
            else if (MegszerzettPowerUps[i].id == 2 && Score(targetScore) <= targetScore - 4)
                return 2;
            else if (MegszerzettPowerUps[i].id == 3 && Score(targetScore) <= targetScore - 5)
                return 3;
            else if (MegszerzettPowerUps[i].id == 4 && Score(targetScore) <= targetScore - 6)
                return 4;
            else if (MegszerzettPowerUps[i].id == 5 && Score(targetScore) <= targetScore - 7)
                return 5;
            else if (MegszerzettPowerUps[i].id == 6 && Score(targetScore) <= 17)
                return 6;
            else if (MegszerzettPowerUps[i].id == 7 && Score(targetScore) > 21)
                return 7;
            else if (MegszerzettPowerUps[i].id == 8 && Score(targetScore) > 21)
                return 8;
            else if (MegszerzettPowerUps[i].id == 9 && Score(targetScore) > targetScore)
                return 9;
            else if (MegszerzettPowerUps[i].id == 10 && player.Score(targetScore) < targetScore)
                return 10;
            else if (MegszerzettPowerUps[i].id == 11)
                return 11;
        }
        return -1;
    }
}
