using System;
using System.Data;
using System.Globalization;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            DataContextDapper dapper = new DataContextDapper(config);
            DataContextEF entityFramework = new DataContextEF(config);

            string SqlCommand = "SELECT GETDATE()"; // This is a SQL command that returns the current date and time.

            DateTime rightNow =  dapper.LoadDataSingle<DateTime>(SqlCommand); // This will return the current date and time.
            
            Console.WriteLine($"The current date and time is {rightNow}.");
            
            Computer mycomputer = new Computer()
            {
                Motherboard = "z690",
                HasWifi = true,
                HasLTE = false,
                ReleaseDate = DateTime.Now,
                Price = 943.78m,
                VideoCard = "RTX 4080"
            };
            entityFramework.Add(mycomputer);
            entityFramework.SaveChanges();
            string sql = @"INSERT INTO TutorialAppSchema.Computer (
                            Motherboard, 
                            HasWifi, 
                            HasLTE, 
                            ReleaseDate, 
                            Price, 
                            VideoCard
                        ) VALUES ('" + mycomputer.Motherboard + "', '" + 
                                    (mycomputer.HasWifi ? 1 : 0) + "', '" + 
                                    (mycomputer.HasLTE ? 1 : 0) + "', '" + 
                                    mycomputer.ReleaseDate.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) + "', '" + 
                                    mycomputer.Price.ToString("0.00", CultureInfo.InvariantCulture) + "', '" + 
                                    mycomputer.VideoCard + "')";

            Console.WriteLine(sql);
            
            //int result = dapper.ExecuteSqlWithRowCount(sql);               
            
            //Console.WriteLine($"The number of rows affected is {result}.");
            
            string sqlSelect = @"
                                SELECT
                                    Computer.ComputerId,  
                                    Computer.Motherboard, 
                                    Computer.HasWifi, 
                                    Computer.HasLTE, 
                                    Computer.ReleaseDate, 
                                    Computer.Price, 
                                    Computer.VideoCard
                                FROM TutorialAppSchema.Computer";

            IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
            IEnumerable<Computer>? computersEF = entityFramework.Computers?.ToList<Computer>();

            if(computersEF != null)
            {
                foreach (Computer computer in computersEF)
                {
                    //Also add computer id at the beginning
                    Console.WriteLine($"My computer has a {computer.ComputerId} id, a {computer.Motherboard} motherboard, a {computer.VideoCard} video card, and costs {computer.Price}.");
                }
            }

            // foreach (Computer computer in computers)
            // {
            //        Console.WriteLine($"My computer has a {computer.ComputerId} id, a {computer.Motherboard} motherboard, a {computer.VideoCard} video card, and costs {computer.Price}.");
            // }
            // Console.WriteLine($"My computer has a {mycomputer.Motherboard} motherboard, a {mycomputer.VideoCard} video card, and costs {mycomputer.Price}.");
        }
    }
}