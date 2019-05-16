using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Emigre.Json;
using System.Linq;

namespace Emigre.Editor
{
    public partial class MainForm : Form
    {
        private static readonly TimeSpan MAX_EDIT_GAP = new TimeSpan(0, 0, 0, 0, 500);
        private static readonly TimeSpan MAX_EDIT_SPAN = new TimeSpan(0, 0, 3);

        private IScriptable scriptable;
        private string scriptablePath;

        public static string ResourcesPath { get; set; }

        private List<EditAction> edits = new List<EditAction>();
        private int editIndex, savedEditIndex; // index of the *next* edit

        private DateTime editGroupStart, editGroupEnd;

        private ClassSelector classSelector = new ClassSelector();

        private string LastFile
        {
            get
            {
                return Properties.Settings.Default.LastFile;
            }
            set
            {
                Properties.Settings.Default.LastFile = value;
                Properties.Settings.Default.Save();
            }
        }

        static MainForm()
        {
            string rootPath = @"C:\Users\Thomas\Documents\Unity\Emigre\"; // Directory.GetParent(Application.StartupPath).Parent.Parent.FullName;
            ResourcesPath = rootPath + @"\Assets\Resources\";
        }

        public MainForm()
        {
            InitializeComponent();
            EditorIndex.Load();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            objectEditor.Visible = false;
            PopulateRecentFiles();
            TryOpenScriptable(LastFile);

            UpdateUndoRedoEnabled();

            objectEditor.OnObjectEdited += RecordEdit;
            objectEditor.Holder = panel;
        }

        #region Undo/Redo

        private void RecordEdit(object sender, EditAction e)
        {
            //Console.WriteLine("Received: " + e.Index0);
            if (editIndex < edits.Count)
            {
                edits.RemoveRange(editIndex, edits.Count - editIndex);
            }
            DateTime now = DateTime.Now;
            if (editIndex > 0)
            {
                EditAction lastEdit = edits[editIndex - 1];

                TimeSpan gap = now - editGroupEnd, span = now - editGroupStart;
                if (gap <= MAX_EDIT_GAP && span <= MAX_EDIT_SPAN)
                {
                    EditAction edit = e.CombineWith(lastEdit);
                    if (edit != null)
                    {
                        edits[editIndex - 1] = edit;
                        editGroupEnd = now;
                        return;
                    }
                }
            }
            edits.Add(e);
            editIndex = edits.Count;
            UpdateUndoRedoEnabled();
            editGroupStart = editGroupEnd = now;
        }

        private void Undo()
        {
            if (editIndex <= 0) return;
            EditAction edit = edits[--editIndex];
            ApplyEdit(edit.Index1, edit.Index0, edit.state0);
            UpdateUndoRedoEnabled();
        }

        private void Redo()
        {
            if (editIndex >= edits.Count) return;
            EditAction edit = edits[editIndex++];
            ApplyEdit(edit.Index0, edit.Index1, edit.state1);
            UpdateUndoRedoEnabled();
        }

        private void ApplyEdit(EditorIndex before, EditorIndex after, EditState state)
        {
            //Console.WriteLine(before + " / " + after);
            this.objectEditor.Index = before;
            Application.DoEvents();
            this.objectEditor.ApplyEdit(after, state);
            this.objectEditor.Index = after;
        }

        private void UpdateUndoRedoEnabled()
        {
            // If they're not enable then the textboxes get to do it...
            //this.undoToolStripMenuItem.Enabled = editIndex > 0;
            //this.redoToolStripMenuItem.Enabled = editIndex < edits.Count;
            UpdateText();
        }

        #endregion

        #region File Handling
        private bool HasUnsavedChanges()
        {
            return editIndex != savedEditIndex;
        }

        private bool ConfirmClose()
        {
            if (scriptable == null) return true;
            if (!HasUnsavedChanges()) return true;
            DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to keep them?", "Save changes?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel) return false;
            if (result == DialogResult.Yes && !SaveScriptable()) return false;
            return true;
        }

        private void NewScriptable()
        {
            if (!ConfirmClose()) return;
            classSelector.Types = Constructor.RegisteredTypes.Where((t) =>
                    !t.IsAbstract && typeof(IScriptable).IsAssignableFrom(t));
            classSelector.StartPosition = FormStartPosition.CenterParent;
            if (classSelector.ShowDialog() != DialogResult.OK) return;
            SetScriptable(Constructor.Construct(classSelector.SelectedType) as IScriptable, null);
        }

        private bool SaveScriptable()
        {
            return SaveScriptable(scriptablePath);
        }

        private bool SaveScriptable(string path)
        {
            if (path == null)
            {
                if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                path = this.saveFileDialog.FileName;
            }
            string contents = Json.JsonSerializer.toJson(scriptable);
            try
            {
                File.WriteAllText(path, contents);
            } 
            catch (Exception e)
            {
                MessageBox.Show("Failed to save file.");
                Console.WriteLine(e.ToString());
                return false;
            }
            savedEditIndex = editIndex;
            SetScriptable(scriptable, path);
            return true;
        }

        private void OpenScriptable(string path = null)
        {
            if (!ConfirmClose()) return;
            if (path == null)
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                path = openFileDialog.FileName;
            }
            
