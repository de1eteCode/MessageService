﻿namespace RepositoryLibrary.Models;

public class ChatGroup : BaseModelEntity {
    public int Id { get; set; }
    public int GroupId { get; set; }
    public virtual Group? Group { get; set; }
    public string? ChatId { get; set; }
    public virtual Chat? Chat { get; set; }
    public bool IsDeleted { get; set; }
}