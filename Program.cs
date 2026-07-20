using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Politicore.Data.ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Servir les fichiers statiques (wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Exposer OpenAPI/Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Pointer explicitement vers le document JSON généré
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Politicore API v1");
    // Laisser le prefix par défaut (swagger) : UI accessible sur /swagger
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Politicore.Data.ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();
