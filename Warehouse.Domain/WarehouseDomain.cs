using System;
using System.Collections.Generic;
using System.Linq;

namespace Warehouse.Domain
{
    public interface INamedEntity
    {
        Guid Id { get; }
        string Name { get; set; }
    }

    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) 
            : base(message) { }
    }

    public abstract class Person : INamedEntity
    {
        public Guid Id { get; protected set; }

        public string Name { get; set; }

        public string LastName { get; set; }


        protected Person(string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Name = firstName;
            LastName = lastName;
        }
    }

    public class Supplier : Person
    {
        public string CompanyName { get; set; }

        public Supplier(string firstName, string lastName, string companyName)
            : base(firstName, lastName)
        {
            CompanyName = companyName;
        }
    }

    public class Customer : Person
    {

        public string PhoneNumber { get; set; }

        public Customer(string firstName, string lastName, string phoneNumber)
            : base(firstName, lastName)
        {
            PhoneNumber = phoneNumber;
        }
    }

    public class ProductDetails
    {
        public string? Description { get; set; }

        public int WarrantyMonths { get; set; }
    }

    public class Product : INamedEntity
    {
        public Guid Id { get; private set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Supplier? ProductSupplier { get; set; }

        private ProductDetails _details;

        public Product(string name, string brand, decimal price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Brand = brand;
            Price = price;
            Quantity = 0;
            _details = new ProductDetails { WarrantyMonths = 12, Description = "Базовий опис" };
        }

        public void UpdateDetails(string description, int warranty)
        {
            _details.Description = description;
            _details.WarrantyMonths = warranty;
        }
    }

    public class Category : INamedEntity
    {
        public Guid Id { get; private set; }

        public string Name { get; set; }

        public List<Product> Products { get; private set; }

        public Category(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Products = new List<Product>();
        }

        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            Products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            if (!Products.Contains(product))
            {
                throw new EntityNotFoundException($"Товар {product.Name} не знайдено у категорії {Name}.");
            }
            Products.Remove(product);
        }
    }

    public class SearchEngine
    {
        public IEnumerable<Product> SearchProducts(IEnumerable<Product> catalog, string keyword)
        {
            var result = catalog.Where(p =>
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            if (result.Count == 0)
            {
                throw new EntityNotFoundException($"Товарів за запитом '{keyword}' не знайдено!");
            }

            return result;
        }

        public IEnumerable<Customer> SearchCustomers(IEnumerable<Customer> customers, string keyword)
        {
            var result = customers.Where(c =>
                c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            if (result.Count == 0)
            {
                throw new EntityNotFoundException($"Замовників за запитом '{keyword}' не знайдено!");
            }

            return result;
        }
    }
}