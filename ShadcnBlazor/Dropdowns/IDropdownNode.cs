namespace ShadcnBlazor.Dropdowns;

public interface IDropdownNode
{
    public IDropdownItem? FocusedItem { get; }
    
    public Task SetFocusAsync(IDropdownItem item);
    public Task HandleSelectionAsync();
}