using System;



namespace ECS
{
    public struct Entity : IEquatable<Entity>
    {
        int _id;

        public int Id { get { return _id; } }

        public Entity(int id)
        {
            _id = id;
        }

        public override int GetHashCode()
        {
            return _id;
        }
        public override bool Equals(object obj)
        {
            return obj is Entity && ((Entity)obj)._id == _id;
        }

        public bool Equals(Entity other)
        {
            return _id == other._id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return a._id != b._id;
        }
    }

}
