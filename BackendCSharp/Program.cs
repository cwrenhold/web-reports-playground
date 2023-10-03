using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

builder.Services.AddControllers();

builder.Services.AddSingleton<DbConnectionStringBuilder>(service =>
{
    var connectionStringBuilder = new DbConnectionStringBuilder
    {
        { "Host", Environment.GetEnvironmentVariable("POSTGRES_SERVER") ?? string.Empty },
        { "Database", Environment.GetEnvironmentVariable("PROJECT_DB") ?? string.Empty },
        { "Username", Environment.GetEnvironmentVariable("POSTGRES_USER") ?? string.Empty },
        { "Password", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty },
        { "Port", Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? string.Empty }
    };

    return connectionStringBuilder;
});

builder.Services.AddDbContext<BackendCSharp.Data.PlaygroundContext>();

// In development, allow all origins, headers, and methods.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();

    app.UseCors();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
