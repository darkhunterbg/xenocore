using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Particles;

namespace XenoCore.Builder.MVVM
{
    public abstract class EditorViewModel : BaseModel
    {
        private bool modified;
        public bool Modified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
                OnPropertyChanged("Modified");
            }
        }

    }

    public class ObjectPropertyModel : BaseModel
    {
        public String DisplayName { get; set; }
        public Type ValueType { get; set; }
        public Func<Object> Getter { get; set; }
        public Action<Object> Setter { get; set; }
        public EditorInfoAttribute Attribite { get; set; }
        public Object Value
        {
            get
            {
                return Getter();
            }
            set
            {
                Setter(value);
                OnPropertyChanged("Value");
            }
        }


        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class ObjectPropertyListModel : ObjectPropertyModel
    {
        private ObjectEditorModel selected = null;

        public ObservableCollection<ObjectEditorModel> Collection { get; private set; } = new ObservableCollection<ObjectEditorModel>();
        public ObjectEditorModel Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("Selected"); }
        }

    }

    public class ObjectPropertyCompositionModel : ObjectPropertyModel
    {
        public ObjectEditorModel EditorModel { get; set; }
    }

    public class ObjectEditorModel : BaseModel
    {
        public String Name { get; set; }

        public Object Object { get; private set; }

        public T GetProperty<T>(String name) where T : ObjectPropertyModel
        {
            return Properties.FirstOrDefault(p => p.DisplayName == name) as T;
        }

        public ObservableCollection<ObjectPropertyModel> Properties { get; private set; }
            = new ObservableCollection<ObjectPropertyModel>();

        public ObjectEditorModel(Object obj, String name = null, EditorInfoAttribute defaultAttr = null)
        {
            this.Name = name;
            if (this.Name == null)
                this.Name = obj.GetType().Name;
            Object = obj;

            var properties = obj.GetType().GetProperties().ToList();

            foreach (var p in properties)
            {
                if (p.GetCustomAttributes(typeof(EditorIgnoreAttribute), true).Length > 0)
                    continue;

                var attribute = p.GetCustomAttributes(typeof(EditorInfoAttribute), true).FirstOrDefault() as EditorInfoAttribute;

                if (attribute == null)
                    attribute = defaultAttr;

                if (attribute == null)
                    continue;

                var property = new ObjectPropertyModel()
                {
                    DisplayName = p.Name,
                    ValueType = p.PropertyType,
                    Attribite = attribute,
                    Getter = () => p.GetValue(obj),
                    Setter = (v) => p.SetValue(obj, v),
                };



                switch (attribute.EditorType)
                {
                    case ValueEditor.Vector:
                        {
                            property.Getter = () => { Vector2 r = (Vector2)p.GetValue(obj); return new Vector((double)r.X, (double)r.Y); };
                            property.Setter = (v) => { Vector val = (Vector)v; p.SetValue(obj, new Vector2((float)val.X, (float)val.Y)); };
                        }
                        break;
                    case ValueEditor.List:
                        {
                            var listProperty = new ObjectPropertyListModel()
                            {
                                DisplayName = property.DisplayName,
                                ValueType = property.ValueType,
                                Attribite = property.Attribite,
                                Getter = property.Getter,
                                Setter = property.Setter
                            };
                            property = listProperty;

                            foreach (var item in p.GetValue(obj) as IList)
                            {
                                var model = new ObjectEditorModel(item);

                                listProperty.Collection.Add(model);
                                model.PropertyChanged += Property_PropertyChanged;
                            }
                        }
                        break;

                    case ValueEditor.Properties:
                        {
                            var compositionProperty = new ObjectPropertyCompositionModel()
                            {
                                DisplayName = property.DisplayName,
                                ValueType = property.ValueType,
                                Attribite = property.Attribite,
                            };
                            property = compositionProperty;

                            var def = p.GetCustomAttributes(typeof(EditorInfoAttribute), true).LastOrDefault() as EditorInfoAttribute;
                            if (def == null || def == attribute)
                                continue;

                            compositionProperty.EditorModel = new ObjectEditorModel(p.GetValue(obj), p.Name, def);
                            compositionProperty.EditorModel.PropertyChanged += Property_PropertyChanged;
                        }
                        break;
                }



                property.PropertyChanged += Property_PropertyChanged;

                Properties.Add(property);
            }
        }

        public void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (sender is ObjectPropertyListModel)
            {
                var property = sender as ObjectPropertyListModel;
                OnPropertyChanged($"{property.DisplayName}.{e.PropertyName}");
            }
            else if (sender is ObjectPropertyModel)
            {
                var property = sender as ObjectPropertyModel;
                OnPropertyChanged(property.DisplayName);
            }
            else
            {
                OnPropertyChanged(sender.ToString());
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
