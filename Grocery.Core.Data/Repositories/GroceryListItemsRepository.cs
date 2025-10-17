using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> groceryListItems = new();

        public GroceryListItemsRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL)");

            GetAll();

            SeedDefaultData();
        }

        private void SeedDefaultData()
        {
            if (!groceryListItems.Any())
            {
                List<string> insertQueries = new()
                {
                    "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1,1,3)",
                    "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1,2,1)",
                    "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1,3,4)",
                    "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2,1,2)",
                    "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2,2,5)"
                };
                InsertMultipleWithTransaction(insertQueries);

                GetAll();
            }
        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    groceryListItems.Add(new(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            return groceryListItems.Where(x => x.GroceryListId == groceryListId).ToList();
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string insertQuery = "INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) " +
                                 "VALUES(@GroceryListId, @ProductId, @Amount); " +
                                 "SELECT last_insert_rowid();";

            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();

            groceryListItems.Add(item);
            return item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            string deleteQuery = $"DELETE FROM GroceryListItems WHERE Id = {item.Id}";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();

            groceryListItems.Remove(item);
            return item;
        }

        public GroceryListItem? Get(int id)
        {
            return groceryListItems.FirstOrDefault(x => x.Id == id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string updateQuery = "UPDATE GroceryListItems SET GroceryListId=@GroceryListId, ProductId=@ProductId, Amount=@Amount " +
                                 "WHERE Id=@Id";

            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);
                command.Parameters.AddWithValue("Id", item.Id);

                command.ExecuteNonQuery();
            }
            CloseConnection();

            var index = groceryListItems.FindIndex(x => x.Id == item.Id);
            if (index >= 0) groceryListItems[index] = item;

            return item;
        }
    }
}
