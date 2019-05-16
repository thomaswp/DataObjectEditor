using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Emigre.Data;
using Emigre.Editor.Reflect;
using Emigre.Editor.Field;
using System.Collections;
using Emigre.Json;
using System.Runtime.InteropServices;
using System.IO;

namespace Emigre.Editor
{
    public partial class DataObjectEditor : UserControl, Lookup
    {

        #region Properties

        public static bool AutoCollapse { get; set; }

        private Json.DataObject _dataObject;
        public Json.DataObject DataObject 
        {
            get { return _dataObject; }
            set
            {
                if (_dataObject == value) return;
                UnloadObject();
                _dataObject = value;
                LoadObject();
            }
        }

        public EditorIndex Index
        {
            get
            {
                EditorIndex index = new EditorIndex();
                index.listIndex = comboBoxList.SelectedIndex;
                index.itemIndex = listBoxList.SelectedIndex;
                if (nextEditor != null && nextEditor.Parent != null)
                {
                    index.nextIndex = nextEditor.Index;
                }
                return index;
            }
            set
            {
                if (comboBoxList.SelectedIndex != value.listIndex)
                {
                    comboBoxList.SelectedIndex = value.listIndex;
                }
                if (listBoxList.SelectedIndex != value.itemIndex)
                {
                    listBoxList.SelectedIndices.Clear();
                    listBoxList.SelectedIndex = value.itemIndex;
                }
                if (nextEditor != null && nextEditor.Parent != null && value.nextIndex != null)
                {
                    nextEditor.Index = value.nextIndex;
                }
                if (value.nextIndex == null)
                {
                    Collapsed = false;
                }
            }
        }

        private Control _holder;
        public Control Holder
        {
            get
            {
                return _holder;
            }
            set
            {
                if (_holder != null) _holder.Resize -= HolderResized;
                _holder = value;
                _holder.Resize += HolderResized;
                HolderResized(_holder, new EventArgs());
            }
        }

