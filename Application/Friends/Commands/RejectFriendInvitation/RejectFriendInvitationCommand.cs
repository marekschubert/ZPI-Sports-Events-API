﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Commands.RejectFriendInvitation;

public class RejectFriendInvitationCommand : IRequest<Unit>
{
    public Guid InviterId { get; set; }
}

public class RejectFriendInvitationCommandHandler : IRequestHandler<RejectFriendInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public RejectFriendInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(RejectFriendInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("Logged user is not found");

        var inviter = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == request.InviterId, cancellationToken);
        if (inviter is null) throw new AppException($"User {request.InviterId} is not found");

        var currentFriendshipState = await _dbContext
            .Friendships
            .Where(x => x.Invitee == user && x.Inviter == inviter)
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (currentFriendshipState is null)
        {
            throw new AppException($"No invitation found from user {inviter.Username}");
        }
        
        if(currentFriendshipState.FriendshipStatus != FriendshipStatus.Invited)
        {
            if (currentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
                throw new AppException($"User {inviter.Username} is already a friend");

            if (currentFriendshipState.FriendshipStatus == FriendshipStatus.Blocked)
                throw new AppException($"User {inviter.Username} is blocked");
        }

        _dbContext.Friendships.Add(new Friendship
        {
            Inviter = inviter,
            Invitee = user,
            FriendshipStatus = FriendshipStatus.Rejected
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(Unit.Value);
    }
}