﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Meeting> Meetings { get; }
    DbSet<Friendship> Friendships { get; }
    DbSet<MeetingParticipant> MeetingParticipants { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Achievement> Achievements { get; }
    DbSet<UserAchievement> UserAchievements { get; }
    DbSet<Post> Posts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}