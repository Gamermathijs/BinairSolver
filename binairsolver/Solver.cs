namespace binairsolver;

//C:\Users\mathi\Development\SimpleBinair.csv
public class Solver
{
    public Binair Binair;
    public long StartTime;
    public long PreviousTime;
    public long Cycles;
    public long AmountSolved;
    public long AdvancedSolved;
    public bool LogEnabled = true;

    public Solver(Binair binair)
    {
        Binair = binair;
        StartTime = DateTime.Now.Ticks;
        PreviousTime = StartTime;
    }

    public bool CheckEnabled { get; set; }

    public void Solve()
    {
        if (LogEnabled)
        {
            Console.Out.WriteLine("Cycle: " + Cycles);
            Console.Out.WriteLine("Amount solved: " + AmountSolved);
            PrintSolution();
            Console.Out.WriteLine("\n" + string.Concat(Enumerable.Repeat("----", Binair.Size)) + "--------");
        }

        if (Binair.Solved)
        {
            double secondsElapsed = new TimeSpan(DateTime.Now.Ticks - StartTime).TotalSeconds;
            Console.Out.WriteLine("Solved in {0} seconds with {1} cycle(s)", GetTimeElapsed(), Cycles);
            Console.Out.WriteLine("Advanced solved: {0}", AdvancedSolved);
            Console.Out.WriteLine("Total Time: {0}s", secondsElapsed);


            Console.Out.WriteLine("\nSolution:");
            
            PrintSolution();
            
            CheckBinair();
            
            return;
        }

        AmountSolved = 0;

        SolveRows();
        SolveColumns();
        
        if(AmountSolved == 0)
        {
            for (int i = 0; i < Binair.Size; i++)
            {
                AdvancedCountingRow(i);
                AdvancedCountingColumn(i);
            }
        }
        
        CheckIfSolved();

        if (AmountSolved == 0)
        {
            Console.Out.WriteLine("No solution found");
            Console.Out.WriteLine("Got stuck in {0} seconds with {1} cycle(s)", GetTimeElapsed(), Cycles);
            double secondsElapsed = new TimeSpan(DateTime.Now.Ticks - StartTime).TotalSeconds;
            Console.Out.WriteLine("Total Time: {0}s", secondsElapsed);

            PrintSolution();
            
            CheckBinair();

            if (!CheckEnabled)
            {
                Console.Out.WriteLine("\nTry enabling the check function with the '--check' argument");
            }
            
            return;
        }

        Cycles++;

        Solve();
    }

    private void CheckBinair()
    {
        if (CheckEnabled)
        {
            Console.Out.WriteLine("\nChecking binair...");
            int problems = CheckIfBinairIsValid();
            if(problems == 0)
            {
                Console.Out.WriteLine("No problems found, the binair is valid");
            }
            else
            {
                Console.Out.WriteLine("{0} problem(s) found, the binair is not valid", problems);
                Console.Out.WriteLine("Note: the coordinates are 1-based, starting at the top left corner");
            }
        }
    }

    private void PrintSolution()
    {
        foreach (Row row in Binair.Rows)
        {
            Console.Out.WriteLine(string.Concat(Enumerable.Repeat("+---", Binair.Size)) + "+");

            foreach (string value in row.Values)
            {
                Console.Out.Write("| {0} ", value);
            }

            Console.Out.WriteLine("|");
        }

        Console.Out.WriteLine(string.Concat(Enumerable.Repeat("+---", Binair.Size)) + "+");
    }

    private void SolveRows()
    {
        for (var x = 0; x < Binair.Rows.Count; x++)
        {
            Row row = Binair.Rows[x];

            for (var y = 0; y < row.Values.Count; y++)
            {
                string value = Binair.Rows[x].Values[y];

                SolveDoubleHorizontal(x, y, value);
                SolveHorizontalGap(x, y, value);
            }

            CountColumn(x);
        }
    }

    private void SolveColumns()
    {
        for (int y = 0; y < Binair.Size; y++)
        {
            for (int x = 0; x < Binair.Size; x++)
            {
                string value = Binair.Rows[x].Values[y];

                SolveDoubleVertical(x, y, value);
                SolveVerticalGap(x, y, value);
            }

            CountRow(y);
        }
    }


