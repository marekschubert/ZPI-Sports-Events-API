﻿using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements;

public class TimeAchievement
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TimeAchievement(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task CheckTimeAchievement(Guid meetingId, CancellationToken cancellationToken)
    {
        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == meetingId);
        if (meeting == null) throw new AppException("Meeting not found");

        await GrantTimeAchievement(meeting.OrganizerId, cancellationToken);

        foreach (var mp in meeting.MeetingParticipants)
        {
            await GrantTimeAchievement(mp.ParticipantId, cancellationToken);
        }
    }

    private async Task GrantTimeAchievement(Guid userId, CancellationToken cancellationToken)
    {
        var meetings = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .ToListAsync(cancellationToken);

        var totalTime = meetings
            .Where(x => x.EndDateTimeUtc < _dateTimeProvider.UtcNow && (x.OrganizerId == userId
                    || (x.MeetingParticipants.ToList().Any(x => x.ParticipantId == userId) && x.MeetingParticipants.FirstOrDefault(x => x.ParticipantId == userId)!.InvitationStatus == InvitationStatus.Accepted)))
            .Sum(x => (int)(x.EndDateTimeUtc - x.StartDateTimeUtc).TotalHours);

        if (totalTime >= 24)
        {
            await _dbContext.AddAchievementAsync(userId, "TIME24", _dateTimeProvider, cancellationToken);
        }
        else if (totalTime >= 10)
        {
            await _dbContext.AddAchievementAsync(userId, "TIME10", _dateTimeProvider, cancellationToken);
        }
        else if (totalTime >= 1)
        {
            await _dbContext.AddAchievementAsync(userId, "TIME01", _dateTimeProvider, cancellationToken);
        }
    }
}
