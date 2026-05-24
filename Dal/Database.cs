using Npgsql;

namespace RefugeAnimaux.Dal;

public static class Database
{
    // À adapter selon PostgreSQL/pgAdmin.
    // Port courant: 5432. Si ton PostgreSQL est sur 5433, remplace Port=5432 par Port=5433.
    public static string ConnectionString =
        "Host=localhost;Port=5432;Database=refuge_animaux;Username=postgres;Password=Samy2001";

    public static NpgsqlConnection GetConnection()
    {
        var cnx = new NpgsqlConnection(ConnectionString);
        cnx.Open();
        return cnx;
    }
}
