using SHSDP.Code.Bases;

namespace SHSDP.Components.Pages.Classes.CP132.RuntimeExamples;

public sealed partial class Lab5 : RuntimeExampleBase
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetupProgram();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
    
    protected override RuntimeExampleProgramBase CreateProgram(
        Action<String, bool> output,
        Func<String> input,
        Action callback
    ) => new Lab5RuntimeExampleProgram(output, input, callback);

    private class Lab5RuntimeExampleProgram : RuntimeExampleProgramBase
    {
        private readonly Random rand = new Random();
        private readonly int[] dice = new int[5];

        private readonly String[] categories =
        [
            "Aces", "Twos", "Threes", "Fours", "Fives",
            "Sixes", "Yahtzee", "Chance"
        ];

        private readonly bool[] categoriesUsed;

        public Lab5RuntimeExampleProgram(Action<String, bool> output, Func<String> input, Action callback)
            : base(output, input, callback)
        {
            categoriesUsed = new bool[categories.Length];
        }

        protected override void Run()
        {
            int points = DoRound();
            while (HasEveryCategoryBeenUsed() == false)
            {
                PrintLine("You currently have " + points + " points.");
                points += DoRound();
            }

            PrintLine("You scored " + points + " total points!");
        }

        private void RollDice(int rollNum)
        {
            Print("Roll #" + rollNum + ": ");

            for (int i = 0; i < dice.Length; i++)
            {
                dice[i] = rand.Next(1, 7);
                Print(dice[i] + " ");
            }

            PrintLine("");
        }

        private bool HasEveryCategoryBeenUsed()
        {
            foreach (bool used in categoriesUsed)
            {
                if (used == false)
                {
                    return false;
                }
            }

            return true;
        }

        private int DoRound()
        {
            int rollNum = 0;
            bool stopRolling = false;

            while (++rollNum <= 3 && stopRolling == false)
            {
                RollDice(rollNum);
                if (rollNum < 3)
                {
                    stopRolling = AskRollAgain() == false;
                }
                else
                {
                    PrintLine("That was your last roll! You must choose a category now.");
                }
            }

            int category = GetCategory();
            return DoScoring(category);
        }

        private bool AskRollAgain()
        {
            PrintLine("Roll again or freeze? Type in either \"roll\" or \"freeze\".");
            Print("> ");
            String choice = readInput();

            HashSet<String> validChoices = new HashSet<String>(StringComparer.OrdinalIgnoreCase)
            {
                "roll", "freeze"
            };

            while (validChoices.Contains(choice.ToLower()) == false)
            {
                PrintLine("Invalid input. Try again.");
                PrintLine("Roll again or freeze? Type in either \"roll\" or \"freeze\".");
                Print("> ");
                choice = readInput();
            }

            return choice.Equals("roll", StringComparison.OrdinalIgnoreCase);
        }

        private int GetCategory()
        {
            PrintLine("Choose a category:");
            Print("| ");

            for (int i = 0; i < categories.Length; i++)
            {
                if (categoriesUsed[i] == false)
                {
                    Print(categories[i] + " | ");
                }
            }

            PrintLine("");
            Print("> ");

            while (true)
            {
                String choice = readInput();

                for (int i = 0; i < categories.Length; i++)
                {
                    if (categories[i].Equals(choice, StringComparison.OrdinalIgnoreCase))
                    {
                        if (categoriesUsed[i] == true)
                        {
                            PrintLine("That category has already been used!");
                            break;
                        }
                        else
                        {
                            categoriesUsed[i] = true;
                            return i;
                        }
                    }
                }
                
                PrintLine("Invalid category. Try again.");
                Print("> ");
            }
        }

        private int DoScoring(int category)
        {
            int sum = 0;

            if (category < 6)
            {
                foreach (int d in dice)
                {
                    if (d == category + 1)
                    {
                        sum += category + 1;
                    }
                }
            }
            else if (category == 6)
            {
                if (CheckYahtzee() == true)
                {
                    sum = 50;
                }
            }
            else if (category == 7)
            {
                foreach (int d in dice)
                {
                    sum += d;
                }
            }

            return sum;
        }

        private bool CheckYahtzee()
        {
            int firstDice = dice[0];

            for (int i = 1; i < dice.Length; i++)
            {
                if (dice[i] != firstDice)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
