using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BriefingRoom4DCS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(@"
        @@@@@@@@@@  .@@@@@@@@@@  @@@&  @@@@@@@@@@  @@@@@@@@@/ @@@   @@@@,   @@@   *@@@@@@@@@#       
        @@@    @@@  .@@@    @@@  @@@&  @@@         @@@        @@@   @@@@@@  @@@  @@@@               
        @@@@@@@@@@  .@@@@@@@@@.  @@@&  @@@@@@@@@   @@@@@@@@@  @@@   @@@ @@@@@@@  @@@   (@@@@@       
        @@@    %@@@ .@@@  @@@@   @@@&  @@@         @@@        @@@   @@@   @@@@@  ,@@@@   /@@@       
        @@@@@@@@@   .@@@    @@@# @@@&  @@@@@@@@@@  @@@        @@@   @@@     @@@     @@@@@@@.        
                                                                                                    
        @@@@@@@@@@@@@@(          *@@@@@@@@@@@      %@@@@@@@@@@%         @@@@@@         &@@@@@       
        @@@@@@@@@@@@@@@@@      @@@@@@&# ,&@@@@@@@@@@@@&&  &&@@@@@%      @@@@@@@      /@@@@@@@       
        @@@@@       @@@@@@   #@@@@   (@@@(   /@@@@@   (&@@&(  ,@@@@     @@@@@@@@#   @@@@@@@@@       
        @@@@@       .@@@@@   @@@@  @@@@@@@@@@@@@@  /@@@@@@@@@& .@@@@    @@@@@@@@@@@@@@@@@@@@@       
        @@@@@@@@@@@@@@@@@   #@@@.  @@@@@@@@@@@%  (@@@@@@@@@@@@  %@@@    @@@@@ &@@@@@@@  @@@@@       
        @@@@@@@@@@@@@@,      @@@@  #@@@@@@@@(  ,@@@.@@@@@@@@@  .@@@#    @@@@@   @@@@@   @@@@@       
        @@@@@    @@@@@@      .@@@@.   .&     @@@@@@(    &&    @@@@@     @@@@@    ,@     @@@@@       
        @@@@@      @@@@@&      #@@@@@@@@@@@@@@@@/@@@@@@@@@@@@@@@@       @@@@@           @@@@@       
        @@@@@       .@@@@@/        @@@@@@@@@/       @@@@@@@@@@          @@@@@           @@@@@     
         ___  _   _  ___   ___                          
        / __|| | | ||_ _| / __| ___  _ _ __ __ ___  _ _ 
       | (_ || |_| | | |  \__ \/ -_)| '_|\ V // -_)| '_|
        \___| \___/ |___| |___/\___||_|   \_/ \___||_|  
                                                  

                                                                                                 
                                                                                                 
    OPEN Browser on https://localhost:5001                                                                                                                                                                                                 
                                                                                                              
Server Logs:");
            Process.Start("explorer.exe", "https://localhost:5001");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
