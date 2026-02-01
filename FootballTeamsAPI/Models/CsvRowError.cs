using FootballTeamsAPI.Dtos;

namespace FootballTeamsAPI.Models
{
    public class CsvRowError
    {
        public int RowNumber { get; set; }
        public FootballTeamDto Row { get; set; } = default!;
        public string Error { get; set; } = string.Empty;
    }
}
