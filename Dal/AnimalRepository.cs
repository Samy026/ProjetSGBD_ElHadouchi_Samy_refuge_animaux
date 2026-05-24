using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class AnimalRepository
{
    public void Ajouter(Animal animal)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO animal(identifiant, nom, type, sexe, particularites, date_deces, description, date_sterilisation, sterilise, date_naissance)
            VALUES (@id, @nom, @type, @sexe, @part, @deces, @desc, @sterDate, @ster, @naiss);";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("id", animal.Identifiant);
        cmd.Parameters.AddWithValue("nom", animal.Nom);
        cmd.Parameters.AddWithValue("type", animal.Type);
        cmd.Parameters.AddWithValue("sexe", animal.Sexe);
        cmd.Parameters.AddWithValue("part", (object?)animal.Particularites ?? DBNull.Value);
        cmd.Parameters.AddWithValue("deces", (object?)animal.DateDeces ?? DBNull.Value);
        cmd.Parameters.AddWithValue("desc", (object?)animal.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("sterDate", (object?)animal.DateSterilisation ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ster", animal.Sterilise);
        cmd.Parameters.AddWithValue("naiss", animal.DateNaissance);
        cmd.ExecuteNonQuery();
    }

    public List<Animal> ListerTous()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM animal ORDER BY identifiant", cnx);
        using var r = cmd.ExecuteReader();
        var animaux = new List<Animal>();
        while (r.Read()) animaux.Add(LireAnimal(r));
        return animaux;
    }

    public Animal? Consulter(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM animal WHERE identifiant=@id", cnx);
        cmd.Parameters.AddWithValue("id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? LireAnimal(r) : null;
    }

    public void Supprimer(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("DELETE FROM animal WHERE identifiant=@id", cnx);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    private static Animal LireAnimal(NpgsqlDataReader r) => new()
    {
        Identifiant = r.GetString(r.GetOrdinal("identifiant")),
        Nom = r.GetString(r.GetOrdinal("nom")),
        Type = r.GetString(r.GetOrdinal("type")),
        Sexe = r.GetString(r.GetOrdinal("sexe")),
        Particularites = r["particularites"] as string,
        DateDeces = r["date_deces"] == DBNull.Value ? null : (DateTime?)r["date_deces"],
        Description = r["description"] as string,
        DateSterilisation = r["date_sterilisation"] == DBNull.Value ? null : (DateTime?)r["date_sterilisation"],
        Sterilise = r.GetBoolean(r.GetOrdinal("sterilise")),
        DateNaissance = r.GetDateTime(r.GetOrdinal("date_naissance"))
    };
}
