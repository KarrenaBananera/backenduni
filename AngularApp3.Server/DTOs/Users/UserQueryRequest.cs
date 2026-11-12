using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.DTOs.Users;

public class UserQueryRequest
{
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "limit")]
    [Range(1, 100)]
    public int Limit { get; set; } = 10;

    [FromQuery(Name = "search")]
    public string? Search { get; set; }
}