    // Solves when there is a double
    private void SolveDoubleVertical(int x, int y, string value)
    {
        if (x < 1)
        {
            return;
        }

        var previousValue = Binair.Rows[x - 1].Values[y];
        if (value != previousValue) return;

        if (x < Binair.Size - 1 &&
            Binair.Rows[x + 1].Values[y] == " " &&
            Binair.SetValue(x + 1, y, value == "0" ? "1" : value == "1" ? "0" : " "))
        {
            AmountSolved++;
        }

        if (x > 1 &&
            Binair.Rows[x - 2].Values[y] == " " &&
            Binair.SetValue(x - 2, y, value == "0" ? "1" : value == "1" ? "0" : " "))
        {
            AmountSolved++;
        }
    }

    private void SolveDoubleHorizontal(int x, int y, string value)
    {
        if (y < 1)
        {
            return;
        }

        String previousValue = Binair.Rows[x].Values[y - 1];

        if (value == previousValue)
        {
            if (Binair.SetValue(x, y + 1, value == "0" ? "1" : value == "1" ? "0" : " "))
            {
                AmountSolved++;
            }

            if (y > 1)
            {
                if (Binair.SetValue(x, y - 2, value == "0" ? "1" : value == "1" ? "0" : " "))
                {
                    AmountSolved++;
                }
            }
        }
    }


    // Solve the gap between two values
    private void SolveVerticalGap(int x, int y, string value)
    {
        if (x < 1 || x > Binair.Size - 2)
        {
            return;
        }

        if (value != " ")
        {
            return;
        }

        string previousValue = Binair.Rows[x - 1].Values[y];
        string nextValue = Binair.Rows[x + 1].Values[y];

        if (previousValue == nextValue)
        {
            if (Binair.SetValue(x, y, previousValue == "0" ? "1" : previousValue == "1" ? "0" : " "))
            {
                AmountSolved++;
            }
        }
    }

    private void SolveHorizontalGap(int x, int y, string value)
    {
        if (y < 1 || y > Binair.Size - 2)
        {
            return;
        }

        if (value != " ")
        {
            return;
        }

        string previousValue = Binair.Rows[x].Values[y - 1];
        string nextValue = Binair.Rows[x].Values[y + 1];

        if (previousValue == nextValue)
        {
            if (Binair.SetValue(x, y, previousValue == "0" ? "1" : previousValue == "1" ? "0" : " "))
            {
                AmountSolved++;
            }
        }
    }


    // Solves with counting
    private void CountColumn(int y)
    {
        List<String> values = Binair.Rows.Select(row => row.Values[y]).ToList();
        int zeroCount = values.FindAll(value => value == "0").Count;
        int oneCount = values.FindAll(value => value == "1").Count;

        if (zeroCount == Binair.Size / 2)
        {
            var indexes = Enumerable.Range(0, values.Count).Where(i => values[i] == " ").ToArray();
            Array.ForEach(indexes, x =>
            {
                if (Binair.SetValue(x, y, "1"))
                {
                    AmountSolved++;
                }
            });
        }

        if (oneCount == Binair.Size / 2)
        {
            var indexes = Enumerable.Range(0, values.Count).Where(i => values[i] == " ").ToArray();
            Array.ForEach(indexes, x =>
            {
                if (Binair.SetValue(x, y, "0"))
                {
                    AmountSolved++;
                }
            });
        }

        if (zeroCount == Binair.Size / 2 - 1 && oneCount == Binair.Size / 2 - 1)
        {
            var completedColumns = Binair
                .Rows
                .Where((row, i) => row.Values.FindAll(rowValue => rowValue == " ").Count == 0)
                .ToList();

            var matchingColumn = completedColumns
                .Where((row, _) =>
                    row.Values.Where((s, i) => values[i] == " " || s == values[i]).Count() == Binair.Size)
                .FirstOrDefault();

            if (matchingColumn == null)
            {
                return;
            }

            for (var x = 0; x < values.Count; x++)
            {
                var value = values[x];
                if (value == " ")
                {
                    if (Binair.SetValue(x, y, matchingColumn.Values[x] == "0" ? "1" : "0"))
                    {
                        AmountSolved++;
                    }
                }
            }
        }
    }

    private void CountRow(int x)
    {
        List<String> values = Binair.Rows[x].Values;
        int zeroCount = values.FindAll(value => value == "0").Count;
        int oneCount = values.FindAll(value => value == "1").Count;

        if (zeroCount == Binair.Size / 2)
        {
            var indexes = Enumerable.Range(0, values.Count).Where(i => values[i] == " ").ToArray();
            Array.ForEach(indexes, y =>
            {
                if (Binair.SetValue(x, y, "1"))
                {
                    AmountSolved++;
                }
            });
        }

        if (oneCount == Binair.Size / 2)
        {
            var indexes = Enumerable.Range(0, values.Count).Where(i => values[i] == " ").ToArray();
            Array.ForEach(indexes, y =>
            {
                if (Binair.SetValue(x, y, "0"))
                {
                    AmountSolved++;
                }
            });
        }
    }

