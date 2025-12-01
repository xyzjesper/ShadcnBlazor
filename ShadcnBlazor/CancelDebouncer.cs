namespace ShadcnBlazor;

public class CancelDebouncer
{
    private readonly TimeSpan Delay;
    private CancellationTokenSource Cts = new();

    public CancelDebouncer(TimeSpan delay)
    {
        Delay = delay;
    }

    public void Start(Func<Task> action)
    {
        var cts = new CancellationTokenSource();
        Cts = cts;
        
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Delay, cts.Token);
                await action.Invoke();
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
        });
    }

    public async Task CancelAsync()
    {
        await Cts.CancelAsync();
        Cts = new();
    }
}