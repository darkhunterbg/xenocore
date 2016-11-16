using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using XenoCore.Builder.Converters;
using XenoCore.Builder.Data;
using XenoCore.Builder.GUI;
using XenoCore.Builder.MVVM;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Resources;

namespace XenoCore.Builder.Services
{
    public static class UIFactory
    {
        public static void GenerateUI(Panel containingPanel, ObjectEditorModel model, bool separator = true)
        {
            foreach (var property in model.Properties)
            {

                var dockPanel = new DockPanel()
                {
                    Margin = new Thickness(4)
                };


                if (separator)
                {
                    var sep = new TextBlock()
                    {
                        Background = Brushes.LightGray,
                        Height = 2,
                        Margin = new Thickness(0, 8, 0, 0)
                    };
                    DockPanel.SetDock(sep, Dock.Bottom);

                    dockPanel.Children.Add(sep);
                }


                dockPanel.Children.Add(new TextBlock()
                {
                    Text = property.DisplayName,
                    VerticalAlignment = VerticalAlignment.Center,
                });

                var binding = new Binding();
                binding.Source = property;
                binding.Path = new PropertyPath("Value");
                FrameworkElement editingControl = null;


                #region Types
                switch (property.Attribite.EditorType)
                {
                    case Engine.Editor.ValueEditor.Float:
                        {

                            editingControl = new DoubleUpDown()
                            {
                                Minimum = property.Attribite.Min,
                                Maximum = property.Attribite.Max,
                                Increment = property.Attribite.Step > 0 ? property.Attribite.Step : (double?)null,
                                FormatString = property.Attribite.Format
                            };

                            binding.Converter = new FloatConverter();
                            editingControl.SetBinding(DoubleUpDown.ValueProperty, binding);
                        }
                        break;
                    case Engine.Editor.ValueEditor.Integer:
                        {
                            editingControl = new IntegerUpDown()
                            {
                                Minimum = (int)property.Attribite.Min,
                                Maximum = (int)property.Attribite.Max,
                                // Increment = property.Attribite.Step > 0 ? (int)property.Attribite.Step : 1,
                                //FormatString = property.Attribite.Format
                            };

                            editingControl.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    case Engine.Editor.ValueEditor.Color:
                        {
                            var control = new ColorPicker();
                            control.ColorMode = ColorMode.ColorCanvas;
                            control.UsingAlphaChannel = true;
                            binding.Converter = new Converters.ColorConverter();
                            editingControl = control;
                            editingControl.SetBinding(ColorPicker.SelectedColorProperty, binding);
                            break;
                        }
                    case Engine.Editor.ValueEditor.Vector:
                        {
                            var control = new Controls.VectorEditor();
                            control.MinValue = property.Attribite.Min;
                            control.MaxValue = property.Attribite.Max;
                            control.Format = property.Attribite.Format;
                            control.Increment = property.Attribite.Step > 0 ? property.Attribite.Step : (double?)null;
                            editingControl = control;
                            editingControl.SetBinding(FrameworkElement.DataContextProperty, binding);
                            control.OnValueChanged += (s, a) => { property.Value = a; };

                            break;
                        }
                    case Engine.Editor.ValueEditor.Slider:
                        {
                            var container = new DockPanel();
                            var label = new TextBlock();
                            var b = new Binding();
                            b.Source = property;
                            b.Converter = new FloatConverter();
                            b.StringFormat = property.Attribite.Format;
                            b.Path = new PropertyPath("Value");
                            label.SetBinding(TextBlock.TextProperty, b);

                            container.Children.Add(label);

                            var control = new Slider()
                            {
                                Minimum = property.Attribite.Min,
                                Maximum = property.Attribite.Max,
                                TickFrequency = property.Attribite.Step,
                                IsSnapToTickEnabled = property.Attribite.Step > 0,
                                Margin = new Thickness(10, 0, 0, 0),
                            };
                            control.SmallChange = property.Attribite.Step;
                            control.LargeChange = (property.Attribite.Max - property.Attribite.Min) / 10.0;
                            binding.Converter = new FloatConverter();
                            control.SetBinding(Slider.ValueProperty, binding);
                            container.Children.Add(control);

                            editingControl = container;
                            break;
                        }
                    case Engine.Editor.ValueEditor.List:
                        {
                            var control = new ComboBox();
                            var listProperty = property as ObjectPropertyListModel;
                            control.ItemsSource = listProperty.Collection;
                            binding.Path.Path = "Selected";
                            control.SetBinding(ComboBox.SelectedItemProperty, binding);
                            editingControl = control;
                            break;
                        }
                    case Engine.Editor.ValueEditor.Enum:
                        {
                            var control = new ComboBox();
                            control.ItemsSource = Enum.GetValues(property.ValueType);
                            control.SetBinding(ComboBox.SelectedItemProperty, binding);
                            editingControl = control;
                            break;
                        }
                    case Engine.Editor.ValueEditor.Texture:
                        {
                            var control = new WindowsFormsHost()
                            {
                                Width = 64,
                                Height = 64,
                            };
                            var host = new Host.TextureRenderControl()
                            {
                                StretchMode = Host.TextureRenderStrech.AspectFit
                            };
                            control.Unloaded += (s, a) => { host.Dispose(); };

                            Texture t = GraphicsService.Cache.GetTexture(property.Value as String);

                            host.Click += (s, a) =>
                            {
                                var dialog = new ResourcesWindow(ResourceType.Texture);
                                dialog.ViewModel.IsSelection = true;
                                dialog.ViewModel.SetSelectedItem(property.Value as String, ResourceType.Texture);

                                Texture texture = GraphicsService.Cache.GetTexture(property.Value as String);

                                if (dialog.ShowDialog().Value == true)
                                {
                                    var selected = dialog.ViewModel.Selection as ResourceObjModel;
                                    property.Value = selected.Resource.XnbPath;
                                    texture = GraphicsService.Cache.GetTexture(property.Value as String);
                                    host.Texture = GraphicsService.Cache[texture];
                                    host.Invalidate();
                                }

                                // host.Screen.Paused = false;
                            };
                            host.Texture = GraphicsService.Cache[t]; 
                            control.Child = host;



                            editingControl = control;
                            break;
                        }
                    case Engine.Editor.ValueEditor.CheckBox:
                        {
                            var control = new CheckBox();
                            control.SetBinding(CheckBox.IsCheckedProperty, binding);
                            editingControl = control;
                        }
                        break;
                    case Engine.Editor.ValueEditor.Properties:
                        {
                            var panel = new StackPanel();
                            editingControl = panel;

                            GenerateUI(panel, (property as ObjectPropertyCompositionModel).EditorModel, false);
                        }
                        break;
                    default:
                        {
                            editingControl = new TextBox();
                            editingControl.SetBinding(TextBox.TextProperty, binding);
                        }
                        break;

                }
                #endregion

                editingControl.Margin = new Thickness(10, 0, 0, 0);

                dockPanel.Children.Add(editingControl);

         

                containingPanel.Children.Add(dockPanel);
            }
        }
    }
}
