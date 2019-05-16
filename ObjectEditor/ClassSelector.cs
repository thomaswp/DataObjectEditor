using ObjectEditor.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ObjectEditor.Editor
{
    public partial class ClassSelector : Form
    {
        private Dictionary<string, List<Type>> categoryMap = new Dictionary<string, List<Type>>();

        private Type[] types;
        public IEnumerable<Type> Types 
        { 
            get
            {
                return types;
            }

            set
            {
                listBoxCategory.Items.Clear();
                listBoxType.Items.Clear();
                categoryMap.Clear();
                if (value == null)
                {
                    types = null;
                    return;
                }
                types = value.ToArray();

                categoryMap.Add("All", types.ToList());
                foreach (Type t in types)
                {
                    Category category = Attribute.GetCustomAttribute(t, typeof(Category)) as Category;
                    if (category != null && category.categories != null)
                    {
                        foreach (string name in category.categories)
                        {

                            if (!categoryMap.ContainsKey(name)) categoryMap.Add(name, new List<Type>());
                            categoryMap[name].Add(t);
                        }
                    }
                }

                listBoxCategory.Items.AddRange(categoryMap.Keys.ToArray());
                listBoxCategory.SelectedIndex = 0;
            }
        }

        private void PopulateTypes(Type[] types)
        {
            listBoxType.Items.Clear();
            listBoxType.Items.AddRange(types.Select(
                type => DataObjectEditor.GetHumanReadableField(type.Name))
                .ToArray());
        }

        public Type SelectedType
        {
            get
            {
                if (types == null) return null;
                int index = listBoxType.SelectedIndex;
                string category = listBoxCategory.SelectedItem as string;
                if (category == null) return null;
                return index < 0 ? null : categoryMap[category][index];
            }
        }

        public ClassSelector()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string category = listBoxCategory.SelectedItem as string;
            if (category != null) PopulateTypes(categoryMap[category].ToArray());
        }

        private void listBoxType_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
