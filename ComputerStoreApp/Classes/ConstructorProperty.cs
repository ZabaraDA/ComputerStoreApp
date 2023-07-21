using ComputerStoreApp.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ComputerStoreApp.Classes
{
    public class ChangeNotification : INotifyPropertyChanged
    {
        #region OnPropertyChanged
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
    public class BaseProperty
    {
        public Property Property { get; set; }
    }

    public class TypeProperty: BaseProperty
    {
        public Databases.Type Type { get; set; }
    }

    public class FilterTypeProperty : BaseProperty
    {
        public ICollection<IsAvailableTypeProperty> Type { get; set; }
    }

    public class NumberProperty : BaseProperty
    {
        public decimal Number { get; set; }
    }

    public class LineProperty : BaseProperty
    {
        public string Line { get; set; }
    }

    public class IsAvailableCategoryProperty
    {
        public bool IsAvailable { get; set; }
        public Category Category { get; set; }
    }

    public class IsAvailableTypeProperty: ChangeNotification
    {
        private bool _isAvailable;
        public bool IsAvailable 
        {   get
            {
                return _isAvailable;
            }
            set
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(IsAvailable));
            }          
        }
        public Databases.Type Type { get; set; }
    } 
}
