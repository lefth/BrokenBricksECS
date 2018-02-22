using System;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class InjectTupleAttribute : Attribute
    {
        public int GroupId;
        public string GroupName;

        public InjectTupleAttribute(string group)
        {
            GroupId = group.GetHashCode();
            GroupName = group;
        }

        public InjectTupleAttribute(int groupId)
        {
            GroupId = groupId;
            GroupName = groupId.ToString();
        }

        public InjectTupleAttribute()
        {
            GroupId = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is InjectTupleAttribute && GroupId.Equals((obj as InjectTupleAttribute).GroupId);
        }

        public override int GetHashCode()
        {
            return GroupId.GetHashCode();
        }
    }
}
