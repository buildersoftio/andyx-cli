using Andy.X.Cli.Models.Configurations;
using System.Text;

namespace Andy.X.Cli.Utilities.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddBasicAuthorizationHeader(this HttpClient httpClient, Node node)
        {
            string encodedPassword = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
               .GetBytes(node.Username + ":" + node.Password));

            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedPassword);
        }
    }
}
