using Mustache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Utils
{
    public class TemplateException : Exception
    {
        public TemplateException(string message, Exception ex) : base(message, ex) { }
    }

    public class TemplateEngine
    {
        private IDictionary<Type, string> _registry;
        private FormatCompiler _compiler;

        public TemplateEngine()
        {
            this._registry = new Dictionary<Type, string>();
            this._compiler = new FormatCompiler();
        }

        public string Render(Type type, object obj)
        {
            var tmpl = this._registry[type];
            if (!string.IsNullOrEmpty(tmpl))
            {
                try
                {
                    var generator = _compiler.Compile(tmpl);
                    return generator.Render(obj);
                }
                catch (Exception ex)
                {
                    throw new TemplateException("Failed to render template", ex);
                }
            }
            return null;
        }

        public string Render(object obj)
        {
            return Render(obj.GetType(), obj);
        }

        public string Render<T>(T obj)
        {
            return Render(typeof(T), obj);
        }

        public string Render(object obj, string template)
        {
            try
            {
                var generator = _compiler.Compile(template);
                return generator.Render(obj);
            }
            catch (Exception ex)
            {
                throw new TemplateException("Failed to render template", ex);
            }
        }

        public void Register(Type type, string template)
        {
            if (this._registry.ContainsKey(type))
            {
                this._registry[type] = template;
            }
            else
            {
                this._registry.Add(type, template);
            }
        }

        public void Register<T>(string template)
        {
            this.Register(typeof(T), template);
        }

        public void RegisterTemplateFile(Type type, string path)
        {
            if (!File.Exists(path)) return;
            var tmpl = File.ReadAllText(path);
            this.Register(type, tmpl);
        }

        public void RegisterTemplateFile<T>(string path)
        {
            this.RegisterTemplateFile(typeof(T), path);
        }
    }
}
