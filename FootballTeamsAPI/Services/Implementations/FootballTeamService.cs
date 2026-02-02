using FootballTeamsAPI.Data;
using FootballTeamsAPI.Dtos;
using FootballTeamsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FootballTeamsAPI.Services.Implementations
{
    public class FootballTeamService : IFootballTeamService
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
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.Team)
                    => data.Where(x => EF.Functions.Like(x.Team, $"%{searchTerm}%")),

                nameof(FootBallTeam.Mascot)
                    => data.Where(x => EF.Functions.Like(x.Mascot, $"%{searchTerm}%")),

                nameof(FootBallTeam.WinningPercentage)
                    => decimal.TryParse(searchTerm, out var wp)
                        ? data.Where(x => x.WinningPercentage >= wp)
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.Wins)
                    => int.TryParse(searchTerm, out var wins)
                        ? data.Where(x => x.Wins >= wins)
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.Losses)
                    => int.TryParse(searchTerm, out var losses)
                        ? data.Where(x => x.Losses >= losses)
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.Ties)
                    => int.TryParse(searchTerm, out var ties)
                        ? data.Where(x => x.Ties >= ties)
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.Games)
                    => int.TryParse(searchTerm, out var games)
                        ? data.Where(x => x.Games >= games)
                        : throw new ArgumentException("Value provided is not a valid number"),

                nameof(FootBallTeam.DateOfLastWin)
                    => DateOnly.TryParse(searchTerm, out var date)
                        ? data.Where(x => x.DateOfLastWin >= date)
                        : throw new ArgumentException("Value provided is not a valid date"),

                _ => data
            };

            return filteredData;
        }

        public async Task<(int inserted, IEnumerable<CsvRowError> errors, List<FootBallTeam> records)> AddNewFootballTeamsData(List<FootballTeamDto> footballTeams)
        {
            _dbContext.FootBallTeam.RemoveRange(_dbContext.FootBallTeam);
            var inserted = 0;
            var errors = new List<CsvRowError>();
            for (int i = 0; i < footballTeams.Count; i++)
            {
                var rowNumber = i + 2;
                var row = footballTeams[i];
                StringBuilder rowErrors = new();

                try
                {
                    if (string.IsNullOrEmpty(row.Team.Trim()))
                        rowErrors.AppendLine("Team is Required");

                    if (string.IsNullOrEmpty(row.Mascot.Trim()))
                        rowErrors.AppendLine("Mascot is required");

                    if (!DateOnly.TryParse(row.DateOfLastWin, out DateOnly dateOfLasWin))
                    {
                        rowErrors.AppendLine("Date of Last Win should be a valid date");
                    }

                    if (!Decimal.TryParse(row.WinningPercentage, out decimal winningPercentage))
                    {
                        rowErrors.AppendLine("Winning Percentage should be a valid decimal number");
                    }

                    if (!int.TryParse(row.Wins, out int wins))
                    {
                        rowErrors.AppendLine("Wins should be a valid integer number");
                    }

                    if (!int.TryParse(row.Losses, out int losses))
                    {
                        rowErrors.AppendLine("Losses should be a valid integer number");
                    }

                    if (!int.TryParse(row.Ties, out int ties))
                    {
                        rowErrors.AppendLine("Ties should be a valid integer number");
                    }

                    if (!int.TryParse(row.Games, out int games))
                    {
                        rowErrors.AppendLine("Games should be a valid integer number");
                    }

                    if (!int.TryParse(row.Rank, out int rank))
                    {
                        rowErrors.AppendLine("Rank should be a valid integer number");
                    }

                    var rowErrorsString = rowErrors.ToString();

                    if (rowErrorsString.Length > 0)
                    {
                        errors.Add(new CsvRowError
                        {
                            RowNumber = rowNumber,
                            Row = row,
                            Error = rowErrorsString.ToString()
                        });
                    }
                    else
                    {
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
