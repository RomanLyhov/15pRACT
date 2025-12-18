using Pract15.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pract15.Service
{
    class CategoryService
    {
        private readonly Pract15Context _db = DBService.Instance.Context;
        public ObservableCollection<Category> Categories { get; set; } = new();
        public int Commit() => _db.SaveChanges();

        public CategoryService()
        {
            Getall();
        }

        public void Getall()
        {
            var categories = _db.Categories.ToList();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        public Category Add(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Название категории не может быть пустым");

            int newId = _db.Categories.Any() ? _db.Categories.Max(c => c.Id) + 1 : 1;
            while (_db.Categories.Any(c => c.Id == newId))
            {
                newId++;
            }

            var newCategory = new Category
            {
                Id = newId,
                Name = category.Name.Trim()
            };

            _db.Categories.Add(newCategory);
            Commit();
            Categories.Add(newCategory);

            return newCategory; 
        }

        public void Update(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Название категории не может быть пустым");

            var exist = _db.Categories.Find(category.Id);
            if (exist != null)
            {
                exist.Name = category.Name.Trim();
                Commit();
                Getall();
            }
        }

        public void Remove(Category category)
        {
            _db.Categories.Remove(category);
            if (Commit() > 0)
            {
                Categories.Remove(category);
            }
        }
    }
}