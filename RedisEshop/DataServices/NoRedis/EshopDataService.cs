﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RedisEshop.Entities;
using RedisEshop.Mapping;
using RedisEshop.ViewModels;

namespace RedisEshop.DataServices.NoRedis
{
	/// <summary>
	/// Služba napojená přímo na SQL databázi
	/// </summary>
	public class EshopDataService : IEshopDataService
	{
		private readonly AppDbContext _db;

		public EshopDataService(AppDbContext db)
		{
			this._db = db;
		}

		public List<ProductViewModel> GetProductsByTags(int[] tagIds)
		{
			IQueryable<Product> dataQuery = _db.Products
				.Include(x => x.ProductTags).ThenInclude(x => x.Tag);

			if (tagIds.Any())
			{
				Dictionary<int, TagType> tags = _db.Tags
					.Where(x => tagIds.Contains(x.TagId))
					.Select(x => new {x.TagId, x.Type})
					.ToDictionary(x => x.TagId, x => x.Type);

				Dictionary<TagType, List<int>> categories = new Dictionary<TagType, List<int>>();
				
				foreach (var tagId in tagIds)
				{
					if (tags.ContainsKey(tagId))
					{
						if (!categories.ContainsKey(tags[tagId]))
						{
							categories.Add(tags[tagId], new List<int>() {tagId});
						}
						else
						{
							categories[tags[tagId]].Add(tagId);
						}
					}
				}

				foreach (var category in categories)
				{
					dataQuery = dataQuery.Where(x => x.ProductTags.Any(t => category.Value.Contains(t.TagId)));
				}
			}

			return dataQuery.ToViewModel();
		}


		public int AddAndGetProductVisits(int productId)
		{
			var product = _db.Products.FirstOrDefault(x => x.ProductId == productId);

			if (product != null)
			{
				product.Views = product.Views + 1;
				_db.SaveChanges();

				return product.Views;
			}

			return 0;
		}

		public ProductViewModel GetProduct(string identifier)
		{
			throw new NotImplementedException();
		}

		public (string, string) NewsletterSubscribe(string email)
		{
			throw new NotImplementedException();
		}

		public List<ProductViewModel> GetMostViewedProducts(int count)
		{
			throw new NotImplementedException();
		}

		public List<ProductViewModel> GetLatestProducts(int count)
		{
			throw new NotImplementedException();
		}

		public List<ProductViewModel> GetRandomProducts(int count)
		{
			throw new NotImplementedException();
		}

		public List<ProductViewModel> Bestsellers(int count)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> SendNewsletters()
		{
			throw new NotImplementedException();
		}
	}
}
