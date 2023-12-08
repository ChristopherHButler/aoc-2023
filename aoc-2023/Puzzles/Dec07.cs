using System.Linq;
using System.Security.Cryptography;
using System.Text;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class CamelCard
{
    private string[] AllowedValues = new string[] { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

    public string Label { get; set; }
    public int Ordinal { get; private set; }

    public CamelCard(string label, int part = 1)
    {
        SetOrdinal(label, part);
        Label = label;
    }

    public bool Equals(CamelCard x, CamelCard y)
    {
        //Check whether any of the compared objects is null.
        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

        //Check whether the products' properties are equal.
        return x.Label == y.Label;
    }

    private void SetOrdinal(string label, int part = 1)
    {
        if (!AllowedValues.Any(o => o == label))
            throw new ArgumentException("Not a valid card");

        if (part == 2 && label == "J")
        {
            Ordinal = 1;
            return;
        }

        //if (label == "J")
        //    throw new Exception("This should never happen");

        Ordinal = label switch
        {
            "A" => 14,
            "K" => 13,
            "Q" => 12,
            "J" => 11,
            "T" => 10,
            "9" => 9,
            "8" => 8,
            "7" => 7,
            "6" => 6,
            "5" => 5,
            "4" => 4,
            "3" => 3,
            "2" => 2,
            _ => throw new ArgumentException("Not a valid card value"),
        };
    }
}

public enum HandType
{
    FiveOfAKind,
    FourOfAKind,
    FullHouse,
    ThreeOfAKind,
    TwoPair,
    OnePair,
    HighCard,
}

public class Hand
{
    public List<CamelCard> Cards = new List<CamelCard>();
    public HandType Type { get => GetHandType(); }
    public int HandStrength { get => GetHandStrength(); }

    public HandType AltType { get; set; }
    public int AltHandStrength { get; set; }
    public string AltFullName { get; set; } = "";

    public string FullHand { get => Print(); }
    public int Bid { get; set; }
    public int Rank { get; set; } = 0;
    public int Winnings { get; set; } = 0;

    public Hand(List<CamelCard> cards, int bid)
    {
        Cards.AddRange(cards);
        Bid = bid;
    }

    public Hand(List<CamelCard> cards, int bid, string altFullname, HandType altType, int altHandStrength )
    {
        Cards.AddRange(cards);
        Bid = bid;

        AltFullName = altFullname;
        AltType = altType;
        AltHandStrength = altHandStrength;
    }

    public string Print()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Cards.Count; i++)
        {
            sb.Append(Cards[i].Label);
        }
        return sb.ToString();
    }

    private HandType GetHandType()
    {
        if (Cards.Count != 5)
            throw new ArgumentOutOfRangeException($"A Hand can only have 5 cards. This hand has {Cards.Count}");

        var groups = GetCardGroups();

        if (HandIsFiveOfAKind(GetDistinct()))
            return HandType.FiveOfAKind;
        else if (HandIsFourOfAKind(groups))
            return HandType.FourOfAKind;
        else if (HandIsFullHouse(groups))
            return HandType.FullHouse;
        else if (HandIsThreeOfAKind(groups))
            return HandType.ThreeOfAKind;
        else if (HandIsTwoPair(groups))
            return HandType.TwoPair;
        else if (HandIsOnePair(groups))
            return HandType.OnePair;
        else if (HandIsHighCard(groups))
            return HandType.HighCard;
        
        throw new Exception("Invalid Hand Type");
    }

    private int GetHandStrength()
    {
        if (Cards.Count != 5)
            throw new ArgumentOutOfRangeException($"A Hand can only have 5 cards. This hand has {Cards.Count}");

        var groups = GetCardGroups();

        if (HandIsFiveOfAKind(GetDistinct()))
            return 7;
        if (HandIsFourOfAKind(groups))
            return 6;
        if (HandIsFullHouse(groups))
            return 5;
        if (HandIsThreeOfAKind(groups))
            return 4;
        if (HandIsTwoPair(groups))
            return 3;
        if (HandIsOnePair(groups))
            return 2;
        if (HandIsHighCard(groups))
            return 1;

        throw new Exception("Invalid Hand Strength");

    }

    #region Helpers
    private int GetDistinct() => Cards.Select(x => x.Label).Distinct().Count();

    private IGrouping<string, CamelCard>[] GetCardGroups() =>
        Cards.GroupBy(c => c.Label).ToArray();

    private int GetCardCount(string label)
    {
        var groups = Cards.GroupBy(c => c.Label).ToArray();

        for (int i = 0; i < groups.Count(); i++)
        {
            if (groups[i].Key == label)
                return groups[i].Count();
        }
        return 0;
    }
    #endregion

    #region Compute Types

    private bool HandIsFiveOfAKind(int distinct) => distinct == 1 ? true : false;

    private bool HandIsFourOfAKind(IGrouping<string, CamelCard>[] groups)
    {
        if (groups.Count() == 2 && (groups[0].Count() == 1 || groups[0].Count() == 4))
            return true;
        return false;
    }

    private bool HandIsFullHouse(IGrouping<string, CamelCard>[] groups)
    {
        if (groups.Count() == 2 && (groups[0].Count() == 2 || groups[0].Count() == 3))
            return true;
        return false;
    }

    private bool HandIsThreeOfAKind(IGrouping<string, CamelCard>[] groups)
    {
        // either group 1 has two, and group 2 or 3 have two
        // or group 1 has one and group 2 or 3 have two
        var group1Count = groups[0].Count();
        var group2Count = groups[1].Count();
        var group3Count = groups[2].Count();

        if (groups.Count() == 3 && ((group1Count == 3 && (group2Count == 1 && group3Count == 1)) ||
                                    (group2Count == 3 && (group1Count == 1 || group3Count == 1)) ||
                                    (group3Count == 3 && (group1Count == 1 || group2Count == 1))))
            return true;
        return false;
    }

    private bool HandIsTwoPair(IGrouping<string, CamelCard>[] groups)
    {
        // either group 1 has two, and group 2 or 3 have two
        // or group 1 has one and group 2 or 3 have two
        var group1Count = groups[0].Count();
        var group2Count = groups[1].Count();
        var group3Count = groups[2].Count();

        if (groups.Count() == 3 && ((group1Count == 2 && (group2Count == 2 || group3Count == 2)) ||
                                    (group2Count == 2 && (group1Count == 2 || group3Count == 2))))
            return true;
        return false;
    }

    private bool HandIsOnePair(IGrouping<string, CamelCard>[] groups)
    {
        var group1Count = groups[0].Count();
        var group2Count = groups[1].Count();
        var group3Count = groups[2].Count();

        if (groups.Count() == 3 && (group1Count == 3 || group2Count == 3 || group3Count == 3))
        {
            return true;
        }

        var group4Count = groups[3].Count();

        var groupCounts = new int[] { group1Count, group2Count, group3Count, group4Count };

        // there should be 1 group of 2 and 3 groups of 1
        var countGroups = groupCounts.GroupBy(i => i).ToArray();

        if (groups.Count() == 4 &&
            countGroups.Count() == 2 &&
            (countGroups[0].Count() == 1 || countGroups[0].Count() == 3))
            return true;
        return false;
    }

    private bool HandIsHighCard(IGrouping<string, CamelCard>[] groups)
    {
        if (groups.Count() == 5) return true;

        return false;
    }

    #endregion
}

