using CsvHelper.Configuration;
using FootballTeamsAPI.Dtos;

namespace FootballTeamsAPI.CsvConfiguration
{
    public class FootballCsvMap : ClassMap<FootballTeamDto>
    {
        public FootballCsvMap()
        {
            Map(m => m.DateOfLastWin).Name("Date of Last Win");
            Map(m => m.WinningPercentage).Name("Winning Percentage");
            Map(m => m.Rank).Name("Rank");
            Map(m => m.Team).Name("Team");
            Map(m => m.Wins).Name("Wins");
            Map(m => m.Losses).Name("Losses");
            Map(m => m.Ties).Name("Ties");
            Map(m => m.Games).Name("Games");
            Map(m => m.Mascot).Name("Mascot");
        }
    }
}
