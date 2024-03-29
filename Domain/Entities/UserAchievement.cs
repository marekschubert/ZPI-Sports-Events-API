﻿namespace Domain.Entities;

public class UserAchievement
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string AchievementId { get; set; }
    public Achievement Achievement { get; set; }
    public DateTime Obtained { get; set; }
}
