using PersonCRUD.Data;
using PersonCRUD.Routes;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PersonContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.PersonRoutes();

//app.UseHttpsRedirection();
app.Run();

