using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace A2.Data // Use o namespace correto do seu DbContext
{
    // A classe deve herdar de IDesignTimeDbContextFactory, passando seu DbContext
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. Configurar a leitura do appsettings.json
            // O caminho aqui deve ser o diretório raiz onde seu appsettings.json está.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Obter a Connection String
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. Configurar as Opções do DbContext (como você fez no Program.cs)
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // ATENÇÃO: Adicione aqui o provedor de banco de dados que você está usando!
            // Assumindo que é SQL Server (baseado na sua Connection String anterior)
            builder.UseSqlServer(connectionString);

            // 4. Retornar a instância do DbContext
            return new ApplicationDbContext(builder.Options);
        }
    }
}