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
    
    public partial class LinePropertyAccessory
    {
        public int AccessoryID { get; set; }
        public int PropertyID { get; set; }
        public string Line { get; set; }
    
        public virtual Accessory Accessory { get; set; }
        public virtual Property Property { get; set; }
    }
}
