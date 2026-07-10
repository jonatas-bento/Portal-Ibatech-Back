// Ibatech.API/DataSeeder.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Infra.Context;
using Ibatech.Services.Security;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.API;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IbatechDbContext>();

        // 🛠️ BYPASS DO LINQ: Executa um comando SQL bruto direto na conexão do MySQL
        // Isso ignora qualquer erro de mapeamento ou tradução do EF Core
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Usuarios;";

        // Garante que a conexão esteja aberta para o ADO.NET puro
        if (db.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
        {
            await db.Database.OpenConnectionAsync();
        }

        var totalUsuarios = Convert.ToInt32(await command.ExecuteScalarAsync());

        if (totalUsuarios > 0)
        {
            Console.WriteLine("[Seed] Tabela de usuários já possui registros — seed ignorado.");
            return;
        }

        // Criação do Admin
        var admin = new Usuario(
            nomeCompleto: "Administrador IBATECH",
            email: "admin@ibatech.com.br",
            senhaHash: PasswordHasher.Hash("Admin@123"),
            role: RoleUsuario.Admin,
            telefone: "(11) 99999-0000"
        );

        await db.Usuarios.AddAsync(admin);
        await db.SaveChangesAsync();

        Console.WriteLine("[Seed] ✅ Usuário Admin criado:");
        Console.WriteLine("[Seed]    E-mail: admin@ibatech.com.br");
        Console.WriteLine("[Seed]    Senha:  Admin@123");
        Console.WriteLine("[Seed]    ⚠️  Troque a senha após o primeiro login!");
    }
}