using System.Linq.Expressions;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Extensions;

public static class UserExtension
{
    public static string GetSortProductProperty(string sortColumn)
        => sortColumn.ToLower() switch
        {
            "username" => "UserName",
            "email" => "Email",
            _ => "CreatedAt"
        };
}
