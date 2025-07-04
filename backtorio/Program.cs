


using backtorio.service;

namespace backtorio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var AlloweOrigins = builder.Configuration.GetSection("AlloweOrigins").Get<string[]>();
            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policybuilder =>
                {
                    policybuilder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
            
            builder.Services.AddControllers();
            builder.Services.AddSingleton<MongoDbService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           /* builder.Services.AddCors(option =>
            {
                option.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(AlloweOrigins)  // metti il dominio del tuo frontend
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });*/
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            
            

            app.Run();
        }
    }
}
