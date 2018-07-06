using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Resources;
using MonoGame.Tools.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using XenoCore.ContentPipeline;
using XenoCore.Builder.Data;
using XenoCore.ContentPipeline.Content;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using Newtonsoft.Json;
using XenoCore.Engine;

//https://github.com/MonoGame/MonoGame/blob/develop/Tools/Pipeline/Common/PipelineController.cs

namespace XenoCore.Builder.Services
{
    public interface IResourceEventReciever
    {
        void ItemAdded(Resource item);
        void ItemRemoved(Resource item);
        void ItemModified(String oldContentPath, Resource item);
    }


    public static class ResourceManagerService
    {
        private static List<ResourceObj> resources = new List<ResourceObj>();
        private static List<ResourceDir> directories = new List<ResourceDir>();

        private static String contentAbsolutePath;
        private static String contentXnbPath;

        private static List<IResourceEventReciever> eventRecievers = new List<IResourceEventReciever>();

        private static ContentPipelineProject project;

        public static IEnumerable<ResourceObj> Resources
        {
            get { return resources; }
        }
        public static IEnumerable<ResourceDir> Directories
        {
            get { return directories; }
        }
        public static String ContentLocation
        {
            get { return contentAbsolutePath; }
        }

        private static ResourceType TypeFromProcessor(string processorName)
        {
            if (processorName == "FontDescriptionProcessor")
                return ResourceType.SpriteFont;

            if (processorName == "TextureProcessor")
                return ResourceType.Texture;

            if (processorName == typeof(ParticleEffectProcessor).Name)
                return ResourceType.ParticleEffect;

            return ResourceType.Unknown;
        }

        public static void UnloadResources()
        {
            directories.Clear();
            resources.Clear();
        }

        private static void LoadDirectories(String absolutePath)
        {
            var dirs = Directory.GetDirectories(absolutePath).ToList();

            var remove = new String[]
            {
            Path.Combine(contentAbsolutePath, "obj"),
            Path.Combine(contentAbsolutePath, "bin"),
            };

            var removeItem = dirs.FirstOrDefault(p => remove.Contains(p));
            if (removeItem != null)
                dirs.Remove(removeItem);

            foreach (var dir in dirs)
            {
                AddDir(dir);
            }

            foreach (var dir in dirs)
            {
                LoadDirectories(dir);
            }
        }

        public static void UsePipelineProject(ContentPipelineProject p)
        {
            project = p;

            contentAbsolutePath = project.Location.Replace('/', '\\');
            contentXnbPath = project.OutputDir.Replace('/', '\\');

            LoadDirectories(contentAbsolutePath);

            foreach (var item in project.ContentItems)
            {
                AddResource(item);
            }
        }

        public static void SubscribeForEvents(IResourceEventReciever reciever)
        {
            Debug.Assert(!eventRecievers.Contains(reciever), "Reciever already added!");
            eventRecievers.Add(reciever);
        }
        public static void UnsubscribeForEvents(IResourceEventReciever reciever)
        {
            eventRecievers.Remove(reciever);
        }

        public static void ChangeOutputDir(String outDir)
        {
            if (outDir == project.OutputDir)
                return;

            if (Directory.Exists(outDir))
            {
                foreach (var dir in Directory.GetDirectories(project.OutputDir))
                    Directory.Delete(dir, true);

                foreach (var file in Directory.GetFiles(project.OutputDir))
                    File.Delete(file);
            }

            project.OutputDir = outDir;
            project.Save();
            project.Build();
        }

        public static void ReloadResource(ResourceObj resource)
        {
            switch (resource.Type)
            {
                case ResourceType.SpriteFont:
                    {
                        ResourcesService.Reload<SpriteFont>(resource.XnbPath);
                    }
                    break;
                case ResourceType.ParticleEffect:
                    {
                        ResourcesService.Reload<ParticleEffectDescription>(resource.XnbPath);
                    }
                    break;
                case ResourceType.Texture:
                    {
                        ResourcesService.Reload<Texture2D>(resource.XnbPath);
                    }
                    break;
                default:
                    throw new Exception("Unknown resource type to reload!");
            }

            foreach (var reciever in eventRecievers)
                reciever.ItemModified(resource.ContentPath, resource);
        }