        private bool _collapsed;
        private bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                _collapsed = value;
                this.buttonCollapse.Text = value ? "Open" : "Collapse";
                float t = 1;
                int targetWidth = Collapsed ? 190 : 475;
                groupBox.Width = (int)(targetWidth * t + groupBox.Width * (1 - t));
                if (nextEditor != null) nextEditor.Location = new Point(groupBox.Width + 5, 0);
            }
        }

        private IList CurrentList
        {
            get
            {
                if (comboBoxList.SelectedIndex < 0) return null;
                return lists[comboBoxList.SelectedIndex];
            }
        }

        private Json.DataObject CurrentItem
        {
            get
            {
                IList list = CurrentList;
                if (list == null || listBoxList.SelectedIndex < 0) return null;
                return (Json.DataObject) list[listBoxList.SelectedIndex];
            }
        }
        #endregion

        public event EventHandler<EditAction> OnObjectEdited;

        private DataObjectEditor parent;

        private List<IList> lists = new List<IList>();
        private List<Accessor> inlineAccessors = new List<Accessor>();
        private DataObjectEditor nextEditor;
        private ClassSelector classSelector;
        private List<FieldEditor> editors = new List<FieldEditor>();

        static DataObjectEditor()
        {
            AutoCollapse = true;
        }

        public DataObjectEditor() : this(null) { }

        public DataObjectEditor(DataObjectEditor parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.tableLayoutPanelFields.HorizontalScroll.Enabled = false;
            this.tableLayoutPanelFields.HorizontalScroll.Visible = false;
            this.classSelector = new ClassSelector();
            this.Collapsed = false;

            OnObjectEdited += RefreshEditors;
        }

        private void HolderResized(object sender, EventArgs e)
        {
            MinimumSize = new Size(MinimumSize.Width, Holder.Height - Holder.Padding.Top - Holder.Padding.Bottom);
            Height = MinimumSize.Height;
        }

        #region Load/Unload Object
        private void UnloadObject()
        {
            //if (nextEditor != null) nextEditor.UnloadObject();
            this.tableLayoutPanelFields.Controls.Clear();
            this.groupBox.Text = "";
            this.listBoxList.Items.Clear();
            this.comboBoxList.Items.Clear();
            editors.Clear();
            lists.Clear();
            inlineAccessors.Clear();
            if (nextEditor != null) Controls.Remove(nextEditor);
            PopulateList();
        }

        private void LoadObject()
        {
            if (DataObject == null)
            {
                if (nextEditor != null) nextEditor.DataObject = null;
                return;
            }

            this.groupBox.Text = GetHumanReadableField(DataObject.GetType().Name);

            AddRows(DataObject, this.tableLayoutPanelFields);
            if (lists.Count > 0)
            {
                tableLayoutPanelObject.ColumnStyles[1].Width = 175;
                comboBoxList.SelectedIndex = 0;
            }
            else
            {
                tableLayoutPanelObject.ColumnStyles[1].Width = 0;
                Collapsed = false;
                PopulateList();
            }

            Width = 500;
        }

        private void AddRows(Json.DataObject obj, TableLayoutPanel tableLayout, string listPrefix = "")
        {
            tableLayout.RowCount = 0;
            List<Accessor> accessors = FieldAccessor.GetForObject(obj).ToList();
            accessors.AddRange(PropertyAccessor.GetForObject(obj));
            foreach (Accessor accessor in accessors)
            {
                IList list = accessor.Get() as IList;
                if (list != null)
                {
                    Type listOf = GetFirstGenericType(list);
                    if (!typeof(Emigre.Json.DataObject).IsAssignableFrom(listOf)) continue;

                    comboBoxList.Items.Add(listPrefix + GetHumanReadableField(accessor.GetName()));
                    lists.Add(list);
                }
            }
            foreach (Accessor accessor in accessors)
            {
                if (!IsGenericList(accessor.GetAccessorType()))
                {
                    AddRow(accessor, tableLayout);
                }
            }
        }

        private void AddRow(Accessor accessor, TableLayoutPanel tableLayout)
        {
            if (accessor.GetTags().Any(x => x.flag == FieldTags.Inline))
            {
                AddInlineRow(accessor, tableLayout);
                return;
            }

            FieldEditor editor = FieldEditor.CreateEditor(accessor, this);
            if (editor == null) return;

            tableLayout.RowCount++;
            Label label = new Label();
            label.AutoSize = true;
            label.Text = GetHumanReadableField(accessor.GetName());
            FieldTag comment = accessor.GetTags().Where(tag => tag.flag == FieldTags.Comment).FirstOrDefault();
            if (comment != null) 
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(label, comment.arg);
            }
            tableLayout.Controls.Add(label, 0, tableLayout.RowCount - 1);

            Control content = editor.GetControl();
            content.Enabled = !editor.accessor.GetTags().Any((tag) => tag.flag == FieldTags.Readonly);
            tableLayout.Controls.Add(content, 1, tableLayout.RowCount - 1);
            int fieldIndex = editors.Count;
            editors.Add(editor);
            editor.OnEdited += (sender, args) =>
            {
                if (OnObjectEdited != null) 
                {
                    EditorIndex index = Index;
                    FieldEditState state0 = new FieldEditState(args.valueBefore, editor.LastSelectedIndex);
                    FieldEditState state1 = new FieldEditState(args.valueAfter, editor.SelectedIndex);
                    OnObjectEdited(sender, new FieldEditAction(state0, state1, index.RemoveNext(), fieldIndex, editor.EditsCanCombine));
                }
            };
        }

        private void AddInlineRow(Accessor accessor, TableLayoutPanel tableLayout)
        {
            Type type = accessor.GetAccessorType();
            if (!typeof(Json.DataObject).IsAssignableFrom(type))
            {
                Console.WriteLine("Cannot have non-DataObject inline fields.");
                return;
            }

            string fieldName = GetHumanReadableField(accessor.GetName());
            string typeName = GetHumanReadableField(accessor.GetAccessorType().Name);

            GroupBox box = new GroupBox();
            box.Text = fieldName;
            box.Padding = new Padding(3);
            tableLayout.RowCount++;
            tableLayout.Controls.Add(box, 0, tableLayout.RowCount - 1);
            tableLayout.SetColumnSpan(box, 2);

            TableLayoutPanel innerPanel = new TableLayoutPanel();
            innerPanel.Location = new Point(3, 20);
            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            innerPanel.RowStyles.Add(new RowStyle());
            box.Controls.Add(innerPanel);

            Json.DataObject obj = accessor.Get() as Json.DataObject;

            if (obj != null) AddRows(obj, innerPanel, "[" + fieldName + "] ");
            
            if (!accessor.IsReadOnly())
            {    
                int index = inlineAccessors.Count;
                inlineAccessors.Add(accessor);

                FlowLayoutPanel flp = new FlowLayoutPanel();
                flp.AutoSize = true;
                innerPanel.RowCount++;
                innerPanel.Controls.Add(flp, 0, innerPanel.RowCount - 1);
                innerPanel.SetColumnSpan(flp, 2);

                Button buttonAdd = new Button();
                buttonAdd.Text = "New " + typeName;
                buttonAdd.AutoSize = true;
                flp.Controls.Add(buttonAdd);

                Button buttonDelete = new Button();
                buttonDelete.Text = "Delete";
                buttonDelete.AutoSize = true;
                flp.Controls.Add(buttonDelete);


                ClassSelector selector = new ClassSelector();
                buttonAdd.Click += (o, e) =>
                    {
                        Json.DataObject nObj = CreateInstance(accessor.GetAccessorType(), selector);
                        if (nObj != null)
                        {
                            if (obj != null) JsonSerializer.copyFields(obj, nObj);
                            accessor.Set(nObj);
                            UnloadObject();
                            LoadObject();
                            OnObjectEdited(this, new InlineEditAction(Index, index, obj, nObj));
                        }
                    };

                buttonDelete.Click += (o, e) =>
                {
                    accessor.Set(null);
                    UnloadObject();
                    LoadObject();
                    OnObjectEdited(this, new InlineEditAction(Index, index, obj, null));
                };
            }

            innerPanel.Height = innerPanel.PreferredSize.Height;
            box.Height = innerPanel.Height + 30;
            box.Width = tableLayout.Width - 10;
        }

        private void RefreshEditors(object sender, EditAction e)
        {
            foreach (FieldEditor editor in editors)
            {
                if (editor.needsRefresh) editor.Refresh();
            }
        }

        #endregion

        #region Lookup Methods
        public IEnumerable<GuidDataObject> ListObjects(Type type)
        {
            foreach (IList list in lists)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    foreach (GuidDataObject obj in ListChildObjects(list[i] as GuidDataObject, type))
                    {
                        yield return obj;
                    }
                }
            }
            if (parent != null)
            {
                foreach (GuidDataObject obj in parent.ListObjects(type))
                {
                    yield return obj;
                }
            }
        }

        private static IEnumerable<GuidDataObject> ListChildObjects(GuidDataObject obj, Type type)
        {
            if (obj == null) yield break;
            if (Match(obj, type)) yield return obj;
            if (!(obj is ISearchable)) yield break;

            foreach (Accessor field in FieldAccessor.GetForObject(obj).ToList())
            {
                Type fieldType = field.GetAccessorType();
                if (IsGenericList(fieldType))
                {
                    IList list = field.Get() as IList;
                    foreach (object item in list)
                    {
                        foreach (GuidDataObject match in ListChildObjects(item as GuidDataObject, type))
                        {
                            yield return match;
                        }
                    }
                }
            }
        }

        private static bool Match(object obj, Type type)
        {
            return obj is GuidDataObject && type.IsInstanceOfType(obj);
        }
        #endregion

        #region Edits

        public void ApplyEdit(EditorIndex index, EditState state)
        {
            if (index.nextIndex != null)
            {
                if (nextEditor != null) nextEditor.ApplyEdit(index.nextIndex, state);
                UpdateSelectedItemText();
                return;
            }

            ApplyEdit(state as FieldEditState);
            ApplyEdit(state as CreateEditState);
            ApplyEdit(state as MoveEditState);
            ApplyEdit(state as InlineEditState);
            RefreshEditors(null, null);
        }

        private void ApplyEdit(InlineEditState state)
        {
            if (state == null) return;

            inlineAccessors[state.Edit.inlineIndex].Set(state.obj);
            UnloadObject();
            LoadObject();
        }

        private void ApplyEdit(FieldEditState state)
        {
            if (state == null) return;
            int fieldIndex = state.Edit.fieldIndex;
            if (fieldIndex >= 0 && fieldIndex < editors.Count)
            {
                // Console.WriteLine(editors[fieldIndex].accessor.Get() + " -> " + state.value);
                editors[fieldIndex].GetControl().Focus();
                editors[fieldIndex].SetValue(state.value, false);
                editors[fieldIndex].SelectedIndex = state.selection;
            }
        }

        private void ApplyEdit(CreateEditState state)
        {
            if (state == null) return;
            
            List<int> indices = state.Edit.objects.Keys.ToList();
            indices.Sort();

            if (state.created)
            {
                foreach (int index in indices)
                {
                    AddItem(state.Edit.objects[index], index, false, false);
                }
            }
            else
            {
                DeleteItems(indices, false, false);
            }
        }

        private void ApplyEdit(MoveEditState state)
        {
            if (state == null) return;
            MoveItem(state.index0, state.index1, false, false);
        }

        #endregion Edits

        #region List Methods

        private void PopulateList()
        {
            this.listBoxList.Items.Clear();
            this.classSelector.Types = null;
            bool enabled = comboBoxList.SelectedIndex >= 0;
            this.buttonAdd.Enabled = this.buttonDelete.Enabled = 
                this.buttonUp.Enabled = this.buttonDown.Enabled = enabled;
            if (!enabled) return;
            IList list = CurrentList;
            for (int i = 0; i < list.Count; i++)
            {
                listBoxList.Items.Add(SafeName(list[i]));
            }
            UpdateSelectedListItem();
        }

        private void UpdateSelectedListItem()
        {
            int index = listBoxList.SelectedIndex;
            bool enabled = index >= 0;
            this.buttonDelete.Enabled = this.buttonUp.Enabled = this.buttonDown.Enabled = enabled;
            if (!enabled)
            {
                Controls.Remove(nextEditor);
                return;
            }

            if (index == 0) this.buttonUp.Enabled = false;
            if (index == CurrentList.Count - 1) this.buttonDown.Enabled = false;
            if (listBoxList.SelectedIndices.Count > 1)
            {
                this.buttonUp.Enabled = false;
                this.buttonDown.Enabled = false;
            }

            Emigre.Json.DataObject obj = CurrentItem;
            if (nextEditor == null)
            {
                nextEditor = new DataObjectEditor(this);
                nextEditor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
                nextEditor.Height = Height;
                nextEditor.Holder = this;
                nextEditor.OnObjectEdited += (sender, e) => OnItemEdited(e);
                nextEditor.Location = new Point(groupBox.Width + 5, 0);
            }
            if (nextEditor.Parent == null)
            {
                this.Collapsed = true;
                Controls.Add(nextEditor);
            }
            if (AutoCollapse)
            {
                if (nextEditor.nextEditor == null || nextEditor.nextEditor.Parent == null)
                {
                    nextEditor.Collapsed = false;
                }
            }
            nextEditor.DataObject = obj;
            
            //Console.WriteLine(ParentForm.HorizontalScroll.Value);
            //Console.WriteLine(ParentForm.HorizontalScroll.Minimum + ", " + ParentForm.HorizontalScroll.Maximum);
            //int relX = listBoxList.FindForm().PointToClient(
            //    listBoxList.Parent.PointToScreen(listBoxList.Location)).X;
            //Console.WriteLine(relX);
            //Console.WriteLine(ParentForm.HorizontalScroll.Value);
            //int value = ParentForm.HorizontalScroll.Value + relX - 75;
            //ParentForm.HorizontalScroll.Value = Math.Max(ParentForm.HorizontalScroll.Minimum, Math.Min(ParentForm.HorizontalScroll.Maximum, value));
            //Console.WriteLine(value + " / " + ParentForm.HorizontalScroll.Value);
            //ParentForm.Refresh();
        }

        private void AddNewItem()
        {
            if (comboBoxList.SelectedIndex < 0) return;

            IList list = CurrentList;
            Type listOf = GetFirstGenericType(list);
            Json.DataObject obj = CreateInstance(listOf, classSelector);
            if (obj == null) return;

            int index = listBoxList.SelectedIndex;
            if (index == -1) index = list.Count - 1;
            AddItem(obj, index + 1, true);
            if (nextEditor != null) nextEditor.Collapsed = false;
        }

        private Json.DataObject CreateInstance(Type type, ClassSelector selector)
        {
            Json.DataObject obj = null;
            if (type.IsAbstract)
            {
                if (selector.Types == null)
                {
                    selector.Types = Constructor.RegisteredTypes.Where((t) =>
                    !t.IsAbstract && type.IsAssignableFrom(t));
                }
                selector.StartPosition = FormStartPosition.CenterParent;
                if (selector.ShowDialog() == DialogResult.OK &&
                    selector.SelectedType != null)
                {
                    obj = Constructor.Construct(selector.SelectedType);
                }
            }
            else
            {
                obj = Constructor.Construct(type);
            }
            return obj;
        }

        private void DeleteSelectedItems()
        {
            IList list = CurrentList;
            if (list == null || CurrentItem == null) return;
            this.nextEditor.DataObject = null;
            List<int> toRemove = new List<int>();
            foreach (int i in this.listBoxList.SelectedIndices) toRemove.Add(i);
            DeleteItems(toRemove, true);
        }

        private void AddItem(Emigre.Json.DataObject obj, int index, bool select, bool report = true)
        {
            List<Json.DataObject> objs = new List<Json.DataObject>();
            objs.Add(obj);
            AddItems(objs, index, select, report);
        }

        private void AddItems(List<Json.DataObject> objs, int index, bool select, bool report = true)
        {
            IList list = CurrentList;
            if (objs.Count == 0 || list == null) return;

            CreateEditAction action = new CreateEditAction(true, Index.RemoveNext(), Index.RemoveNext());
            foreach (Json.DataObject item in objs)
            {
                CurrentList.Insert(index, item);
                listBoxList.Items.Insert(index, SafeName(item));
                action.objects.Add(index, item);
                index++;
            }
            index--;
            if (select)
            {
                listBoxList.SelectedIndices.Clear();
                this.listBoxList.SelectedIndex = index;
            }
            action.Index1.itemIndex = index;
            if (report && OnObjectEdited != null) OnObjectEdited(this, action);
        }

        private void DeleteItem(int index, bool select, bool report = true)
        {
            List<int> indicies = new List<int>();
            indicies.Add(index);
            DeleteItems(indicies, select, report);
        }

        private void DeleteItems(List<int> toRemove, bool select, bool report = true)
        {
            IList list = CurrentList;
            if (toRemove.Count == 0 || list == null) return;

            CreateEditAction action = new CreateEditAction(false, Index.RemoveNext(), Index.RemoveNext());
            toRemove.Sort();
            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                int index = toRemove[i];
                object item = list[index];
                action.objects.Add(index, (Json.DataObject)item);

                list.RemoveAt(index);
                listBoxList.Items.RemoveAt(index);
            }
            int startIndex = toRemove[0];
            if (startIndex > list.Count - 1) startIndex--;
            if (select)
            {
                listBoxList.SelectedIndices.Clear();
                listBoxList.SelectedIndex = startIndex;
            }
            action.Index1.itemIndex = startIndex;
            if (report && OnObjectEdited != null) OnObjectEdited(this, action);
        }

        private void MoveItem(int fromIndex, int toIndex, bool select, bool report = true)
        {
            IList list = CurrentList;
            if (list == null) return;
            if (fromIndex < 0 || fromIndex >= list.Count) return;
            if (toIndex < 0 || toIndex >= list.Count) return;

            MoveEditAction action = new MoveEditAction(fromIndex, toIndex, Index.RemoveNext(), Index.RemoveNext());

            Json.DataObject item = (Json.DataObject)list[fromIndex];
            DeleteItem(fromIndex, false, false);
            AddItem(item, toIndex, select, false);

            action.Index1.itemIndex = Index.itemIndex;
            if (report && OnObjectEdited != null) OnObjectEdited(this, action);
        }

        private void MoveSelectedItem(int dir)
        {
            IList list = CurrentList;
            if (list == null || CurrentItem == null) return;
            int index = listBoxList.SelectedIndex;
            MoveItem(index, index + dir, true);
        }

        private void OnItemEdited(EditAction e)
        {
            if (OnObjectEdited != null) 
            {
                e.AddIndexParent(Index);
                OnObjectEdited(null, e);
            }
            UpdateSelectedItemText();
        }

        private void UpdateSelectedItemText()
        {
            Emigre.Json.DataObject obj = CurrentItem;
            if (obj == null) return;
            int index = listBoxList.SelectedIndex;
            string name = SafeName(obj);
            if ((string)listBoxList.Items[index] != name)
            {
                listBoxList.SelectedIndexChanged -= listBoxList_SelectedIndexChanged;
                listBoxList.Items[index] = name;
                listBoxList.SelectedIndex = index;
                listBoxList.SelectedIndexChanged += listBoxList_SelectedIndexChanged;
            }
        }

        #region Menu Handlers

        private void UpdateListMenuItems()
        {
            this.importToolStripMenuItem.Enabled =
                this.pasteToolStripMenuItem.Enabled =
                this.comboBoxList.SelectedIndex >= 0;

            this.exportToolStripMenuItem.Enabled =
                this.copyToolStripMenuItem.Enabled =
                this.cutToolStripMenuItem.Enabled =
                this.listBoxList.SelectedIndex >= 0;
            
            if (CurrentList != null)
            {
                Type listOf = GetFirstGenericType(CurrentList);
                object obj = Clipboard.GetData(listOf.FullName);
                this.pasteToolStripMenuItem.Enabled &= obj != null;

                UpdateIgnoredMenuItem();
            }
        }

        private bool UpdateIgnoredMenuItem()
        {
            if (CurrentList == null) return false;
            bool check = true;
            foreach (int i in this.listBoxList.SelectedIndices)
            {
                IIgnorable ignoreable = CurrentList[i] as IIgnorable;
                if (ignoreable == null) this.ignoredToolStripMenuItem.Enabled = false;
                else if (!ignoreable.Ignored) check = false;
            }
            this.ignoredToolStripMenuItem.Checked = check;
            return check;
        }

        private void ToggleIgnoredItems()
        {
            if (CurrentList == null) return;
            bool check = !UpdateIgnoredMenuItem();
            foreach (int i in this.listBoxList.SelectedIndices)
            {
                IIgnorable ignoreable = CurrentList[i] as IIgnorable;
                if (ignoreable == null) continue;
                ignoreable.Ignored = check;
            }
        }

        private void ImportItems()
        {
            IList list = CurrentList;
            if (list == null) return;
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try 
                {
                    string file = this.openFileDialog.FileName;
                    if (!File.Exists(file)) return;
                    string content = File.ReadAllText(file);
                    IWritableContext context = CreateRootContext();
                    var obj = Emigre.Json.JsonSerializer.fromJson(content, true, context);

                    DataObjectList objs = obj as DataObjectList;
                    if (objs == null)
                    {
                        objs = new DataObjectList();
                        objs.Add(obj);
                    }

                    int index = this.listBoxList.SelectedIndex;
                    AddItems(objs, index + 1, true);
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Unable to import file");
                }
            }
        }

        private void ExportItems()
        {
            IList list = CurrentList;
            if (list == null || listBoxList.SelectedIndices.Count == 0) return;
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int index = this.listBoxList.SelectedIndex;
                    DataObjectList objs = new DataObjectList();
                    foreach (int i in listBoxList.SelectedIndices)
                    {
                        objs.Add((Json.DataObject)list[i]);
                    }

                    string file = this.saveFileDialog.FileName;
                    string content = JsonSerializer.toJson(objs.Count == 1 ? objs[0] : objs);
                    File.WriteAllText(file, content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Unable to export file");
                }
            }
        }

        private bool CopyItems()
        {
            if (listBoxList.SelectedIndices.Count == 0) return false;
            IList list = CurrentList;
            if (list == null) return false;
            try
            {
                DataObjectList objs = new DataObjectList();
                foreach (int i in listBoxList.SelectedIndices)
                {
                    objs.Add((Json.DataObject)list[i]);
                }
                Clipboard.SetData(GetFirstGenericType(CurrentList).FullName, Json.JsonSerializer.toJson(objs));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CutItems()
        {
            if (CopyItems())
            {
                DeleteSelectedItems();
            }
        }

        private void PasteItems(bool copy)
        {
            Type listOf = GetFirstGenericType(CurrentList);
            if (listOf == null) return;
            object str = Clipboard.GetData(listOf.FullName);
            if (!(str is string)) return;
            IWritableContext context = CreateRootContext();
            DataObjectList objs = Json.JsonSerializer.fromJson((string)str, copy, context) as DataObjectList;
            if (objs == null) return;

            int index = listBoxList.SelectedIndex + 1;
            AddItems(objs, index, true);
        }

        private IWritableContext CreateRootContext()
        {
            DataObjectEditor parent = this;
            while (parent.parent != null) parent = parent.parent;
            Json.DataObject root = parent.DataObject;
            IWritableContext context = JsonSerializer.createContext(root);
            return context;
        }

        #endregion

        #endregion

        #region Helper Methods

        public static Type GetFirstGenericType(object obj)
        {
            if (obj == null) return null;
            return GetFirstGenericType(obj.GetType());
        }

        public static Type GetFirstGenericType(Type type)
        {
            var args = type.GetGenericArguments();
            if (args == null || args.Length == 0) return null;
            return args[0];
        }

        private static string SafeName(object obj, bool children = true)
        {
            string name = obj.ToString();
            if (name == null) name = "<null>";
            name = name.Replace("\n", "");
            if (children)
            {
                if (obj is IHasEvents)
                {
                    List<StoryEvent> events = (obj as IHasEvents).GetEvents();
                    name += AddNameChildren(events);
                }
                else if (obj is Json.DataObject)
                {
                    var accessors = FieldAccessor.GetForObject(obj);
                    foreach (Accessor accessor in accessors)
                    {
                        Type type = accessor.GetAccessorType();
                        if (IsGenericList(type))
                        {
                            IList list = (IList)accessor.Get();
                            if (list == null) continue;
                            Type listOf = GetFirstGenericType(list);
                            if (typeof(Outcome).IsAssignableFrom(listOf))
                            {
                                name += AddNameChildren(list);
                            }
                        }
                    }
                }
            }
            return name;
        }

        private static string AddNameChildren(IList list)
        {
            string name = "";
            string prefex = "\n >> ";
            if (list.Count > 0) name += ":";
            int max = 3;
            for (int i = 0; i < list.Count; i++)
            {
                if (i == max)
                {
                    name += prefex + "...";
                    break;
                }
                string s = SafeName(list[i], false);
                name += prefex + s;
            }
            return name;
        }

        private static bool IsGenericList(Type t)
        {
            if (t == null) return false;
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static string GetHumanReadableField(String name) 
        {
		    string hName = "";
		    bool capitalize = true;
		    foreach (char c in name) {
			    if (!char.IsLower(c) && hName.Length > 0) {
				    hName += " " + c;
			    } else if (capitalize) {
				    hName += char.ToUpper(c);
				    capitalize = false;
			    } else {
				    hName += c;
				    capitalize = false;
			    }
		    }
		    return hName;
	    }

        #endregion

        #region NoScrollListBox
        private class NoScrollListBox : ListBox
        {
            private readonly DataObjectEditor parentEditor;

            public NoScrollListBox(DataObjectEditor parentEditor)
            {
                this.parentEditor = parentEditor;
                this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            }

            protected override void WndProc(ref Message m)
            {
                // Send WM_MOUSEWHEEL messages to the parent
                if (m.Msg == 0x20a && this.Items.Count * this.ItemHeight <= this.Height)
                {
                    SendMessage(this.Parent.Handle, m.Msg, m.WParam, m.LParam);
                    return;
                }
                base.WndProc(ref m);
            }

            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                if (e.Index > -1 && e.Index < Items.Count)
                {
                    string s = Items[e.Index].ToString();
                    SizeF sf = e.Graphics.MeasureString(s, Font);
                    e.ItemHeight = (int)(sf.Height + 0.5f);
                }
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index > -1 && e.Index < Items.Count)
                {
                    string s = Items[e.Index].ToString();

                    bool ignored = false;
                    if (e.Index < parentEditor.CurrentList.Count)
                    {
                        IIgnorable ignorable = parentEditor.CurrentList[e.Index] as IIgnorable;
                        ignored = ignorable != null && ignorable.Ignored;
                    }

                    bool selected = e.State.HasFlag(DrawItemState.Selected);
                    Color fillColor = selected ? SystemColors.Highlight: SystemColors.Window;
                    Color textColor = selected ? SystemColors.HighlightText : (ignored ? SystemColors.GrayText : SystemColors.WindowText);

                    e.Graphics.FillRectangle(
                        new SolidBrush(fillColor),
                        e.Bounds);

                    int firstNewline = s.IndexOf("\n");
                    if (firstNewline >= 0 && !selected)
                    {
                        string firstLine = s.Substring(0, firstNewline);
                        string restOfLine = s.Substring(firstNewline + 1);
                        e.Graphics.DrawString(firstLine, Font,
                            new SolidBrush(textColor),
                            new PointF(e.Bounds.X, e.Bounds.Y));
                        e.Graphics.DrawString(restOfLine, Font,
                            new SolidBrush(SystemColors.GrayText),
                            new PointF(e.Bounds.X, e.Bounds.Y + e.Graphics.MeasureString(firstLine, Font).Height));
                    }
                    else
                    {
                        e.Graphics.DrawString(s, Font,
                            new SolidBrush(textColor),
                            new PointF(e.Bounds.X, e.Bounds.Y));
                    }
                }
            }
        }
        #endregion

        #region Event Handlers

        private void comboBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void listBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedListItem();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void buttonCollapse_Click(object sender, EventArgs e)
        {
            Collapsed = !Collapsed;
        }
        private void listBoxList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;
            int index = listBoxList.IndexFromPoint(e.X, e.Y);
            if (index >= 0 && index < listBoxList.Items.Count)
            {
                if (listBoxList.SelectedIndices.Count == 1)
                {
                    listBoxList.SelectedIndices.Clear();
                }
                listBoxList.SelectedIndex = index;
            }
        }

        private void contextMenuStripList_Opening(object sender, CancelEventArgs e)
        {
            UpdateListMenuItems();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportItems();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportItems();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyItems();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutItems();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteItems(true);
        }

        private void pasteExactlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteItems(false);
        }

        private void ignoredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleIgnoredItems();
        }


        private void listBoxList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.X: CopyItems(); break;
                    case Keys.C: CutItems(); break;
                    case Keys.V: PasteItems(true); break;
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItems();
            }
        }

        #endregion
    }
}
