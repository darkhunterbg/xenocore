using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Input;
using XenoCore.Engine.Resources;

namespace XenoCore.Engine.Console
{
    public class ConsoleEntry
    {
        public String Text;
        public Color Color;

        public ConsoleEntry(String text, Color color)
        {
            this.Text = text;
            this.Color = color;
        }
    }

    public class ConsoleService
    {
        const int MAX_UI_SUGGESTIONS = 5;
        const int MAX_HISTORY = 50;

        private static ConsoleService instance;

        private Object mutex = new object();
        private bool isVisible;
        private Dictionary<String, ConsoleActionCompleter> completers = new Dictionary<string, ConsoleActionCompleter>();
        private Dictionary<String, ConsoleAction> actions  = new Dictionary<string, ConsoleAction>();
        private List<ConsoleEntry> entries = new List<ConsoleEntry>();

        private float consoleHeight = 0.5f;

        private IEnumerable<String> suggestions = null;
        private List<ConsoleInput> enteredCommands = new List<ConsoleInput>();
        private int oldCommandIndex = -1;
        private int autoCompleteIndex = -1;

        private StringBuilder input = new StringBuilder();

        private InputAction openCloseAction = new InputAction((state) => state.WasKeyReleased(Keys.OemTilde));

        private SpriteFont font;
        private SpriteBatch spriteBatch;

        private bool disableObjectsLoading = false;

        private ConsoleInput parsedInput = new ConsoleInput();

        public static bool DisableObjectsLoading
        {
            get { return instance.disableObjectsLoading; }
            set { instance.disableObjectsLoading = value; }
        }

        public static bool IsVisible
        {
            get { return instance.isVisible; }
            set { instance.isVisible = value; }
        }
        public static IEnumerable<ConsoleEntry> Entries
        {
            get
            {
                return instance.entries;
            }
        }

        [ConsoleVariable(Name = "con_height")]
        public static float ConsoleHeight
        {
            get { return instance.consoleHeight; }
            set { instance.consoleHeight = value; }
        }

        private ConsoleService() { }

        public static void Initialize(bool noGraphics = false)
        {
            Debug.Assert(instance == null, "ConsoleService is already initialized!");
            instance = new ConsoleService();

            if (!noGraphics)
            {
                instance.spriteBatch = new SpriteBatch(GraphicsService.Device);
                instance.font = GraphicsService.Cache[GraphicsService.Cache.GetFont("default")];
            }

            LoadFromAssembly(typeof(ConsoleService).GetTypeInfo().Assembly);
            LoadFromObject(instance);
        }
        public static void Uninitialize()
        {
            Debug.Assert(instance != null, "ConsoleService is not initialized!");
            UnloadFromObject(instance);
            instance = null;

        }

        [ConsoleCommand(Name = "clear")]
        public static void Clear()
        {
            instance.entries.Clear();
        }

        public static void AddCommand(String name, Action callback)
        {
            AddCommand(name, (p) => callback());
        }
        public static void AddCommand(String name, ConsoleCommandCallback callback, params ConsoleActionCompleter[] completers)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception($"ConsoleCommandAttribute.Name is empty!");

            if (instance.actions.ContainsKey(name))
                throw new Exception($"Command\\Property with name \"{name}\" exists!");

            instance.actions.Add(name, new ConsoleAction()
            {
                Name = name,
                Callback = callback,
                Completers = completers.Length == 0 ? null : completers,
            });
        }

        public static void AddVariable<T>(String name, ConsoleActionCompleter completer = null)
        {
            AddVariable(name, default(T), completer);
        }
        public static void AddVariable<T>(String name, T obj, ConsoleActionCompleter completer = null)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception($"ConsoleCommandAttribute.Name is empty!");

            if (instance.actions.ContainsKey(name))
                throw new Exception($"Command\\Property with name \"{name}\" exists!");

            T value = obj;

            ConsoleVariableGetter getter = () => { return value; };
            ConsoleVariableSetter setter = (p) => { value = (T)p; };

            if (completer == null)
                completer = GetDefaultCompleter(typeof(T));

