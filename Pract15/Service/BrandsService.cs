using Pract15.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pract15.Service
{
    public class BrandsService
    {
        private readonly Pract15Context _db = DBService.Instance.Context;
        public ObservableCollection<Brand> Brands { get; set; } = new();
        public int Commit() => _db.SaveChanges();

        public BrandsService()
        {
            Getall();
        }

        public void Getall()
        {
            var brands = _db.Brands.ToList();
            Brands.Clear();
            foreach (var brand in brands)
            {
                Brands.Add(brand);
            }
        }

        public void Add(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
                throw new ArgumentException("Название бренда не может быть пустым");

            int newId = _db.Brands.Any() ? _db.Brands.Max(b => b.Id) + 1 : 1;
            while (_db.Brands.Any(b => b.Id == newId))
            {
                newId++;
            }

            var _brand = new Brand
            {
                Id = newId,
                Name = brand.Name.Trim()
            };

            _db.Brands.Add(_brand);
            Commit();
            Brands.Add(_brand);
        }

        public void Update(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
                throw new ArgumentException("Название бренда не может быть пустым");

            var exist = _db.Brands.Find(brand.Id);
            if (exist != null)
            {
                exist.Name = brand.Name.Trim();
                Commit();
                Getall();
            }
        }

        public void Remove(Brand brand)
        {
            _db.Brands.Remove(brand);
            if (Commit() > 0)
            {
                Brands.Remove(brand);
            }
        }
    }
}