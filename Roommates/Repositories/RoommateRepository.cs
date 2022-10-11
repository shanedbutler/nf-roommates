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
                    cmd.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate, RoomId FROM Roommate m JOIN Room r ON r.Id = m.RoomId ";
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int idValue = reader.GetInt32(reader.GetOrdinal("Id"));
                        string firstNameValue = reader.GetString(reader.GetOrdinal("FirstName"));
                        string lastNameValue = reader.GetString(reader.GetOrdinal("LastName"));
                        int rentPortionValue = reader.GetInt32(reader.GetOrdinal("RentPortion"));
                        DateTime moveInDateValue = reader.GetDateTime(reader.GetOrdinal("MoveInDate"));
                        // How do I get the Room object?
                        int roomIdValue = reader.GetInt32(reader.GetOrdinal("RoomId"));

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = firstNameValue,
                            Lastname = lastNameValue,
                            RentPortion = rentPortionValue,
                            MovedInDate = moveInDateValue,
                            RoomId = roomIdValue
                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }
            }
        }
    }
}
