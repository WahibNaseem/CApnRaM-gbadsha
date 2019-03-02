using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace JKApi.Data.DAL
{

    public partial class jkDatabaseEntities : DbContext
    {
        //public jkDatabaseEntities() : base("name=jkDatabaseEntities")
        //{
        //    this.Configuration.ProxyCreationEnabled = false;
        //} 
        private static readonly Lazy<jkDatabaseEntities> lazy = new Lazy<jkDatabaseEntities>(() => new jkDatabaseEntities());
        public static jkDatabaseEntities Instance { get { return lazy.Value; } }
    }
}

//namespace JKApi.Data.JkControl
//{
//    using System;
//    using System.Data.Entity;
//    using System.Data.Entity.Infrastructure;

//    public partial class jkControlEntities : DbContext
//    {
//        //public jkControlEntities() : base("name=jkControlEntities", "jkControlEntities")
//        //{
//        //    this.ContextOptions.ProxyCreationEnabled = false;

//        //}

//        private static readonly Lazy<jkControlEntities> lazy = new Lazy<jkControlEntities>(() => new jkControlEntities());
//        public static jkControlEntities Instance { get { return lazy.Value; } }
//    }

//}
