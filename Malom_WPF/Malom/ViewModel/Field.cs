using System;
using Malom.Persistence;

namespace Malom.ViewModel
{
    class FieldViewModel : ViewModelBase
    {
        public int Number { get; set; }
        private FIELDS fieldType;
        public FIELDS FieldType { 
            get { return fieldType; }
            set 
            {
                fieldType = value;
                OnPropertyChanged();
            } }

        public DelegateCommand StepCommand { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
