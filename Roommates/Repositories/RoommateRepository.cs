using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roommates.Repositories
{
    public class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT m.Id, m.FirstName, m.LastName, m.RentPortion, m.MoveInDate, m.RoomId, r.Id, r.[Name], r.MaxOccupancy
                                        FROM Roommate m
                                        JOIN Room r ON r.Id = m.RoomId ";
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int idValue = reader.GetInt32(reader.GetOrdinal("m.Id"));
                        string firstNameValue = reader.GetString(reader.GetOrdinal("m.FirstName"));
                        string lastNameValue = reader.GetString(reader.GetOrdinal("m.LastName"));
                        int rentPortionValue = reader.GetInt32(reader.GetOrdinal("m.RentPortion"));
                        DateTime moveInDateValue = reader.GetDateTime(reader.GetOrdinal("m.MoveInDate"));
                        int roomIdValue = reader.GetInt32(reader.GetOrdinal("m.RoomId"));

                        // Get values to create room object for Roommate
                        int nameColumnPosition = reader.GetOrdinal("r.Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        int maxOccupancyColumnPosition = reader.GetOrdinal("r.MaxOccupancy");
                        int maxOccupancy = reader.GetInt32(maxOccupancyColumnPosition);

                        Room room = new Room
                        {
                            Id = roomIdValue,
                            Name = nameValue,
                            MaxOccupancy = maxOccupancy
                        };

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = firstNameValue,
                            Lastname = lastNameValue,
                            RentPortion = rentPortionValue,
                            MovedInDate = moveInDateValue,
                            Room = room
                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }
            }
        }

        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT m.FirstName, m.LastName, m.RentPortion, m.MoveInDate, r.Id, r.[Name], r.MaxOccupancy 
                                        FROM Roommate m 
                                        JOIN Room r ON r.Id = m.RoomId WHERE m.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    int roomIdValue = reader.GetInt32(reader.GetOrdinal("m.RoomId"));

                    // Get values to create room object for Roommate
                    int nameColumnPosition = reader.GetOrdinal("r.[Name]");
                    string nameValue = reader.GetString(nameColumnPosition);

                    int maxOccupancyColumnPosition = reader.GetOrdinal("r.MaxOccupancy");
                    int maxOccupancy = reader.GetInt32(maxOccupancyColumnPosition);

                    Roommate roommate = null;
                    Room room = null;

                    if (reader.Read())
                    {
                        room = new Room
                        {
                            Id = roomIdValue,
                            Name = nameValue,
                            MaxOccupancy = maxOccupancy
                        };

                        roommate = new Roommate
                        {
                            Id = id,
                            Firstname = reader.GetString(reader.GetOrdinal("FirstName")),
                            Lastname = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate"))
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }
        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@firstName, @lastName, @rentPortion, @moveInDate, @roomId)";
                    cmd.Parameters.AddWithValue("@firstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@lastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@moveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@roomId", roommate.Room.Id);
                    int id = (int)cmd.ExecuteScalar();

                    roommate.Id = id;
                }
            }
        }

        public void Update(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Roommate
                                    SET FirstName = @firstName,
                                        LastName = @lastName,
                                        RentPortion = @rentPortion,
                                        MoveInDate = @moveInDate,
                                        RoomId = @roomId
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@firstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@lastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@moveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@roomId", roommate.Room.Id);
                    cmd.Parameters.AddWithValue("@id", roommate.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
