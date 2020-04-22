// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Test.DataLayer
{
    public class MyDbContext : DbContext
    {
        public string InstanceKey { get; }

        public DbSet<LogData> Logs { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            InstanceKey = Guid.NewGuid().ToString();
        }
    }
}