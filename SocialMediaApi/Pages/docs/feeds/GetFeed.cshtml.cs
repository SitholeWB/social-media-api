using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace SocialMediaApi.Pages.docs.feeds
{
    public class GetFeedModel : PageModel
    {
        public string Diagram { get; set; } = string.Empty;

        public void OnGet()
        {
            var sb = new StringBuilder();
            sb.AppendLine("sequenceDiagram");
            sb.AppendLine("participant User");
            sb.AppendLine("participant API");
            sb.AppendLine("activate API");
            sb.AppendLine("User->>API: Get Feed (Posts) for User.");
            sb.AppendLine("API->>API: Get Groups linked to the user from the UserDetails table.");
            sb.AppendLine("API->>API: Get recent Posts for linked Groups, with pagination limit.");
            sb.AppendLine("API->>User: Done");
            sb.AppendLine("deactivate API");

            Diagram = sb.ToString();
        }
    }
}