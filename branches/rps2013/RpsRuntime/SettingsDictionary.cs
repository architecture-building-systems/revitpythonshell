using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RevitPythonShell.RpsRuntime
{
    /// <summary>
    /// A subclass of Dictionary<string, string>, that writes changes back to a settings xml file.
    /// </summary>
    public class SettingsDictionary : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _dict;
        private readonly string _settingsPath;
        private XDocument _settings;

        public SettingsDictionary(string settingsPath)
        {
            _settingsPath = settingsPath;
            _settings = XDocument.Load(_settingsPath);

            _dict = _settings.Root.Descendants("StringVariable").ToDictionary(
                v => v.Attribute("name").Value,
                v => v.Attribute("value").Value);
        }

        private void SetVariable(string name, string value)
        {
            var variable = _settings.Root.Descendants("StringVariable").Where(x => x.Attribute("name").Value == name).FirstOrDefault();
            if (variable != null)
            {
                variable.Attribute("value").Value = value.ToString();
            }
            else
            {
                _settings.Root.Descendants("Variables").First().Add(
                    new XElement("StringVariable", new XAttribute("name", name), new XAttribute("value", value)));
            }
            _settings.Save(_settingsPath);
        }

        private void RemoveVariable(string name)
        {
            var variable = _settings.Root.Descendants("StringVariable").Where(x => x.Attribute("name").Value == name).FirstOrDefault();
            if (variable != null)
            {
                variable.Remove();
                _settings.Save(_settingsPath);
            }
        }

        private void ClearVariables()
        {
            var variables = _settings.Root.Descendants("StringVariable");
            foreach (var variable in variables)
            {
                variable.Remove();
            }
            _settings.Save(_settingsPath);
        }

        void IDictionary<string, string>.Add(string key, string value)
        {
            _dict.Add(key, value);
            SetVariable(key, value);
        }

        bool IDictionary<string, string>.ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        ICollection<string> IDictionary<string, string>.Keys
        {
            get { return _dict.Keys; }
        }

        bool IDictionary<string, string>.Remove(string key)
        {
            RemoveVariable(key);
            return _dict.Remove(key);            
        }

        bool IDictionary<string, string>.TryGetValue(string key, out string value)
        {
            return _dict.TryGetValue(key, out value);
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get { return _dict.Values; }
        }

        string IDictionary<string, string>.this[string key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
                SetVariable(key, value);
            }
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            _dict.Add(item);
            SetVariable(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, string>>.Clear()
        {
            ClearVariables();
            _dict.Clear();            
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return _dict.Contains(item);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _dict.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, string>>.Count
        {
            get { return _dict.Count; }
        }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            RemoveVariable(item.Key);
            return _dict.Remove(item);
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }
    } 
}
