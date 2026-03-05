using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.Roles.Infrastructure.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
