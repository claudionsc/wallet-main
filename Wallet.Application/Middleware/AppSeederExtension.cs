using Microsoft.AspNetCore.Builder;

namespace Wallet.Middleware
{
    public static class AppSeederExtension
    {
        public static IApplicationBuilder UseAppSeeder(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AppSeederMiddleware>();
        }
    }
}
