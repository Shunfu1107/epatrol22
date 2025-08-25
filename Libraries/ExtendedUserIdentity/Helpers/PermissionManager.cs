using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public class PermissionManager
    {
        private IList<IContent> missingPermissionContents { get; set; }
        private Dictionary<string, IContent> Contents { get; set; }
        private Dictionary<string, PermissionKeyValue> Roots { get; set; }

        internal PermissionManager()
        {
            missingPermissionContents = new List<IContent>(13);
            Contents = new Dictionary<string, IContent>(13);
            Roots = new Dictionary<string, PermissionKeyValue>(13);
        }

        internal void Initialize()
        {
            collectPermission();
            checkMissingPermission();
        }

        public IList<IContent> ToList()
        {
            return Contents.Select((m) =>
            {
                return ((ContentGenericAttribute)m.Value).ConvertToContentPermission();
            }).ToList();
        }
        public bool HasPermission(string key)
        {
            return Contents.Keys.Contains(key);
        }
        public IContent GetRootPermission(string key)
        {
            if (!Roots.ContainsKey(key)) return null;
            var root = Roots[key];
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            return root.Content;
        }
        public IList<IContent> GetChildContents(string key)
        {
            if (!Roots.ContainsKey(key)) return new List<IContent>();
            var root = Roots[key];
            return root.IContents;
        }
        public IContent GetContent(string key)
        {
            if (!Roots.ContainsKey(key)) return null;
            var root = Roots[key];
            return root.Content;
        }

        private void checkMissingPermission()
        {
            IList<IContent> temp = missingPermissionContents.Clone();
            missingPermissionContents.Clear();

            for (var i = 0; i < temp.Count; i++) setContent(temp[i]);
            assertMissingPermission();
        }
        private void collectPermission()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            loadPermissionFrom(assemblies);
        }
        private void loadPermissionFrom(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0) return;

            for (var i = 0; i < assemblies.Length; i++)
            {
                getContentAccess(assemblies[i]);
            }
        }
        private void getContentAccess(Assembly assembly)
        {
            if (assembly == null) return;
            var assemblyName = assembly.FullName;
            if (assemblyName.StartsWith("mscorlib") ||
                assemblyName.StartsWith("System.Web") ||
                assemblyName.StartsWith("System") ||
                assemblyName.StartsWith("Microsoft") ||
                assemblyName.StartsWith("CppCodeProvider")) return;

            Type[] types = assembly.GetTypes();

            for (var i = 0; i < types.Length; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var className = types[i].Name;
                for (var m = 0; m < methods.Length; m++)
                {
                    MethodInfo method = methods[m];
                    getContentPermission(className, method);
                }
            }
        }
        private void getContentPermission(string className, MethodInfo method)
        {
            object[] attrs = method.GetCustomAttributes(typeof(ContentPermissionAttribute), false);
            for (var a = 0; a < attrs.Length; a++)
            {
                ContentPermissionAttribute actionAttr = attrs[a] as ContentPermissionAttribute;
                if (actionAttr != null && method.DeclaringType.Name == className)
                {
                    if (!isAuthorizeUserAttrExist(method)) throw new Exception(String.Format("Permission Content Attribute required AuthorizeUser Attribute '{0}.{1}'.", className, method.Name));
                    if (string.IsNullOrWhiteSpace(actionAttr.Key)) throw new Exception(string.Format("Empty Permission Key of '{0}.{1}'", className, method.Name));
                    setContent(actionAttr);
                }
            }
        }

        private bool isAuthorizeUserAttrExist(MethodInfo method)
        {
            object[] attrs = method.GetCustomAttributes(typeof(ExtendedAuthorizeAttribute), false);
            for (var i = 0; i < attrs.Length; i++)
            {
                ExtendedAuthorizeAttribute role = attrs[i] as ExtendedAuthorizeAttribute;
                if (role != null) return true;
            }

            object[] attrs2 = method.DeclaringType.GetCustomAttributes(typeof(ExtendedAuthorizeAttribute), false);
            for (var i = 0; i < attrs2.Length; i++)
            {
                ExtendedAuthorizeAttribute role = attrs2[i] as ExtendedAuthorizeAttribute;
                if (role != null) return true;
            }

            return false;

        }
        private void setContent(IContent content)
        {
            if (string.IsNullOrWhiteSpace(content.AssociatedKey))
            {
                setInnerContent(content);
                //Roots.Add(content.Key, new PermissionKeyValue
                //{
                //    Contents = new List<PermissionKeyValue>(),
                //    Content = content,
                //    Key = content.Key
                //});
                setRootBasedValue(content);
            }
            else if (!Contents.ContainsKey(content.AssociatedKey))
            {
                missingPermissionContents.Add(content);
            }
            else
            {
                var parent = Contents[content.AssociatedKey];

                //DISABLE BY IVAN
                //if (!string.IsNullOrWhiteSpace(parent.AssociatedKey)) throw new Exception("Deep Level more then 1 is not allowed on current implementation.");

                setInnerContent(content);
                setRootBasedValue(content);
            }
        }
        private void setInnerContent(IContent content)
        {
            if (Contents.ContainsKey(content.Key))
                throw new Exception(String.Format("Duplicate Permission Key. Probably because you have used the same primary key on two or more for ContentPermission above Action.", content.Key));
            Contents.Add(content.Key, content);
        }
        private void assertMissingPermission()
        {
            if (missingPermissionContents.Count > 0)
            {
                StringBuilder list = new StringBuilder();
                for (var i = 0; i < missingPermissionContents.Count; i++)
                {
                    list.AppendLine(missingPermissionContents[i].Key);
                }

                throw new Exception("Associated Permission Key not found. Probably because you have forgotten to put the primary key for ContentPermission above Action.");
            }
        }
        private void setRootBasedValue(IContent content)
        {
            var child = new PermissionKeyValue
            {
                Key = content.Key,
                Content = content,
                Parent = null
            };

            if (!string.IsNullOrEmpty(content.AssociatedKey) && Roots.ContainsKey(content.AssociatedKey))
            {
                var root = Roots[content.AssociatedKey];
                child.Parent = root;
                root.Contents.Add(child);
                root.IContents.Add(content);

                var temproot = root;
                while (temproot.Parent != null)
                {
                    temproot = temproot.Parent;
                }

                if (!temproot.Key.Equals(root.Key))
                {
                    temproot.Contents.Add(child);
                    temproot.IContents.Add(content);
                }
            }

            if (!Roots.ContainsKey(content.Key))
            {
                Roots.Add(content.Key, child);
            }



            //foreach (var item in Roots.Values)
            //{
            //    if (item.Key.Equals(content.AssociatedKey))
            //    {
            //        item.Contents.Add(child);
            //        child.Parent = item;

            //        var root = item;
            //        while (!string.IsNullOrEmpty(root.Content.AssociatedKey))
            //        {
            //            root = root.Parent;
            //        }

            //        if (root.Key != item.Key) root.Contents.Add(child);
            //        break;
            //    }
            //}
        }

        [DebuggerDisplay("{Key}, Contents Count = {Contents.Count}")]
        internal class PermissionKeyValue
        {
            public PermissionKeyValue()
            {
                Contents = new List<PermissionKeyValue>(3);
                IContents = new List<IContent>(3);
            }
            public PermissionKeyValue Parent { get; set; }
            public string Key { get; set; }
            public IContent Content { get; set; }
            public IList<PermissionKeyValue> Contents { get; set; }
            public IList<IContent> IContents { get; set; }
        }
    }
}