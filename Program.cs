using System;
using System.Data;
using System.Globalization;
using System.Text.Json;
using AutoMapper;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            // DataContextEF entityFramework = new DataContextEF(config);

            // string SqlCommand = "SELECT GETDATE()"; // This is a SQL command that returns the current date and time.

            // DateTime rightNow =  dapper.LoadDataSingle<DateTime>(SqlCommand); // This will return the current date and time.
            
            // Console.WriteLine($"The current date and time is {rightNow}.");
            
            Computer mycomputer = new Computer()
            {
                Motherboard = "z690",
                HasWifi = true,
                HasLTE = false,
                ReleaseDate = DateTime.Now,
                Price = 943.78m,
                VideoCard = "RTX 4080"
            };
            // entityFramework.Add(mycomputer);
            // entityFramework.SaveChanges();
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
                                    mycomputer.ReleaseDate?.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) + "', '" + 
                                    mycomputer.Price.ToString("0.00", CultureInfo.InvariantCulture) + "', '" + 
                                    mycomputer.VideoCard + "')";

            
            //int result = dapper.ExecuteSqlWithRowCount(sql);               
            
            //Console.WriteLine($"The number of rows affected is {result}.");
            
            // string sqlSelect = @"
            //                     SELECT
            //                         Computer.ComputerId,  
            //                         Computer.Motherboard, 
            //                         Computer.HasWifi, 
            //                         Computer.HasLTE, 
            //                         Computer.ReleaseDate, 
            //                         Computer.Price, 
            //                         Computer.VideoCard
            //                     FROM TutorialAppSchema.Computer";

            // IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
            // IEnumerable<Computer>? computersEF = entityFramework.Computers?.ToList<Computer>();

            // JsonSerializerOptions options = new JsonSerializerOptions() // This will set the naming policy to camel case. Because the default naming policy is Pascal case. And we want to use camel case since our properties are in pascal case.
            // {
            //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase // burada camelcase dememizin sebebi json içindeki property'lerin camelcase olduğunu belitmek istememizdir.
            // };

            string computersJson = File.ReadAllText("ComputersSnake.json");

            Mapper mapper = new Mapper(new MapperConfiguration((cfg) => {
                cfg.CreateMap<ComputerSnake, Computer>()
                    .ForMember(destination => destination.ComputerId, options => 
                        options.MapFrom(source => source.computer_id))
                    .ForMember(destination => destination.CPUcores, options => 
                        options.MapFrom(source => source.cpu_cores))
                    .ForMember(destination => destination.HasLTE, options => 
                        options.MapFrom(source => source.has_lte))
                    .ForMember(destination => destination.HasWifi, options => 
                        options.MapFrom(source => source.has_wifi))
                    .ForMember(destination => destination.Motherboard, options => 
                        options.MapFrom(source => source.motherboard))
                    .ForMember(destination => destination.VideoCard, options => 
                        options.MapFrom(source => source.video_card))
                    .ForMember(destination => destination.ReleaseDate, options => 
                        options.MapFrom(source => source.release_date))
                    .ForMember(destination => destination.Price, options => 
                        options.MapFrom(source => source.price));
            }));

            IEnumerable<ComputerSnake>? computersSystem = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(computersJson);

            if (computersSystem != null)
            {
                IEnumerable<Computer> computerResult = mapper.Map<IEnumerable<Computer>>(computersSystem);
                Console.WriteLine("Automapper Count: " +  computerResult.Count());
                // foreach (Computer computer in computerResult)
                // {
                //     Console.WriteLine(computer.Motherboard);
                // }
            }

            IEnumerable<Computer>? computersJsonPropertyMapping = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson);
            if (computersJsonPropertyMapping != null)
            {
                Console.WriteLine("JSON Property Count: " + computersJsonPropertyMapping.Count());
                // foreach (Computer computer in computersJsonPropertyMapping)
                // {
                //     Console.WriteLine(computer.Motherboard);
                // }
            }

            // Console.WriteLine(computersJson);
            // IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson, options);           
        
            // IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);           

            // if(computers != null)
            // {
            //     foreach(Computer computer in computers)
            //     {
            //         // Console.Write($"Computer ID: {computer.ComputerId} || ");
            //         // Console.WriteLine($"Motherboard: {computer.Motherboard}");

            //         sql = @"INSERT INTO TutorialAppSchema.Computer (
            //                 Motherboard, 
            //                 HasWifi, 
            //                 HasLTE, 
            //                 ReleaseDate, 
            //                 Price, 
            //                 VideoCard
            //             ) VALUES ('" + EscapeSingleQuotes(computer.Motherboard) + "', '" + 
            //                         (computer.HasWifi ? 1 : 0) + "', '" + 
            //                         (computer.HasLTE ? 1 : 0) + "', '" + 
            //                         computer.ReleaseDate?.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) + "', '" + 
            //                         computer.Price.ToString("0.00", CultureInfo.InvariantCulture) + "', '" + 
            //                         EscapeSingleQuotes(computer.VideoCard) + "')";
            //         dapper.ExecuteSqlWithRowCount(sql);
            //     }
            // }

            // JsonSerializerSettings settings = new JsonSerializerSettings()
            // {
            //     ContractResolver = new CamelCasePropertyNamesContractResolver()
            // };

            // string computersCopyNewtonsoft = JsonConvert.SerializeObject(computers, settings);
            // File.WriteAllText("computersCopyNewtonsoft.txt", computersCopyNewtonsoft);
        
            // string computersCopySystem = System.Text.Json.JsonSerializer.Serialize(computers, options); // bunun için naming policy camelcase aayarı yapılmış olmalı
            // File.WriteAllText("computersCopySystem.txt", computersCopySystem);


        }

        static string EscapeSingleQuotes(string input)
        {
            return input.Replace("'", "''");
        }    
    }
}