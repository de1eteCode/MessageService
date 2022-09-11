﻿using System.Collections;

namespace MessageService.Datas.Models;

public class Chat {
    public string? ChatId { get; set; }
    public string? Name { get; set; }

    public override bool Equals(object? obj) {
        if (obj == null)
            return false;

        if (string.IsNullOrEmpty(ChatId)) {
            return false;
        }

        if (obj is Chat otherChat) {
            if (string.IsNullOrEmpty(otherChat.ChatId)) {
                return false;
            }

            return ChatId.Equals(otherChat.ChatId);
        }

        return false;
    }

    public override int GetHashCode() {
        return ChatId?.GetHashCode() ?? string.Empty.GetHashCode();
    }
}