namespace ShadcnBlazor.Menu;

public interface IMenuParent
{
    public IMenuItem? FocusedItem { get; }

    public Task ChangeFocusAsync(IMenuItem item);
    public Task CloseMenuAsync();
}