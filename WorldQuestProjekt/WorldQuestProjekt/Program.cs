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

            SqlConnection connection = new SqlConnection(cs);

            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine("|     Velkommen til World Quest     |");
            Console.WriteLine("+-----------------------------------+");
            Console.WriteLine($"Database: {SServer.database} | Bruger: {SServer.user}");
            Console.WriteLine();
            Console.WriteLine("Venligst skriv navn for at oprette en player");

        }
    }
}