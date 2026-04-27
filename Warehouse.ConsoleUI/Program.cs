using System;
using System.Collections.Generic;
using System.Linq;
using Warehouse.Domain;

namespace Warehouse.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Supplier> allSuppliers = new List<Supplier>
            {
                new Supplier("Андрій", "Сава", "Alpha Group"),
                new Supplier("Богдан", "Антонов", "Z-Tech"),
                new Supplier("Вікторія", "Бондар", "Smart Logistics")
            };

            List<Category> allCategories = new List<Category>
            {
                new Category("Побутова техніка"),
                new Category("Інструменти"),
                new Category("Електроніка")
            };

            List<Customer> allCustomers = new List<Customer>
            {
                new Customer("Микола", "Сидоренко", "0501112233"),
                new Customer("Анна", "Ткаченко", "0674445566")
            };

            var pcCategory = allCategories.First(c => c.Name == "Електроніка");
            pcCategory.AddProduct(new Product("Монітор", "Dell", 8500m) { Quantity = 5, ProductSupplier = allSuppliers[0] });
            pcCategory.AddProduct(new Product("Клавіатура", "Logitech", 1200m) { Quantity = 15, ProductSupplier = allSuppliers[1] });
            pcCategory.AddProduct(new Product("Мишка", "Apple", 9500m) { Quantity = 10, ProductSupplier = allSuppliers[2] });

            try
            {
                SearchEngine engine = new SearchEngine();

                DisplayAllCategories(allCategories);
                string catToRemove = "Інструменти";
                var categoryToRemove = allCategories.First(c => c.Name == catToRemove);
                allCategories.Remove(categoryToRemove);
                Console.WriteLine($"Категорію '{categoryToRemove.Name}' видалено.");
                DisplayAllCategories(allCategories);

                Console.WriteLine("\n~~~Постачальники~~~");
                string sortOrderSuplier = "Ім'я";  // Ім'я Прізвище
                DisplaySuppliers(allSuppliers, sortOrderSuplier);

                Console.WriteLine("\nСортування товарів(електроніка):");
                string sortOrderProduct = "Бренд";  // Назва Бренд Ціна
                DisplayProducts(pcCategory, sortOrderProduct);
                string prodToRemove = "Мишка";
                var productToRemove = pcCategory.Products.First(d => d.Name == prodToRemove);
                pcCategory.RemoveProduct(productToRemove);
                Console.WriteLine($"Товар '{productToRemove.Name}' видалено.");
                DisplayProducts(pcCategory, sortOrderProduct);

                string productToFind = "Dell";
                Console.WriteLine($"\nПошук товарів({productToFind}):");
                var foundProducts = engine.SearchProducts(pcCategory.Products, productToFind);
                foreach (var p in foundProducts) Console.WriteLine($"- Знайдено: {p.Name} {p.Brand}");

                string customerToFind = "Ткаченко";
                Console.WriteLine($"\nПошук замовників({customerToFind}):");
                var foundCustomers = engine.SearchCustomers(allCustomers, customerToFind);
                foreach (var c in foundCustomers) Console.WriteLine($"- Знайдено: {c.Name} {c.LastName}");

                Console.WriteLine("\nТест виключення (пошук неіснуючого):");
                engine.SearchProducts(pcCategory.Products, "Xiaomi");

            }
            catch (EntityNotFoundException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            Console.ReadLine();
        }

        static void DisplayAllCategories(List<Category> categories)
        {
            Console.WriteLine("\nПерелік категорій: ");
            foreach (var c in categories) Console.WriteLine($"- {c.Name}");
        }

        static void DisplaySuppliers(List<Supplier> suppliers, string sortBy)
        {
            IEnumerable<Supplier> query = sortBy.ToLower() switch
            {
                "ім'я" => suppliers.OrderBy(s => s.Name),
                "прізвище" => suppliers.OrderBy(s => s.LastName),
                _ => suppliers
            };

            Console.WriteLine($"Список постачальників (сортування за {sortBy}):");
            foreach (var s in query) Console.WriteLine($"  > {s.Name,-10} {s.LastName,-15} ({s.CompanyName})");
        }

        static void DisplayProducts(Category category, string sortBy)
        {
            IEnumerable<Product> query = sortBy.ToLower() switch
            {
                "назва" => category.Products.OrderBy(p => p.Name),
                "бренд" => category.Products.OrderBy(p => p.Brand), 
                "ціна" => category.Products.OrderBy(p => p.Price), 
                _ => category.Products
            };

            Console.WriteLine($"Товари в '{category.Name}' (сортування за {sortBy}):");
            foreach (var p in query) Console.WriteLine($"  * {p.Name,-15} {p.Brand,-10} {p.Price,8} грн.");
        }
    }
}