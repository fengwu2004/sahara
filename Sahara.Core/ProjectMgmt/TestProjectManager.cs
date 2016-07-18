using Sahara.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sahara.Core
{
    public sealed class TestProjectManager
    {
        private XmlSerializer _serializer;

        public TestProjectManager()
        {
            _serializer = new XmlSerializer(typeof(TestProject));
        }

        /// <summary>
        /// The current test project instance
        /// </summary>
        public TestProject Current { get; set; }

        /// <summary>
        /// Load project information from the specified file system location
        /// </summary>
        /// <param name="path">The place where the project XML file locates</param>
        /// <returns>The deserialized test project object</returns>
        public TestProject LoadFromXml(string path)
        {
            if (!File.Exists(path)) return null;

            TestProject project;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                project = (TestProject)this._serializer.Deserialize(stream);
                project.ProjectConfigPath = path;
            }
            return project;
        }

        /// <summary>
        /// Serialize a test project object to an XML file. If the object's path was not set, 
        /// it will create a file using the project's name.
        /// </summary>
        /// <param name="project">The test project object to save</param>
        public void Save(TestProject project)
        {
            var testProjectDir = Path.GetDirectoryName(project.ProjectConfigPath);

            using (TextWriter writer = new StreamWriter(project.ProjectConfigPath))
            {
                var testSuiteCopyDir = Path.Combine(testProjectDir, "TestSuite");
                if (!Directory.Exists(testSuiteCopyDir))
                {
                    Directory.CreateDirectory(testSuiteCopyDir);
                    // Copies the existing test suite directory when selected
                    if (!string.IsNullOrEmpty(project.TestSuitePath))
                    {
                        IoUtilities.DirectoryCopy(project.TestSuitePath, testSuiteCopyDir, true);

                        var srcLib = Path.Combine(testSuiteCopyDir, "Lib");
                        var srcDll = Path.Combine(testSuiteCopyDir, "Dll");
                        var destLib = Path.Combine(testProjectDir, "Lib");
                        var destDll = Path.Combine(testProjectDir, "Dll");

                        if (Directory.Exists(srcLib))
                        {
                            Directory.Move(srcLib, destLib);
                        }

                        if (Directory.Exists(srcDll))
                        {
                            Directory.Move(srcDll, destDll);
                        }

                        project.Paths.Add(destLib);
                        project.Paths.Add(destDll);
                    }
                }

                var testResultsDir = Path.Combine(testProjectDir, "TestResults");
                if (!Directory.Exists(testResultsDir))
                {
                    Directory.CreateDirectory(testResultsDir);
                }

                if (project.CreatedOn == DateTime.MinValue)
                {
                    project.CreatedOn = DateTime.Now;
                }
                project.LastModifiedOn = DateTime.Now;

                this._serializer.Serialize(writer, project);
            }
        }
    }
}
