using Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Repositories.Implements;
using Repositories.Intefaces;
using Repository.Services;
using Services.Implements;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var conString = builder.Configuration.GetConnectionString("sqlConnection");
builder.Services.AddDbContext<DataContext>(opts => opts.UseSqlServer(conString));

var acceptAllCors = "AcceptAllCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: acceptAllCors,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddHttpClient<UserServices>();
builder.Services.AddScoped<IHasingServices, HasingServices>();
builder.Services.AddScoped<IJwtSupport, JwtServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IPostServices, PostServices>();
builder.Services.AddScoped<IReportServices, ReportServices>();
builder.Services.AddScoped<ISlotServices, SlotServices>();
builder.Services.AddScoped<IWalletServices, WalletServices>();
builder.Services.AddScoped<ITransactionServices, TransactionServices>();

builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();
var port = Environment.GetEnvironmentVariable("PORT");
app.Urls.Add($"http://*:{port}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHangfireDashboard("/badminton_hangfire");

app.UseRouting(); 
app.UseCors(acceptAllCors);

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
}); 

app.Run();
