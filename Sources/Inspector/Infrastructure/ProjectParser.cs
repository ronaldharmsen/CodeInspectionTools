﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using MSBuild = Microsoft.Build.Evaluation;

namespace Inspector.Infrastructure
{
    public class ProjectParser : IProjectParser
    {
        private const string compiled = "Compile";
        private const string autoGenerated = "AutoGen";
        private readonly IFileSystemAdapter fileSystem;

        public ProjectParser(IFileSystemAdapter fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ProjectParser() : this(new FileSystemAdapter())
        {

        }
        public IEnumerable<SourceFile> GetSourceFiles(Project project)
        {
            var pc = MSBuild.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(p => p.FullPath == project.Path);
            MSBuild.Project msBuildProject = pc ?? new MSBuild.Project(project.Path);

            return msBuildProject.Items
                .Where(i => i.ItemType == compiled && !i.HasMetadata(autoGenerated))
                .Select(i => i.EvaluatedInclude)
                    .Select(srcFile =>
                    {
                        var absPath = Path.Combine(Path.GetDirectoryName(msBuildProject.FullPath), srcFile);
                        var code = fileSystem.ReadAllTextFromFile(absPath);
                        var source = new SourceFile(project, srcFile, code);
                        return source;
                    });
        }
    }
}