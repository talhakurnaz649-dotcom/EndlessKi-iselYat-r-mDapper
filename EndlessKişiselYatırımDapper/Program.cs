using Microsoft.Data.Sqlite;
using Dapper;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=finance.db";

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Varliklar (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Sembol TEXT NOT NULL,
            Ad TEXT NOT NULL,
            Miktar REAL NOT NULL,
            Fiyat REAL NOT NULL
        );
    ");

    var count = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Varliklar;");
    if (count == 0)
    {
        connection.Execute(@"
            INSERT INTO Varliklar (Sembol, Ad, Miktar, Fiyat) VALUES ('THYAO', 'Türk Hava Yolları', 100, 312.5);
            INSERT INTO Varliklar (Sembol, Ad, Miktar, Fiyat) VALUES ('EREGL', 'Ereğli Demir Çelik', 500, 48.2);
            INSERT INTO Varliklar (Sembol, Ad, Miktar, Fiyat) VALUES ('BTC', 'Bitcoin', 0.05, 67250.0);
        ");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
