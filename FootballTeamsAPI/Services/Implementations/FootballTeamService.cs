using FootballTeamsAPI.Data;
using FootballTeamsAPI.Dtos;
using FootballTeamsAPI.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FootballTeamsAPI.Services.Implementations
{
    public class FootballTeamService: IFootballTeamService
    {
        private readonly AppDbContext _dbContext;

        public FootballTeamService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<FootBallTeam> FilterTeams(string searchTerm, string column)
        {
            var data = _dbContext.FootBallTeam.AsQueryable();
            var filteredData = column switch
            {
                nameof(FootBallTeam.Rank)
                    => int.TryParse(searchTerm, out var rank)
                        ? data.Where(x => x.Rank == rank)
                        : data,

                nameof(FootBallTeam.Team)
                    => data.Where(x => x.Team.ToLower().Contains(searchTerm.ToLower())),

                nameof(FootBallTeam.Mascot)
                    => data.Where(x => x.Team.ToLower().Contains(searchTerm.ToLower())),

                nameof(FootBallTeam.WinningPercentage)
                    => decimal.TryParse(searchTerm, out var wp)
                        ? data.Where(x => x.WinningPercentage >= wp)
                        : data,

                nameof(FootBallTeam.Wins)
                    => int.TryParse(searchTerm, out var wins)
                        ? data.Where(x => x.Wins >= wins)
                        : data,

                nameof(FootBallTeam.Losses)
                    => int.TryParse(searchTerm, out var losses)
                        ? data.Where(x => x.Losses >= losses)
                        : data,

                nameof(FootBallTeam.Ties)
                    => int.TryParse(searchTerm, out var ties)
                        ? data.Where(x => x.Ties >= ties)
                        : data,

                nameof(FootBallTeam.Games)
                    => int.TryParse(searchTerm, out var games)
                        ? data.Where(x => x.Games >= games)
                        : data,

                nameof(FootBallTeam.DateOfLastWin)
                    => DateOnly.TryParse(searchTerm, out var date)
                        ? data.Where(x => x.DateOfLastWin >= date)
                        : data,

                _ => data
            };

            return filteredData;
        }

        public async Task <(int inserted, IEnumerable<CsvRowError> errors, List<FootBallTeam> records)>AddNewFootballTeamsData(List<FootballTeamDto> footballTeams)
        {
            _dbContext.FootBallTeam.RemoveRange(_dbContext.FootBallTeam);
            var inserted = 0;
            var errors = new List<CsvRowError>();
            for (int i = 0; i < footballTeams.Count; i++)
            {
                var rowNumber = i + 2;
                var row = footballTeams[i];

                try
                {
                    if (string.IsNullOrEmpty(row.Team.Trim()))
                        throw new ArgumentException("Team name is required");

                    if (string.IsNullOrEmpty(row.Mascot.Trim()))
                        throw new ArgumentException("Team name is required");

                    if (!DateOnly.TryParse(row.DateOfLastWin, out DateOnly dateOfLasWin))
                    {
                        throw new ArgumentException("Date of Last Win should be a valid date");
                    }

                    if (!Decimal.TryParse(row.WinningPercentage, out decimal winningPercentage))
                    {
                        throw new ArgumentException("Winning Percentage should be a valid decimal number");
                    }

                    if (!int.TryParse(row.Wins, out int wins))
                    {
                        throw new ArgumentException("Wins should be a valid integer number");
                    }

                    if (!int.TryParse(row.Losses, out int losses))
                    {
                        throw new ArgumentException("Losses should be a valid integer number");
                    }

                    if (!int.TryParse(row.Ties, out int ties))
                    {
                        throw new ArgumentException("Ties should be a valid integer number");
                    }

                    if (!int.TryParse(row.Games, out int games))
                    {
                        throw new ArgumentException("Games should be a valid integer number");
                    }

                    if (!int.TryParse(row.Rank, out int rank))
                    {
                        throw new ArgumentException("Games should be a valid integer number");
                    }

                    var footBallTeam = new FootBallTeam
                    {
                        Team = row.Team.Trim(),
                        Mascot = row.Mascot.Trim(),
                        DateOfLastWin = dateOfLasWin,
                        WinningPercentage = winningPercentage,
                        Wins = wins,
                        Ties = ties,
                        Losses = losses,
                        Games = games,
                        Rank = rank
                    };

                    _dbContext.FootBallTeam.Add(footBallTeam);
                    inserted++;
                }
                catch (Exception ex)
                {
                    errors.Add(new CsvRowError
                    {
                        RowNumber = rowNumber,
                        Row = row,
                        Error = ex.Message
                    });

                }
            }

            await _dbContext.SaveChangesAsync();
            var records = _dbContext.FootBallTeam.ToList();
            return (inserted, errors, records);
        }
    }
}