public class Dec07
{

    public static void SolvePt1(string? date, bool useTestData = false)
    {
        // ordered List
        //AAAAA 765  - 5 of a kind
        //AA8AA 684 - 4 of a kind
        //23332 28 - full house
        //TTT98 220 - three of a kind
        //23432 483 - two pair
        //A23A4 666 - 1 pair
        //23456 666 - highest card

        List<Hand> Hands = new List<Hand>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        CreateHands(lines: dfr.Lines, hands: Hands);

        for (int i = 0; i < Hands.Count; i++)
        {
            Console.WriteLine($"Unordered Hands. Hand: {i} [{Hands[i].FullHand}] type is: {Hands[i].Type}");
        }

        Hands = OrderHands(hands: Hands);

        int total = ComputeRankAndWinnings(hands: Hands);

        for (int i = 0; i < Hands.Count; i++)
        {
            Console.WriteLine($"Ordered Hands. Hand: {i} [{Hands[i].FullHand} - Rank: {Hands[i].Rank}] type is: {Hands[i].Type}");
        }

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        List<Hand> Hands = new List<Hand>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        CreateHands(lines: dfr.Lines, hands: Hands, part: 2);

        Hands = OrderHands2(hands: Hands);

        int total = ComputeRankAndWinnings(hands: Hands);

        for (int i = 0; i < Hands.Count; i++)
        {
            Console.WriteLine($"Ordered Hands. Hand: {i} [{Hands[i].FullHand} -> {Hands[i].AltFullName} - Rank: {Hands[i].Rank}] type is: {Hands[i].AltType}");
        }

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

    }

