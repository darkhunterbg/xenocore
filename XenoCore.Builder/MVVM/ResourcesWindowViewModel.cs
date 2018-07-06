using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.Data;
using XenoCore.Builder.Services;
using XenoCore.ContentPipeline;
using XenoCore.Engine;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;

namespace XenoCore.Builder.MVVM
{
    public class NewResourceModel : BaseModel
    {
        public ResourceType Type { get; set; }
        public String OptionText { get; set; }

        public NewResourceModel(ResourceType type, String optionText = null)
        {
            Type = type;
            if (!String.IsNullOrEmpty(optionText))
                OptionText = optionText;
            else
                OptionText = type.ToString();
        }
    }

    public class ResourcesWindowViewModel : BaseModel, IResourceEventReciever
    {
        private ResourceModel selection;
        private bool isSelection;
        private String searchTerm;
        private ResourceType? typeFilter;
        public Engine.Screens.Screen Screen { get; private set; }
        public bool Initialized { get; private set; }
        public ObservableCollection<ResourceModel> Resources { get; private set; } = new ObservableCollection<ResourceModel>();
        public ResourceType? TypeFilter
        {
            get { return typeFilter; }
            set
            {
                bool reload = typeFilter == value;
                typeFilter = value;
                if (reload)
                    RegenerateResourcesCollection();
            }
        }
        public bool OkEnabled
        {
            get { return Selection != null && Selection is ResourceObjModel; }
        }
        public ResourceModel Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (selection != null && selection.Selected)
                    selection.Selected = false;

                selection = value;

                if (selection != null && !selection.Selected)
                    selection.Selected = true;

