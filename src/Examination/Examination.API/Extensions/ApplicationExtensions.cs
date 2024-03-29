﻿namespace Examination.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this IApplicationBuilder app)
        {
           // var env = services.GetService<IWebHostEnvironment>();
            app.UseSwagger();

            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI();
            // app.UseMiddleware<ErrorWrappingMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
