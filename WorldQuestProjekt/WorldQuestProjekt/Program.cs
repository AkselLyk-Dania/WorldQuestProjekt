using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldQuestProjekt
{
    internal class Program
    {
        static void Main(string[] args)
        {

            SServer.database = "worldquest";
            SServer.user = "sql_user";
            SServer.password = "1234";

            string cs = $"Server=.\\SQLEXPRESS;Database={SServer.database};User Id={SServer.user};Password={SServer.password};Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(cs)) //SqlConnection connection = new SqlConnection(cs);
            {
                connection.Open();

                Console.WriteLine("+-----------------------------------+");
                Console.WriteLine("|     Velkommen til World Quest     |");
                Console.WriteLine("+-----------------------------------+");
                Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
                Console.WriteLine();
                Console.WriteLine("Venligst skriv navn for at oprette en player");

                SPlayer.pName = Console.ReadLine();

                //SQL command som tæller all classes fra tabellen...
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM class", connection);

                //...til int32
                int count = (Int32)command.ExecuteScalar();

                //Ny command som tæller strings
                command = new SqlCommand("SELECT * FROM class", connection);
                SqlDataReader reader = command.ExecuteReader();

                //List alle classes
                Console.WriteLine("Venligst vælg class:");
                for(int i = 0; i< count; i++)
                {
                    reader.Read();
                    Console.WriteLine(i+1 + ". " + (string)reader[1]);
                }
                
            }
        }
    }
}