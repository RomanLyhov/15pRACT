using Pract15.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pract15.Service
{
    public class TagService
    {
        private readonly Pract15Context _db = DBService.Instance.Context;
        public ObservableCollection<Tag> Tags { get; set; } = new();
        public int Commit() => _db.SaveChanges();

        public TagService()
        {
            Getall();
        }

        public void Getall()
        {
            var tags = _db.Tags.ToList();

            Tags.Clear();
            foreach (var tag in tags)
            {
                Tags.Add(tag);
            }
        }

        public void Add(Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Name))
                throw new ArgumentException("Название тега не может быть пустым");
            int newId = _db.Tags.Any() ? _db.Tags.Max(t => t.Id) + 1 : 1;

            while (_db.Tags.Any(t => t.Id == newId))
            {
                newId++;
            }

            var newTag = new Tag
            {
                Id = newId,
                Name = tag.Name.Trim()
            };

            _db.Tags.Add(newTag);
            Commit();
            Tags.Add(newTag);
        }

        public void Update(Tag tag)
        {
            var exist = _db.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (exist != null)
            {
                exist.Name = tag.Name;
                Commit();
            }
        }

        public void Remove(Tag tag)
        {
            var tagWithProducts = _db.Tags
                .Include(t => t.Products)
                .FirstOrDefault(t => t.Id == tag.Id);

            if (tagWithProducts != null)
            {
                foreach (var product in tagWithProducts.Products.ToList())
                {
                    product.Tags.Remove(tagWithProducts);
                }

                _db.Tags.Remove(tagWithProducts);

                if (Commit() > 0)
                {
                    Tags.Remove(tag);
                }
            }
        }
    }
}