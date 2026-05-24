using RefugeAnimaux.Dal;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Presentation;

public class ConsoleApp
{
    private readonly ContactRepository _contactRepo = new();
    private readonly AnimalRepository _animalRepo = new();
    private readonly EntreeRepository _entreeRepo = new();
    private readonly VaccinRepository _vaccinRepo = new();

    public void Demarrer()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        while (true)
        {
            AfficherMenu();
            Console.Write("Choix : ");
            var choix = Console.ReadLine();
            try
            {
                switch (choix)
                {
                    case "1": AjouterAnimal(); break;
                    case "2": ConsulterAnimal(); break;
                    case "3": ListerAnimaux(); break;
                    case "4": SupprimerAnimal(); break;
                    case "5": AjouterEntree(); break;
                    case "6": AjouterVaccin(); break;
                    case "7": AjouterContact(); break;
                    case "8": ListerContacts(); break;
                    case "9": SupprimerContact(); break;
                    case "0": return;
                    default: Console.WriteLine("Choix invalide."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }

    private static void AfficherMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Refuge Animaux - SamyElHAdouchi ===");
        Console.WriteLine("1. Ajouter un animal");
        Console.WriteLine("2. Consulter un animal");
        Console.WriteLine("3. Lister les animaux");
        Console.WriteLine("4. Supprimer un animal");
        Console.WriteLine("5. Ajouter une entrée à un animal");
        Console.WriteLine("6. Ajouter un vaccin à un animal");
        Console.WriteLine("7. Ajouter une personne de contact");
        Console.WriteLine("8. Lister les contacts");
        Console.WriteLine("9. Supprimer un contact");
        Console.WriteLine("0. Quitter");
    }

    private void AjouterContact()
    {
        var c = new Contact
        {
            Nom = LireObligatoire("Nom"),
            Prenom = LireObligatoire("Prénom"),
            RegistreNational = LireOptionnel("Registre national (yy.mm.dd-999.99)"),
            Rue = LireOptionnel("Rue"),
            Cp = LireOptionnel("Code postal"),
            Localite = LireOptionnel("Localité"),
            Gsm = LireOptionnel("GSM"),
            Telephone = LireOptionnel("Téléphone"),
            Email = LireOptionnel("Email")
        };
        var id = _contactRepo.Ajouter(c);
        Console.WriteLine($"Contact ajouté avec l'id {id}.");
    }

    private void ListerContacts()
    {
        var contacts = _contactRepo.ListerTous();
        if (contacts.Count == 0) { Console.WriteLine("Aucun contact."); return; }
        foreach (var c in contacts) Console.WriteLine(c);
    }

    private void SupprimerContact()
    {
        Console.Write("Id contact à supprimer : ");
        _contactRepo.Supprimer(int.Parse(Console.ReadLine() ?? "0"));
        Console.WriteLine("Contact supprimé.");
    }

    private void AjouterAnimal()
    {
        var a = new Animal
        {
            Identifiant = LireObligatoire("Identifiant (yymmdd99999)"),
            Nom = LireObligatoire("Nom"),
            Type = LireObligatoire("Type (chat/chien)"),
            Sexe = LireObligatoire("Sexe (M/F)"),
            Particularites = LireOptionnel("Particularités"),
            Description = LireOptionnel("Description"),
            Sterilise = LireBool("Stérilisé (true/false)"),
            DateNaissance = LireDate("Date naissance (yyyy-mm-dd)")
        };
        var ds = LireOptionnel("Date stérilisation (yyyy-mm-dd, vide si aucune)");
        if (!string.IsNullOrWhiteSpace(ds)) a.DateSterilisation = DateTime.Parse(ds);
        _animalRepo.Ajouter(a);
        Console.WriteLine("Animal ajouté.");
    }

    private void ListerAnimaux()
    {
        var animaux = _animalRepo.ListerTous();
        if (animaux.Count == 0) { Console.WriteLine("Aucun animal."); return; }
        foreach (var a in animaux) Console.WriteLine(a);
    }

    private void ConsulterAnimal()
    {
        var id = LireObligatoire("Identifiant animal");
        var a = _animalRepo.Consulter(id);
        Console.WriteLine(a == null ? "Animal introuvable." : a.ToString());
    }

    private void SupprimerAnimal()
    {
        var id = LireObligatoire("Identifiant animal à supprimer");
        _animalRepo.Supprimer(id);
        Console.WriteLine("Animal supprimé.");
    }

    private void AjouterEntree()
    {
        var e = new Entree
        {
            AniIdentifiant = LireObligatoire("Identifiant animal"),
            Raison = LireObligatoire("Raison (abandon/errant/deces_proprietaire/saisie/retour_adoption/retour_famille_accueil)"),
            DateEntree = LireDate("Date entrée (yyyy-mm-dd)"),
            EntreeContact = int.Parse(LireObligatoire("Id contact"))
        };
        _entreeRepo.Ajouter(e);
        Console.WriteLine("Entrée ajoutée.");
    }

    private void AjouterVaccin()
    {
        var animalId = LireObligatoire("Identifiant animal");
        var vaccin = LireObligatoire("Nom vaccin");
        var date = LireDate("Date vaccination (yyyy-mm-dd)");
        _vaccinRepo.AjouterVaccination(animalId, vaccin, date);
        Console.WriteLine("Vaccin ajouté.");
    }

    private static string LireObligatoire(string libelle)
    {
        string? valeur;
        do { Console.Write($"{libelle} : "); valeur = Console.ReadLine(); } while (string.IsNullOrWhiteSpace(valeur));
        return valeur.Trim();
    }

    private static string? LireOptionnel(string libelle)
    {
        Console.Write($"{libelle} : ");
        var valeur = Console.ReadLine();
        return string.IsNullOrWhiteSpace(valeur) ? null : valeur.Trim();
    }

    private static DateTime LireDate(string libelle) => DateTime.Parse(LireObligatoire(libelle));
    private static bool LireBool(string libelle) => bool.Parse(LireObligatoire(libelle));
}