    private void AdvancedCountingColumn(int y)
    {
        List<String> values = Binair.Rows.Select(row => row.Values[y]).ToList();

        List<List<String>> possibleValues = GetAllOptions(values, new List<List<string>>());

        List<List<string>> validPossibilities = possibleValues.Where(list => CheckIfRowIsValid(list,true, -1 ) == 0).ToList();

        if(validPossibilities.Count == 0)
        {
            return;
        }
        
        Dictionary<int, String> emptyIndexes = new();
        for (var x = 0; x < Binair.Size; x++)
        {
            if (values[x] == " ")
            {
                emptyIndexes.Add(x, validPossibilities[0][x]);
            }
        }

        foreach (List<string> possibility in validPossibilities)
        {
            foreach (var emptyIndex in emptyIndexes)
            {
                if (emptyIndex.Value == " ")
                {
                    emptyIndexes[emptyIndex.Key] = possibility[emptyIndex.Key];
                }

                if (possibility[emptyIndex.Key] != emptyIndex.Value)
                {
                    emptyIndexes.Remove(emptyIndex.Key);
                }
            }
        }

        foreach (var emptyIndex in emptyIndexes)
        {
            if (Binair.SetValue(emptyIndex.Key, y, emptyIndex.Value))
            {
                AmountSolved++;
                AdvancedSolved++;
            }
        }
    }
    private void AdvancedCountingRow(int x)
    {
        List<String> values = Binair.Rows[x].Values;;

        List<List<String>> possibleValues = GetAllOptions(values, new List<List<string>>());

        List<List<string>> validPossibilities = possibleValues.Where(list => CheckIfColumnIsValid(list, true, -1) == 0).ToList();

        Dictionary<int, String> emptyIndexes = new();
        for (var y = 0; y < Binair.Size; y++)
        {
            if (values[y] == " ")
            {
                emptyIndexes.Add(y, validPossibilities[0][y]);
            }
        }

        foreach (List<string> possibility in validPossibilities)
        {
            foreach (var emptyIndex in emptyIndexes)
            {
                if (emptyIndex.Value == " ")
                {
                    emptyIndexes[emptyIndex.Key] = possibility[emptyIndex.Key];
                }

                if (possibility[emptyIndex.Key] != emptyIndex.Value)
                {
                    emptyIndexes.Remove(emptyIndex.Key);
                }
            }
        }

        foreach (var emptyIndex in emptyIndexes)
        {
            if (Binair.SetValue(x, emptyIndex.Key, emptyIndex.Value))
            {
                AmountSolved++;
                AdvancedSolved++;
            }
        }
    }
    
    private List<List<String>> GetAllOptions(List<String> values, List<List<String>> allOptions)
    {
        string[] possibleValues = new string[] {"0", "1"};

        double emptyCount = values.FindAll(value => value == " ").Count;

        if (emptyCount == 0)
        {
            allOptions.Add(values);
            return allOptions;
        }

        int index = values.FindIndex(value => value == " ");

        foreach (var option in possibleValues)
        {
            List<String> newValues = values.GetRange(0, values.Count);
            newValues[index] = option;
            allOptions = GetAllOptions(newValues, allOptions);
        }

        return allOptions;
    }

    // Check if the puzzle is solved
    private void CheckIfSolved()
    {
        foreach (Row row in Binair.Rows)
        {
            foreach (string value in row.Values)
            {
                if (value == " ")
                {
                    Binair.Solved = false;
                    return;
                }
            }
        }

        Binair.Solved = true;
    }

    private int CheckIfBinairIsValid()
    {
        int problems = 0;
        
        for (var i = 0; i < Binair.Rows.Count; i++)
        {
            Row row = Binair.Rows[i];
            problems += CheckIfRowIsValid(row.Values, true, i);
        }

        for (int i = 0; i < Binair.Size; i++)
        {
            List<String> values = Binair.Rows.Select(row => row.Values[i]).ToList();
            problems += CheckIfColumnIsValid(values, true, i);
        }
        
        return problems;
    }
    
