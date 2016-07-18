using Autofac;
using Sahara.Core.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sahara.Core
{
    [Serializable]
    [XmlRoot(ElementName = "Project", IsNullable = false)]
    public class TestProject
    {
        private ITestSuiteReader _testSuiteReader;
        private XmlSerializer _serializer;

        public TestProject()
        {
            this._testSuiteReader = Container.Current.Resolve<ITestSuiteReader>();
            this._serializer = new XmlSerializer(typeof(List<Variable>), new XmlRootAttribute("Variables"));
            this._resultTemplate = File.ReadAllText("Data/log-template.txt");

            this.ProjectVariables = new VariableList();
            this.TestLabAddress = new Address();
            this.CustomerAddress = new Address();
            this.Paths = new List<string>();
        }

        /// <summary>
        /// The name of this test project. It will be used as the default 
        /// file name when serialize to the file system.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Test project description
        /// </summary>
        public string Description { get; set; }

        public string Tester { get; set; }

        public string Device { get; set; }

        public string Version { get; set; }

        public List<string> Paths { get; set; }

        /// <summary>
        /// A list of variables that are specific to this test project
        /// </summary>
        public VariableList ProjectVariables { get; set; }

        public string TestSuitePath { get; set; }

        public Address TestLabAddress { get; set; }

        public Address CustomerAddress { get; set; }

        public string Comments { get; set; }

        /// <summary>
        /// The file system location where the test project is persisted to
        /// </summary>
        [XmlIgnore]
        public string ProjectConfigPath { get; set; }

        /// <summary>
        /// The project root directory
        /// </summary>
        [XmlIgnore]
        public string ProjectRoot
        {
            get
            {
                if (string.IsNullOrEmpty(ProjectConfigPath)) return string.Empty;
                return Path.GetDirectoryName(ProjectConfigPath);
            }
        }

        [XmlIgnore]
        public ITestSuiteNode TestSuite
        {
            get
            {
                var testSuitePath = Path.Combine(ProjectRoot, "TestSuite");
                this._testSuiteReader.TestSuiteRoot = testSuitePath;
                return _testSuiteReader.Read();
            }
        }

        private string _resultTemplate;
        [XmlIgnore]
        public string ResultTemplate
        {
            get
            {
                return _resultTemplate.Replace("{{#newline}}", "\r\n");
            }
            set
            {
                _resultTemplate = System.Text.RegularExpressions.Regex.Replace(value, @"\r\n|\r|\n", "{{#newline}}");
            }
        }

        [XmlIgnore]
        public string RawResultTemplate
        {
            get { return _resultTemplate; }
        }

        [XmlIgnore]
        public string ResultStatePath
        {
            get
            {
                return Path.Combine(ProjectRoot, "result-states.bin");
            }
        }

        [XmlElement("ResultTemplate")]
        public XmlCDataSection ResultTemplateData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(ResultTemplate);
            }
            set
            {
                ResultTemplate = value.Value;
            }
        }

        [XmlAttribute]
        public DateTime CreatedOn { get; set; }

        [XmlAttribute]
        public DateTime LastModifiedOn { get; set; }

        public void ImportVariables(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var variables = (List<Variable>)_serializer.Deserialize(stream);
                ProjectVariables.Variables = variables;
            }
        }

        public void ExportVariables(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                _serializer.Serialize(writer, ProjectVariables.Variables);
            }
        }
    }
}