                OnPropertyChanged("Selection");
                OnPropertyChanged("Title");
                OnPropertyChanged("OkEnabled");
            }
        }
        public String SearchTerm
        {
            get { return searchTerm; }
            set
            {
                searchTerm = value;
                OnPropertyChanged(searchTerm);
                RegenerateResourcesCollection();
            }
        }

        public bool IsSelection
        {
            get { return isSelection; }
            set
            {
                isSelection = value;
                OnPropertyChanged("IsSelection");
            }
        }
        public String Title
        {
            get
            {
                return Selection?.ContentPath ?? "Resources";
            }
        }

        public void Initialize(ResourceType? typeFilter = null)
        {
            this.typeFilter = typeFilter;

            ResourceManagerService.SubscribeForEvents(this);

            if (Resources.Count == 0)
                RegenerateResourcesCollection();
        }

        public void Uninitialize()
        {
            ResourceManagerService.UnsubscribeForEvents(this);
        }

        void IResourceEventReciever.ItemAdded(Resource item)
        {
            var root = Resources.First() as ResourceDirModel;

            switch (item.Type)
            {
                case ResourceType.Directory:
                    {
                        GetContainingDir(root, item.ContentPath).Items.Add(new ResourceDirModel(item as ResourceDir));
                    }
                    break;
                default:
                    {
                        GetContainingDir(root, item.ContentPath).Items.Add(new ResourceObjModel(item as ResourceObj));
                    }
                    break;
            }
        }
        void IResourceEventReciever.ItemRemoved(Resource item)
        {
            var root = Resources.First() as ResourceDirModel;
            var dir = GetContainingDir(root, item.ContentPath);
            var i = dir.Items.First(p => p.ContentPath == item.ContentPath);
            dir.Items.Remove(i);
        }
        void IResourceEventReciever.ItemModified(String oldContentPath, Resource item)
        {
            var root = Resources.First() as ResourceDirModel;
            var i = GetContainingDir(root, oldContentPath).Items.First(p => p.ContentPath == oldContentPath);
            i.OnModified();
        }

        public void SetSelectedItem(String path , ResourceType? type)
        {
            if (!String.IsNullOrEmpty(path))
            {
                Selection = FindItem(path, type);

            }
            else
                Selection = null;
        }


        public void New(ResourceType type)
        {
            var parent = Selection as ResourceDirModel;
            Selection.Expanded = true;

            var name = $"{type}{Selection.Items.Count}";
            var path = Path.Combine(parent.Directory.ContentPath, name);

            switch (type)
            {
                case ResourceType.Directory:
                    ResourceManagerService.CreateDirectory(path);
                    break;

                case ResourceType.Unknown:
                    Debug.Assert(false, "Cannot create resoure of type Unknown");
                    break;
                default:
                    ResourceManagerService.CreateResource(path, type);
                    break;
            }
        }

        public void AddNewItems(IEnumerable<String> itemsPath, bool select = true)
        {
            var root = Selection as ResourceDirModel;

            if (root == null)
                root = GetContainingDir(Resources.First() as ResourceDirModel, Selection.ContentPath);

            String selection = null;

            foreach (var item in itemsPath)
            {
                FileAttributes attr = File.GetAttributes(item);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    ResourceManagerService.CreateDirectory(Path.Combine(root.ContentPath, Path.GetFileName(item)));
                    var content = Directory.GetFiles(item).ToList();
                    content.AddRange(Directory.GetDirectories(item));
                    AddNewItems(itemsPath, false);
                }
                else
                {
                    var path = Path.Combine(root.Directory.AbsolutePath, Path.GetFileName(item));
                    if (File.Exists(path) && path != item)
                        File.Delete(path);

                    if (path != item)
                        File.Copy(item, path);
                    selection = ResourceManagerService.AddResource(path).XnbPath;
                }
            }

            if (select)
                Selection = FindItem(selection);

        }

        public void RenameSelectedItem()
        {

            if (Selection != Resources.First() &&
                Selection.Name != Selection.EditingText)
            {
                Selection.Editing = false;

                ResourceManagerService.Rename(Selection.Item, Selection.EditingText);

                /*

                if (Selection.IsDirectory)
                {
                    var oldPath = GetAbsolutePath(Selection);
                    Selection.Rename(Selection.EditingText);
                    var newPath = GetAbsolutePath(Selection);

                    Directory.Move(oldPath, newPath);
                }
                else
                {
                    var oldXnb = Selection.XnbPath;
                    var oldPath = GetAbsolutePath(Selection);
                    Selection.Rename(Selection.EditingText);
                    var newPath = GetAbsolutePath(Selection);

                    File.Move(oldPath, newPath);

                    PipelineService.Save(pipelineProject);


                    switch (Selection.Type)
                    {
                        case ResourceType.SpriteFont:
                            ResourcesService.UnloadFont(oldXnb);
                            break;
                        case ResourceType.Texture:
                            ResourcesService.UnloadTexture(oldXnb);
                            break;
                    }

                }
                */

                Selection = null;
            }
        }
        public void DeleteSelectedItem()
        {

            if (Selection == Resources.First())
                return;

            if (Selection is ResourceDirModel)
            {
                ResourceManagerService.DeleteDirectory((Selection as ResourceDirModel).Directory);
            }
            else if
                (Selection is ResourceObjModel)
            {
                ResourceManagerService.DeleteResource((Selection as ResourceObjModel).Resource);
            }

            Selection = null;

        }

        private ResourceDirModel GetContainingDir(ResourceDirModel root, String contentPath)
        {
            ResourceDirModel result = root;

            if (String.IsNullOrEmpty(contentPath))
                return root;

            var path = Path.GetDirectoryName(contentPath).
                Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < path.Count; ++i)
            {
                var dir = path[i];
                var dirItem = result.Items.Where(p => p is ResourceDirModel).Select(p => p as ResourceDirModel)
                    .FirstOrDefault(p => p.Directory.Name == dir);

                if (dirItem == null)
                {
                    break;
                }
                result = dirItem;
            }

            return result;
        }

        private void RegenerateResourcesCollection()
        {
            Resources.Clear();

            Resources.Add(new ResourceDirModel(new ResourceDir()
            {
                Name = "Content",
                XnbPath = "",
                ContentPath = "",
                AbsolutePath = ResourceManagerService.ContentLocation,
            }));
            var root = Resources.First() as ResourceDirModel;
            root.Expanded = true;
            foreach (var dir in ResourceManagerService.Directories)
            {
                GetContainingDir(root, dir.ContentPath).Items.Add(new ResourceDirModel(dir));
            }

            foreach (var r in ResourceManagerService.Resources)
            {
                if (!String.IsNullOrEmpty(searchTerm) && !r.Name.ToLower().Contains(searchTerm))
                    continue;

                if (TypeFilter.HasValue && r.Type != TypeFilter.Value)
                    continue;

                GetContainingDir(root, r.ContentPath).Items.Add(new ResourceObjModel(r));

            }

        }

        private ResourceModel FindItem(String xnbPath, ResourceType? type = null)
        {
            if (String.IsNullOrEmpty(xnbPath))
                return null;

            var dir = GetContainingDir(Resources.First() as ResourceDirModel, Path.GetDirectoryName(xnbPath));

            foreach (var item in dir.Items)
            {
                if (Path.GetFileNameWithoutExtension(item.Name) != Path.GetFileName(xnbPath))
                    continue;

                if (type == ResourceType.Directory && item is ResourceDirModel)
                    return item;

                if (type.HasValue && (item as ResourceObjModel).Resource.Type == type.Value)
                    return item;

                return item;

            }
            return null;
        }

    }
}
