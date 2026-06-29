using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Configuration;
using EndlessKişiselYatırımDapper.Core.Entities;

namespace EndlessKişiselYatırımDapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VarliklarController : ControllerBase
    {
        private readonly string _connectionString;

        public VarliklarController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=finance.db";
        }

        [HttpGet]
        public async Task<IActionResult> HepsiniGetir()
        {
            using var connection = new SqliteConnection(_connectionString);
            var varliklar = await connection.QueryAsync<YatirimVarligi>("SELECT * FROM Varliklar;");
            return Ok(varliklar);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> IdIleGetir(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var varlik = await connection.QueryFirstOrDefaultAsync<YatirimVarligi>(
                "SELECT * FROM Varliklar WHERE Id = @Id;", new { Id = id });

            if (varlik == null)
                return NotFound(new { Mesaj = $"ID değeri {id} olan varlık bulunamadı." });

            return Ok(varlik);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] YatirimVarligi varlik)
        {
            if (varlik == null)
                return BadRequest();

            using var connection = new SqliteConnection(_connectionString);
            string sql = @"
                INSERT INTO Varliklar (Sembol, Ad, Miktar, Fiyat)
                VALUES (@Sembol, @Ad, @Miktar, @Fiyat);
                SELECT last_insert_rowid();";
            
            var yeniId = await connection.ExecuteScalarAsync<int>(sql, varlik);
            varlik.Id = yeniId;

            return CreatedAtAction(nameof(IdIleGetir), new { id = yeniId }, varlik);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Guncelle(int id, [FromBody] YatirimVarligi varlik)
        {
            if (varlik == null || varlik.Id != id)
                return BadRequest();

            using var connection = new SqliteConnection(_connectionString);
            string sql = @"
                UPDATE Varliklar 
                SET Sembol = @Sembol, Ad = @Ad, Miktar = @Miktar, Fiyat = @Fiyat 
                WHERE Id = @Id;";

            var etkilenenSatir = await connection.ExecuteAsync(sql, varlik);
            if (etkilenenSatir == 0)
                return NotFound(new { Mesaj = $"ID değeri {id} olan varlık bulunamadı." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Sil(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var etkilenenSatir = await connection.ExecuteAsync(
                "DELETE FROM Varliklar WHERE Id = @Id;", new { Id = id });

            if (etkilenenSatir == 0)
                return NotFound(new { Mesaj = $"ID değeri {id} olan varlık bulunamadı." });

            return NoContent();
        }
    }
}
