﻿namespace BlogSystem.Data.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using BlogSystem.Data.Models;
    using BlogSystem.Data.Repositories;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class BlogSystemData : IBlogSystemData
    {
        private readonly DbContext context;
        private readonly IDictionary<Type, object> repositories;

        private IUserStore<ApplicationUser> userStore;

        public BlogSystemData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IUserStore<ApplicationUser> UserStore => this.userStore ?? (this.userStore = new UserStore<ApplicationUser>(this.context));

        public IDbRepository<ApplicationUser> Users => this.GetRepository<ApplicationUser>();

        public IDbRepository<BlogPost> Posts => this.GetRepository<BlogPost>();

        public IDbRepository<PostComment> PostComments => this.GetRepository<PostComment>();

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IDbRepository<T> GetRepository<T>() 
            where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                this.repositories.Add(typeof(T), Activator.CreateInstance(typeof(DbRepository<T>), this.context));
            }

            return (IDbRepository<T>) this.repositories[typeof(T)];
        }
    }
}