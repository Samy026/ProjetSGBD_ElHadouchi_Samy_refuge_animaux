-- Script de création des tables - SamyElHAdouchi
-- Base de données : refuge_animaux

DROP TABLE IF EXISTS vaccination CASCADE;
DROP TABLE IF EXISTS animal_couleur CASCADE;
DROP TABLE IF EXISTS ani_compatibilite CASCADE;
DROP TABLE IF EXISTS famille_accueil CASCADE;
DROP TABLE IF EXISTS adoption CASCADE;
DROP TABLE IF EXISTS ani_sortie CASCADE;
DROP TABLE IF EXISTS ani_entree CASCADE;
DROP TABLE IF EXISTS personne_role CASCADE;
DROP TABLE IF EXISTS contact CASCADE;
DROP TABLE IF EXISTS role_refuge CASCADE;
DROP TABLE IF EXISTS vaccin CASCADE;
DROP TABLE IF EXISTS compatibilite CASCADE;
DROP TABLE IF EXISTS couleur CASCADE;
DROP TABLE IF EXISTS animal CASCADE;

CREATE TABLE animal (
    identifiant CHAR(11) PRIMARY KEY CHECK (identifiant ~ '^[0-9]{11}$'),
    nom VARCHAR(80) NOT NULL CHECK (length(trim(nom)) >= 2),
    type VARCHAR(10) NOT NULL CHECK (type IN ('chat', 'chien')),
    sexe CHAR(1) NOT NULL CHECK (sexe IN ('M', 'F')),
    particularites TEXT,
    date_deces DATE,
    description TEXT,
    date_sterilisation DATE,
    sterilise BOOLEAN NOT NULL,
    date_naissance DATE NOT NULL CHECK (date_naissance <= CURRENT_DATE),
    CONSTRAINT chk_sterilisation CHECK (
        (sterilise = FALSE AND date_sterilisation IS NULL)
        OR (sterilise = TRUE AND (date_sterilisation IS NULL OR date_sterilisation >= date_naissance))
    ),
    CONSTRAINT chk_deces CHECK (date_deces IS NULL OR date_deces >= date_naissance)
);

CREATE TABLE couleur (
    col_identifiant SERIAL PRIMARY KEY,
    nom_couleur VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE animal_couleur (
    col_identifiant INTEGER REFERENCES couleur(col_identifiant) ON DELETE CASCADE,
    ani_identifiant CHAR(11) REFERENCES animal(identifiant) ON DELETE CASCADE,
    PRIMARY KEY (col_identifiant, ani_identifiant)
);

CREATE TABLE compatibilite (
    identifiant SERIAL PRIMARY KEY,
    type VARCHAR(30) NOT NULL UNIQUE CHECK (type IN ('chat','chien','jeune enfant','enfant','jardin','poney'))
);

CREATE TABLE vaccin (
    identifiant SERIAL PRIMARY KEY,
    nom VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE role_refuge (
    rol_identifiant SERIAL PRIMARY KEY,
    rol_nom VARCHAR(30) NOT NULL UNIQUE CHECK (rol_nom IN ('benevole','adoptant','candidat','famille_accueil','autres'))
);

CREATE TABLE contact (
    contact_identifiant SERIAL PRIMARY KEY,
    nom VARCHAR(80) NOT NULL CHECK (length(trim(nom)) >= 2),
    prenom VARCHAR(80) NOT NULL CHECK (length(trim(prenom)) >= 2),
    registre_national VARCHAR(20) UNIQUE CHECK (registre_national IS NULL OR registre_national ~ '^[0-9]{2}\.[0-9]{2}\.[0-9]{2}-[0-9]{3}\.[0-9]{2}$'),
    rue VARCHAR(120),
    cp VARCHAR(10),
    localite VARCHAR(80),
    gsm VARCHAR(25),
    telephone VARCHAR(25),
    email VARCHAR(120),
    CONSTRAINT chk_moyen_contact CHECK (gsm IS NOT NULL OR telephone IS NOT NULL OR email IS NOT NULL),
    CONSTRAINT chk_email CHECK (email IS NULL OR email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

CREATE TABLE personne_role (
    pers_identifiant INTEGER REFERENCES contact(contact_identifiant) ON DELETE CASCADE,
    rol_identifiant INTEGER REFERENCES role_refuge(rol_identifiant) ON DELETE CASCADE,
    PRIMARY KEY (pers_identifiant, rol_identifiant)
);

CREATE TABLE ani_entree (
    entree_id SERIAL PRIMARY KEY,
    raison VARCHAR(40) NOT NULL CHECK (raison IN ('abandon','errant','deces_proprietaire','saisie','retour_adoption','retour_famille_accueil')),
    date_entree DATE NOT NULL,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    entree_contact INTEGER REFERENCES contact(contact_identifiant)
);

CREATE TABLE ani_sortie (
    sortie_id SERIAL PRIMARY KEY,
    raison VARCHAR(40) NOT NULL CHECK (raison IN ('adoption','retour_proprietaire','deces_animal','famille_accueil')),
    date_sortie DATE NOT NULL,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    sortie_contact INTEGER REFERENCES contact(contact_identifiant)
);

CREATE TABLE adoption (
    adoption_id SERIAL PRIMARY KEY,
    statut VARCHAR(40) NOT NULL CHECK (statut IN ('demande','acceptee','rejet_environnement','rejet_comportement')),
    date_demande DATE NOT NULL,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    adop_contact INTEGER REFERENCES contact(contact_identifiant)
);

CREATE TABLE famille_accueil (
    accueil_id SERIAL PRIMARY KEY,
    date_debut DATE NOT NULL,
    date_fin DATE,
    fa_ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    fa_contact INTEGER NOT NULL REFERENCES contact(contact_identifiant),
    CONSTRAINT chk_dates_accueil CHECK (date_fin IS NULL OR date_fin >= date_debut)
);

CREATE TABLE ani_compatibilite (
    valeur VARCHAR(10) NOT NULL CHECK (valeur IN ('oui','non','non testé')),
    description TEXT,
    comp_identifiant INTEGER REFERENCES compatibilite(identifiant) ON DELETE CASCADE,
    ani_identifiant CHAR(11) REFERENCES animal(identifiant) ON DELETE CASCADE,
    PRIMARY KEY (comp_identifiant, ani_identifiant)
);

CREATE TABLE vaccination (
    vaccination_id SERIAL PRIMARY KEY,
    vaccination_date DATE NOT NULL,
    vac_animal CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    id_vaccin INTEGER NOT NULL REFERENCES vaccin(identifiant),
    UNIQUE (vaccination_date, vac_animal, id_vaccin)
);

INSERT INTO role_refuge(rol_nom) VALUES
('benevole'), ('adoptant'), ('candidat'), ('famille_accueil'), ('autres');

INSERT INTO compatibilite(type) VALUES
('chat'), ('chien'), ('jeune enfant'), ('enfant'), ('jardin'), ('poney');
