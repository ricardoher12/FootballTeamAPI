using CsvHelper;
using FootballTeamsAPI.CsvConfiguration;
using FootballTeamsAPI.Data;
using FootballTeamsAPI.Dtos;
using FootballTeamsAPI.Models;
using FootballTeamsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace FootballTeamsAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Basic")]
    [Route("api/[controller]")]
    [ApiController]
    public class FootballTeamController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly HashSet<string> _allowedColumns;
        private readonly IFootballTeamService _footballTeamService;

        public FootballTeamController(AppDbContext appDbContext, IConfiguration configuration, IFootballTeamService footballTeamService)
        {
            _appDbContext = appDbContext;
            _allowedColumns = configuration.GetSection("AllowedColumns").Get<List<string>>()?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];
            _footballTeamService = footballTeamService;
        }

        [HttpPost("csv-file")]
        public async Task<ActionResult> UploadCsv(IFormFile teamsFile)
        {
            if (teamsFile == null || teamsFile.Length == 0)
            {
                return BadRequest("CSV file is required");
            }

            if (!teamsFile.FileName.EndsWith(".csv"))
            {
                return BadRequest("Only CSV files are allowed");
            }

            using var stream = teamsFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csvFile = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            csvFile.Read();
            csvFile.ReadHeader();

            var fileHeaders = new HashSet<string>(csvFile.HeaderRecord!, StringComparer.OrdinalIgnoreCase);

            var missingHeaders = _allowedColumns
                    .Where(h => !fileHeaders.Contains(h))
                    .ToList();

            if (missingHeaders.Count != 0)
            {
                string error = $"There are missing headers in the CSV file: {string.Join(", ", missingHeaders)}";
                return BadRequest(error);
            }

            csvFile.Context.RegisterClassMap<FootballCsvMap>();
            var footballTeamRecords = csvFile.GetRecords<FootballTeamDto>().ToList();
            var (inserted, errors, records) = await _footballTeamService.AddNewFootballTeamsData(footballTeamRecords);

            return Ok(new
            {
                Inserted = inserted,
                Failed = errors.Count(),
                Errors = errors,
                Records = records
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetFilteredFootballTeam(string term = "", string column = "")
        {
            //if (string.IsNullOrEmpty(term))
            //{
            //    return BadRequest(new  {message = "Please provide a valid search term" });
            //}

            //if(string.IsNullOrEmpty(column) || string.Equals(column, "all", StringComparison.OrdinalIgnoreCase))
            //{
            //    return BadRequest(new { message = "Please provide a valid column to search" });
            //}


            //if (IsInvalidSearchValue(term))
            //{
            //    return BadRequest(new { message = "Invalid search value." });
            //}

            return Ok(_footballTeamService.FilterTeams(EscapeLike(term), column));
        }

        //private static bool IsInvalidSearchValue(string value)
        //{
        //    var cleaned = value.Replace("%", "").Replace("_", "").Trim();
        //    return cleaned.Length < 2;
        //}

        private static string EscapeLike(string value)
        {
            return value
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }
    }
}
