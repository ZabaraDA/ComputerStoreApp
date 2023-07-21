using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStoreApp.Databases
{
    public class ComputerStoreDatabase
    {
        private static ComputerStoreDatabaseEntities _databaseEntities;

        public static ComputerStoreDatabaseEntities GetContext() 
        { 
            if (_databaseEntities == null)
            {
                _databaseEntities = new ComputerStoreDatabaseEntities();
            }

            return _databaseEntities; 
        }
    }
}
