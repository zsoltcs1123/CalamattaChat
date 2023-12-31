﻿using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Teams;

public interface ITeamService
{
    Team? GetActiveTeam();
    Team? GetOverflowTeam();
}