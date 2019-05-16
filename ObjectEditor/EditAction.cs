using ObjectEditor.Json;
using System;
using System.Collections.Generic;

namespace ObjectEditor.Editor
{
    public abstract class EditAction : EventArgs
    {
        public readonly EditState state0, state1;

        public abstract EditorIndex Index0 { get; }
        public abstract EditorIndex Index1 { get; }

        public EditAction(EditState state0, EditState state1)
        {
            this.state0 = state0;
            this.state1 = state1;
        }

        public virtual EditAction CombineWith(EditAction previous)
        {
            return null;
        }

        public void AddIndexParent(EditorIndex index)
        {
            Index0.AddParent(index);
            if (Index1 != Index0) Index1.AddParent(index);
        }
    }

    internal abstract class TwoIndexEditAction : EditAction
    {
        private readonly EditorIndex index0, index1;

        public override EditorIndex Index0
        {
            get { return index0; }
        }

        public override EditorIndex Index1
        {
            get { return index1; }
        }

        public TwoIndexEditAction(EditState state0, EditState state1, EditorIndex index0, EditorIndex index1)
            : base(state0, state1)
        {
            this.index0 = index0;
            this.index1 = index1;
        }
    }

    public abstract class EditState
    {
    }

    public abstract class EditState<T> : EditState where T : EditAction
    {
        public T Edit
        {
            get;
            internal set;
        }
    }

    internal class InlineEditAction : EditAction
    {
        public readonly EditorIndex index;
        public readonly int inlineIndex;

        public override EditorIndex Index0
        {
            get { return index; }
        }

        public override EditorIndex Index1
        {
            get { return index; }
        }

        public InlineEditAction(EditorIndex index, int inlineIndex, DataObject obj1, DataObject obj2)
            : base(new InlineEditState(obj1), new InlineEditState(obj2))
        {
            this.index = index;
            this.inlineIndex = inlineIndex;
            ((InlineEditState)this.state0).Edit = this;
            ((InlineEditState)this.state1).Edit = this;
        }
    }

    internal class InlineEditState : EditState<InlineEditAction>
    {
        public readonly DataObject obj;

        public InlineEditState(DataObject obj)
        {
            this.obj = obj;
        }
    }

    internal class MoveEditAction : TwoIndexEditAction
    {
        public MoveEditAction(int startIndex, int endIndex, EditorIndex index0, EditorIndex index1)
            : base(new MoveEditState(endIndex, startIndex), new MoveEditState(startIndex, endIndex), index0, index1)
        {
            ((MoveEditState)this.state0).Edit = this;
            ((MoveEditState)this.state1).Edit = this;
        }
    }

    internal class MoveEditState : EditState<MoveEditAction>
    {
        public readonly int index0, index1;

        public MoveEditState(int index0, int index1)
        {
            this.index0 = index0;
            this.index1 = index1;
        }
    }

    internal class CreateEditAction : TwoIndexEditAction
    {
        public readonly Dictionary<int, DataObject> objects = new Dictionary<int, DataObject>();

        public CreateEditAction(bool create, EditorIndex index0, EditorIndex index1)
            : base(new CreateEditState(!create), new CreateEditState(create), index0, index1)
        {
            ((CreateEditState)this.state0).Edit = this;
            ((CreateEditState)this.state1).Edit = this;
        }
    }
    
    internal class CreateEditState : EditState<CreateEditAction>
    {
        public readonly bool created;

        public CreateEditState(bool created)
        {
            this.created = created;
        }
    }

    internal class FieldEditAction : EditAction
    {
        public readonly EditorIndex index;
        public readonly int fieldIndex;
        public readonly bool canCombime;

        public override EditorIndex Index0
        {
            get { return index; }
        }

        public override EditorIndex Index1
        {
            get { return index; }
        }

        public FieldEditAction(FieldEditState state0, FieldEditState state1, EditorIndex index, int fieldIndex, bool canCombime)
            :base(state0, state1)
        {
            state0.Edit = state1.Edit = this;
            this.index = index;
            this.fieldIndex = fieldIndex;
            this.canCombime = canCombime;
        }

        public override EditAction CombineWith(EditAction previous)
        {
            FieldEditAction edit = previous as FieldEditAction;
            if (previous == null) return null;

            if (!(canCombime && edit.canCombime &&
                edit.fieldIndex == fieldIndex &&
                edit.index.Equals(index))) return null;

            return new FieldEditAction(edit.state0 as FieldEditState, state1 as FieldEditState, index, fieldIndex, true);
        }
    }

    internal class FieldEditState : EditState<FieldEditAction>
    {
        public readonly object value;
        public readonly int selection;

        public FieldEditState(object value, int selection)
        {
            this.value = value;
            this.selection = selection;
        }
    }
}