            instance.actions.Add(name, new ConsoleAction()
            {
                IsVariable = true,
                Name = name,
                DefaultValue = obj?.ToString() ?? string.Empty,
                Completers = completer == null ? null : new[] { completer },
                Callback = (p =>
                {
                    VariableCallback(p, name, typeof(T), getter, setter, completer);
                })
            });
        }

        public static void AddEntry(String text, Color color)
        {
            var split = text.Split('\n');
            lock (instance.mutex)
            {
                foreach (var s in split)
                    instance.entries.Add(new ConsoleEntry(s, color));
            }
        }
        public static void DevMsg(String text)
        {
            AddEntry(text, Color.DeepSkyBlue);
        }
        public static void Msg(String text)
        {
            AddEntry(text, Color.White);
        }
        public static void Warning(String text)
        {
            AddEntry(text, Color.Red);
        }

        public static void LoadFromAssembly(String assemblyName, String nameSpace = null)
        {
            LoadFromAssembly(Assembly.Load(new AssemblyName(assemblyName)), nameSpace);
        }
        public static void LoadFromAssembly(Assembly assembly, String nameSpace = null)
        {
            return;

            if (DisableObjectsLoading)
                return;

            List<Type> types = null;
            if (!String.IsNullOrEmpty(nameSpace))
                types = assembly.ExportedTypes.Where(p => p.Namespace == nameSpace).ToList();
            else
                types = assembly.ExportedTypes.ToList();

            foreach (var type in types)
            {
                var methods = type.GetTypeInfo().DeclaredMethods.Where(p => p.IsStatic).ToList();

                foreach (var method in methods)
                {
                    var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCompleterAttribute));
                    if (attribute == null)
                        continue;

                    instance.AddCompleterFromAttribute(type, method, attribute);
                }

                foreach (var method in methods)
                {
                    var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCommandAttribute));
                    if (attribute == null)
                        continue;

                    instance.AddCommandFromAttribute(type, method, attribute);
                }

                var properties = type.GetTypeInfo().DeclaredProperties;

                foreach (var property in properties)
                {
                    var attribute = property.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleVariableAttribute));
                    if (attribute == null)
                        continue;

                    if (!property.GetMethod.IsStatic || !property.SetMethod.IsStatic)
                        continue;

                    instance.AddVariableFromAttribute(type, property, attribute);
                }
            }


            var names = assembly.GetManifestResourceNames();
        }
        public static void LoadFromObject(Object obj)
        {
            if (DisableObjectsLoading)
                return;

            var type = obj.GetType();

            var properties = type.GetTypeInfo().DeclaredProperties.ToList();
            var methods = type.GetTypeInfo().DeclaredMethods.ToList();

            var t = type.GetTypeInfo().BaseType;
            while(t!=typeof(Object))
            {
                properties.AddRange(t.GetTypeInfo().DeclaredProperties);
                methods.AddRange(t.GetTypeInfo().DeclaredMethods);
                t = t.GetTypeInfo().BaseType;
            }

            foreach (var method in methods)
            {
                var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCompleterAttribute));
                if (attribute == null)
                    continue;

                if (method.IsStatic)
                    continue;

                instance.AddCompleterFromAttribute(type, method, attribute, obj);
            }

            foreach (var method in methods)
            {
                var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCommandAttribute));
                if (attribute == null)
                    continue;

                if (method.IsStatic)
                    continue;

                instance.AddCommandFromAttribute(type, method, attribute, obj);
            }


            foreach (var property in properties)
            {
                var attribute = property.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleVariableAttribute));
                if (attribute == null)
                    continue;

                if (property.GetMethod.IsStatic || property.SetMethod.IsStatic)
                    continue;

                instance.AddVariableFromAttribute(type, property, attribute, obj);
            }
        }
        public static void UnloadFromObject(Object obj)
        {
            var type = obj.GetType();
            var properties = type.GetTypeInfo().DeclaredProperties;
            var methods = type.GetTypeInfo().DeclaredMethods;

            foreach (var method in methods)
            {
                var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCompleterAttribute));
                if (attribute == null)
                    continue;

                if (method.IsStatic)
                    continue;

                var name = attribute.NamedArguments.FirstOrDefault(p => p.MemberName == "Name").TypedValue.Value as String;

                if (String.IsNullOrEmpty(name))
                    continue;

                ConsoleActionCompleter action = null;
                if (instance.completers.TryGetValue(name, out action))
                    instance.actions.Remove(name);
            }


            foreach (var method in methods)
            {
                var attribute = method.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleCommandAttribute));
                if (attribute == null)
                    continue;

                if (method.IsStatic)
                    continue;

                var name = attribute.NamedArguments.FirstOrDefault(p => p.MemberName == "Name").TypedValue.Value as String;

                if (String.IsNullOrEmpty(name))
                    continue;

                ConsoleAction action = null;
                if (instance.actions.TryGetValue(name, out action))
                {
                    if (action.IsVariable)
                        throw new Exception($"Tried to removed command {name} , but it was a variable!");
                    instance.actions.Remove(name);
                }
            }

            foreach (var property in properties)
            {
                var attribute = property.CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ConsoleVariableAttribute));
                if (attribute == null)
                    continue;

                if (property.GetMethod.IsStatic || property.SetMethod.IsStatic)
                    continue;

                var name = attribute.NamedArguments.FirstOrDefault(p => p.MemberName == "Name").TypedValue.Value as String;

                if (String.IsNullOrEmpty(name))
                    continue;

                ConsoleAction action = null;
                if (instance.actions.TryGetValue(name, out action))
                {
                    if (!action.IsVariable)
                        throw new Exception($"Tried to removed variable {name} , but it was a command!");
                    instance.actions.Remove(name);
                }
            }
        }

        public static void Render(GameTime gameTime)
        {
            if (InputService.IsInputActionTriggered(instance.openCloseAction))
            {
                IsVisible = !IsVisible;
            }
            if (!IsVisible)
                return;

            instance.ProcessTextInput();

            if (InputService.InputState.WasKeyReleased(Keys.Enter))
            {
                instance.EnterCommand(instance.parsedInput.Command, instance.parsedInput.Parameters.ToArray());
                instance.parsedInput.Clear();
                instance.input.Clear();
                instance.oldCommandIndex = -1;
                instance.autoCompleteIndex = -1;
                instance.suggestions = null;
            }


            Point size = GraphicsService.WindowSize;
            size.Y = (int)(size.Y * ConsoleHeight);

            var font = instance.font;
            var spriteBatch = instance.spriteBatch;
            var white = GraphicsService.Cache[GraphicsService.Cache.WhiteTexture];

            int fontHeight = (int)(font.MeasureString("0").Y);

            spriteBatch.Begin();

            spriteBatch.Draw(white, new Rectangle(0, 0, size.X, size.Y), Color.Black * 0.66f);
            spriteBatch.Draw(white, new Rectangle(0, size.Y - fontHeight - 4, size.X, 2), Color.DarkGray);

            spriteBatch.DrawString(font, instance.input, new Vector2(0, size.Y - fontHeight), Color.White);

            Vector2 inputSize = font.MeasureString(instance.input);

            spriteBatch.Draw(white, new Rectangle((int)(inputSize.X), size.Y - fontHeight, fontHeight / 2, fontHeight - 4), Color.White);

            spriteBatch.Draw(white, new Rectangle(0, size.Y - 2, size.X, 2), Color.DarkGray);

            int maxEntries = size.Y / (fontHeight + 1);

            int take = Math.Min(instance.entries.Count, maxEntries);

            for (int i = 0; i < take; ++i)
            {
                int index = instance.entries.Count - i - 1;
                int height = size.Y - fontHeight - 4 - (i + 1) * fontHeight;
                spriteBatch.DrawString(font, instance.entries[index].Text, new Vector2(0, height), instance.entries[index].Color);
            }

            if (instance.ShowAutoComplete())
            {
                if (instance.suggestions == null)
                    instance.suggestions = instance.Suggest(instance.parsedInput);

                if (instance.suggestions != null)
                {
                    int count = Math.Min(instance.suggestions.Count(), MAX_UI_SUGGESTIONS);
                    var commands = instance.suggestions.Take(count).ToList();

                    if (count > 0)
                    {
                        int offsetX = 0;

                        //offsetX = (int)font.MeasureString(parsedInput.ToString()).X;

                        int width = commands.Max(p => font.MeasureString(p).ToPoint().X) + 4;
                        int height = count * fontHeight;

                        spriteBatch.Draw(white, new Rectangle(offsetX, size.Y, width, height), Color.Black * 0.66f);

                        for (int i = 0; i < count; ++i)
                        {
                            if (instance.autoCompleteIndex == i)
                                spriteBatch.Draw(white, new Rectangle(offsetX, size.Y + i * fontHeight, width, fontHeight), Color.LightGray * 0.66f);
                            height = i * fontHeight + size.Y;
                            spriteBatch.DrawString(font, commands[i], new Vector2(offsetX, height), Color.White * 0.8f);
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private void ProcessTextInput()
        {
            int oldLength = input.Length;
            InputService.InputState.UpdateInputText(input, '`');
            bool isModified = oldLength != input.Length;

            bool oldEntry = false;

            if (InputService.InputState.WasKeyReleased(Keys.Down))
            {
                if (suggestions == null)
                {
                    //if (enteredCommands.Count > 0)
                    //{
                    //    oldCommandIndex++;
                    //    if (oldCommandIndex >= enteredCommands.Count)
                    //        oldCommandIndex = 0;
                    //    oldEntry = true;
                    //}
                }
                else if (suggestions.Count() > 0)
                {
                    autoCompleteIndex++;
                    if (autoCompleteIndex >= suggestions.Count())
                        autoCompleteIndex = 0;

                }
            }
            if (InputService.InputState.WasKeyReleased(Keys.Up))
            {
                if (suggestions == null)
                {
                    if (enteredCommands.Count > 0)
                    {
                        oldCommandIndex--;
                        if (oldCommandIndex < 0)
                            oldCommandIndex = enteredCommands.Count - 1;
                        oldEntry = true;
                    }
                }
                else if (suggestions.Count() > 0)
                {
                    autoCompleteIndex--;
                    if (autoCompleteIndex < 0)
                        autoCompleteIndex = suggestions.Count() - 1;
                }
            }
            if (oldEntry)
            {
                input.Clear();
                input.Append(enteredCommands[oldCommandIndex]);
                isModified = true;
            }
            if (InputService.InputState.WasKeyReleased(Keys.Delete))
            {
                input.Clear();
                isModified = true;
            }

            if (InputService.InputState.WasKeyReleased(Keys.Tab) ||
                InputService.InputState.WasKeyReleased(Keys.Enter))
            {
                if (suggestions?.Count() > 0)
                {
                    if (autoCompleteIndex < 0)
                        autoCompleteIndex = 0;
                    AutoCompleteInput(parsedInput, autoCompleteIndex);
                    isModified = true;
                }
            }

            if (isModified)
            {
                ParseInput(input.ToString(), parsedInput);
                suggestions = null;
                autoCompleteIndex = -1;
            }
        }


        private bool ShowAutoComplete()
        {
            return input.Length > 0 &&
                ((parsedInput.Parameters.Count == 0 || parsedInput.IsLastOK) || (parsedInput.Parameters.Count > 0));
        }

        private void ParseInput(String input, ConsoleInput parsedInput)
        {
            var split = SplitString(input);
            parsedInput.Command = split.FirstOrDefault() ?? String.Empty;
            parsedInput.Parameters.Clear();

            for (int i = 1; i < split.Length; ++i)
                parsedInput.Parameters.Add(split[i]);

            parsedInput.IsLastOK = input.LastOrDefault() == ' ';
        }
        private IEnumerable<String> Suggest(ConsoleInput input)
        {
            if (String.IsNullOrEmpty(input.Command))
                return new String[0];

            if (input.Parameters.Count == 0 && !input.IsLastOK)
            {
                return actions.Keys.Where(p => p.StartsWith(input.Command)).OrderBy(p => p.Length).ThenBy(p => p);
            }
            else
            {
                if (!actions.ContainsKey(input.Command))
                    return new String[0];

                var consoleCommand = actions[input.Command];

                int paramIndex = input.LastParameterIndex;
                String paramText = String.Empty;
                if (input.IsLastOK)
                    paramIndex += 1;
                else
                    paramText = input.LastParameter;

                if (consoleCommand.IsVarArgs)
                    paramIndex = 0;

                if (consoleCommand.IsVariable && paramIndex > 0)
                    return new String[0];

                if (consoleCommand.Completers == null ||
                   paramIndex >= consoleCommand.Completers.Length ||
                   consoleCommand.Completers[paramIndex] == null)
                    return new String[0];

                var completeParam = consoleCommand.Completers[paramIndex](new CompletionContext()
                {
                    ParameterText = paramText
                });
                var result = completeParam?.Where(p => p.StartsWith(paramText));
                if (result == null)
                    return new String[0];
                else
                    return result;
            }

        }

        private void AppendNormalized(String text)
        {
            bool hasWhiteSpace = text.Contains(' ');

            if (hasWhiteSpace)
                input.Append('"');

            input.Append($"{text}");

            if (hasWhiteSpace)
                input.Append('"');
        }
        private void AutoCompleteInput(ConsoleInput input, int suggestionIndex)
        {
            if (suggestions == null)
                return;

            var commands = suggestions.ToArray();

            Debug.Assert(suggestionIndex > -1 && suggestionIndex < commands.Length, "Suggestion index is invalid!");

            this.input.Clear();

            if (input.Parameters.Count > 0 || input.IsLastOK)
            {
                AppendNormalized(input.Command);
                this.input.Append(' ');

                int max = input.Parameters.Count - 1;
                if (input.IsLastOK)
                    ++max;

                for (int i = 0; i < max; ++i)
                {
                    AppendNormalized(input.Parameters[i]);
                    this.input.Append(' ');
                }
            }

            AppendNormalized(commands[suggestionIndex]);
        }

        public static void ExecuteCommand(String cmd,params String[] parameters)
        {
            instance.EnterCommand(cmd, parameters);
        }
        private void EnterCommand(String cmd, params String[] parameters)
        {
            if (String.IsNullOrEmpty(cmd))
                return;

            var input = new ConsoleInput(cmd, parameters);
            enteredCommands.Add(input);
            if (enteredCommands.Count > MAX_HISTORY)
                enteredCommands.RemoveAt(enteredCommands.Count - 1);

            AddEntry($"] {input}", Color.LightGray);

            ConsoleAction command = null;
            if (actions.TryGetValue(cmd, out command))
            {
                command.Callback(parameters);
            }
            else
            {
                AddEntry($"Unknown command: \"{input}\"", Color.Red);
            }
        }

        private void AddCompleterFromAttribute(Type type, MethodInfo method, CustomAttributeData attribute, Object obj = null)
        {
            var name = attribute.NamedArguments.Where(p => p.MemberName == "Name")
               .Select(p => p.TypedValue.Value as String).FirstOrDefault();
            if (String.IsNullOrEmpty(name))
                throw new Exception($"ConsoleCompleterAttributer.Name is empty! {type.FullName}.{method.Name}");

            if (completers.ContainsKey(name))
                throw new Exception($"Completer with name \"{name}\" exists!");


            bool isStatic = obj == null;

            if (isStatic != method.IsStatic)
                throw new Exception($"Property \"{name}\" IsStatic={!isStatic}, but it was expected to be {isStatic}");

            ConsoleActionCompleter completer = (p) =>
            {
                return method.Invoke(obj, new Object[] { p }) as List<String>;
            };
            // method.CreateDelegate(typeof(ConsoleActionCompleter)) as ConsoleActionCompleter;
            completers.Add(name, completer);
        }
        private void AddCommandFromAttribute(Type type, MethodInfo method, CustomAttributeData attribute, Object obj = null)
        {
            var name = attribute.NamedArguments.Where(p => p.MemberName == "Name")
                .Select(p => p.TypedValue.Value as String).FirstOrDefault();
            if (String.IsNullOrEmpty(name))
                throw new Exception($"ConsoleCommandAttribute.Name is empty! {type.FullName}.{method.Name}");

            if (actions.ContainsKey(name))
                throw new Exception($"Command\\Property with name \"{name}\" exists!");

            var completerName = attribute.NamedArguments.Where(p => p.MemberName == "CompleterName")
                .Select(p => p.TypedValue.Value as String).FirstOrDefault();

            List<ConsoleActionCompleter> commandCompleters = new List<ConsoleActionCompleter>();

            var parameters = method.GetParameters();

            if (!String.IsNullOrEmpty(completerName))
            {
                if (!completers.ContainsKey(completerName))
                    throw new Exception($"Completer with name \"{completerName}\" does not exists!");

                commandCompleters.Add(completers[completerName]);
            }


            for (int i = 0; i < parameters.Length; ++i)
            {
                var attr = parameters[i].CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(ConsoleCmdParam));

                if (attr != null)
                {
                    var complName = attr.NamedArguments.Where(p => p.MemberName == "CompleterName")
                .Select(p => p.TypedValue.Value as String).FirstOrDefault();

                    if (!String.IsNullOrEmpty(complName))
                    {
                        if (!completers.ContainsKey(complName))
                            throw new Exception($"Completer with name \"{complName}\" does not exists!");

                        commandCompleters.Add(completers[complName]);

                        continue;
                    }
                }

                if (commandCompleters.Count <= i)
                    commandCompleters.Add(GetDefaultCompleter(parameters[i].ParameterType));
                else if (commandCompleters[i] == null)
                    commandCompleters[i] = GetDefaultCompleter(parameters[i].ParameterType);
            }


            bool isStatic = obj == null;

            if (isStatic != method.IsStatic)
                throw new Exception($"Property \"{name}\" IsStatic={!isStatic}, but it was expected to be {isStatic}");
            bool useParams = parameters.Length > 0;
            bool isVarArgs = useParams && parameters[0].GetCustomAttribute(typeof(ParamArrayAttribute)) != null;

            ConsoleCommandCallback command;

            if (useParams)
                command = (p) => { ParamCommandCallback(p, method, obj); };
            else
                command = (p) => { method.Invoke(obj, new object[0]); };// method.Invoke(obj, new Object[0]); };

            actions.Add(name, new ConsoleAction()
            {
                IsVarArgs = isVarArgs,
                Callback = command,
                Name = name,
                Completers = useParams ? commandCompleters.ToArray() : null,
            });

        }
        private void AddVariableFromAttribute(Type type, PropertyInfo property, CustomAttributeData attribute, Object obj = null)
        {
            var name = attribute.NamedArguments.Where(p => p.MemberName == "Name")
              .Select(p => p.TypedValue.Value as String).FirstOrDefault();
            if (String.IsNullOrEmpty(name))
                throw new Exception($"ConsoleVariableAttribute.Name is empty! {type.FullName}.{property.Name}");

            if (actions.ContainsKey(name))
                throw new Exception($"Command\\Property with name \"{name}\" exists!");

            var completerName = attribute.NamedArguments.Where(p => p.MemberName == "CompleterName")
                .Select(p => p.TypedValue.Value as String).FirstOrDefault();

            ConsoleActionCompleter completer = null;

            if (!String.IsNullOrEmpty(completerName))
            {
                if (!completers.ContainsKey(completerName))
                    throw new Exception($"Completer with name \"{completerName}\" does not exists!");

                completer = completers[completerName];
            }

            var def = attribute.NamedArguments.Where(p => p.MemberName == "Default")
             .Select(p => p.TypedValue.Value as String).FirstOrDefault();

            bool isStatic = obj == null;

            if (isStatic != (property.GetMethod.IsStatic && property.SetMethod.IsStatic))
                throw new Exception($"Property \"{name}\" IsStatic={!isStatic}, but it was expected to be {isStatic}");

            ConsoleVariableGetter getter = () => { return property.GetValue(obj); };
            ConsoleVariableSetter setter = (p) => { property.SetValue(obj, p); };

            if (def != null)
                VariableCallback(new string[] { def }, name, property.PropertyType, getter, setter, completer);
            else
                def = getter.Invoke().ToString() ?? String.Empty;

            if (completer == null)
                completer = GetDefaultCompleter(property.PropertyType);

            actions.Add(name, new ConsoleAction()
            {
                IsVariable = true,
                Name = name,
                DefaultValue = def,
                Callback = (p) =>
                {
                    VariableCallback(p, name, property.PropertyType, getter, setter, completer);
                },
                Completers = completer == null ? null : new[] { completer },
            });
        }

        private static Object ConvertString(String obj, Type type)
        {
            if (type == typeof(bool))
            {
                bool result;
                if (!Boolean.TryParse(obj, out result))
                    return null;
                return result;
            }
            if (type == typeof(int))
            {
                int result;
                if (!Int32.TryParse(obj, out result))
                    return null;
                return result;
            }
            if (type == typeof(uint))
            {
                uint result;
                if (!UInt32.TryParse(obj, out result))
                    return null;
                return result;
            }
            if (type == typeof(float))
            {
                float result;
                if (!float.TryParse(obj, out result))
                    return null;
                return result;
            }

            if (type.GetTypeInfo().BaseType == typeof(Enum))
            {
                var names = Enum.GetNames(type);
                var values = Enum.GetValues(type);
                for (int i = 0; i < names.Length; ++i)
                    if (names[i] == obj)
                        return values.GetValue(i);

                return null;

            }

            return obj;
        }

        private static ConsoleActionCompleter GetDefaultCompleter(Type type)
        {
            if (type == typeof(bool))
                return DefaultCompleters.BooleanCompleter;
            if (type.GetTypeInfo().BaseType == typeof(Enum))
                return (p) => { return Enum.GetNames(type).ToList(); };

            return null;
        }

        private static void ParamCommandCallback(String[] p, MethodInfo method, Object obj)
        {
            try
            {
                Object[] param = null;

                var parameters = method.GetParameters();

                if (parameters.Length > 0
                    && parameters[0].GetCustomAttribute(typeof(ParamArrayAttribute)) != null)
                {
                    //varargs
                    param = new object[] { p };
                }
                else if (parameters.Length != p.Length)
                    throw new System.Reflection.TargetParameterCountException();
                else
                {
                    param = new object[p.Length];
                    for (int i = 0; i < param.Length; ++i)
                        param[i] = ConvertString(p[i], parameters[i].ParameterType);
                }
                method.Invoke(obj, param);
            }
            catch (TargetParameterCountException ex)
            {
                Warning(ex.Message);
            }

        }

        private static void VariableCallback(String[] p, String name, Type varType, ConsoleVariableGetter getter,
            ConsoleVariableSetter setter, ConsoleActionCompleter completer)
        {
            if (p.Length == 0)
            {
                var result = getter.Invoke();
                Msg(result?.ToString() ?? String.Empty);
            }
            else
            {
                String value = p[0];
                var suggested = completer?.Invoke(new CompletionContext());
                bool canSet = true;
                if (suggested != null && suggested.Count > 0)
                {
                    canSet = suggested.Any(q => q == value);
                }

                if (canSet)
                {
                    var converted = ConvertString(value, varType);
                    if (converted != null)
                        setter.Invoke(converted);
                }

                else
                    Warning($"Invalid value specified for variable \"{name}\"");

            }
        }

        private static String[] SplitString(String input)
        {
            var split = input.Split(new char[] { '"' });

            List<String> results = new List<string>();

            for (int i = 0; i < split.Length; ++i)
            {
                if (i % 2 == 0)
                {
                    if (split[i].Length == 0)
                        continue;

                    var split2 = split[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    results.AddRange(split2);
                }
                else
                {
                    results.Add(split[i]);
                }
            }

            return results.ToArray();
        }
    }
}

