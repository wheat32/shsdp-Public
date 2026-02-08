namespace SHSDP.Services;

public class ProgramCompletionService
{
    public event Action? OnProgramCompleted;
    
    public void NotifyProgramCompleted()
    {
        OnProgramCompleted?.Invoke();
    }
}

