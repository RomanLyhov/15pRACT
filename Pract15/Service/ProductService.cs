using Pract15.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pract15.Service
{
    public class ProductService
    {
        private readonly Pract15Context _db = DBService.Instance.Context;
        public ObservableCollection<Product> Products { get; set; } = new();
        public int Commit() => _db.SaveChanges();

        public ProductService()
        {
            Getall();
        }

        public void Getall()
        {
            var products = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Tags)
                .ToList();

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        public void Add(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Название товара не может быть пустым");

            if (!product.CategoryId.HasValue || product.CategoryId <= 0)
                throw new ArgumentException("Не выбрана категория");

            if (!product.BrandId.HasValue || product.BrandId <= 0)
                throw new ArgumentException("Не выбран бренд");
            int newId = _db.Products.Any() ? _db.Products.Max(p => p.Id) + 1 : 1;
            while (_db.Products.Any(p => p.Id == newId))
            {
                newId++;
            }

            var newProduct = new Product
            {
                Id = newId, 
                Name = product.Name.Trim(),
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Rating = product.Rating,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            foreach (var tag in product.Tags)
            {
                var existingTag = _db.Tags.Find(tag.Id);
                if (existingTag != null)
                {
                    newProduct.Tags.Add(existingTag);
                }
            }

            _db.Products.Add(newProduct);
            Commit();
            Products.Add(newProduct);
        }

        public void Update(Product product)
        {
            var oldProduct = _db.Products
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.Id == product.Id);

            if (oldProduct == null) return;

            oldProduct.Name = product.Name;
            oldProduct.Description = product.Description;
            oldProduct.Price = product.Price;
            oldProduct.Stock = product.Stock;
            oldProduct.Rating = product.Rating;
            oldProduct.CategoryId = product.CategoryId;
            oldProduct.BrandId = product.BrandId;

            oldProduct.Tags.Clear();

            foreach (var tag in product.Tags)
            {
                var existingTag = _db.Tags.Find(tag.Id);
                if (existingTag != null)
                {
                    oldProduct.Tags.Add(existingTag);
                }
            }

            _db.SaveChanges();

            Getall();
        }

        public void Remove(Product product)
        {
            _db.Products.Remove(product);
            if (Commit() > 0)
            {
                Products.Remove(product);
            }
        }
    }
}