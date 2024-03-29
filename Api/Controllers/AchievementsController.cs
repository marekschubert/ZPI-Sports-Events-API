﻿using Application.Achievements.Queries.GetAllAchievements;
using Application.Achievements.Queries.GetAllWithFriendAchievements;
using Application.Achievements.Queries.GetAllWithUserAchievements;
using Application.Achievements.Queries.GetFriendAchievements;
using Application.Achievements.Queries.GetUserAchievements;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AchievementsController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetAll()
    {
        var groupedAchievementsDtos = await Mediator.Send(new GetAllAchievementsQuery());
        return Ok(groupedAchievementsDtos);
    }

    [HttpGet("user_achievements")]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetUserAchievements()
    {
        var groupedUserAchievements = await Mediator.Send(new GetUserAchievementsQuery());
        return Ok(groupedUserAchievements); 
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetAllWithUserAchievements()
    {
        var groupedAllWithUserAchievements = await Mediator.Send(new GetAllWithUserAchievementsQuery());
        return Ok(groupedAllWithUserAchievements);
    }

    [HttpGet("{friendId}/user_achievements")]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetFriendAchievements(Guid friendId)
    {
        var groupedUserAchievements = await Mediator.Send(new GetFriendAchievementsQuery() { FriendId = friendId});
        return Ok(groupedUserAchievements);
    }

    [HttpGet("{friendId}")]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetAllWithFriendAchievements(Guid friendId)
    {
        var groupedUserAchievements = await Mediator.Send(new GetAllWithFriendAchievementsQuery() { FriendId = friendId });
        return Ok(groupedUserAchievements);
    }
}
