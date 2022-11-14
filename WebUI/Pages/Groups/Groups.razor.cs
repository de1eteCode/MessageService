using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Pages.Groups;

public class GroupsBase : ComponentBase {

    [Inject]
    public IDataContext Context { get; set; } = default!;

    public IEnumerable<Group> Groups { get; set; } = default!;

    protected override async Task OnInitializedAsync() {
        Groups = await Context.Groups.ToListAsync();
    }
}