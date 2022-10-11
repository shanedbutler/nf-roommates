using Roommates.Repositories;
using Roommates.Models;
using System;
using System.Collections.Generic;

namespace Roommates
{
    internal class Program
    {
        /// <summary>
        ///  This is the address of the database.
        ///  We define it here as a constant since it will never change.
        /// </summary>
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;TrustServerCertificate=True";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);

            Room singleRoom = roomRepo.GetById(1);
            Console.WriteLine($"Single room, {singleRoom.Name} has an Id of {singleRoom.Id} and a max occupancy of {singleRoom.MaxOccupancy}");

            Room bathroom = new Room
            {
                Name = "Bathroom",
                MaxOccupancy = 1
            };

            roomRepo.Insert(bathroom);

            bathroom.MaxOccupancy = 3;
            bathroom.Name = "Primary Bathroom";
            roomRepo.Update(bathroom);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Updated the Room with id {bathroom.Id}");

            roomRepo.Delete(bathroom.Id);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Deleted the Room with id {bathroom.Id}");


            List<Room> allRooms = roomRepo.GetAll();
            Console.WriteLine("\nGetting All Rooms:");
            allRooms.ForEach(room => Console.WriteLine($"{room.Name} has an Id of {room.Id} and a max occupancy of {room.MaxOccupancy}"));

        }
    }
}
