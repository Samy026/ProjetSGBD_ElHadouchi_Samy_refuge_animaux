using Npgsql;

namespace RefugeAnimaux.Dal;

public class VaccinRepository
{
    public int AjouterVaccinSiAbsent(string nom)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO vaccin(nom) VALUES (@nom)
            ON CONFLICT (nom) DO UPDATE SET nom = EXCLUDED.nom
            RETURNING identifiant;", cnx);
        cmd.Parameters.AddWithValue("nom", nom);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void AjouterVaccination(string animalId, string nomVaccin, DateTime dateVaccination)
    {
        var idVaccin = AjouterVaccinSiAbsent(nomVaccin);
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO vaccination(vaccination_date, vac_animal, id_vaccin)
            VALUES (@date, @animal, @vaccin);", cnx);
        cmd.Parameters.AddWithValue("date", dateVaccination);
        cmd.Parameters.AddWithValue("animal", animalId);
        cmd.Parameters.AddWithValue("vaccin", idVaccin);
        cmd.ExecuteNonQuery();
    }
}
