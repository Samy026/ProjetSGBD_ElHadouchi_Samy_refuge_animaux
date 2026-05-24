using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class EntreeRepository
{
    public void Ajouter(Entree entree)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO ani_entree(raison, date_entree, ani_identifiant, entree_contact)
            VALUES (@raison, @date, @animal, @contact);";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("raison", entree.Raison);
        cmd.Parameters.AddWithValue("date", entree.DateEntree);
        cmd.Parameters.AddWithValue("animal", entree.AniIdentifiant);
        cmd.Parameters.AddWithValue("contact", entree.EntreeContact);
        cmd.ExecuteNonQuery();
    }
}
