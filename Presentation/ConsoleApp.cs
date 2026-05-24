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
                    case "10": ModifierContact(); break;
                    case "11": AjouterFamilleAccueil(); break;
                    case "12": AjouterAdoption(); break;
                    case "13": ModifierStatutAdoption(); break;
                    case "14": ListerFamillesAccueilAnimal(); break;
                    case "15": AjouterSortie(); break;
                    case "16": AjouterCompatibilite(); break;
                    case "0": return; break;
    
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
        Console.WriteLine("10. Modifier un contact");
        Console.WriteLine("11. Ajouter une famille d'accueil");
        Console.WriteLine("12. Ajouter une adoption");
        Console.WriteLine("13. Modifier le statut d'une adoption");
        Console.WriteLine("14. Lister les familles d'accueil d'un animal");
        Console.WriteLine("15. Ajouter une sortie à un animal");
        Console.WriteLine("16. Ajouter une compatibilité à un animal");
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

    private void ModifierContact()
    {
        int id = int.Parse(LireObligatoire("Identifiant du contact : "));

        string adresse = LireObligatoire("Nouvelle adresse : ");
        string gsm = LireObligatoire("Nouveau GSM : ");
        string telephone = LireObligatoire("Nouveau téléphone : ");
        string email = LireObligatoire("Nouvel email : ");

        _contactRepo.ModifierContact(id, adresse, gsm, telephone, email);

        Console.WriteLine("Contact modifié.");
    }

    private void AjouterFamilleAccueil()
    {
        string animalId = LireObligatoire("Identifiant de l'animal : ");

        int contactId = int.Parse(LireObligatoire("Identifiant du contact : "));

        DateOnly dateDebut = DateOnly.Parse(
            LireObligatoire("Date d'arrivée (yyyy-mm-dd) : ")
        );

        _animalRepo.AjouterFamilleAccueil(animalId, contactId, dateDebut);

        Console.WriteLine("Famille d'accueil ajoutée.");
    }

    private void AjouterAdoption()
    {
        string animalId = LireObligatoire("Identifiant de l'animal : ");

        int contactId = int.Parse(
            LireObligatoire("Identifiant du contact : ")
        );

        DateOnly dateDemande = DateOnly.Parse(
            LireObligatoire("Date de demande (yyyy-mm-dd) : ")
        );

        _animalRepo.AjouterAdoption(animalId, contactId, dateDemande);

        Console.WriteLine("Adoption ajoutée.");
    }

    private void ModifierStatutAdoption()
    {
        int idAdoption = int.Parse(
            LireObligatoire("Identifiant de l'adoption : ")
        );

        Console.WriteLine(
            "Statuts possibles : demande, acceptee, rejet_environnement, rejet_comportement"
        );

        string statut = LireObligatoire("Nouveau statut : ");

        _animalRepo.ModifierStatutAdoption(idAdoption, statut);

        Console.WriteLine("Statut de l'adoption modifié.");
    }

    private void ListerFamillesAccueilAnimal()
    {
        string animalId = LireObligatoire(
            "Identifiant de l'animal : "
        );

        _animalRepo.ListerFamillesAccueilAnimal(animalId);
    }

    private void AjouterSortie()
    {
        string animalId = LireObligatoire(
            "Identifiant de l'animal : "
        );

        int contactId = int.Parse(
            LireObligatoire("Identifiant du contact : ")
        );

        Console.WriteLine(
            "Raisons possibles : adoption, retour_proprietaire, deces_animal, famille_accueil"
        );

        string raison = LireObligatoire("Raison : ");

        DateOnly dateSortie = DateOnly.Parse(
            LireObligatoire("Date sortie (yyyy-mm-dd) : ")
        );

        _animalRepo.AjouterSortie(
            animalId,
            contactId,
            raison,
            dateSortie
        );

        Console.WriteLine("Sortie ajoutée.");
    }
    private static string LireObligatoire(string libelle)
    {
        string? valeur;
        do { Console.Write($"{libelle} : "); valeur = Console.ReadLine(); } while (string.IsNullOrWhiteSpace(valeur));
        return valeur.Trim();
    }

    private void AjouterCompatibilite()
    {
        string animalId = LireObligatoire(
            "Identifiant de l'animal : "
        );

        Console.WriteLine("1 = chat");
        Console.WriteLine("2 = chien");
        Console.WriteLine("3 = jeune enfant");
        Console.WriteLine("4 = enfant");
        Console.WriteLine("5 = jardin");
        Console.WriteLine("6 = poney");

        int idCompatibilite = int.Parse(
            LireObligatoire("Id compatibilité : ")
        );

        string valeur = LireObligatoire(
            "Valeur (oui/non/non teste) : "
        );

        _animalRepo.AjouterCompatibilite(
            animalId,
            idCompatibilite,
            valeur
        );

        Console.WriteLine("Compatibilité ajoutée.");
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
