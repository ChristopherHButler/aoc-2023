using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Card
{
    public string CardNumber { get; set; } = "";
    public int Score { get; set; } = 0;
    public List<int> WinningNumbers = new List<int>();
    public List<int> YourNumbers = new List<int>();
    public List<int> Matches = new List<int>();
    public int Copies = 1;

    // 1 for the first match, then doubled three times for each of the three matches after the first
    public int GetCardScore()
    {
        for (int i = 0; i < Matches.Count; i++)
        {
            if (i == 0) Score = 1;
            else Score *= 2;
        }
        return Score;
    }
}

public class Dec04
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        List<Card> Cards = new List<Card>();
        int total = 0;  // total points

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        CreateCards(dfr.Lines, Cards);

        for (int i = 0; i < Cards.Count; i++)
        {
            total += Cards[i].GetCardScore();
        }

        var outputString = useTestData ? "Part 1 Total [using test data]" : "Part 1 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        List<Card> Cards = new List<Card>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        CreateCards(dfr.Lines, Cards);

        int total = ComputeCards(Cards);

        var outputString = useTestData ? "Part 1 Total [using test data]" : "Part 1 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");
    }

    private static void CreateCards(List<string> lines, List<Card> cards)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            var tmp = lines[i].Split(':');
            var tmpWinners = tmp[1].Split('|')[0];
            var tmpYours = tmp[1].Split('|')[1];

            Card newCard = new Card { CardNumber = tmp[0].Split(' ')[1] };

            var winners = tmpWinners.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var yours = tmpYours.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            foreach (var winner in winners)
            {
                newCard.WinningNumbers.Add(Convert.ToInt32(winner.Trim()));
            }

            foreach (var y in yours)
            {
                newCard.YourNumbers.Add(Convert.ToInt32(y.Trim()));
            }

            var matches = yours.Select(c => c).Intersect(winners).ToArray();

            foreach (var match in matches)
            {
                newCard.Matches.Add(Convert.ToInt32(match));
            }

            cards.Add(newCard);
        }
    }

    private static int ComputeCards(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            // starting at the card after the current card
            // going to the count of the number of matches
            for (int j = i + 1; j <= i + cards[i].Matches.Count; j++)
            {
                cards[j].Copies += cards[i].Copies;
            }
        }

        var total = 0;

        foreach (var card in cards)
        {
            total += card.Copies;
        }

        return total;
    }

}
