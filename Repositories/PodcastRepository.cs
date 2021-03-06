﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataContexts;
using mjjames.DataEntities;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DataEntities;
using System.Configuration;
using System.Globalization;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	public class PodcastRepository : IRepository<Podcast>
	{
		private readonly CMSDataContext _dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);

		/// <summary>
		/// Get all the Podcasts within the system
		/// </summary>
		/// <returns></returns>
		public IQueryable<Podcast> FindAll()
		{

            return GetPodcastsByType(PodcastType.Podcast);
		}

		/// <summary>
		/// Get all the active Podcasts within the system
		/// </summary>
		/// <returns></returns>
		public IQueryable<Podcast> FindAllActive()
		{

            return GetPodcastsByType(PodcastType.Podcast).Where(p => p.Active);
		}

		/// <summary>
		/// Get the podcast with the provided key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Podcast Get(int key)
		{
			return (from p in _dc.Medias
					join kv in _dc.KeyValues
						on p.media_key equals kv.link_fkey
					where p.active && p.media_key == key
						&& kv.lookup.lookup_id == "podcast_publishdate" && kv.Lookup1.lookup_id == "medialookup"
					select new Podcast
					{
						Active = p.active,
						Description = p.description,
						Filename = p.filename,
						Published = DateTime.Parse(kv.value),
						Title = p.title
					}).FirstOrDefault();
		}

		/// <summary>
		/// Get the podcast with the provided filename
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Podcast Get(string id)
		{
			return (from p in _dc.Medias
					join kv in _dc.KeyValues
						on p.media_key equals kv.link_fkey
					where p.active && p.filename == id
						&& kv.lookup.lookup_id == "podcast_publishdate" && kv.Lookup1.lookup_id == "medialookup"
					select new Podcast
					{
						Active = p.active,
						Description = p.description,
						Filename = p.filename,
						Published = DateTime.Parse(kv.value),
						Title = p.title
					}).FirstOrDefault();
		}

        /// <summary>
		/// Get all podcasts for a specific month and the provided type
		/// </summary>
		/// <param name="date">Month to get podcasts for</param>
        /// <param name="type">Type of podcast to get</param>
		/// <returns></returns>
        public IQueryable<Podcast> GetPodcastsByDate(PodcastType type, DateTime date)
        {
            //get the first day of the month
            var startDate = new DateTime(date.Year, date.Month, 1);
            //caclulate the end date
            var endDate = startDate.AddMonths(1);
            //now filter all the active podcasts to those within our search criteria
            return GetPodcastsByType(type).Where(p => p.Active  && p.Published >= startDate && p.Published < endDate);
        }

		/// <summary>
		/// Get all podcasts for a specific month
		/// </summary>
		/// <param name="date">Month to get podcasts for</param>
		/// <returns></returns>
		public IQueryable<Podcast> GetPodcastsByDate(DateTime date)
		{
           return GetPodcastsByDate(PodcastType.Podcast, date);
		}

        /// <summary>
        /// Returns all podcasts of the provided type
        /// </summary>
        /// <param name="type">Type of podcasts to return</param>
        /// <returns></returns>
        public IQueryable<Podcast> GetPodcastsByType(PodcastType type)
        {
            return from p in _dc.Medias
                   join kv in _dc.KeyValues
                        on p.media_key equals kv.link_fkey
                   where p.Lookup.lookup_id == type.ToString().ToLower()
                        && kv.lookup.lookup_id == "podcast_publishdate" && kv.Lookup1.lookup_id == "medialookup"
                        && p.active
                   orderby Convert.ToDateTime(kv.value)
                   select new Podcast
                   {
                       Active = p.active,
                       Description = p.description,
                       Filename = p.filename,
                       Published = Convert.ToDateTime(kv.value),
                       Title = p.title
                   };
        }
	}
}
