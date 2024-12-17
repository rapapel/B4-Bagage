using System;
using System.Collections.Generic;
using System.IO;

namespace BagageSolo
{
    class Program
    {
        static readonly string FilePath = @"C:\Users\rapha\OneDrive\Documenten\School\Blok B\PRA\B4 Bagage\bin\Debug\net8.0\lockers.txt";


        static void Main(string[] args)
        {
            Console.WriteLine("Welkom bij zwembad de golf!\n");

            while (true)
            {
                Console.WriteLine("Hoofdmenu:");
                Console.WriteLine("1. Laat het aantal kluizen zien.");
                Console.WriteLine("2. Huur een nieuwe kluis.");
                Console.WriteLine("3. Open een gehuurde kluis.");
                Console.WriteLine("4. Geef een kluis terug.");
                Console.WriteLine("5. Exit.");
                Console.Write("Maak een keuze: ");

                string keuze = Console.ReadLine();

                if (keuze == "1")
                {
                    int vrijeKluizen = LockerCount();
                    Console.WriteLine($"Er zijn {vrijeKluizen} kluizen beschikbaar.");
                }
                else if (keuze == "2")
                {
                    int result = NewLocker();
                    if (result == 0)
                        Console.WriteLine("Nieuwe kluis succesvol gehuurd!");
                    else if (result == -1)
                        Console.WriteLine("Fout: Het wachtwoord voldoet niet aan de eisen.");
                    else
                        Console.WriteLine("Er is iets mis gegaan bij het huren van de kluis.");
                }
                else if (keuze == "3")
                {
                    bool geopend = OpenLocker();
                    Console.WriteLine(geopend
                        ? "Kluis succesvol geopend."
                        : "Fout: Kluisnummer of wachtwoord onjuist.");
                }
                else if (keuze == "4")
                {
                    bool teruggegeven = ReturnLocker();
                    Console.WriteLine(teruggegeven
                        ? "Kluis succesvol teruggegeven."
                        : "Fout: Kluisnummer of wachtwoord onjuist.");
                }
                else if (keuze == "5")
                {
                    Console.WriteLine("Programma wordt afgesloten. Tot ziens!");
                    break;
                }
                else
                {
                    Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
                }
            }
        }

        static int LockerCount()
        {
            List<Kluis> lockers = GetLockersFromFile();
            return 100 - lockers.Count;
        }

        static int NewLocker()
        {
            List<Kluis> lockers = GetLockersFromFile();
            List<int> usedLockerNumbers = new List<int>();
            foreach (var locker in lockers)
            {
                usedLockerNumbers.Add(int.Parse(locker.Code));
            }

            Console.WriteLine("Beschikbare kluizen: ");
            for (int i = 1; i <= 100; i++)
            {
                if (!usedLockerNumbers.Contains(i))
                {
                    Console.Write(i + " ");
                }
            }
            Console.WriteLine();

            Console.Write("Kies een kluisnummer: ");
            if (!int.TryParse(Console.ReadLine(), out int chosenLocker) || chosenLocker < 1 || chosenLocker > 100 || usedLockerNumbers.Contains(chosenLocker))
            {
                Console.WriteLine("Ongeldig kluisnummer of kluis is al bezet.");
                return 1;
            }

            Console.Write("Kies een wachtwoord (4-20 tekens, geen ';'): ");
            string password = Console.ReadLine();
            if (password.Length < 4 || password.Length > 20 || password.Contains(";"))
            {
                return -1;
            }

            lockers.Add(new Kluis { Code = chosenLocker.ToString(), Password = password });
            SaveLockersToFile(lockers);

            return 0;
        }

        static bool OpenLocker()
        {
            List<Kluis> lockers = GetLockersFromFile();

            Console.Write("Voer uw kluisnummer in: ");
            string kluisNummer = Console.ReadLine();
            Console.Write("Voer uw wachtwoord in: ");
            string wachtwoord = Console.ReadLine();

            foreach (var locker in lockers)
            {
                if (locker.Code == kluisNummer && locker.Password == wachtwoord)
                {
                    return true; // Kluis gevonden en geopend
                }
            }

            return false; // Kluis niet gevonden
        }

        static bool ReturnLocker()
        {
            List<Kluis> lockers = GetLockersFromFile();

            Console.Write("Voer uw kluisnummer in: ");
            string kluisNummer = Console.ReadLine();
            Console.Write("Voer uw wachtwoord in: ");
            string wachtwoord = Console.ReadLine();

            for (int i = 0; i < lockers.Count; i++)
            {
                if (lockers[i].Code == kluisNummer && lockers[i].Password == wachtwoord)
                {
                    lockers.RemoveAt(i); // Kluis verwijderen
                    SaveLockersToFile(lockers);
                    return true; // Succesvol teruggegeven
                }
            }

            return false; // Kluis niet gevonden
        }

        static List<Kluis> GetLockersFromFile()
        {
            List<Kluis> lockers = new List<Kluis>();

            if (File.Exists(FilePath))
            {
                try
                {
                    foreach (string line in File.ReadAllLines(FilePath))
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length == 2)
                        {
                            lockers.Add(new Kluis { Code = parts[0], Password = parts[1] });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fout bij lezen van het bestand: {ex.Message}");
                }
            }

            return lockers;
        }

        static void SaveLockersToFile(List<Kluis> lockers)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath, false))
                {
                    foreach (Kluis locker in lockers)
                    {
                        writer.WriteLine($"{locker.Code};{locker.Password}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij opslaan: {ex.Message}");
            }
        }
    }
}

