using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stock.Controller.DBController
{
    public enum DB_ERROR : byte
    {
        DB_OK,
        DB_CANT_CONNECT,
        DB_TABLE_CRACK,
        DB_TABLE_CRACK_FIX,
        DB_DATA_NOT_EXISTS,
        DB_USER_TABLE_EXISTS,
        DB_USER_EXISTS,
        DB_DATA_CANT_USE,
    }
}
