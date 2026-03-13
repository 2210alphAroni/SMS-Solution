using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantService tenantService)
        {
            var host = context.Request.Host.Host;
            var parts = host.Split('.');

            // Support subdomain-based multi-tenancy: e.g., schoolname.edumanagepro.com
            if (parts.Length >= 3)
            {
                var subdomain = parts[0];
                if (subdomain != "www" && subdomain != "app" && subdomain != "api")
                {
                    var tenant = await tenantService.GetBySubdomainAsync(subdomain);
                    if (tenant != null)
                    {
                        context.Items["Tenant"] = tenant;
                        context.Items["TenantId"] = tenant.Id;
                        context.Items["TenantSubdomain"] = subdomain;
                    }
                }
            }

            // Also support query param for development: ?tenant=subdomain
            if (context.Items["Tenant"] == null)
            {
                var tenantParam = context.Request.Query["tenant"].ToString();
                if (!string.IsNullOrEmpty(tenantParam))
                {
                    var tenant = await tenantService.GetBySubdomainAsync(tenantParam);
                    if (tenant != null)
                    {
                        context.Items["Tenant"] = tenant;
                        context.Items["TenantId"] = tenant.Id;
                    }
                }
            }

            await _next(context);
        }
    }

    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