            if (!TryOpenScriptable(path))
            {
                MessageBox.Show("Failed to open file.");
            }
        }

        private void CloseScriptable()
        {
            if (!ConfirmClose()) return;
            SetScriptable(null, null);
        }

        private bool TryOpenScriptable(string path)
        {
            if (path == null || !File.Exists(path)) return false;
            try
            {
                string contents = File.ReadAllText(path);
                IScriptable scriptable = Json.JsonSerializer.fromJson<IScriptable>(contents);
                SetScriptable(scriptable, path);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        private void SetScriptable(IScriptable scriptable, string scriptablePath, bool reload = false)
        {
            if (reload || scriptable != this.scriptable)
            {
                edits.Clear();
                savedEditIndex = editIndex = 0;
            }
            
            this.scriptable = scriptable;
            bool sameFile = scriptablePath == LastFile;
            this.scriptablePath = LastFile = scriptablePath;
            AddRecentFile(scriptablePath);
            UpdateText();
            bool loading = scriptable != objectEditor.DataObject || reload;
            if (reload) objectEditor.DataObject = null;
            objectEditor.DataObject = scriptable;
            objectEditor.Visible = scriptable != null;

            if (loading && sameFile)
            {
                string indexString = Properties.Settings.Default.LastIndex;
                EditorIndex index = JsonSerializer.fromJson<EditorIndex>(indexString);
                if (index != null) objectEditor.Index = index;
            }
            Properties.Settings.Default.LastIndex = JsonSerializer.toJson(objectEditor.Index);
            Properties.Settings.Default.Save();
        }

        private void UpdateText()
        {
            string file = "No File Open";
            if (scriptable != null)
            {
                file = "New " + scriptable.GetType().Name;
            }
            if (scriptablePath != null)
            {
                file = Path.GetFileName(scriptablePath);
            }
            this.Text = "Emigre Editor - " + file + (HasUnsavedChanges() ? "*" : "");
        }

        private void AddRecentFile(string path)
        {
            if (path == null || !File.Exists(path)) return;

            var files = Properties.Settings.Default.RecentFiles;
            if (files == null) files = new System.Collections.Specialized.StringCollection();
            files.Remove(path);
            while (files.Count > 4) files.RemoveAt(files.Count - 1);
            files.Insert(0, path);
            Properties.Settings.Default.RecentFiles = files;
            Properties.Settings.Default.Save();

            PopulateRecentFiles();
        }

        private void PopulateRecentFiles()
        {
            this.openRecentToolStripMenuItem.DropDownItems.Clear();
            if (Properties.Settings.Default.RecentFiles == null) return;
            foreach (string file in Properties.Settings.Default.RecentFiles)
            {
                string name = Path.GetFileName(file);
                ToolStripItem item = this.openRecentToolStripMenuItem.DropDownItems.Add(name);
                item.Tag = file;
                item.Click += OpenRecentFile;
            }
        }

        private void OpenRecentFile(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item == null || item.Tag == null || !(item.Tag is string)) return;
            OpenScriptable(item.Tag as string);
        }

        private void ExportHtml()
        {
            HtmlExporter exporter = new HtmlExporter(scriptable);
            string content = exporter.Export();
            File.WriteAllText(@"C:\Users\Thomas\Desktop\out.html", content);
        }
        
        private void ExportMarkdown()
        {
            if (this.saveFileDialogMD.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(this.saveFileDialogMD.FileName, Script.ScriptWriter.Write(scriptable));
            }
            
        }

        private void ImportMarkdown()
        {
            if (this.openFileDialogMD.ShowDialog() == DialogResult.OK)
            {
                Script.ScriptWriter.Read(scriptable, File.ReadAllText(this.openFileDialogMD.FileName));
                SetScriptable(scriptable, scriptablePath, true);
            }
        }

        private void CompareScriptText()
        {
            if (this.openFileDialogTxt.ShowDialog() == DialogResult.OK)
            {
                string file = this.openFileDialogTxt.FileName;
                string correction = Script.ScriptCorrector.Correct(scriptable, File.ReadAllText(file, System.Text.Encoding.UTF8));
                File.WriteAllText(file, correction, System.Text.Encoding.UTF8);
                SetScriptable(scriptable, scriptablePath, true);
            }
        }

        #endregion

        #region EventHandlers
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScriptable();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenScriptable();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveScriptable();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveScriptable(null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmClose()) e.Cancel = true;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void importMarkdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportMarkdown();
        }

        private void compareScriptTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareScriptText();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseScriptable();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportHtml();
        }

        private void exportMarkdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportMarkdown();
        }

        #endregion
    }

    class PanelNoScrollOnFocus : Panel
    {
        public PanelNoScrollOnFocus()
    	{
    		Enter += PanelNoScrollOnFocus_Enter;
    		Leave += PanelNoScrollOnFocus_Leave;
    	}

    	private System.Drawing.Point scrollLocation;

    	void PanelNoScrollOnFocus_Enter(object sender, System.EventArgs e)
    	{
    		// Set the scroll location back when the control regains focus.
    		HorizontalScroll.Value = scrollLocation.X;
    		VerticalScroll.Value = scrollLocation.Y;
    	}

    	void PanelNoScrollOnFocus_Leave(object sender, System.EventArgs e)
    	{
    		// Remember the scroll location when the control loses focus.
    		scrollLocation.X = HorizontalScroll.Value;
    		scrollLocation.Y = VerticalScroll.Value;
    	}

    	protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            // When there's only 1 control in the panel and the user clicks
            //  on it, .NET tries to scroll to the control. This invariably
            //  forces the panel to scroll up. This little hack prevents that.

            Point point;;
            if (activeControl is DataObjectEditor)
            {
                point = DisplayRectangle.Location;
                point.Offset(-6, -6); // Why is this necessary? I have no idea, but DON'T touch it!
            }
            else
            {
                point = base.ScrollToControl(activeControl);
            }

    		return point;
    	}
    }
}
