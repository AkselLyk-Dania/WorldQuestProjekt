using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorldQuestProjekt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Få server information ved hjælp af en statisk klasse
            SServer.database = "worldquest";
            SServer.user = "sql_user";
            SServer.password = "1234";
            SServer.cs = $"Server=.\\SQLEXPRESS;Database={SServer.database};User Id={SServer.user};Password={SServer.password};Trusted_Connection=True;TrustServerCertificate=True;";

            Menu();
        }

        static void Menu()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Du har følgende muligheder:");
            Console.WriteLine("1. Ny Player");
            Console.WriteLine("2. List alle players");
            Console.WriteLine("3. Slet alle players");

            //Hvad der skrives som character
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine("");

            //Lav en ny player
            if (key == '1') PlayerName();

            //List alle players der er lavet i tabellen med player
            else if (key == '2')
            {
                //Opret forbindelse til databasen
                using (SqlConnection connection = new SqlConnection(SServer.cs))
                {
                    connection.Open();

                    //En command som tæller all classes fra tabellen f.eks. player hvis der er nogen...
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM player", connection);

                    //...til int32
                    int count = (Int32)command.ExecuteScalar();

                    //Den vil svare dette hvis der ikke var fundet nogen række i tabellen
                    if (count == 0) Console.WriteLine("Ingen players fundet");
                    else
                    {
                        //Command der finder nødvendige informationer
                        command = new SqlCommand("SELECT name, level, race, class, weapon, item FROM player", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();

                        //En lykke, der skrive informationer ned mens den tæller antal rækker
                        for (int i = 0; i < count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Name: {reader.GetString(reader.GetOrdinal("name"))} | Level: {Convert.ToInt32(reader["level"])} | Race: {reader.GetString(reader.GetOrdinal("race"))}" +
                                $" | Class: {reader.GetString(reader.GetOrdinal("class"))} | Weapon: {reader.GetString(reader.GetOrdinal("weapon"))} | Item: {reader.GetString(reader.GetOrdinal("item"))}");
                            reader.Read();
                        }
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Menu();
                }
            }
            else if (key == '3')
            {
                using (SqlConnection connection = new SqlConnection(SServer.cs))
                {
                    connection.Open();

                    Console.WriteLine("Er du sikke på at du vil slette alle rækker med players? (Y/N)");

                    //(Y/y eller N/n)
                    key = Console.ReadKey().KeyChar;
                    if (key == 'Y' || key == 'y')
                    {
                        //Sletter alle rækker fra tabellen
                        SqlCommand command = new SqlCommand("DELETE FROM PLAYER", connection);
                        command.ExecuteNonQuery();

                        Console.WriteLine("");
                        Console.WriteLine("Alle rækker med player er slettet");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Menu();
                    }
                    else if (key == 'N' || key == 'n') Menu();
                    else Menu();
                }
            }
            else Menu();

        }

        static void PlayerName()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst skriv navn for at oprette en player");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                //SPlayer er en ny statisk klasse der gemmer information indtil de bruges til at lave en ny række
                SPlayer.pName = Console.ReadLine();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM player", connection);
                int count = (Int32)command.ExecuteScalar();

                //Name er UNIQUE, så der skal helst ikke være identiske navne på spil (koden vil også fejle hvis man prøver at oprette en ny player med samme navn)
                bool found = false;

                if (count > 0)
                {
                    command = new SqlCommand("SELECT * FROM player", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    //En lykke, der identificer om det navn man har skrevet sammenlignes med nogle af de navne fra tabellen
                    //Der bruges en bool til bekræftelse
                    for(int i = 0; i < count; i++)
                    {
                        if(SPlayer.pName == reader.GetString(reader.GetOrdinal("name")))
                        {
                            found = true;
                            break;
                        }
                        reader.Read();
                    }
                }
                else PlayerLevel();

                if(found)
                {
                    Console.WriteLine($"Der eksisterer allerede et navn i players med {SPlayer.pName}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PlayerName();
                }
                else PlayerLevel();
            }
        }

        static void PlayerLevel()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst skriv player level");

            //Det man skriver ned skal helst være et tal og ikke andre ting
            try
            {
                SPlayer.pLevel = Int32.Parse(Console.ReadLine()); //prøver at convertere en string til en int
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                PlayerLevel();
            }

            PlayerRace();
        }

        static void PlayerRace()
        {

            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst vælg race:");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM race", connection);
                int count = (Int32)command.ExecuteScalar();

                //Ny command som tæller strings (from race)
                command = new SqlCommand("SELECT * FROM race", connection);
                SqlDataReader reader = command.ExecuteReader();

                //List alle races fra tabellen race
                for (int i = 0; i < count; i++)
                {
                    reader.Read();
                    Console.WriteLine(i + 1 + ". " + (string)reader[1]);
                }

                //De er nu listed med numre, hvor du skal skrive et nummer for a vælge
                Console.WriteLine("Venligst indtast nummer for at vælge...");

                try
                {
                    SPlayer.selected = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PlayerRace();
                }

                //Hvis et nummer der stammer fra listen er skrevet ned
                if (SPlayer.selected <= count && SPlayer.selected > 0)
                {
                    reader.Close();
                    reader = command.ExecuteReader();
                    int x = 0;

                    //Den gemmer racen i den statiske player klasse
                    while (x < count)
                    {
                        if (x == SPlayer.selected) SPlayer.pRace = (string)reader[1];
                        reader.Read();
                        x++;
                    }
                    if (SPlayer.selected == count) SPlayer.pRace = (string)reader[1];
                }
                else PlayerRace();
            }
            PlayerClass();
        }

        static void PlayerClass()
        {

            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst vælg class:");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM class", connection);
                int count = (Int32)command.ExecuteScalar();

                command = new SqlCommand("SELECT * FROM class", connection);
                SqlDataReader reader = command.ExecuteReader();

                for (int i = 0; i < count; i++)
                {
                    reader.Read();
                    Console.WriteLine(i + 1 + ". " + (string)reader[1]);
                }

                Console.WriteLine("Venligst indtast nummer for at vælge...");

                try
                {
                    SPlayer.selected = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PlayerClass();
                }

                if (SPlayer.selected <= count && SPlayer.selected > 0)
                {
                    reader.Close();
                    reader = command.ExecuteReader();
                    int x = 0;
                    while (x < count)
                    {
                        if (x == SPlayer.selected) SPlayer.pClass = (string)reader[1];
                        reader.Read();
                        x++;
                    }
                    if (SPlayer.selected == count) SPlayer.pClass = (string)reader[1];
                }
                else PlayerClass();
            }
            PlayerWeapon();
        }

        static void PlayerWeapon()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst vælg våben:");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM weapon WHERE typeclass IN ('{SPlayer.pClass}')", connection);
                int count = (Int32)command.ExecuteScalar();

                //Nu skal den vælge våben (or items) ud fra den klasse man har valgt, siden de kun hører til den rette klasse
                command = new SqlCommand($"SELECT * FROM weapon WHERE typeclass IN ('{SPlayer.pClass}')", connection);
                SqlDataReader reader = command.ExecuteReader();

                for (int i = 0; i < count; i++)
                {
                    reader.Read();
                    Console.WriteLine(i + 1 + ". " + (string)reader[1]);
                }

                Console.WriteLine("Venligst indtast nummer for at vælge...");

                try
                {
                    SPlayer.selected = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PlayerWeapon();
                }

                if (SPlayer.selected <= count && SPlayer.selected > 0)
                {
                    reader.Close();
                    reader = command.ExecuteReader();
                    int x = 0;
                    while (x < count)
                    {
                        if (x == SPlayer.selected) SPlayer.pWeapon = (string)reader[1];
                        reader.Read();
                        x++;
                    }
                    if (SPlayer.selected == count) SPlayer.pWeapon = (string)reader[1];
                }
                else PlayerWeapon();
            }
            PlayerItem();
        }

        static void PlayerItem()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst vælg en item:");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM item WHERE typeclass IN ('{SPlayer.pClass}')", connection);
                int count = (Int32)command.ExecuteScalar();

                command = new SqlCommand($"SELECT * FROM item WHERE typeclass IN ('{SPlayer.pClass}')", connection);
                SqlDataReader reader = command.ExecuteReader();

                for (int i = 0; i < count; i++)
                {
                    reader.Read();
                    Console.WriteLine(i + 1 + ". " + (string)reader[1]);
                }

                Console.WriteLine("Venligst indtast nummer for at vælge...");

                try
                {
                    SPlayer.selected = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    PlayerItem();
                }

                if (SPlayer.selected <= count && SPlayer.selected > 0)
                {
                    reader.Close();
                    reader = command.ExecuteReader();
                    int x = 0;
                    while (x < count)
                    {
                        if (x == SPlayer.selected) SPlayer.pItem = (string)reader[1];
                        reader.Read();
                        x++;
                    }
                    if (SPlayer.selected == count) SPlayer.pItem = (string)reader[1];
                }
                else PlayerItem();
            }
            UpdatePlayerTable();
        }

       static void UpdatePlayerTable()
        {
            Console.Clear();

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine($"Navn: {SPlayer.pName}");
            Console.WriteLine($"Level: {SPlayer.pLevel}");
            Console.WriteLine($"Race: {SPlayer.pRace}");
            Console.WriteLine($"Class: {SPlayer.pClass}");
            Console.WriteLine($"Weapon: {SPlayer.pWeapon}");
            Console.WriteLine($"Item: {SPlayer.pItem}");
            Console.WriteLine("Lav en ny række med disse informationer? (Y/N)");

            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                //Test
                int rows = 0;

                //Alle data samlet i den statiske player klasse laves til en række i player tabellen
                char key = Console.ReadKey().KeyChar;
                if (key == 'Y' || key == 'y')
                {

                    string query = $"INSERT INTO player (name, weapon, class, item, race, level) VALUES ('{SPlayer.pName}', '{SPlayer.pWeapon}', '{SPlayer.pClass}', '{SPlayer.pItem}', '{SPlayer.pRace}', '{SPlayer.pLevel}')";

                    SqlCommand command = new SqlCommand(query, connection);

                    //Skulle gerne kun være 1
                    rows = command.ExecuteNonQuery();

                }
                else if (key == 'N' || key == 'n') Menu();
                else UpdatePlayerTable();

                //Test
                TestTable(rows);
            }
        }

        //En funktion, der viser antal rækker og "affected rows" altså hvilke rækker der er ændret
        static void TestTable(int rows)
        {
            using (SqlConnection connection = new SqlConnection(SServer.cs))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM player", connection);
                int count = (Int32)command.ExecuteScalar();
                Console.WriteLine("");
                Console.WriteLine("All rows: " + count);
                Console.WriteLine("Affected rows: " + rows);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                Menu();
            }
        }
    }
}