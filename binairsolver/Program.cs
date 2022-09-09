namespace binairsolver;

public class Program
{
    public static void Main(string[] args)
    {
        Solver solver = new Solver(new Binair());
        
        List<String> arguments = args.ToList();

        if(arguments.Contains("--no-log"))
        {
            solver.LogEnabled = false;
            arguments.Remove("--no-log");
        }

        if (arguments.Contains("--check"))
        {
            solver.CheckEnabled = true;
            arguments.Remove("--check");
        }

        if(arguments.Contains("--help"))
        {
            Console.WriteLine("Usage: binairsolver <path> [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  --check         Check if the solution is correct");
            Console.WriteLine("  --no-log        Disable logging");
            Console.WriteLine("  --help          Show this help");
            Console.Out.WriteLine("Note: '<path>' is the path to the csv file containing the binair puzzle");
            return;
        }
        
        
        
        String fileName;
        if (arguments.Count > 0)
        {
            fileName = arguments[0];
        }
        else
        {
            Console.Out.WriteLine("Please enter a file name:");
            fileName = Console.In.ReadLine() ?? string.Empty;
        }
        
        solver.OpenFile(fileName);
        solver.Solve();
    }
}