        public static void Rename(Resource resource, String newName)
        {
            var oldContentPath = resource.ContentPath;
            var oldAbsolutePath = resource.AbsolutePath;
            var oldXnbPath = resource.XnbPath;

            resource.ContentPath = Path.Combine(Path.GetDirectoryName(resource.ContentPath), newName);
            resource.AbsolutePath = Path.Combine(contentAbsolutePath, newName);
            resource.XnbPath = Path.Combine(Path.GetDirectoryName(resource.XnbPath), Path.GetFileNameWithoutExtension(newName));
            resource.Name = newName;

            if (resource.Type == ResourceType.Directory)
            {
                if (Directory.GetFiles(oldAbsolutePath).Any())
                    throw new NotImplementedException();

                Directory.Move(oldAbsolutePath, resource.AbsolutePath);

                //foreach (var r in resources.
                //    Where(p => Path.GetDirectoryName(p.ContentPath) == resource.ContentPath).ToList())
                //{
                //    DeleteResource(r, false);
                //}

                //foreach (var dir in directories.
                //   Where(p => Path.GetDirectoryName(p.ContentPath) == resource.ContentPath).ToList())
                //{
                //    DeleteDirectory(dir, false);
                //}
            }
            else
            {
                var r = resource as ResourceObj;
                File.Move(oldAbsolutePath, resource.AbsolutePath);
                r.PipelineItem.OriginalPath = resource.ContentPath;

                switch (r.Type)
                {
                    case ResourceType.SpriteFont:
                        GraphicsService.Cache.RemoveFont(oldXnbPath);
                        ResourcesService.Unload(oldXnbPath);
                        break;
                    case ResourceType.Texture:
                        GraphicsService.Cache.RemoveTexture(oldXnbPath);
                        ResourcesService.Unload(oldXnbPath);
                        break;
                }

                project.Save();
                project.Build();

                LoadResource(r);
            }


            foreach (var reciever in eventRecievers)
                reciever.ItemModified(oldContentPath, resource);
        }

        public static ResourceDir CreateDirectory(String dirPath)
        {
            var fullpath = Path.Combine(contentAbsolutePath, dirPath);
            Directory.CreateDirectory(fullpath);

            var dir = AddDir(fullpath);


            foreach (var reciever in eventRecievers)
                reciever.ItemAdded(dir);

            return dir;
        }

