using FootballTeamsAPI.Dtos;
using FootballTeamsAPI.Models;

namespace FootballTeamsAPI.Services
{
    public interface IFootballTeamService
    {
        Task<(int inserted, IEnumerable<CsvRowError> errors, List<FootBallTeam> records)> AddNewFootballTeamsData(List<FootballTeamDto> footballTeams);
        IEnumerable<FootBallTeam> FilterTeams(string searchTerm, string column);
    }
}
