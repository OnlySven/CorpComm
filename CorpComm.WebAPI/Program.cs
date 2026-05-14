using CorpComm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CorpComm.Application.Features.Users.Commands;
using CorpComm.Domain.Repositories;
using CorpComm.Infrastructure.Repositories;
using CorpComm.Application.Common.Behaviors;
using CorpComm.WebAPI.Hubs;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(CorpComm.Application.Features.Users.Commands.CreateUserCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Автоматична реєстрація всіх валідаторів з Application шару
builder.Services.AddValidatorsFromAssembly(typeof(CorpComm.Application.Features.Users.Commands.CreateUserCommand).Assembly);

builder.Services.AddSignalR(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/users", async (CreateUserCommand command, IMediator mediator) =>
{
    try
    {
        var userId = await mediator.Send(command);
        return Results.Ok(new { Id = userId });
    }

    catch (ValidationException valEx)
    {
        return Results.BadRequest(new 
        { 
            Message = "Validation failed", 
            Errors = valEx.Errors.Select(e => new 
            { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }) 
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

app.UseHttpsRedirection();

app.MapHub<MeetingHub>("/hubs/meeting");

app.Run();