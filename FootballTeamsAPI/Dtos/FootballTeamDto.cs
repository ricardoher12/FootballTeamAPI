namespace FootballTeamsAPI.Dtos
{
    public class FootballTeamDto
    {
        public string Rank { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public string Mascot { get; set; } = string.Empty;
        public string DateOfLastWin { get; set; } = string.Empty;
        public string WinningPercentage { get; set; } = string.Empty;
        public string Wins { get; set; } = string.Empty;
        public string Losses { get; set; } = string.Empty;
        public string Ties { get; set; } = string.Empty;
        public string Games { get; set; } = string.Empty;
    }
}