        public static ResourceObj AddResource(String absoluteResourcePath, bool save = true)
        {
            var contentItem = project.AddNewItem(absoluteResourcePath);
            if (contentItem == null)
                return null;

            if (save)
            {
                project.Save();
            }

            project.Build();

            var item = AddResource(contentItem);


            foreach (var reciever in eventRecievers)
                reciever.ItemAdded(item);


            return item;
        }
        public static ResourceObj CreateResource(String resourcePath, ResourceType resourceType)
        {
            ContentItem pipelineItem = null;
            Object serializeObject = null;
            var name = Path.GetFileName(resourcePath);
            String extension = null;

            switch (resourceType)
            {
                case ResourceType.SpriteFont:
                    pipelineItem = project.AddNewSpriteFont(resourcePath);
                    break;
                case ResourceType.ParticleEffect:
                    serializeObject = new ParticleEffectDescription() { Name = name };
                    extension = ".pe";
                    break;
                default:
                    throw new Exception($"Invalid new resource type {resourceType}!");
            }

            if (serializeObject != null)
            {
                var absolutePath = Path.Combine(contentAbsolutePath, resourcePath) + extension;
                var json = XenoCoreJson.Serialize(serializeObject);
                File.WriteAllText(absolutePath, json);
                pipelineItem = project.AddNewItem(absolutePath);

            }

            project.Save();
            project.Build();

            var item = AddResource(pipelineItem);


            foreach (var reciever in eventRecievers)
                reciever.ItemAdded(item);


            return item;
        }
        public static void DeleteDirectory(ResourceDir dir, bool save = true)
        {
            foreach (var resource in resources.
                Where(p => Path.GetDirectoryName(p.ContentPath) == dir.ContentPath).ToList())
            {
                DeleteResource(resource, false);
            }

            foreach (var directory in directories.
               Where(p => Path.GetDirectoryName(p.ContentPath) == dir.ContentPath).ToList())
            {
                DeleteDirectory(directory, false);
            }

            if (Directory.Exists(dir.AbsolutePath))
                Directory.Delete(dir.AbsolutePath);

            var xnb = Path.Combine(contentXnbPath, dir.XnbPath);

            if (Directory.Exists(xnb))
                Directory.Delete(xnb, true);

            if (save)
                project.Save();

            foreach (var reciever in eventRecievers)
                reciever.ItemRemoved(dir);

        }
        public static void DeleteResource(ResourceObj resource, bool save = true)
        {
            resources.Remove(resource);

            project.RemoveItem(resource.PipelineItem);
            resource.PipelineItem = null;

            if (File.Exists(resource.AbsolutePath))
                File.Delete(resource.AbsolutePath);

            if (save)
                project.Save();

            foreach (var reciever in eventRecievers)
                reciever.ItemRemoved(resource);
        }

        public static void SaveResource(ResourceObj resource)
        {
            switch(resource.Type)
            {
                case ResourceType.SpriteFont:
                    {
                        var descr = resource.Instance as SpriteFontDescription;
                        descr.Save();
                 
                        //descr.ContentFont = ResourcesService.LoadSpriteFont(resource.XnbPath);
                    }
                    break;
                default:
                    {
                        var json = XenoCoreJson.Serialize(resource.Instance);
                        File.WriteAllText(resource.AbsolutePath, json);
                    }
                    break;
            }

            project.Build();

            ReloadResource(resource);

        }

        private static ResourceObj AddResource(ContentItem item)
        {
            var normalizedPath = item.OriginalPath.Replace('/', '\\');

            var resource = new ResourceObj()
            {
                PipelineItem = item,
                Name = item.Name,
                AbsolutePath = Path.Combine(contentAbsolutePath, normalizedPath),
                XnbPath = Path.Combine(Path.GetDirectoryName(normalizedPath), Path.GetFileNameWithoutExtension(item.Name)),// + ".xnb",
                ContentPath = normalizedPath,
                Type = TypeFromProcessor(item.ProcessorName),
            };
            resources.Add(resource);
            LoadResource(resource);

            return resource;
        }
        private static ResourceDir AddDir(String dir)
        {
            var xnb = dir.Substring(contentAbsolutePath.Length + 1);
            var item = new ResourceDir()
            {
                Name = Path.GetFileName(dir),
                AbsolutePath = dir,
                ContentPath = xnb,
                XnbPath = xnb,
                Type = ResourceType.Directory,
            };
            directories.Add(item);

            return item;
        }
        private static void LoadResource(ResourceObj resource)
        {
            switch (resource.Type)
            {
                case ResourceType.Texture:
                    resource.Instance = ResourcesService.LoadTexture(resource.XnbPath);
                    break;
                case ResourceType.SpriteFont:
                    resource.Instance = new SpriteFontDescription(resource.AbsolutePath, resource.XnbPath)
                    {
                        ContentFont = ResourcesService.LoadSpriteFont(resource.XnbPath),
                    };

                    break;
                case ResourceType.ParticleEffect:
                    resource.Instance = ResourcesService.LoadParticleEffect(resource.XnbPath);
                    break;
            }
        }
    }
}