    private int CheckIfRowIsValid(List<String> values, bool oneCopyAllowed, int index)
    {
        bool log = index != -1;
        int problemCount = 0;
        
        for (var i = 1; i < values.Count - 1; i++)
        {
            string value = values[i];
            if (value == " ")
            {
                continue;
            }
            
            string previousValue = values[i - 1];
            string nextValue = values[i + 1];
            
            if (value == previousValue && value == nextValue)
            {
                problemCount++;
                if (log)
                {
                    LogInvalid(i, index, "three in a row");
                }
            }
        }
        
        if(values.All(value => value != " "))
        {
            int oneCount = values.FindAll(value => value == "1").Count;
            int zeroCount = values.FindAll(value => value == "0").Count;
                
            if(oneCount != zeroCount)
            {
                problemCount++;
                if (log)
                {
                    LogInvalid(index, -1, "one count != zero count, one count:" + oneCount + " zero count: " + zeroCount);
                }
            }

            int duplicateCount = 0;
            
            foreach (Row row in Binair.Rows)
            {
                bool rowNoCopy = false;
                for (var i = 0; i < row.Values.Count; i++)
                {
                    if (row.Values[i] != values[i])
                    {
                        rowNoCopy = true;
                        break;
                    }
                }

                if (!rowNoCopy)
                {
                    duplicateCount++;
                }
            }
            
            if((duplicateCount == 1 && !oneCopyAllowed) || duplicateCount > 1)
            {
                problemCount++;
                if (log)
                {
                    LogInvalid(index, -1, "duplicate row");
                }
            }
        }

        return problemCount;
    }
    private int CheckIfColumnIsValid(List<String> values, bool oneCopyAllowed, int index)
    {
        bool log = index != -1;
        int problemCount = 0;
        
        for (var i = 1; i < values.Count - 1; i++)
        {
            string value = values[i];
            if (value == " ")
            {
                continue;
            }
            
            string previousValue = values[i - 1];
            string nextValue = values[i + 1];
            
            if (value == previousValue && value == nextValue)
            {
                if (log)
                {
                    LogInvalid(index, i, "three in a row");
                }
                problemCount++;
            }
        }
        
        if(values.All(value => value != " "))
        {
            int oneCount = values.FindAll(value => value == "1").Count;
            int zeroCount = values.FindAll(value => value == "0").Count;
                
            if(oneCount != zeroCount)
            {
                if (log)
                {
                    LogInvalid(index, -1, "one count != zero count, one count:" + oneCount + " zero count: " + zeroCount);
                }
                problemCount++;
            }

            int duplicateCount = 0;

            for (int i = 0; i < Binair.Size; i++)
            {
                List<String> columnValues = Binair.Rows.Select(row => row.Values[i]).ToList();
                bool rowNoCopy = false;
                for (var j = 0; j < columnValues.Count; j++)
                {
                    if (columnValues[j] != values[j])
                    {
                        rowNoCopy = true;
                        break;
                    }
                }

                if (!rowNoCopy)
                {
                    duplicateCount++;
                }
            }
            
            if((duplicateCount == 1 && !oneCopyAllowed) || duplicateCount > 1)
            {
                problemCount++;
                if (log)
                {
                    LogInvalid(index, -1, "duplicate column");
                }            
            }
        }

        return problemCount;
    }

    private void LogInvalid(int index, int i, string type)
    {
        if(index == -1)
        {
            Console.Out.WriteLine("Problem found at: {0} ({1})", index + 1, type);
        }
        else
        {
            Console.Out.WriteLine("Problem found at: {0},{1} ({2})", index + 1, i + 1, type);
        }
    }


    // Opens the file and reads the puzzle
    public void OpenFile(String fileName)
    {
        Console.Out.WriteLine("Opening file: " + fileName);

        Binair = new Binair();

        using var reader = new StreamReader(fileName);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line == null)
            {
                Console.Error.WriteLine("Error: Invalid file");
                System.Environment.Exit(0);
                return;
            }

            var values = line.Split(',');
            Binair.Size = values.Length;

            Row row = new Row();

            foreach (var value in values)
            {
                row.Values.Add(value == "1" ? "1" : value == "0" ? "0" : " ");
            }

            Binair.Rows.Add(row);
        }

        Console.Out.WriteLine("Read the file in {0} seconds", GetTimeElapsed());
        Console.Out.WriteLine("\n" + string.Concat(Enumerable.Repeat("----", Binair.Size)) + "--------");
    }


    // Gets the time elapsed since the previous call
    private double GetTimeElapsed()
    {
        double secondsElapsed = new TimeSpan(DateTime.Now.Ticks - PreviousTime).TotalSeconds;
        PreviousTime = DateTime.Now.Ticks;
        return secondsElapsed;
    }
}