namespace SHSDP.Code.Bases;

public abstract class RuntimeExampleProgramBase
{
    protected readonly Action<String, bool> writeOutput;
    protected readonly Func<String> readInput;
    private Action? onComplete;

    protected RuntimeExampleProgramBase(Action<String, bool> output, Func<String> input, Action callback)
    {
        this.writeOutput = output;
        this.readInput = input;
        this.onComplete = callback;
    }
    
    public void Execute()
    {
        try
        {
            Run();
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    protected abstract void Run();
    
    // ─────────────────────────────
    // Printing helpers
    // ─────────────────────────────
    protected void Print(String text)
    {
        writeOutput(text, false);
    }

    protected void PrintLine(String text)
    {
        writeOutput(text, true);
    }
}