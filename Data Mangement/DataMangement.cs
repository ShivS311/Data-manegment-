using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DataMangement
{
     class UserItem
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Cart { get; set; }
    }

    public class ProductItem
    {
        public string Product { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MainLogin();
        }

        // LOGIN MENU LOOP
        static void MainLogin()
        {
            List<UserItem> users = LoadUsers();

            while (true)
            {
                string selection = GetUserInput("USER LOGIN SYSTEM\n1. CREATE NEW USER\n2. LOGIN\nSelection (1-2): ");

                if (selection == "1")
                {
                    Console.WriteLine("\nREGISTRATION");
                    string username = GetUserInput("ENTER USERNAME: ");
                    string password = GetUserInput("ENTER PASSWORD: ");
                    RegisterUser(users, username, password);
                }
                else if (selection == "2")
                {
                    Console.WriteLine("\nLOGIN");
                    string username = GetUserInput("USERNAME: ");
                    string password = GetUserInput("PASSWORD: ");
                    CheckUser(users, username, password);
                }
                else
                {
                    Console.WriteLine("Invalid Selection");
                }
            }
        }

        // LOAD USERS FROM JSON FILE
        static List<UserItem> LoadUsers()
        {
            string usersJson = File.ReadAllText("users.json");
            List<UserItem> users = JsonConvert.DeserializeObject<List<UserItem>>(usersJson);
            return users;
        }

        // SAVE USERS TO JSON FILE
        static void SaveUsers(List<UserItem> users)
        {
            string usersJson = JsonConvert.SerializeObject(users);
            File.WriteAllText("users.json", usersJson);
        }

        // CREATE NEW USER
        static UserItem NewUser(string username, string password)
        {
            return new UserItem
            {
                Username = username,
                Password = password,
                Cart = new List<string>()
            };
        }

        // REGISTRATION
        static void RegisterUser(List<UserItem> users, string username, string password)
        {
            int search = LinearSearch(users, username, "Username");

            if (search == -1)
            {
                Console.WriteLine("SUCCESSFULLY REGISTERED");
                users.Add(NewUser(username, password));
                SaveUsers(users);
                MainMenu(users, username);
            }
            else
            {
                Console.WriteLine("USERNAME TAKEN");
            }
        }

        // LINEAR SEARCH FUNCTION
        static int LinearSearch<T>(List<T> list, string searchTerm, string propertyName)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i].GetType().GetProperty(propertyName).GetValue(list[i]);
                if (value.ToString() == searchTerm)
                {
                    return i;
                }
            }
            return -1;
        }

        // LOGIN USER CHECKER
        static void CheckUser(List<UserItem> users, string username, string password)
        {
            int userIndex = LinearSearch(users, username, "Username");

            if (userIndex == -1)
            {
                Console.WriteLine("USERNAME NOT FOUND");
            }
            else
            {
                UserItem user = users[userIndex];
                if (user.Password == password)
                {
                    Console.WriteLine($"\nWELCOME {username}");
                    MainMenu(users, username);
                }
                else
                {
                    Console.WriteLine("PASSWORD INCORRECT");
                }
            }
        }

        // MAIN MENU FOR SIMULATOR
        static void MainMenu(List<UserItem> users, string username)
        {
            List<ProductItem> products = LoadProducts();

            while (true)
            {
                string selection = GetUserInput(
                    "\nMENU\n1. DISPLAY ALL ITEMS\n2: FILTER PRODUCTS\n3: SORT ITEMS\n" +
                    "4. ADD TO CART\n5. REMOVE FROM CART\n6. SHOW CART\n7. EXIT\nSelection (1-7): ");

                if (selection == "1")
                {
                    DisplayAllProducts(products);
                }
                else if (selection == "2")
                {
                    string category = GetUserInput("Enter a category to search for: ").ToLower();
                    FilterCategories(products, category);
                }
                else if (selection == "3")
                {
                    BubbleSort(products);
                }
                else if (selection == "4")
                {
                    AddToCart(users, username, products);
                }
                else if (selection == "5")
                {
                    RemoveFromCart(users, username);
                }
                else if (selection == "6")
                {
                    ShowCart(users, username);
                }
                else if (selection == "7")
                {
                    Console.WriteLine("EXIT");
                    break;
                }
                else
                {
                    Console.WriteLine("PLS CHOOSE BETWEEN 1-7");
                }
            }
        }

        // LOAD PRODUCTS FROM JSON FILE
        static List<ProductItem> LoadProducts()
        {
            string productsJson = File.ReadAllText("products.json");
            List<ProductItem> products = JsonConvert.DeserializeObject<List<ProductItem>>(productsJson);
            return products;
        }

        // DISPLAY ALL PRODUCTS
        static void DisplayAllProducts(List<ProductItem> products)
        {
            Console.WriteLine("\nPRODUCTS:");
            foreach (ProductItem product in products)
            {
                Console.WriteLine($"Product: {product.Product}, Description: {product.Description}, Price: {product.Price}, Category: {product.Category}");
            }
        }

        // FILTER PRODUCTS BY CATEGORIES
        static void FilterCategories(List<ProductItem> products, string category)
        {
            Console.WriteLine($"\nFILTERED PRODUCTS (Category: {category}):");
            foreach (ProductItem product in products)
            {
                if (product.Category.ToLower() == category)
                {
                    Console.WriteLine($"Product: {product.Product}, Description: {product.Description}, Price: {product.Price}, Category: {product.Category}");
                }
            }
        }

        // BUBBLE SORT FOR PRICE
        static void BubbleSort(List<ProductItem> products)
        {
            for (int i = 0; i < products.Count - 1; i++)
            {
                for (int j = 0; j < products.Count - i - 1; j++)
                {
                    if (products[j].Price > products[j + 1].Price)
                    {
                        ProductItem temp = products[j];
                        products[j] = products[j + 1];
                        products[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\nPRODUCTS SORTED BY PRICE:");
            DisplayAllProducts(products);
        }

        // ADD TO CART
        static void AddToCart(List<UserItem> users, string username, List<ProductItem> products)
        {
            string addToCart = GetUserInput("ADD SOMETHING TO CART: ").ToLower();

            int productIndex = LinearSearch(products, addToCart, "Product");

            if (productIndex == -1)
            {
                Console.WriteLine("PRODUCT NOT FOUND");
            }
            else
            {
                UserItem user = users.Find(u => u.Username == username);

                if (!user.Cart.Contains(addToCart))
                {
                    user.Cart.Add(addToCart);
                    SaveUsers(users);
                    Console.WriteLine("PRODUCT ADDED TO CART");
                }
                else
                {
                    Console.WriteLine("PRODUCT ALREADY IN CART");
                }
            }
        }

        // REMOVE FROM CART
        static void RemoveFromCart(List<UserItem> users, string username)
        {
            UserItem user = users.Find(u => u.Username == username);

            if (user.Cart.Count > 0)
            {
                Console.WriteLine("\nYOUR CART:");
                for (int i = 0; i < user.Cart.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {user.Cart[i]}");
                }

                int itemIndex = GetNumericUserInput("\nEnter the item number to remove from the cart: ");

                if (itemIndex >= 1 && itemIndex <= user.Cart.Count)
                {
                    user.Cart.RemoveAt(itemIndex - 1);
                    SaveUsers(users);
                    Console.WriteLine("PRODUCT REMOVED FROM CART");
                }
                else
                {
                    Console.WriteLine("Invalid item number");
                }
            }
            else
            {
                Console.WriteLine("YOUR CART IS EMPTY");
            }
        }

        // SHOW CART
        static void ShowCart(List<UserItem> users, string username)
        {
            UserItem user = users.Find(u => u.Username == username);

            if (user.Cart.Count > 0)
            {
                Console.WriteLine("\nYOUR CART:");
                for (int i = 0; i < user.Cart.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {user.Cart[i]}");
                }
            }
            else
            {
                Console.WriteLine("YOUR CART IS EMPTY");
            }
        }

        // GET USER INPUT
        static string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        // GET NUMERIC USER INPUT
        static int GetNumericUserInput(string prompt)
        {
            Console.Write(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine("Invalid input. Please enter a numeric value.");
                Console.Write(prompt);
            }
            return value;
        }
    }
}
