using DataLibrary;
using DataLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Pages;

public class ViewChatsBase : ComponentBase {
    [Inject] public DataContext Context { get; set; } = default!;
    protected IEnumerable<Chat>? _chats;

    protected override async Task OnInitializedAsync() {
        _chats = await Context.Chats.ToListAsync();
    }
}