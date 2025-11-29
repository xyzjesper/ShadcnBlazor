namespace ShadcnBlazor.ContextMenus;

public interface IContextMenuParent
{
    public bool IsOpen { get; }
    public Task ChangeFocusAsync(IContextMenuItem item);
    public Task CloseMenuAsync();
}