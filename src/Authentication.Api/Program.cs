using Authentication.Api.Validators;
using Authentication.Infrastructure.Extensions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationInfrastructure(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateGuestAccountDtoValidator));
ValidatorOptions.Global.LanguageManager.Enabled = false;

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions => swaggerGenOptions.SupportNonNullableReferenceTypes());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace Authentication.Api
{
    public partial class Program { }
}
