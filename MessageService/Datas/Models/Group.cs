﻿namespace MessageService.Datas.Models;

public class Group {
    public int GroupId { get; set; }
    public string? Title { get; set; }
    public virtual ICollection<User>? Users { get; set; }
}