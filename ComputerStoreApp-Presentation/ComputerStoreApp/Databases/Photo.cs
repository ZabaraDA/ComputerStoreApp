//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComputerStoreApp.Databases
{
    using System;
    using System.Collections.Generic;
    
    public partial class Photo
    {
        public int ID { get; set; }
        public byte[] Photo1 { get; set; }
        public int AccessoryID { get; set; }
    
        public virtual Accessory Accessory { get; set; }
    }
}
