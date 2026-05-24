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
    public void ModifierContact(int id, string adresse, string gsm, string telephone, string email)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            UPDATE contact
            SET rue = @adresse,
                gsm = @gsm,
                telephone = @telephone,
                email = @email
            WHERE contact_identifiant = @id";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("adresse", adresse);
        cmd.Parameters.AddWithValue("gsm", gsm);
        cmd.Parameters.AddWithValue("telephone", telephone);
        cmd.Parameters.AddWithValue("email", email);

        cmd.ExecuteNonQuery();
    }

    public void AjouterFamilleAccueil(string animalId, int contactId, DateOnly dateDebut)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO famille_accueil
            (date_debut, fa_ani_identifiant, fa_contact)
            VALUES
            (@dateDebut, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("dateDebut", dateDebut);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void AjouterAdoption(string animalId, int contactId, DateOnly dateDemande)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO adoption
            (statut, date_demande, ani_identifiant, adop_contact)
            VALUES
            ('demande', @dateDemande, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("dateDemande", dateDemande);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void ModifierStatutAdoption(int idAdoption, string statut)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            UPDATE adoption
            SET statut = @statut
            WHERE id_adoption = @idAdoption";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("statut", statut);
        cmd.Parameters.AddWithValue("idAdoption", idAdoption);

        cmd.ExecuteNonQuery();
    }

    public void ListerFamillesAccueilAnimal(string animalId)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            SELECT fa_contact, date_debut, date_fin
            FROM famille_accueil
            WHERE fa_ani_identifiant = @animalId";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int contact = reader.GetInt32(0);

            DateOnly dateDebut =
                reader.GetFieldValue<DateOnly>(1);

            string dateFin = reader.IsDBNull(2)
                ? "En cours"
                : reader.GetFieldValue<DateOnly>(2).ToString();

            Console.WriteLine(
                $"Contact : {contact} | Début : {dateDebut} | Fin : {dateFin}"
            );
        }
    }

    public void AjouterSortie(
        string animalId,
        int contactId,
        string raison,
        DateOnly dateSortie
    )
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO ani_sortie
            (raison, date_sortie, ani_identifiant, sortie_contact)
            VALUES
            (@raison, @dateSortie, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("raison", raison);
        cmd.Parameters.AddWithValue("dateSortie", dateSortie);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void AjouterCompatibilite(
        string animalId,
        int idCompatibilite,
        string valeur
    )
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO ani_compatibilite
            (ani_identifiant, comp_identifiant, valeur)
            VALUES
            (@animalId, @compIdentifiant, @valeur)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        cmd.Parameters.AddWithValue(
            "compIdentifiant",
            idCompatibilite
        );

        cmd.Parameters.AddWithValue("valeur", valeur);

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
