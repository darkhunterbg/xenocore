using Microsoft.Xna.Framework.Graphics;
using MonoGame.Tools.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.ContentPipeline
{
    public class ContentPipelineProject
    {
        public PipelineProject Project { get; private set; }

        private static readonly string[] _mgcbSearchPaths = new[]
        {
            @"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools",
            "../MGCB",
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../MGCB"),
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "",
        };

        public String OutputDir
        {
            get { return Project.OutputDir; }
            set
            {
                Project.OutputDir = value;
            }
        }
        public String Location
        {
            get { return Project.Location; }
        }
        public IEnumerable<ContentItem> ContentItems
        {
            get { return Project.ContentItems; }
        }

        public String FilePath
        {
            get { return Project.OriginalPath; }

        }


        public static String MGCBPath
        {
            get
            {
                foreach (var root in _mgcbSearchPaths)
                {
                    var mgcbPath = Path.Combine(root, "MGCB.exe");
                    if (File.Exists(mgcbPath))
                        return Path.GetFullPath(mgcbPath);
                }

                throw new FileNotFoundException("MGCB.exe is not in the search path!");
            }
        }
        private static String GetRelativePath(String from, String to)
        {
            Uri path1 = new Uri(from);
            Uri path2 = new Uri(to);
            Uri diff = path1.MakeRelativeUri(path2);
            return diff.OriginalString.Replace("/", @"\");
        }

        private ContentPipelineProject() { }
        public ContentPipelineProject(String file, String outputTo = null)
        {
            var project = new PipelineProject();
            project.OutputDir = outputTo ?? "bin/$(Platform)";
            project.IntermediateDir = "obj/$(Platform)";
            project.Platform = Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform.DesktopGL;
            project.Profile = GraphicsProfile.HiDef;
            project.Compress = false;
            project.OriginalPath = file;

            UriBuilder uri = new UriBuilder(typeof(ContentPipelineProject).Assembly.CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            project.References.Add(path);

            if (!Directory.Exists(project.Location))
                Directory.CreateDirectory(project.Location);


            PipelineTypes.Load(project);

            var parser = new PipelineProjectParser(null, project);
            AddNewSpriteFont(parser, project, "default");

            if (File.Exists(file))
                File.Delete(file);

            using (StreamWriter writer = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate)))
            {
                parser.SaveProject(writer, null);
            }

        }

        public static ContentPipelineProject Open(String file)
        {
            var result = new ContentPipelineProject();
            var project = new PipelineProject();
            result.Project = project;
            var parser = new PipelineProjectParser(null, project);

            parser.OpenProject(file, (msg, arg) =>
            {
                Debug.WriteLine("Open failed: " + msg);
            });

            PipelineTypes.Load(project);
            foreach (var item in project.ContentItems)
                item.ResolveTypes();

            return result;
        }
        public void Save()
        {
            new PipelineProjectParser(null, Project).SaveProject();
        }

        public void Build(bool rebuild = false)
        {
            var commands = string.Format("/@:\"{0}\" {1}", Project.OriginalPath, rebuild ? "/rebuild" : string.Empty);
            // if (LaunchDebugger)
            //  commands += " /launchdebugger";
            DoBuild(Project, commands);

        }
        public ContentItem AddNewItem(String absolutePath)
        {
            ContentItem item = null;
            var parser = new PipelineProjectParser(null, Project);

            if (parser.AddContent(absolutePath, true))
            {
                item = Project.ContentItems.Last();
                // item.ImporterName = tempalte.ImporterName;
                // item.ProcessorName = tempalte.ProcessorName;
                item.ResolveTypes();
            }
            return item;
        }
        public ContentItem AddNewSpriteFont(String fontName)
        {
            var parser = new PipelineProjectParser(null, Project);
            var item = AddNewSpriteFont(parser, Project, fontName);
            Save();
            return item;
        }
        public void RemoveItem(ContentItem item)
        {
            Project.ContentItems.Remove(item);
        }

        private static ContentItem AddNewSpriteFont(PipelineProjectParser parser, PipelineProject project, String fontName)
        {
            var mgcbDir = Path.GetDirectoryName(MGCBPath);
            var resourceFile = Path.Combine(mgcbDir, "Templates", "SpriteFont.template");

            var lines = File.ReadAllText(resourceFile).Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var tempalte = new ContentItemTemplate()
            {
                Label = lines[0],
                Icon = lines[1],
                ImporterName = lines[2],
                ProcessorName = lines[3],
                TemplateFile = lines[4],
            };

            resourceFile = Path.Combine(mgcbDir, "Templates", "SpriteFont.spritefont");

            var f = Path.Combine(project.Location, $"{fontName}.spritefont");

            if (File.Exists(f))
                File.Delete(f);

            File.Copy(resourceFile, f);

            ContentItem item = null;
            if (parser.AddContent(f, true))
            {
                item = project.ContentItems.Last();
                item.ImporterName = tempalte.ImporterName;
                item.ProcessorName = tempalte.ProcessorName;
                item.ResolveTypes();
            }
            return item;
        }

        private static void DoBuild(PipelineProject project, string commands)
        {
            try
            {
                // Prepare the process.
                var _buildProcess = new Process();
                _buildProcess.StartInfo.FileName = MGCBPath;
                _buildProcess.StartInfo.Arguments = commands;
                _buildProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(project.OriginalPath);
                _buildProcess.StartInfo.CreateNoWindow = true;
                _buildProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _buildProcess.StartInfo.UseShellExecute = false;
                _buildProcess.StartInfo.RedirectStandardOutput = true;
                _buildProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
                _buildProcess.OutputDataReceived += (sender, args) => Debug.WriteLine(args.Data);

                Debug.WriteLine($"{ _buildProcess.StartInfo.FileName} {_buildProcess.StartInfo.Arguments}");

                // Fire off the process.
                _buildProcess.Start();
                _buildProcess.BeginOutputReadLine();
                _buildProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                // If we got a message assume it has everything the user needs to know.
                if (!string.IsNullOrEmpty(ex.Message))
                    Debug.WriteLine("Build failed:  " + ex.Message);
                else
                {
                    // Else we need to get verbose.
                    Debug.WriteLine("Build failed:" + Environment.NewLine);
                    Debug.WriteLine(ex.ToString());
                }
            }

            // Clear the process pointer, so that cancel
            // can run after we've already finished.
            //lock (_buildTask)
            //    _buildProcess = null;
        }
    }
}