    private static void CreateHands(List<string> lines, List<Hand> hands, int part = 1)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            List<CamelCard> cards = new List<CamelCard>();
            var parts = lines[i].Split(' ');
            var bid = Convert.ToInt32(parts[1].Trim());
            var cardValues = parts[0].ToCharArray();

            for (int j = 0; j < cardValues.Length; j++)
            {
                cards.Add(new CamelCard(cardValues[j].ToString(), part));
            }

            if (part == 1)
            {
                hands.Add(new Hand(cards, bid));
            }

            else if (part == 2)
            {
                var altHand = CreateAltHand(cards, bid);

                hands.Add(new Hand(cards, bid, altFullname: altHand.FullHand, altType: altHand.Type, altHandStrength: altHand.HandStrength));
            }
        }
    }

    private static Hand CreateAltHand(List<CamelCard> cards, int bid)
    {
        // create alternate hand for part 2
        var groups = cards.GroupBy(c => c.Label).ToArray();
        int max = 0;
        IGrouping<string, CamelCard> jGrp = groups[0];
        for (int j = 0; j < groups.Count(); j++)
        {
            if (groups[j].Count() >= groups[max].Count() && groups[j].Key != "J") max = j;
        }
        StringBuilder sb = new StringBuilder();
        for (int j = 0; j < cards.Count; j++)
        {
            sb.Append(cards[j].Label);
        }

        string altFullHand = sb.ToString().Replace("J", groups[max].Key);

        List<CamelCard> newCards = new List<CamelCard>();
        var cardValues = altFullHand.ToCharArray();
        for (int j = 0; j < cardValues.Length; j++)
        {
            newCards.Add(new CamelCard(cardValues[j].ToString(), part: 2));
        }
        return new Hand(newCards, bid);
    }

    private static List<Hand> OrderHands(List<Hand> hands)
    {
        bool isSorted;
        for (int i = 0; i < hands.Count; i++)
        {
            isSorted = true;
            for (int j = 1; j < hands.Count - i; j++)
            {
                if (CompareHands(hands[j], hands[j - 1]))
                {
                    hands = SwapHands(hands, indexA: j, indexB: j-1);
                    isSorted = false;
                }
            }
            if (isSorted) return hands;
        }
        return hands;
    }

    private static bool CompareHands(Hand handA, Hand handB)
    {
        if (handA.HandStrength < handB.HandStrength) return true;

        // Additional sorting rules
        if (handA.HandStrength == handB.HandStrength)
        {
            for (int i = 0; i < handA.Cards.Count; i++)
            {
                if (handA.Cards[i].Ordinal < handB.Cards[i].Ordinal) return true;
                if (handA.Cards[i].Ordinal > handB.Cards[i].Ordinal) return false;
                // else, compare the next card in the hand
                if (handA.Cards[i].Ordinal == handB.Cards[i].Ordinal) continue;
            }
        }
        // don't swap
        return false;
    }

    private static List<Hand> OrderHands2(List<Hand> hands)
    {
        bool isSorted;
        for (int i = 0; i < hands.Count; i++)
        {
            isSorted = true;
            for (int j = 1; j < hands.Count; j++)
            {
                if (CompareHands2(hands[j], hands[j - 1]))
                {
                    hands = SwapHands(hands, indexA: j, indexB: j - 1);
                    //isSorted = false;
                }
            }
            //if (isSorted) return hands;
        }
        return hands;
    }

    private static bool CompareHands2(Hand handA, Hand handB)
    {
        if (handA.AltHandStrength < handB.AltHandStrength) return true;

        // Additional sorting rules
        if (handA.AltHandStrength == handB.AltHandStrength)
        {
            for (int i = 0; i < handA.Cards.Count; i++)
            {
                // if (handA.Cards[i].Label == "J") Console.WriteLine($"");
                if (handA.Cards[i].Ordinal < handB.Cards[i].Ordinal) return true;
                if (handA.Cards[i].Ordinal > handB.Cards[i].Ordinal) return false;
                // else, compare the next card in the hand
                if (handA.Cards[i].Ordinal == handB.Cards[i].Ordinal) continue;
            }
        }
        // don't swap
        return false;
    }

    private static List<Hand> SwapHands<Hand>(List<Hand> hands, int indexA, int indexB)
    {
        Hand tmp = hands[indexA];
        hands[indexA] = hands[indexB];
        hands[indexB] = tmp;
        return hands;
    }

    private static int ComputeRankAndWinnings(List<Hand> hands)
    {
        int total = 0;
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].Rank = i + 1;
            hands[i].Winnings = ((i + 1) * hands[i].Bid);
            total += ((i + 1) * hands[i].Bid);
        }
        return total;
    }
}

